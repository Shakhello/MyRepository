using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.UI
{
    public class PermissionDefinition : Definition
    {
        public int RoleId { get; set; }

        public int? ViewId { get; set; }

        public string ViewDisplayName { get; set; }

        public int? DictionaryId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PermissionType PermissionType { get; set; }

    }
}
