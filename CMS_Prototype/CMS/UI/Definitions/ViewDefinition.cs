using CMS.Behaviours;
using CMS.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace CMS.UI
{
    public class ViewDefinition : Definition
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DAL.Models.ViewType ViewType { get; set; }

        public int? TemplateId { get; set; }

        public int? ParentViewId { get; set; }

        public int? SectionId { get; set; }

        public int? StyleId { get; set; }

        public int? LinkedFieldId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public int OrderIndex { get; set; }

        public int GridWidth { get; set; }

        public StyleDefinition Style { get; set; }

        public virtual List<ViewDefinition> ChildViews { get; set; } = new List<ViewDefinition>();

        public virtual List<ControlDefinition> Controls { get; set; } = new List<ControlDefinition>();

        public virtual List<FilterDefinition> Filters { get; set; } = new List<FilterDefinition>();

        public virtual List<EventDefinition> Events { get; set; } = new List<EventDefinition>();

        
    }

}
