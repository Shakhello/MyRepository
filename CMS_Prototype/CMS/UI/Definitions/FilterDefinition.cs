using CMS.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace CMS.UI
{
    public class FilterDefinition : Definition
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DAL.Models.FilterType FilterType { get; set; }

        public int ViewId { get; set; }

        public string DisplayName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OperationType Operation { get; set; }

        public string DefaultValue { get; set; }

        public int Order { get; set; }

        public int Width { get; set; }

        public virtual List<FilterFieldDefinition> FilterFields { get; set; } = new List<FilterFieldDefinition>();

        public virtual List<FieldDefinition> Fields { get; set; } = new List<FieldDefinition>();

        
    }


}
