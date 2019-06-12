using CMS.Behaviours;
using System.Collections.Generic;
using System.Reflection;

namespace CMS.UI
{
    public abstract class Instance
    {
        public abstract DefinitionType Type { get; }

        public bool Virtual { get; set; }

        public int? DocId { get; set; }

        public Dictionary<string, object> Props { get; set; } = new Dictionary<string, object>();

        internal Instance ParentNode { get; set; }

        internal Definition Definition { get; set; }
        
    }
}
