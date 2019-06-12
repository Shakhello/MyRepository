using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace CMS.UI
{
    public class Action : Instance
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public override DefinitionType Type => DefinitionType.Action;

        [JsonConverter(typeof(StringEnumConverter))]
        public UI.ActionType ActionType { get; set; }

        public List<Parameter> Parameters { get; set; } = new List<Parameter>();

        public View ViewData { get; set; }

        public object Value { get; set; }
    }
}
