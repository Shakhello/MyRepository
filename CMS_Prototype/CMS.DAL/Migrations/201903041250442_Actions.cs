namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Actions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Event",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ControlId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Control", t => t.ControlId, cascadeDelete: true)
                .Index(t => t.ControlId);
            
            CreateTable(
                "dbo.Action",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        ActionType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Event", t => t.EventId, cascadeDelete: true)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Parameter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ActionId = c.Int(nullable: false),
                        FieldId = c.Int(),
                        DefaultValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Field", t => t.FieldId)
                .ForeignKey("dbo.Action", t => t.ActionId, cascadeDelete: true)
                .Index(t => t.ActionId)
                .Index(t => t.FieldId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Event", "ControlId", "dbo.Control");
            DropForeignKey("dbo.Action", "EventId", "dbo.Event");
            DropForeignKey("dbo.Parameter", "ActionId", "dbo.Action");
            DropForeignKey("dbo.Parameter", "FieldId", "dbo.Field");
            DropIndex("dbo.Parameter", new[] { "FieldId" });
            DropIndex("dbo.Parameter", new[] { "ActionId" });
            DropIndex("dbo.Action", new[] { "EventId" });
            DropIndex("dbo.Event", new[] { "ControlId" });
            DropTable("dbo.Parameter");
            DropTable("dbo.Action");
            DropTable("dbo.Event");
        }
    }
}
