namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParameterOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parameter", "Order", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Parameter", "Order");
        }
    }
}
