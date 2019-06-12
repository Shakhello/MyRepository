using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CMS.UI
{
    public class ActionResult
    {
        public bool Success { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public UI.ActionType ActionType { get; set; }

        public object Data { get; set; }
    }
}
