using CMS.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CMS.UI
{
    public class ParameterDefinition : Definition
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DAL.Models.ParameterType ParameterType { get; set; }

        public string Name { get; set; }

        public int ActionId { get; set; }

        public int? ControlId { get; set; }

        public int? FieldId { get; set; }

        public int Order { get; set; }

        public string DefaultValue { get; set; }

    }
}
