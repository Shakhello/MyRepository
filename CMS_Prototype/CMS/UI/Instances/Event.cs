using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace CMS.UI
{
    public class Event : Instance
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public UI.EventType EventType { get; set; }

        public List<Action> Actions { get; set; } = new List<Action>();

        [JsonConverter(typeof(StringEnumConverter))]
        public override DefinitionType Type => DefinitionType.Event;
    }
}
