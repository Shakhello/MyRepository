using CMS.Behaviours;
using CMS.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace CMS.UI
{
    public abstract class Definition
    {
        public int Id { get; set; }

        internal Entity Entity { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DefinitionType Type { get; set; }

    }
}
