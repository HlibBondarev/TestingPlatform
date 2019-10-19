namespace SiteWithAuthentication.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SubscrimentForModerators", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.Subscriments", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Tests", "SubscrimentId", "dbo.Subscriments");
            DropForeignKey("dbo.Subscriments", "UserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.SubscrimentForModerators", new[] { "UserProfileId" });
            DropIndex("dbo.Subscriments", new[] { "UserProfileId" });
            DropIndex("dbo.Subscriments", new[] { "CourseId" });
            DropIndex("dbo.Tests", new[] { "SubscrimentId" });
            CreateTable(
                "dbo.SubscriptionForModerators",
                c => new
                    {
                        SubscriptionForModeratorId = c.Int(nullable: false, identity: true),
                        UserProfileId = c.String(maxLength: 128),
                        StartDate = c.DateTime(nullable: false),
                        SubscrimentPeriod = c.Int(nullable: false),
                        CourseCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SubscriptionForModeratorId)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId)
                .Index(t => t.UserProfileId);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        SubscriptionId = c.Int(nullable: false, identity: true),
                        UserProfileId = c.String(maxLength: 128),
                        CourseId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        SubscrimentPeriod = c.Int(nullable: false),
                        IsApproved = c.Boolean(),
                    })
                .PrimaryKey(t => t.SubscriptionId)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId)
                .Index(t => t.UserProfileId)
                .Index(t => t.CourseId);
            
            AddColumn("dbo.Tests", "Subscription_SubscriptionId", c => c.Int());
            CreateIndex("dbo.Tests", "Subscription_SubscriptionId");
            AddForeignKey("dbo.Tests", "Subscription_SubscriptionId", "dbo.Subscriptions", "SubscriptionId");
            DropTable("dbo.SubscrimentForModerators");
            DropTable("dbo.Subscriments");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Subscriments",
                c => new
                    {
                        SubscrimentId = c.Int(nullable: false, identity: true),
                        UserProfileId = c.String(maxLength: 128),
                        CourseId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        SubscrimentPeriod = c.Int(nullable: false),
                        IsApproved = c.Boolean(),
                    })
                .PrimaryKey(t => t.SubscrimentId);
            
            CreateTable(
                "dbo.SubscrimentForModerators",
                c => new
                    {
                        SubscrimentForModeratorId = c.Int(nullable: false, identity: true),
                        UserProfileId = c.String(maxLength: 128),
                        StartDate = c.DateTime(nullable: false),
                        SubscrimentPeriod = c.Int(nullable: false),
                        CourseCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SubscrimentForModeratorId);
            
            DropForeignKey("dbo.Subscriptions", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.Tests", "Subscription_SubscriptionId", "dbo.Subscriptions");
            DropForeignKey("dbo.Subscriptions", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.SubscriptionForModerators", "UserProfileId", "dbo.UserProfiles");
            DropIndex("dbo.Tests", new[] { "Subscription_SubscriptionId" });
            DropIndex("dbo.Subscriptions", new[] { "CourseId" });
            DropIndex("dbo.Subscriptions", new[] { "UserProfileId" });
            DropIndex("dbo.SubscriptionForModerators", new[] { "UserProfileId" });
            DropColumn("dbo.Tests", "Subscription_SubscriptionId");
            DropTable("dbo.Subscriptions");
            DropTable("dbo.SubscriptionForModerators");
            CreateIndex("dbo.Tests", "SubscrimentId");
            CreateIndex("dbo.Subscriments", "CourseId");
            CreateIndex("dbo.Subscriments", "UserProfileId");
            CreateIndex("dbo.SubscrimentForModerators", "UserProfileId");
            AddForeignKey("dbo.Subscriments", "UserProfileId", "dbo.UserProfiles", "UserProfileId");
            AddForeignKey("dbo.Tests", "SubscrimentId", "dbo.Subscriments", "SubscrimentId", cascadeDelete: true);
            AddForeignKey("dbo.Subscriments", "CourseId", "dbo.Courses", "CourseId", cascadeDelete: true);
            AddForeignKey("dbo.SubscrimentForModerators", "UserProfileId", "dbo.UserProfiles", "UserProfileId");
        }
    }
}
