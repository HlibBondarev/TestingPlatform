namespace SiteWithAuthentication.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tests", "Subscription_SubscriptionId", "dbo.Subscriptions");
            DropForeignKey("dbo.TestResults", "TestId", "dbo.Tests");
            DropForeignKey("dbo.Tests", "TopicId", "dbo.Topics");
            DropIndex("dbo.Tests", new[] { "TopicId" });
            DropIndex("dbo.Tests", new[] { "Subscription_SubscriptionId" });
            DropIndex("dbo.TestResults", new[] { "TestId" });
            DropColumn("dbo.UserProfiles", "FirstName");
            DropColumn("dbo.UserProfiles", "LastName");
            DropColumn("dbo.UserProfiles", "FullName");
            DropColumn("dbo.UserProfiles", "Discriminator");
            DropTable("dbo.Tests");
            DropTable("dbo.TestResults");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TestResults",
                c => new
                    {
                        TestResultId = c.Int(nullable: false, identity: true),
                        TestId = c.Int(nullable: false),
                        IsProperAnswer = c.Boolean(),
                    })
                .PrimaryKey(t => t.TestResultId);
            
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        TestId = c.Int(nullable: false, identity: true),
                        SubscrimentId = c.Int(nullable: false),
                        TestDateTime = c.DateTime(nullable: false),
                        TopicId = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Subscription_SubscriptionId = c.Int(),
                    })
                .PrimaryKey(t => t.TestId);
            
            AddColumn("dbo.UserProfiles", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.UserProfiles", "FullName", c => c.String());
            AddColumn("dbo.UserProfiles", "LastName", c => c.String());
            AddColumn("dbo.UserProfiles", "FirstName", c => c.String());
            CreateIndex("dbo.TestResults", "TestId");
            CreateIndex("dbo.Tests", "Subscription_SubscriptionId");
            CreateIndex("dbo.Tests", "TopicId");
            AddForeignKey("dbo.Tests", "TopicId", "dbo.Topics", "TopicId", cascadeDelete: true);
            AddForeignKey("dbo.TestResults", "TestId", "dbo.Tests", "TestId", cascadeDelete: true);
            AddForeignKey("dbo.Tests", "Subscription_SubscriptionId", "dbo.Subscriptions", "SubscriptionId");
        }
    }
}
