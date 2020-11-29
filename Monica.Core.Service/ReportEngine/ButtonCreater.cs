using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelDto.Report;
using Newtonsoft.Json.Linq;

namespace Monica.Core.Service.ReportEngine
{
    public class ButtonCreater : IButtonCreater
    {
        private IConnectorManager _connectorManager;

        public ButtonCreater(IConnectorManager connectorManager)
        {
            _connectorManager = connectorManager;
        }

        /// <summary>
        /// Получить список доступных действий для данного режима
        /// </summary>
        /// <param name="buttons">Список описанныз</param>
        /// <param name="filterData"></param>
        /// <returns></returns>
        public async Task<JArray> GetButtonJson(IEnumerable<ButtonAccessDto> buttons, IDictionary<string, object> filterData)
        {
            var jArray = new JArray();
            foreach (var button in buttons)
            {
                var jButton = new JObject();
                jButton.Add("location", new JValue(await GetLocation(button)));
                jButton.Add("id", new JValue(button.Id));
                jButton.Add("sysName", new JValue(button.SysName));
                jButton.Add("typeAction", new JValue(button.TypeActionBtn.ToString()));
                jButton.Add("dopInfo", new JValue(button.SqlData));
                jButton.Add("DialogFormModelId", new JValue(button.DialogFormModelId ?? 0));
                jButton.Add("Message", new JValue(button.Message ?? ""));
                jButton.Add("CaptionDialogFormModel", new JValue(button.CaptionDialogFormModel ?? string.Empty));
                await AddOptions(jButton, button, filterData);
                jButton.Add("onClick", new JValue("action"));
                jArray.Add(jButton);
            }

            return jArray;
        }

        private async Task AddOptions(JObject jbutton, ButtonAccessDto button, IDictionary<string, object> filterData)
        {
            var jOptions = new JObject();
            if (string.IsNullOrEmpty(button.SqlData) || button.TypeActionBtn != TypeActionBtn.Text)
            {
                jOptions.Add("icon", new JValue(button.IconName));
                jOptions.Add("hint", new JValue(button.ToolTip));
                jOptions.Add("text", new JValue(button.DisplayName));
                if (!string.IsNullOrWhiteSpace(button.ValidationGroup))
                    jOptions.Add("validationGroup", new JValue(button.ValidationGroup));
                jOptions.Add("type", new JValue(await GetTypeBtn(button)));
                jOptions.Add("stylingMode", new JValue(await GetStyleMode(button)));
                jbutton.Add("widget", new JValue("dxButton"));
            }
            else if (!string.IsNullOrEmpty(button.SqlData) && button.TypeActionBtn != TypeActionBtn.Standart)
            {
                using var connection = _connectorManager.GetConnection();
                if (filterData != null)
                    foreach (var o in filterData)
                    {
                        var value = o.Value is DateTime
                            ? ((DateTime) o.Value).AddHours(3).ToString("yyyy-MM-dd") as object
                            : o.Value;
                        button.SqlData = button.SqlData.Replace($"[{o.Key}]", value?.ToString() ?? string.Empty);
                    }
                var resultQuery = await connection.QueryFirstAsync(button.SqlData) as IDictionary<string, object>;
                object result;
                if (resultQuery != null && resultQuery.FirstOrDefault().Value is decimal query)
                    result = query.ToString("n2", new CultureInfo("Ru-ru"));
                else result = resultQuery?.FirstOrDefault().Value;
                jbutton.Add("displayName", new JValue($"{button.DisplayName}:"));
                jbutton.Add("value", new JValue(result));
                jbutton.Add("template", new JValue("textInfoBtn"));
                jbutton.Add("widget", new JValue("dxTextBox"));


            }

            jbutton.Add("options", jOptions);
        }

        private Task<string> GetLocation(ButtonAccessDto btn)
        {
            if (btn.Location == Location.After)
                return Task.FromResult("after");
            if (btn.Location == Location.Center)
                return Task.FromResult("center");
            if (btn.Location == Location.Before)
                return Task.FromResult("before");
            return Task.FromResult("after");
        }

        private Task<string> GetStyleMode(ButtonAccessDto btn)
        {
            if (btn.StylingMode == StylingMode.Contained)
                return Task.FromResult("contained");
            if (btn.StylingMode == StylingMode.Outlined)
                return Task.FromResult("outlined");
            return Task.FromResult("contained");
        }

        private Task<string> GetTypeBtn(ButtonAccessDto btn)
        {
            if (btn.TypeBtn == TypeBtn.Success)
                return Task.FromResult("success");
            if (btn.TypeBtn == TypeBtn.Danger)
                return Task.FromResult("danger");
            return Task.FromResult("normal");
        }
    }
 }
