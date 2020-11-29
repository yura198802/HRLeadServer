using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.EngineReport;
using Monica.Core.Service.Extension;
using Monica.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Monica.Core.Service.ReportEngine
{
    public class FilterPanelPropertysCreater : IFilterPanelPropertysCreater
    {
        private readonly ReportDbContext _reportDbContext;
        private readonly IConnectorManager _connectorManager;

        public FilterPanelPropertysCreater(ReportDbContext reportDbContext, IConnectorManager connectorManager)
        {
            _reportDbContext = reportDbContext;
            _connectorManager = connectorManager;
        }

        /// <summary>
        /// Получить список фильтров
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<JArray> GetProperiesFilterPanel(int formId, string userName)
        {
            var models = await _reportDbContext.FilterFormModel.Include(s => s.FormModel)
                .Where(f => f.FormModel.Id == formId && (f.IsVisibleFilter ?? false)).OrderBy(o => o.Order ?? 0).ToListAsync();

            return await CreateProperties(models, formId, userName);
        }


        /// <summary>
        /// Получить список фильтров для диалоговой формы
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="userName"></param>
        /// <param name="buttonId"></param>
        /// <returns></returns>
        public async Task<JArray> GetProperiesFilterDialog(int formId, string userName, int buttonId)
        {
            var models = await _reportDbContext.DialogButtonForm
                .Include(s => s.FormModel)
                .Include(s => s.ButtonForm)
                .Where(f => f.FormModel.Id == formId && f.ButtonForm.Id == buttonId).OrderBy(o => o.Order ?? 0).ToListAsync();

            return await CreateProperties(models, formId, userName, buttonId, true);
        }
        /// <summary>
        /// Получить данные для панели фильтров в основном режиме
        /// </summary>
        /// <param name="filterModel"></param>
        /// <param name="idFilter"></param>
        /// <returns></returns>
        public async Task<JArray> GetLoadDataFilterModel(IDictionary<string, object> filterModel, int idFilter)
        {
            var model = await _reportDbContext.FilterFormModel.FirstOrDefaultAsync(f => f.Id == idFilter);
            return await LoadData(model, filterModel);
        }
        /// <summary>
        /// Получить данные для формы фильтра, которая вызывается через диалоговую форму фильтров
        /// </summary>
        /// <param name="filterModel"></param>
        /// <param name="idFilter"></param>
        /// <returns></returns>
        public async Task<JArray> GetLoadDialogDataFilter(IDictionary<string, object> filterModel, int idFilter)
        {
            var model = await _reportDbContext.DialogButtonForm.FirstOrDefaultAsync(f => f.Id == idFilter);
            return await LoadData(model, filterModel);
        }

        private async Task<JArray> LoadData(IFilterForm model, IDictionary<string, object> filterModel)
        {
            var actionFilter = AutoFac.ResolveNamed<ILoadDataFilter>(model.Sysname, true);
            if (actionFilter == null)
            {
                switch (model.TypeFilter)
                {
                    case TypeFilter.Calendar when !string.IsNullOrEmpty(model.SqlData):
                    {
                        using var connection = _connectorManager.GetConnection();
                        var data = await connection.QueryAsync(model.SqlData);
                        return JArray.FromObject(data.Select(s => new
                            {Date = ((DateTime) s.Date).ToString("yyyy-MM-dd")}));
                    }
                    case TypeFilter.SelectBox when model.SqlData?.ToLower().StartsWith("select") ?? false:
                    {
                        using var connection = _connectorManager.GetConnection();
                        var data = await connection.QueryAsync<ListBoxValueModel>(model.SqlData);
                        return JArray.FromObject(data);
                    }
                    case TypeFilter.SelectBox:
                        return  JArray.Parse(model.SqlData ?? string.Empty);
                }
            }
            else return  await actionFilter.ActionLoadDataFilter(filterModel);

            return null;
        }



        private async Task<JArray> CreateProperties(IEnumerable<IFilterForm> models, int formId, string userName, int? buttonId = null, bool isRequred = false)
        {
            var jItems = new JArray();
            var modelsFilter = buttonId == null ?  await GetFilterValue(formId, userName) : await GetFilterValueButton(formId, userName, buttonId ?? 0);
            foreach (var model in models)
            {
                var filter = modelsFilter == null ? null :  modelsFilter.ContainsKey(model.Sysname)? modelsFilter[model.Sysname] : null;
                JObject property = null;
                if (model.TypeFilter == TypeFilter.Calendar)
                    property = await GetProperyCalendar(model, filter, modelsFilter);
                if (model.TypeFilter == TypeFilter.DateTimePicker)
                    property = await GetProperyDateTimePicker(model, filter, isRequred);
                if (model.TypeFilter == TypeFilter.SelectBox)
                    property = await GetProperyCombobox(model, filter, isRequred, modelsFilter);
                if (model.TypeFilter == TypeFilter.TagBox)
                    property = await GetProperyTagBox(model, filter, isRequred, modelsFilter);
                if (property != null)
                    jItems.Add(property);
            }
        

            return jItems;
        }

        private async Task<IDictionary<string, object>> GetFilterValue(int formModelId, string userName)
        {
            var modelFilter = await _reportDbContext.FilterUser
                .Include(c => c.FormModel)
                .Include(s => s.ButtonForm)
                .FirstOrDefaultAsync(f => f.FormModel.Id == formModelId && f.UserName == userName);
            if (modelFilter == null)
                return null;
            return JsonConvert.DeserializeObject<IDictionary<string, object>>(modelFilter.ModelFilter);
        }


        private async Task<IDictionary<string, object>> GetFilterValueButton(int formModelId, string userName, int buttonId)
        {
            var modelFilter = await _reportDbContext.FilterUser
                .Include(c => c.FormModel)
                .Include(s => s.ButtonForm)
                .FirstOrDefaultAsync(f => f.FormModel.Id == formModelId && f.UserName == userName && f.ButtonForm != null && f.ButtonForm.Id == buttonId);
            if (modelFilter == null)
                return null;
            return JsonConvert.DeserializeObject<IDictionary<string, object>>(modelFilter.ModelFilter);
        }

        private async Task<JObject> GetProperyCalendar(IFilterForm model, object filterValue, IDictionary<string, object> filterModel)
        {
            if (model == null)
                return null;
            var jProperty = new JObject();
            jProperty.Add("editorType", new JValue("dxCalendar"));
            jProperty.Add("id", new JValue("calendar"));
            jProperty.Add("idProp", new JValue(model.Id));
            jProperty.Add("parentId", new JValue(model.ParentId));
            jProperty.Add("value", new JValue(filterValue == null ? DateTime.Now : ((DateTime)filterValue).AddHours(3)));
            jProperty.Add("dataField", new JValue(model.Sysname.GetFieldName()));
            
            var jLabel = new JObject();
            jLabel.Add("visible", new JValue(false));
            jProperty.Add("label", jLabel);
            var joptions = new JObject();
            joptions.Add("onValueChanged", new JValue(""));
            joptions.Add("cellTemplate", "calendar");
            jProperty.Add("editorOptions", joptions);
            var actionFilter = AutoFac.ResolveNamed<ILoadDataFilter>(model.Sysname, true);
            if (actionFilter == null)
            {
                if (!string.IsNullOrEmpty(model.SqlData))
                {
                    using var connection = _connectorManager.GetConnection();
                    var data = await connection.QueryAsync(model.SqlData);
                    jProperty.Add("sqlData", JArray.FromObject(data.Select(s => new { Date = ((DateTime)s.Date).ToString("yyyy-MM-dd") })));
                }
            }
            else jProperty.Add("sqlData", await actionFilter.ActionLoadDataFilter(filterModel));

            return jProperty;
        }


        private Task<JObject> GetProperyDateTimePicker(IFilterForm model, object filterValue, bool isRequred)
        {
            if (model == null)
                return null;
            var jProperty = new JObject();
            jProperty.Add("editorType", new JValue("dxDateBox"));
            jProperty.Add("id", new JValue(model.Sysname));
            jProperty.Add("idProp", new JValue(model.Id));
            jProperty.Add("parentId", new JValue(model.ParentId));
            jProperty.Add("value", new JValue(filterValue == null ? DateTime.Now : ((DateTime)filterValue).AddHours(3)));
            jProperty.Add("dataField", new JValue(model.Sysname.GetFieldName()));
            jProperty.Add("isRequired", isRequred);
            var jLabel = new JObject();
            jLabel.Add("visible", new JValue(true));
            jLabel.Add("location", new JValue("top"));
            jLabel.Add("text", new JValue(model.Caption));
            jProperty.Add("label", jLabel);
            var joptions = new JObject();
            joptions.Add("onValueChanged", new JValue(""));
            joptions.Add("useMaskBehavior", new JValue(true));
            jProperty.Add("editorOptions", joptions);

            return Task.FromResult(jProperty);
        }


        private async Task<JObject> GetProperyCombobox(IFilterForm model, object filterValue, bool isRequred, IDictionary<string, object> filterModel)
        {
            if (model == null)
                return null;
            var jProperty = new JObject();
            jProperty.Add("editorType", new JValue("dxSelectBox"));
            jProperty.Add("id", new JValue(model.Sysname));
            jProperty.Add("idProp", new JValue(model.Id));
            jProperty.Add("parentId", new JValue(model.ParentId));
            jProperty.Add("value", new JValue(filterValue));
            jProperty.Add("dataField", new JValue(model.Sysname.GetFieldName()));

            var jLabel = new JObject();
            jProperty.Add("isRequired", isRequred);
            jLabel.Add("visible", new JValue(true));
            jLabel.Add("text", new JValue(model.Caption));
            jLabel.Add("location", new JValue("top"));
            jProperty.Add("label", jLabel);
            var joptions = new JObject();
            joptions.Add("onValueChanged", new JValue(""));
            joptions.Add("displayExpr", new JValue("value"));
            joptions.Add("valueExpr", new JValue("id"));
            joptions.Add("searchEnabled", new JValue(true));

            var actionFilter = AutoFac.ResolveNamed<ILoadDataFilter>(model.Sysname, true);
            if (actionFilter == null)
            {
                if (model.SqlData.ToLower().StartsWith("select"))
                {
                    using var connection = _connectorManager.GetConnection();
                    var data = await connection.QueryAsync<ListBoxValueModel>(model.SqlData);
                    joptions.Add("dataSource", JObject.FromObject(new { store = JArray.FromObject(data), paginate = true }));
                }
                else
                {
                    joptions.Add("dataSource", JObject.FromObject(new { store = JArray.Parse(model.SqlData), paginate = true }));
                }
            }
            else joptions.Add("dataSource", JObject.FromObject(new { store = await actionFilter.ActionLoadDataFilter(filterModel), paginate = true }));
            
            jProperty.Add("editorOptions", joptions);

            return jProperty;
        }


        private async Task<JObject> GetProperyTagBox(IFilterForm model, object filterValue, bool isRequred, IDictionary<string, object> filterModel)
        {
            if (model == null)
                return null;
            var jProperty = new JObject();
            jProperty.Add("editorType", new JValue("dxTagBox"));
            jProperty.Add("id", new JValue(model.Sysname));
            jProperty.Add("idProp", new JValue(model.Id));
            jProperty.Add("parentId", new JValue(model.ParentId));
            if (filterValue is JArray)
                jProperty.Add("value", (JArray)filterValue);
            else
                jProperty.Add("value", new JValue(filterValue));
            jProperty.Add("dataField", new JValue(model.Sysname.GetFieldName()));

            var jLabel = new JObject();
            jProperty.Add("isRequired", isRequred);
            jLabel.Add("visible", new JValue(true));
            jLabel.Add("text", new JValue(model.Caption));
            jLabel.Add("location", new JValue("top"));
            jProperty.Add("label", jLabel);
            var joptions = new JObject();
            joptions.Add("onValueChanged", new JValue(""));
            joptions.Add("displayExpr", new JValue("value"));
            joptions.Add("valueExpr", new JValue("id"));
            joptions.Add("searchEnabled", new JValue(true));

            var actionFilter = AutoFac.ResolveNamed<ILoadDataFilter>(model.Sysname, true);
            if (actionFilter == null)
            {
                if (model.SqlData.ToLower().StartsWith("select"))
                {
                    using var connection = _connectorManager.GetConnection();
                    var data = await connection.QueryAsync<ListBoxValueModel>(model.SqlData);
                    joptions.Add("dataSource", JObject.FromObject(new { store = JArray.FromObject(data), paginate = true }));
                }
                else
                {
                    joptions.Add("dataSource", JObject.FromObject(new { store = JArray.Parse(model.SqlData), paginate = true }));
                }
            }
            else joptions.Add("dataSource", JObject.FromObject(new { store = await actionFilter.ActionLoadDataFilter(filterModel), paginate = true }));

            jProperty.Add("editorOptions", joptions);

            return jProperty;
        }
    }
}
