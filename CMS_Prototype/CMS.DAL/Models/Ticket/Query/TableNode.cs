using CMS.DAL.Interfaces;
using System.Collections.Generic;

namespace CMS.DAL.Models
{
    internal abstract class TableNode
    {
        public bool IsHead { get; set; }

        public abstract string TableName { get; }

        public TableNode Parent { get; set; }

        public List<TableNode> Children { get; set; } = new List<TableNode>();

        public Dictionary<IFilter, List<Field>> Conditions { get; set; } = new Dictionary<IFilter, List<Field>>();

        public string Alias { get; set; }
    }
}
