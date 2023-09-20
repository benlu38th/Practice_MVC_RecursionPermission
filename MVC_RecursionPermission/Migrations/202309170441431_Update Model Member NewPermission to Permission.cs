namespace MVC_RecursionPermission.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModelMemberNewPermissiontoPermission : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "Permission", c => c.String(maxLength: 500));
            DropColumn("dbo.Members", "NewPermission");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Members", "NewPermission", c => c.String(maxLength: 500));
            DropColumn("dbo.Members", "Permission");
        }
    }
}
