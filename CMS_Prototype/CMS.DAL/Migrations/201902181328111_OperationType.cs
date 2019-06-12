namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OperationType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Field", "IsSystem", c => c.Boolean(nullable: false));
            AddColumn("dbo.View", "TemplateId", c => c.Int());
            AlterColumn("dbo.Dictionary", "DictionaryType", c => c.Int(nullable: false));
            AlterColumn("dbo.Filter", "Operation", c => c.Int(nullable: false));
            CreateIndex("dbo.View", "TemplateId");
            AddForeignKey("dbo.View", "TemplateId", "dbo.Template", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.View", "TemplateId", "dbo.Template");
            DropIndex("dbo.View", new[] { "TemplateId" });
            AlterColumn("dbo.Filter", "Operation", c => c.String(maxLength: 30));
            AlterColumn("dbo.Dictionary", "DictionaryType", c => c.String());
            DropColumn("dbo.View", "TemplateId");
            DropColumn("dbo.Field", "IsSystem");
        }
    }
}
