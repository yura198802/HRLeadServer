using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelDto.Report;
using Monica.Core.Service.Extension;
using Newtonsoft.Json.Linq;

namespace Monica.Core.Service.ReportEngine
{
    public class ColumnCreater : IColumnCreater
    {
        private IConnectorManager _connectorManager;

        public ColumnCreater(IConnectorManager connectorManager)
        {
            _connectorManager = connectorManager;
        }

        /// <summary>
        /// Получить колонки в формате json
        /// </summary>
        /// <param name="fields">Список доступных колонок</param>
        /// <returns></returns>
        public async Task<string> GetColumns(IEnumerable<FieldAccessDto> fields, FormModelDto formModel)
        {
            var jArray = new JArray();
            foreach (var field in fields)
            {
                if (!(field.IsVisibleList ?? false))
                    continue;
                var fieldName = ColumnHelper.GetFieldName(field.Name);


                var jColumn = new JObject();
                jColumn.Add("caption", new JValue(field.DisplayName));
                jColumn.Add("dataField", new JValue(fieldName));
                if (formModel.IsNotAutoWidthColumn ?? false)
                {
                    jColumn.Add("width", new JValue($"{field.WidthList}"));
                }
                else jColumn.Add("width", new JValue($"{field.WidthList}%"));

                if (!string.IsNullOrWhiteSpace(field.Mask) && field.TypeControl != TypeControl.TextEdit)
                {
                    if (field.Mask.StartsWith("{"))
                        jColumn.Add("format", JObject.Parse(field.Mask));
                    else jColumn.Add("format", new JValue(field.Mask));

                }
                else if (!string.IsNullOrWhiteSpace(field.Mask) && field.TypeControl == TypeControl.TextEdit)
                {
                    var jOptions = new JObject();
                    if (field.Mask.StartsWith("{"))
                        jOptions.Add("mask", JObject.Parse(field.Mask));
                    else jOptions.Add("mask", new JValue(field.Mask));
                    //else jColumn.Add("format", new JValue(field.Mask));
                    jColumn.Add("editorOptions", jOptions);
                }

                jColumn.Add("dataType", new JValue(await GetDataTypeColumn(field)));

                jArray.Add(jColumn);
            }

            return jArray.ToString();
        }


        /// <summary>
        /// Получить список полей для редактирования
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<string> GetProperty(IEnumerable<FieldAccessDto> fields, FormModelDto entity)
        {
            var fieldAccessDtos = fields.Where(f => f.IsDetail ?? false).OrderBy(o => o.OrderDetail ?? 0).ToList();
            var jArray = new JArray();
            var count = (entity.ColCount ?? 0) == 0 ? 1 : entity.ColCount;

            var fieldsGroup = fieldAccessDtos.ToList();
            JObject jTabItem = null;
            JArray jTabs = new JArray();
            if (fieldsGroup.Any(f => f.TypeGroup == TypeGroup.Tab))
            {
                jTabItem = new JObject();
                jTabItem.Add("itemType", "tabbed");
                jTabItem.Add("tabs", jTabs);
                jArray.Add(jTabItem);
            }
            for (int i = 1; i <= count; i++)
            {
                foreach (var field in fieldsGroup.Where(f => (f.ParentId ?? 0) == 0))
                {
                    JObject jObject;
                    switch (field.TypeGroup)
                    {
                        case TypeGroup.None:
                            jObject = await CreateSimpleItem(field);
                            break;
                        case TypeGroup.Group:
                            jObject = await CreateGroupItem(field, fieldsGroup);
                            break;
                        case TypeGroup.Tab:
                            {
                                jObject = await CreateTabItem(field, fieldsGroup);
                                jTabs.Add(jObject);
                                break;
                            }

                        default:
                            jObject = null;
                            break;
                    }
                    if (jObject == null)
                        continue;
                    if (field.TypeGroup != TypeGroup.Tab)
                        jArray.Add(jObject);
                }
            }

            return jArray.ToString();
        }

