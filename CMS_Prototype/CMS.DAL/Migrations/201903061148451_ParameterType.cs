namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParameterType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parameter", "ParameterType", c => c.Int(nullable: false));
            DropColumn("dbo.Parameter", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Parameter", "Name", c => c.String());
            DropColumn("dbo.Parameter", "ParameterType");
        }
    }
}
