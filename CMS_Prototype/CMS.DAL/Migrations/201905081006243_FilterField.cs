namespace CMS.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FilterField : DbMigration
    {
        public override void Up()
        {
            // *************** NOT AUTO-GENERATED

            DropForeignKey("dbo.FilterField", "Field_Id", "dbo.Field");
            DropForeignKey("dbo.FilterField", "Filter_Id", "dbo.Filter");

            DropIndex("dbo.FilterField", new[] { "Field_Id" });
            DropIndex("dbo.FilterField", new[] { "Filter_Id" });

            DropTable("dbo.FilterField");

            // ***************


            DropForeignKey("dbo.FilterField", "Filter_Id", "dbo.Filter");
            DropForeignKey("dbo.FilterField", "Field_Id", "dbo.Field");
            DropIndex("dbo.FilterField", new[] { "Filter_Id" });
            DropIndex("dbo.FilterField", new[] { "Field_Id" });
            CreateTable(
                "dbo.FilterField",
                c => new
                    {
                        FilterId = c.Int(nullable: false),
                        FieldId = c.Int(nullable: false),
                        Depth = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FilterId, t.FieldId })
                .ForeignKey("dbo.Field", t => t.FieldId, cascadeDelete: true)
                .ForeignKey("dbo.Filter", t => t.FilterId, cascadeDelete: true)
                .Index(t => t.FilterId)
                .Index(t => t.FieldId);
            
            //DropTable("dbo.FilterField");
        }
        
        public override void Down()
        {
            // *************** NOT AUTO-GENERATED

            CreateTable(
                "dbo.FilterField",
                c => new
                {
                    Filter_Id = c.Int(nullable: false),
                    Field_Id = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.Filter_Id, t.Field_Id })
                .ForeignKey("dbo.Filter", t => t.Filter_Id, cascadeDelete: true)
                .ForeignKey("dbo.Field", t => t.Field_Id, cascadeDelete: true)
                .Index(t => t.Filter_Id)
                .Index(t => t.Field_Id);

            // ***************

            CreateTable(
                "dbo.FilterField",
                c => new
                    {
                        Filter_Id = c.Int(nullable: false),
                        Field_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Filter_Id, t.Field_Id });
            
            DropForeignKey("dbo.FilterField", "FilterId", "dbo.Filter");
            DropForeignKey("dbo.FilterField", "FieldId", "dbo.Field");
            DropIndex("dbo.FilterField", new[] { "FieldId" });
            DropIndex("dbo.FilterField", new[] { "FilterId" });
            DropTable("dbo.FilterField");
            CreateIndex("dbo.FilterField", "Field_Id");
            CreateIndex("dbo.FilterField", "Filter_Id");
            AddForeignKey("dbo.FilterField", "Field_Id", "dbo.Field", "Id", cascadeDelete: true);
            AddForeignKey("dbo.FilterField", "Filter_Id", "dbo.Filter", "Id", cascadeDelete: true);
        }
    }
}
