namespace SiteWithAuthentication.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestResultDetails",
                c => new
                    {
                        TestResultDetailId = c.Int(nullable: false, identity: true),
                        TestResultId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        IsProperAnswer = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TestResultDetailId)
                .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
                .ForeignKey("dbo.TestResults", t => t.TestResultId, cascadeDelete: true)
                .Index(t => t.TestResultId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.TestResults",
                c => new
                    {
                        TestResultId = c.Int(nullable: false, identity: true),
                        UserProfileId = c.String(maxLength: 128),
                        TestDate = c.DateTime(nullable: false),
                        Result = c.Int(nullable: false),
                        IsPassedTest = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TestResultId)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId)
                .Index(t => t.UserProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestResults", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.TestResultDetails", "TestResultId", "dbo.TestResults");
            DropForeignKey("dbo.TestResultDetails", "QuestionId", "dbo.Questions");
            DropIndex("dbo.TestResults", new[] { "UserProfileId" });
            DropIndex("dbo.TestResultDetails", new[] { "QuestionId" });
            DropIndex("dbo.TestResultDetails", new[] { "TestResultId" });
            DropTable("dbo.TestResults");
            DropTable("dbo.TestResultDetails");
        }
    }
}
