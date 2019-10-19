namespace SiteWithAuthentication.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestResults", "MaxScore", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestResults", "MaxScore");
        }
    }
}
