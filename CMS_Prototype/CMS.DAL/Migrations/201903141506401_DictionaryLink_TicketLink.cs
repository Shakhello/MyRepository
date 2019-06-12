namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DictionaryLink_TicketLink : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DictionaryLink",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FieldId = c.Int(nullable: false),
                        DocId = c.Int(nullable: false),
                        DictionaryId = c.Int(nullable: false),
                        DictionaryKeyInt = c.Int(),
                        DictionaryKeyString = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.FieldId, t.DocId, t.DictionaryId });
            
            CreateTable(
                "dbo.TicketLink",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FieldId = c.Int(nullable: false),
                        DocId = c.Int(nullable: false),
                        TemplateId = c.Int(nullable: false),
                        LinkedTicketId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.FieldId, t.DocId, t.TemplateId });
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TicketLink", new[] { "FieldId", "DocId", "TemplateId" });
            DropIndex("dbo.DictionaryLink", new[] { "FieldId", "DocId", "DictionaryId" });
            DropTable("dbo.TicketLink");
            DropTable("dbo.DictionaryLink");
        }
    }
}
