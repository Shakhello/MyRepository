using CMS.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace CMS.UI
{
    public class ControlDefinition : Definition
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DAL.Models.ControlType ControlType { get; set; }

        public int? StyleId { get; set; }

        public int ViewId { get; set; }

        public int? FieldId { get; }

        public string DisplayName { get; set; }

        public bool ShowDisplayName { get; set; }

        public int OrderIndex { get; set; }

        public string DefaultValue { get; set; }

        public bool Required { get; set; }

        public string Pattern { get; set; }

        public int GridIndex { get; set; }

        public int? MaxLength { get; set; }

        public int? Width { get; set; }

        public StyleDefinition Style { get; set; }

        public virtual List<EventDefinition> Events { get; set; } = new List<EventDefinition>();

        public virtual List<ControlFieldDefinition> ControlFields { get; set; } = new List<ControlFieldDefinition>();
    }


}
