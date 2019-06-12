using CMS.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CMS.UI
{
    public class FieldDefinition : Definition
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DAL.Models.FieldType FieldType { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public int TemplateId { get; set; }

        public int? LinkedFieldId { get; set; }

        public int? LinkedTemplateId { get; set; }

        public int? DictionaryId { get; set; }

        public int? Length { get; set; }

        
    }

}
