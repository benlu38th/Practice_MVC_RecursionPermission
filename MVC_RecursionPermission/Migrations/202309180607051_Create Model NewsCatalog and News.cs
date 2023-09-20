namespace MVC_RecursionPermission.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateModelNewsCatalogandNews : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.News",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 20),
                        Description = c.String(nullable: false, maxLength: 100),
                        NewsCatalogId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NewsCatalogs", t => t.NewsCatalogId)
                .Index(t => t.NewsCatalogId);
            
            CreateTable(
                "dbo.NewsCatalogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Catalos = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.News", "NewsCatalogId", "dbo.NewsCatalogs");
            DropIndex("dbo.News", new[] { "NewsCatalogId" });
            DropTable("dbo.NewsCatalogs");
            DropTable("dbo.News");
        }
    }
}
