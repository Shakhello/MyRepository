namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeFilterFieldsNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Filter", "Operation", c => c.Int());
            AlterColumn("dbo.Filter", "FilterType", c => c.Int());
            AlterColumn("dbo.Filter", "DataType", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Filter", "DataType", c => c.Int(nullable: false));
            AlterColumn("dbo.Filter", "FilterType", c => c.Int(nullable: false));
            AlterColumn("dbo.Filter", "Operation", c => c.Int(nullable: false));
        }
    }
}
