namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FieldDataType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Filter", "DataType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Filter", "DataType", c => c.String(maxLength: 50));
        }
    }
}
