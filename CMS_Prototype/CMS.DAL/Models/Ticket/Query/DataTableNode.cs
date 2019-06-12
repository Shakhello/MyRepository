using CMS.DAL.Common;
using Common.Exceptions;
using System.Collections.Generic;

namespace CMS.DAL.Models
{
    internal class DataTableNode : TableNode
    {
        public int TemplateId { get; set; }

        public string TemplateName { get; set; }

        public override string TableName { get => Constants.DATA_TABLE_PREFIX + TemplateName; }

        public List<Field> DisplayFields { get; set; } = new List<Field>();

        public List<Field> SortFields { get; set; } = new List<Field>();

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != typeof(DataTableNode))
                return false;

            var dataNode = (DataTableNode)other;

            if (dataNode.Parent != null && !(dataNode.Parent is LinkTableNode))
                throw new CustomValidationException("data_Table can't have another data_Table as a parent.");

            return dataNode.TableName == TableName && ((LinkTableNode)dataNode.Parent)?.FieldId == ((LinkTableNode)Parent)?.FieldId;
        }

        public override int GetHashCode()
        {
            unchecked
            { 
                int hash = 27;
                hash = (13 * hash) + TableName?.GetHashCode() ?? 0;
                hash = (13 * hash) + ((LinkTableNode)Parent)?.FieldId.GetHashCode() ?? 0;

                return hash;
            }

        }
    }
}
