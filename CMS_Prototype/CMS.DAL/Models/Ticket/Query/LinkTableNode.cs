namespace CMS.DAL.Models
{
    internal class LinkTableNode : TableNode
    {
        public override string TableName { get => "TicketLink"; }

        public int FieldId { get; set; }

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != typeof(LinkTableNode))
                return false;

            return ((LinkTableNode)other).FieldId == FieldId;
        }

        public override int GetHashCode()
        {
            return FieldId.GetHashCode();
        }
    }
}
