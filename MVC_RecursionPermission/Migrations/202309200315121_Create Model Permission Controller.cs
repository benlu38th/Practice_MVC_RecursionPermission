namespace MVC_RecursionPermission.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateModelPermissionController : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Permissions", "ControllerName", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Permissions", "ControllerName");
        }
    }
}
