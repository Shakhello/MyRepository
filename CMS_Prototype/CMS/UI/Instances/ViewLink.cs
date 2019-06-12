using CMS.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.UI
{
    public class ViewLink : Instance
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public override DefinitionType Type => DefinitionType.ViewLink;

        [JsonConverter(typeof(StringEnumConverter))]
        public ViewLinkType ViewLinkType { get; set; }

        public List<Event> Events { get; set; } = new List<Event>();


    }
}
