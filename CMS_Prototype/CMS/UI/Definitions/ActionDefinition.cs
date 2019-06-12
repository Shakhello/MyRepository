using CMS.Behaviours;
using CMS.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace CMS.UI
{
    public class ActionDefinition : Definition
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DAL.Models.ActionType ActionType { get; set; }

        public int EventId { get; set; }

        public int Order { get; set; }

        public virtual List<ParameterDefinition> Parameters { get; set; } = new List<ParameterDefinition>();
   
    }
}
