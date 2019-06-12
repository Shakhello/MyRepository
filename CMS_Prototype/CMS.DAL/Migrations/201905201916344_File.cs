namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class File : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileLink",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FieldId = c.Int(nullable: false),
                        DocId = c.Int(nullable: false),
                        FileId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.File",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreateDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Name = c.String(),
                        ContentType = c.String(),
                        Comment = c.String(),
                        ContentData = c.Binary(),
                        Deleted = c.Boolean(nullable: false),
                        DeleteDate = c.DateTime(),
                        DeletedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.File");
            DropTable("dbo.FileLink");
        }
    }
}
