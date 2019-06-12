namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersRolesPermissions : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TicketLink", new[] { "FieldId", "DocId" });
            CreateTable(
                "dbo.Permission",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        ViewId = c.Int(),
                        PermissionType = c.Int(nullable: false),
                        Dictionary_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.View", t => t.ViewId, cascadeDelete: true)
                .ForeignKey("dbo.Dictionary", t => t.Dictionary_Id)
                .Index(t => t.RoleId)
                .Index(t => t.ViewId)
                .Index(t => t.Dictionary_Id);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        DisplayName = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        RoleId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        UserCanChangeRole = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Login, unique: true);
            
            AlterColumn("dbo.Style", "BorderWidth", c => c.String(maxLength: 100));
            AlterColumn("dbo.Style", "TextWeight", c => c.String(maxLength: 100));
            DropTable("dbo.TicketLink");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TicketLink",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FieldId = c.Int(nullable: false),
                        DocId = c.Int(nullable: false),
                        LinkedTicketId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Permission", "Dictionary_Id", "dbo.Dictionary");
            DropForeignKey("dbo.Permission", "ViewId", "dbo.View");
            DropForeignKey("dbo.UserRole", "UserId", "dbo.User");
            DropForeignKey("dbo.UserRole", "RoleId", "dbo.Role");
            DropForeignKey("dbo.Permission", "RoleId", "dbo.Role");
            DropIndex("dbo.User", new[] { "Login" });
            DropIndex("dbo.UserRole", new[] { "UserId" });
            DropIndex("dbo.UserRole", new[] { "RoleId" });
            DropIndex("dbo.Permission", new[] { "Dictionary_Id" });
            DropIndex("dbo.Permission", new[] { "ViewId" });
            DropIndex("dbo.Permission", new[] { "RoleId" });
            AlterColumn("dbo.Style", "TextWeight", c => c.Int());
            AlterColumn("dbo.Style", "BorderWidth", c => c.Int());
            DropTable("dbo.User");
            DropTable("dbo.UserRole");
            DropTable("dbo.Role");
            DropTable("dbo.Permission");
            CreateIndex("dbo.TicketLink", new[] { "FieldId", "DocId" });
        }
    }
}
