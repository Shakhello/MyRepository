using CMS.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CMS.UI
{
    public class DictionaryDefinition : Definition
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DAL.Models.DictionaryType DictionaryType { get; set; }

        public string Name { get; set; }
        
    }
}
