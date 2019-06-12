using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace CMS.UI
{
    public class Control : Instance
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public override DefinitionType Type => DefinitionType.Control;

        [JsonConverter(typeof(StringEnumConverter))]
        public UI.ControlType ControlType { get; set; }

        public List<Event> Events { get; set; } = new List<Event>();

        public object Value { get; set; }

    }
}
