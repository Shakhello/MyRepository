namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FilterDeleteTwoFields : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Filter", "Static");
            DropColumn("dbo.Filter", "DataType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Filter", "DataType", c => c.Int());
            AddColumn("dbo.Filter", "Static", c => c.Boolean(nullable: false));
        }
    }
}
