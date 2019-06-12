namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ControlFields : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Control", "FieldId", "dbo.Field");
            DropIndex("dbo.Control", new[] { "FieldId" });
            CreateTable(
                "dbo.ControlField",
                c => new
                    {
                        ControlId = c.Int(nullable: false),
                        FieldId = c.Int(nullable: false),
                        Depth = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ControlId, t.FieldId })
                .ForeignKey("dbo.Control", t => t.ControlId, cascadeDelete: true)
                .ForeignKey("dbo.Field", t => t.FieldId, cascadeDelete: true)
                .Index(t => t.ControlId)
                .Index(t => t.FieldId);
            
            DropColumn("dbo.Control", "FieldId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Control", "FieldId", c => c.Int());
            DropForeignKey("dbo.ControlField", "FieldId", "dbo.Field");
            DropForeignKey("dbo.ControlField", "ControlId", "dbo.Control");
            DropIndex("dbo.ControlField", new[] { "FieldId" });
            DropIndex("dbo.ControlField", new[] { "ControlId" });
            DropTable("dbo.ControlField");
            CreateIndex("dbo.Control", "FieldId");
            AddForeignKey("dbo.Control", "FieldId", "dbo.Field", "Id", cascadeDelete: true);
        }
    }
}
