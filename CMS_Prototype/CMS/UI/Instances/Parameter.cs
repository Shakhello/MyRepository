using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CMS.UI
{
    public class Parameter : Instance
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public override DefinitionType Type => DefinitionType.Parameter;

        [JsonConverter(typeof(StringEnumConverter))]
        public UI.ParameterType ParameterType { get; set; }

        public string DefaultValue { get; set; }

        public string Name
        {
            get
            {
                return Props.ContainsKey("Name") ? Props["Name"] as string : null;
            }
        }
    }
}
