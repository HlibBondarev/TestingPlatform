namespace SiteWithAuthentication.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Subscriptions", "IsApproved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Subscriptions", "IsApproved", c => c.Boolean());
        }
    }
}
