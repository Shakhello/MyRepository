namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicketLink2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Field", "ReferenceTemplateId", "dbo.Template");
            DropIndex("dbo.Field", new[] { "ReferenceTemplateId" });
            CreateTable(
                "dbo.TicketLink",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FieldId = c.Int(nullable: false),
                        DocId1 = c.Int(nullable: false),
                        DocId2 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.FieldId, t.DocId1 });
            
            AddColumn("dbo.Field", "LinkedFieldId", c => c.Int());
            CreateIndex("dbo.Field", "LinkedFieldId");
            AddForeignKey("dbo.Field", "LinkedFieldId", "dbo.Field", "Id");
            DropColumn("dbo.Field", "ReferenceTemplateId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Field", "ReferenceTemplateId", c => c.Int());
            DropForeignKey("dbo.Field", "LinkedFieldId", "dbo.Field");
            DropIndex("dbo.TicketLink", new[] { "FieldId", "DocId1" });
            DropIndex("dbo.Field", new[] { "LinkedFieldId" });
            DropColumn("dbo.Field", "LinkedFieldId");
            DropTable("dbo.TicketLink");
            CreateIndex("dbo.Field", "ReferenceTemplateId");
            AddForeignKey("dbo.Field", "ReferenceTemplateId", "dbo.Template", "Id");
        }
    }
}
