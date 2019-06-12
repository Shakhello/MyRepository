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
    public class EventDefinition : Definition
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DAL.Models.EventType EventType { get; set; }

        public int ControlId { get; set; }

        public virtual List<ActionDefinition> Actions { get; set; } = new List<ActionDefinition>();

        
    }
}
