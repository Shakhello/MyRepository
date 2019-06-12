namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DictionaryLink_TicketLink_RemovedExtraColumn : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DictionaryLink", new[] { "FieldId", "DocId", "DictionaryId" });
            DropIndex("dbo.TicketLink", new[] { "FieldId", "DocId", "TemplateId" });
            CreateIndex("dbo.DictionaryLink", new[] { "FieldId", "DocId" });
            CreateIndex("dbo.TicketLink", new[] { "FieldId", "DocId" });
            DropColumn("dbo.DictionaryLink", "DictionaryId");
            DropColumn("dbo.TicketLink", "TemplateId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TicketLink", "TemplateId", c => c.Int(nullable: false));
            AddColumn("dbo.DictionaryLink", "DictionaryId", c => c.Int(nullable: false));
            DropIndex("dbo.TicketLink", new[] { "FieldId", "DocId" });
            DropIndex("dbo.DictionaryLink", new[] { "FieldId", "DocId" });
            CreateIndex("dbo.TicketLink", new[] { "FieldId", "DocId", "TemplateId" });
            CreateIndex("dbo.DictionaryLink", new[] { "FieldId", "DocId", "DictionaryId" });
        }
    }
}
