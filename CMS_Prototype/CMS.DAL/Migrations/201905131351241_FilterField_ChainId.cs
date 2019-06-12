namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FilterField_ChainId : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.FilterField");
            AddColumn("dbo.FilterField", "ChainId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.FilterField", new[] { "FilterId", "FieldId", "ChainId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.FilterField");
            DropColumn("dbo.FilterField", "ChainId");
            AddPrimaryKey("dbo.FilterField", new[] { "FilterId", "FieldId" });
        }
    }
}
