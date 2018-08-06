namespace DUE_Mernokinfo_Bot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DueDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Data",
                c => new
                    {
                        EventId = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        SubjectCode = c.String(),
                        ClassCode = c.String(),
                        ZH = c.Boolean(nullable: false),
                        CanBeWrite = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EventId);
            
            CreateTable(
                "dbo.UserEnrolleds",
                c => new
                    {
                        UserEnrolledId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        EventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserEnrolledId)
                .ForeignKey("dbo.Data", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        NeptunCode = c.String(),
                        Name = c.String(),
                        ChatId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserEnrolleds", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserEnrolleds", "EventId", "dbo.Data");
            DropIndex("dbo.UserEnrolleds", new[] { "EventId" });
            DropIndex("dbo.UserEnrolleds", new[] { "UserId" });
            DropTable("dbo.Users");
            DropTable("dbo.UserEnrolleds");
            DropTable("dbo.Data");
        }
    }
}
