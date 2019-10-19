namespace SiteWithAuthentication.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubscriptionForModerators", "SubscriptionPeriod", c => c.Int(nullable: false));
            AddColumn("dbo.Subscriptions", "SubscriptionPeriod", c => c.Int(nullable: false));
            DropColumn("dbo.SubscriptionForModerators", "SubscrimentPeriod");
            DropColumn("dbo.Subscriptions", "SubscrimentPeriod");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Subscriptions", "SubscrimentPeriod", c => c.Int(nullable: false));
            AddColumn("dbo.SubscriptionForModerators", "SubscrimentPeriod", c => c.Int(nullable: false));
            DropColumn("dbo.Subscriptions", "SubscriptionPeriod");
            DropColumn("dbo.SubscriptionForModerators", "SubscriptionPeriod");
        }
    }
}
