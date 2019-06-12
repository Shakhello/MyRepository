using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace CMS.UI
{
    public class SectionDefinition : Definition
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public virtual List<ViewDefinition> Views { get; set; } = new List<ViewDefinition>();
        
    }


}