        private async Task<JObject> CreateSimpleItem(FieldAccessDto field)
        {
            var jProperty = new JObject();

            jProperty.Add("editorType", new JValue(await GetDataTypeProperty(field)));
            jProperty.Add("isRequired", new JValue(field.TypeProfileForm == TypeProfileForm.Required));
            if (field.TypeControl == TypeControl.MultiArea)
                jProperty.Add("template", new JValue("multiarea"));
            jProperty.Add("dataField", new JValue(ColumnHelper.GetFieldName(field.Name)));
            var jLabel = new JObject();
            jLabel.Add("text", new JValue(field.DisplayName));
            jLabel.Add("location", new JValue("left"));
            if (string.IsNullOrEmpty(field.DisplayName))
                jLabel.Add("visible", new JValue(false));
            jProperty.Add("label", jLabel);
            var joptions = new JObject();
            if (field.IsKey ?? false)
                joptions.Add("readOnly", new JValue(field.IsKey));
            else joptions.Add("readOnly", new JValue(field.TypeAccec == TypeAccec.ReadOnly));
            if (field.OrderDetail <= -100)
                joptions.Add("visible", new JValue(false));

            joptions.Add("onValueChanged", new JValue(""));

            if (field.TypeControl == TypeControl.MultiLine)
                joptions.Add("height", new JValue(90));
            await AddPropSimpleFormSelect(field, joptions);
            await AddPropSimleComboBox(field, joptions);
            await AddPropSimleNumberBox(field, jProperty);//создать аналог для tboxa  с кнопкой
            if (field.TypeControl == TypeControl.DateEdit)
            {
                joptions.Add("useMaskBehavior", new JValue(true));
            }

            if (field.MaxLength != 0)
                joptions.Add("maxLength", new JValue(field.MaxLength));

            string nameMask = field.TypeControl == TypeControl.TextEdit ? "mask" : "format";
            if (!string.IsNullOrWhiteSpace(field.Mask))
            {
                if (field.Mask.StartsWith("{"))
                    joptions.Add(nameMask, JObject.Parse(field.Mask));
                else joptions.Add(nameMask, new JValue(field.Mask));
            }

            jProperty.Add("editorOptions", joptions);
            return jProperty;
        }



        private Task AddPropSimleNumberBox(FieldAccessDto field, JObject jProperty)
        {
            if (field.TypeControl != TypeControl.NumericEdit)
                return Task.CompletedTask;
            jProperty.Add("value", new JValue(0));
            return Task.CompletedTask;
        }

        private async Task AddPropSimpleFormSelect(FieldAccessDto field, JObject joptions)
        {
            if (field.TypeControl == TypeControl.FormSelect)
            {
                var jBtnMoreOptions = new JObject();
                jBtnMoreOptions.Add("icon", "more");
                jBtnMoreOptions.Add("type", "normal");

                var jBtnMore = new JObject();
                jBtnMore.Add("sysName", new JValue(field.Name));
                jBtnMore.Add("DialogFormModelId", new JValue(field.FormModelDetailId));
                jBtnMore.Add("CaptionDialogFormModel", new JValue(field.CaptionDialogFormModel));
                jBtnMore.Add("name", "more");
                jBtnMore.Add("location", "after");
                jBtnMore.Add("onClick", "action");
                jBtnMore.Add("options", jBtnMoreOptions);
                if (field.Name.StartsWith("{"))
                {
                    if (JObject.Parse(field.Name)["DialogResult"] != null)
                        jBtnMore.Add("OtherParam", JObject.Parse(field.Name)["DialogResult"]);
                }

                var jBtnClear = new JObject();
                jBtnClear.Add("name", "clear");

                var btns = new JArray();
                btns.Add(jBtnClear);
                btns.Add(jBtnMore);

                joptions.Add("mode", new JValue("text"));
                joptions.Add("showClearButton", new JValue(true));
                joptions.Add("buttons", btns);
            }
        }
        private async Task AddPropSimleComboBox(FieldAccessDto field, JObject joptions)
        {
            if (field.TypeControl == TypeControl.ComboBox && !string.IsNullOrWhiteSpace(field.ValueListBox))
            {
                joptions.Add("displayExpr", new JValue("value"));
                joptions.Add("valueExpr", new JValue("id"));
                joptions.Add("searchEnabled", new JValue(true));
                if (field.ValueListBox.ToLower().StartsWith("select"))
                {
                    using var connection = _connectorManager.GetConnection();
                    var data = await connection.QueryAsync<ListBoxValueModel>(field.ValueListBox);
                    joptions.Add("dataSource", JObject.FromObject(new { store = JArray.FromObject(data), paginate = true }));
                }
                else
                {
                    joptions.Add("dataSource", JArray.Parse(field.ValueListBox));
                }
            }
        }


