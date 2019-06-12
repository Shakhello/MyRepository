namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Parameter_Additional_Props : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parameter", "Name", c => c.String());
            AddColumn("dbo.Parameter", "ControlId", c => c.Int());
            CreateIndex("dbo.Parameter", "ControlId");
            AddForeignKey("dbo.Parameter", "ControlId", "dbo.Control", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Parameter", "ControlId", "dbo.Control");
            DropIndex("dbo.Parameter", new[] { "ControlId" });
            DropColumn("dbo.Parameter", "ControlId");
            DropColumn("dbo.Parameter", "Name");
        }
    }
}
