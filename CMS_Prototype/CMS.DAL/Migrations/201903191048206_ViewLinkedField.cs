namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ViewLinkedField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.View", "LinkedFieldId", c => c.Int());
            CreateIndex("dbo.View", "LinkedFieldId");
            AddForeignKey("dbo.View", "LinkedFieldId", "dbo.Field", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.View", "LinkedFieldId", "dbo.Field");
            DropIndex("dbo.View", new[] { "LinkedFieldId" });
            DropColumn("dbo.View", "LinkedFieldId");
        }
    }
}
