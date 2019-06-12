namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Control",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StyleId = c.Int(),
                        ViewId = c.Int(nullable: false),
                        FieldId = c.Int(),
                        DisplayName = c.String(maxLength: 100),
                        ShowDisplayName = c.Boolean(nullable: false),
                        OrderIndex = c.Int(nullable: false),
                        ControlType = c.Int(nullable: false),
                        DefaultValue = c.String(),
                        Required = c.Boolean(nullable: false),
                        Pattern = c.String(),
                        GridIndex = c.Int(nullable: false),
                        MaxLength = c.Int(),
                        Width = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Field", t => t.FieldId, cascadeDelete: true)
                .ForeignKey("dbo.View", t => t.ViewId, cascadeDelete: true)
                .ForeignKey("dbo.Style", t => t.StyleId)
                .Index(t => t.StyleId)
                .Index(t => t.ViewId)
                .Index(t => t.FieldId);
            
            CreateTable(
                "dbo.Field",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TemplateId = c.Int(nullable: false),
                        ReferenceTemplateId = c.Int(),
                        DictionaryId = c.Int(),
                        Name = c.String(maxLength: 100),
                        DisplayName = c.String(maxLength: 100),
                        Length = c.Int(),
                        FieldType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dictionary", t => t.DictionaryId)
                .ForeignKey("dbo.Template", t => t.TemplateId, cascadeDelete: true)
                .ForeignKey("dbo.Template", t => t.ReferenceTemplateId)
                .Index(t => t.TemplateId)
                .Index(t => t.ReferenceTemplateId)
                .Index(t => t.DictionaryId);
            
            CreateTable(
                "dbo.Dictionary",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DictionaryType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Filter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ViewId = c.Int(nullable: false),
                        DisplayName = c.String(maxLength: 100),
                        Static = c.Boolean(nullable: false),
                        Operation = c.String(maxLength: 30),
                        FilterType = c.Int(nullable: false),
                        DefaultValue = c.String(),
                        DataType = c.String(maxLength: 50),
                        Order = c.Int(),
                        Width = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.View", t => t.ViewId, cascadeDelete: true)
                .Index(t => t.ViewId);
            
            CreateTable(
                "dbo.View",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentViewId = c.Int(),
                        SectionId = c.Int(),
                        StyleId = c.Int(),
                        Name = c.String(maxLength: 100),
                        DisplayName = c.String(maxLength: 100),
                        OrderIndex = c.Int(nullable: false),
                        ViewType = c.Int(nullable: false),
                        GridWidth = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.View", t => t.ParentViewId)
                .ForeignKey("dbo.Section", t => t.SectionId)
                .ForeignKey("dbo.Style", t => t.StyleId)
                .Index(t => t.ParentViewId)
                .Index(t => t.SectionId)
                .Index(t => t.StyleId);
            
            CreateTable(
                "dbo.Section",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        DisplayName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Style",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BorderWidth = c.Int(),
                        BorderColor = c.String(maxLength: 100),
                        BackgroundColor = c.String(maxLength: 100),
                        TextColor = c.String(maxLength: 100),
                        TextWeight = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Template",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        DisplayName = c.String(maxLength: 100),
                        TemplateType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FilterField",
                c => new
                    {
                        Filter_Id = c.Int(nullable: false),
                        Field_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Filter_Id, t.Field_Id })
                .ForeignKey("dbo.Filter", t => t.Filter_Id, cascadeDelete: true)
                .ForeignKey("dbo.Field", t => t.Field_Id, cascadeDelete: true)
                .Index(t => t.Filter_Id)
                .Index(t => t.Field_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Field", "ReferenceTemplateId", "dbo.Template");
            DropForeignKey("dbo.Field", "TemplateId", "dbo.Template");
            DropForeignKey("dbo.Filter", "ViewId", "dbo.View");
            DropForeignKey("dbo.View", "StyleId", "dbo.Style");
            DropForeignKey("dbo.Control", "StyleId", "dbo.Style");
            DropForeignKey("dbo.View", "SectionId", "dbo.Section");
            DropForeignKey("dbo.Control", "ViewId", "dbo.View");
            DropForeignKey("dbo.View", "ParentViewId", "dbo.View");
            DropForeignKey("dbo.FilterField", "Field_Id", "dbo.Field");
            DropForeignKey("dbo.FilterField", "Filter_Id", "dbo.Filter");
            DropForeignKey("dbo.Field", "DictionaryId", "dbo.Dictionary");
            DropForeignKey("dbo.Control", "FieldId", "dbo.Field");
            DropIndex("dbo.FilterField", new[] { "Field_Id" });
            DropIndex("dbo.FilterField", new[] { "Filter_Id" });
            DropIndex("dbo.View", new[] { "StyleId" });
            DropIndex("dbo.View", new[] { "SectionId" });
            DropIndex("dbo.View", new[] { "ParentViewId" });
            DropIndex("dbo.Filter", new[] { "ViewId" });
            DropIndex("dbo.Field", new[] { "DictionaryId" });
            DropIndex("dbo.Field", new[] { "ReferenceTemplateId" });
            DropIndex("dbo.Field", new[] { "TemplateId" });
            DropIndex("dbo.Control", new[] { "FieldId" });
            DropIndex("dbo.Control", new[] { "ViewId" });
            DropIndex("dbo.Control", new[] { "StyleId" });
            DropTable("dbo.FilterField");
            DropTable("dbo.Template");
            DropTable("dbo.Style");
            DropTable("dbo.Section");
            DropTable("dbo.View");
            DropTable("dbo.Filter");
            DropTable("dbo.Dictionary");
            DropTable("dbo.Field");
            DropTable("dbo.Control");
        }
    }
}
