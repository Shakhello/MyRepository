using System.Collections.Generic;

namespace CMS.DAL.Models
{
    internal class LinkDictionaryNode : TableNode
    {
        public override string TableName { get => "DictionaryLink"; }

        public int FieldId { get; set; }

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != typeof(LinkDictionaryNode))
                return false;

            return ((LinkDictionaryNode)other).FieldId == FieldId;
        }

        public override int GetHashCode()
        {
            return FieldId.GetHashCode();
        }
    }
}
