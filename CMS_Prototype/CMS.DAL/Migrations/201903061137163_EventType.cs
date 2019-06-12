namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Event", "EventType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Event", "EventType");
        }
    }
}
