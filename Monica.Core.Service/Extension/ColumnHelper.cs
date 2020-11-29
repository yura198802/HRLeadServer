using System;
using System.Collections.Generic;
using System.Linq;
using DynamicExpresso;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelDto.Report;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Monica.Core.Service.Extension
{
    public static class ColumnHelper
    {
        public static string GetFieldName(this string fieldName)
        {
            if (fieldName.Contains("{"))
            {
                var obj = JObject.Parse(fieldName);
                fieldName = (obj["Alias"] ?? obj["Fk"])?.ToString() ?? fieldName;
            }

            return fieldName;
        }

        public static bool IsDefaultValueByNew(this string fieldName)
        {
            if (!fieldName.Contains("{"))
                return false;
            var obj = JObject.Parse(fieldName);
            if (obj["DefaultValueSave"]?["OnlyNew"] == null)
                return false;

            return Convert.ToBoolean(obj["DefaultValueSave"]?["OnlyNew"]);
        }


        public static bool IsDefaultValueByEdit(this string fieldName)
        {
            if (!fieldName.Contains("{"))
                return false;
            var obj = JObject.Parse(fieldName);
            if (obj["DefaultValueSave"] == null)
                return false;
            if (obj["DefaultValueSave"]?["OnlyNew"] == null)
                return true;
            if (Convert.ToBoolean(obj["DefaultValueSave"]?["OnlyNew"]))
                return false;
            return true;
        }

        public static object GetDefaultValue(this string fieldName, IDictionary<string, object> staticDefaultValue)
        {
            if (!fieldName.Contains("{"))
                return null;
            var obj = JObject.Parse(fieldName);
            if (obj["DefaultValueSave"]?["Value"] == null)
                return null;
            var value = obj["DefaultValueSave"]["Value"].ToString();
            if (value == "{CurrentUser}")
                return staticDefaultValue[value];
            if (value == "{DateTime.Now}")
                return staticDefaultValue[value];
            return value;
        }

        private static IDictionary<string, object> ToDictionary(dynamic data)
        {
            if (data is IDictionary<string, object>)
                return data as IDictionary<string, object>;
            var jObject = data as JObject;
            if (jObject != null)
            {
                var dict = new Dictionary<string, object>();
                var res = JsonConvert.DeserializeAnonymousType(jObject.ToString(), dict);
                return res;
            }
            return null;
        }

        public static List<FieldAccessDto> WhereFieldAccess(this List<FieldAccessDto> fieldss, dynamic data)
        {
            var results = ToDictionary(data);
            var fieldsExpress = fieldss.Where(f => !string.IsNullOrEmpty(f.Express)).ToList();

            var interpreter = new Interpreter();
            if (results != null)
                foreach (var result in results)
                {
                    interpreter.SetVariable(result.Key, result.Value);
                }
            else
                foreach (var result in fieldss)
                {
                    interpreter.SetVariable(result.Name.GetFieldName(), (result.IsKey ?? true) ? (object)0 : null);
                }

            foreach (var dto in fieldsExpress)
            {
                var isEvaluate = interpreter.Eval<bool>(dto.Express);
                if (isEvaluate)
                    fieldss.Remove(dto);
            }

            foreach (var dto in fieldss)
            {
                if (string.IsNullOrEmpty(dto.RequiredExpress))
                    continue;
                var isEvaluate = interpreter.Eval<bool>(dto.RequiredExpress);
                if (isEvaluate)
                    dto.TypeAccec = TypeAccec.ReadOnly;

            }

            return fieldss;
        }
    }
}
