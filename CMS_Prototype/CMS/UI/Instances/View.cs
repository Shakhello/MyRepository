using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace CMS.UI
{
    public class View : Instance
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public override DefinitionType Type => DefinitionType.View;

        [JsonConverter(typeof(StringEnumConverter))]
        public UI.ViewType ViewType { get; set; }

        public List<View> ChildViews { get; set; } = new List<View>();

        public List<Control> Controls { get; set; } = new List<Control>();

        public List<Filter> Filters { get; set; } = new List<Filter>();

        
    }
}
