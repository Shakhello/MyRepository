using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;

namespace CMS.UI
{
    public class Section : Instance
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public override DefinitionType Type => DefinitionType.Section;

        [JsonConverter(typeof(StringEnumConverter))]
        public SectionType SectionType { get; set; }

        public List<ViewLink> ViewLinks { get; set; } = new List<ViewLink>();

        public UI.Control SettingsButton { get; set; }
    }
}