        private async Task<JObject> CreateTabItem(FieldAccessDto field, List<FieldAccessDto> fields)
        {
            var jGroupBase = new JObject();
            jGroupBase.Add("title", field.DisplayName);
            jGroupBase.Add("colCount", new JValue(field.GroupCol));
            var jItems = new JArray();
            foreach (var item in fields.Where(f => (f.ParentId ?? 0) == field.Id))
            {
                JObject jObject;
                switch (item.TypeGroup)
                {
                    case TypeGroup.None:
                        jObject = await CreateSimpleItem(item);
                        break;
                    case TypeGroup.Group:
                        jObject = await CreateGroupItem(item, fields);
                        break;
                    case TypeGroup.Tab:
                        jObject = await CreateGroupItem(item, fields);
                        break;
                    default:
                        jObject = null;
                        break;
                }
                if (jObject == null)
                    continue;
                jItems.Add(jObject);
            }

            jGroupBase.Add("items", jItems);
            return jGroupBase;
        }

        private async Task<JObject> CreateGroupItem(FieldAccessDto field, List<FieldAccessDto> fields)
        {
            var jGroupBase = new JObject();
            jGroupBase.Add("itemType", field.TypeGroup == TypeGroup.Group ? "group" : "tabbed");
            jGroupBase.Add("caption", field.DisplayName);
            jGroupBase.Add("colCount", new JValue(field.GroupCol));
            var jItems = new JArray();
            foreach (var item in fields.Where(f => (f.ParentId ?? 0) == field.Id))
            {
                JObject jObject;
                switch (item.TypeGroup)
                {
                    case TypeGroup.None:
                        jObject = await CreateSimpleItem(item);
                        break;
                    case TypeGroup.Group:
                        jObject = await CreateGroupItem(item, fields);
                        break;
                    case TypeGroup.Tab:
                        jObject = await CreateGroupItem(item, fields);
                        break;
                    default:
                        jObject = null;
                        break;
                }
                if (jObject == null)
                    continue;
                jItems.Add(jObject);
            }

            jGroupBase.Add("items", jItems);
            return jGroupBase;
        }

        private Task<string> GetDataTypeProperty(FieldAccessDto field)
        {
            var result = field.TypeControl switch
            {
                TypeControl.TextEdit => "dxTextBox",
                TypeControl.CheckBox => "dxCheckBox",
                TypeControl.ComboBox => "dxSelectBox",
                TypeControl.DateEdit => "dxDateBox",
                TypeControl.MultiLine => "dxTextArea",
                TypeControl.NumericEdit => "dxNumberBox",
                _ => "dxTextBox"
            };
            return Task.FromResult(result);
        }

        private Task<string> GetDataTypeColumn(FieldAccessDto field)
        {
            if (field.Name.Contains("{") && field.TypeControl == TypeControl.ComboBox)
            {
                return Task.FromResult("string");
            }
            var result = field.TypeControl switch
            {
                TypeControl.TextEdit => "string",
                TypeControl.CheckBox => "boolean",
                TypeControl.DateEdit => "date",
                TypeControl.MultiLine => "string",
                TypeControl.NumericEdit => "number",
                _ => "object"
            };
            return Task.FromResult(result);
        }
    }

    public class ListBoxValueModel
    {
        public object id { get; set; }
        public string value { get; set; }
    }
}
