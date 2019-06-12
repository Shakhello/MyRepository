using CMS.DAL.Interfaces;
using CMS.DAL.Models;
using CMS.DAL.Services;
using Common.ExtensionMethods;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS.UI
{
    public class Filter : Instance, IFilterWithValue //, IFilter
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public override DefinitionType Type => DefinitionType.Filter;

        [JsonConverter(typeof(StringEnumConverter))]
        public UI.FilterType? FilterType { get; set; }

        public object Value { get; set; }

        public List<Field> Fields { get; set; } = new List<Field>();

        #region IFilterWithValue

        public IFilter GetFilter()
        {
            var definition = (DAL.Models.Filter)((FilterDefinition)Definition)?.Entity;

            return definition 
                ?? new DbEditorService().GetFilter(Convert.ToInt32(Props["Id"]));
        }


        public object GetValue()
        {
            if (IsIntDictionary())
                Value = ((JArray)Value).Select(jv => (int)jv).ToArray();
            else if (IsStringDictionary())
                Value = ((JArray)Value).Select(jv => (string)jv).ToArray();
            else if (IsDateRange())
                Value = ((JArray)Value).Select(jv => DateTime.Parse(jv.ToString()));

            return Value;
        }

        #endregion


        private bool IsIntDictionary()
        {
            return Value is JArray &&
                IsDictionary() &&
                Fields.First().GetDictionaryType() == DAL.Models.DictionaryType.Int;
        }

        private bool IsStringDictionary()
        {
            return Value is JArray &&
                IsDictionary() &&
                Fields.First().GetDictionaryType() == DAL.Models.DictionaryType.String;
        }

        public bool IsDictionary()
        {
            return Fields.Any(f => f.FieldType == FieldType.Dictionary);
        }

        private bool IsDateRange()
        {
            return Value is JArray &&
                Fields.First().FieldType == FieldType.DateTime;
        }



    }
}
