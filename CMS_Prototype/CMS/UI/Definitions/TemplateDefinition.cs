using CMS.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace CMS.UI
{
    public class TemplateDefinition : Definition
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DAL.Models.TemplateType TemplateType { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public List<FieldDefinition> Fields { get; set; } = new List<FieldDefinition>();

        public List<FieldDefinition> ReferringFields { get; set; } = new List<FieldDefinition>();

    }


}
