namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DictionaryLink_TicketLink_Indices : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.DictionaryLink", new[] { "FieldId", "DocId" });
            CreateIndex("dbo.TicketLink", new[] { "FieldId", "DocId" });
        }
        
        public override void Down()
        {
            DropIndex("dbo.TicketLink", new[] { "FieldId", "DocId" });
            DropIndex("dbo.DictionaryLink", new[] { "FieldId", "DocId" });
        }
    }
}
