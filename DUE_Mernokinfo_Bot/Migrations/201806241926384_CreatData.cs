namespace DUE_Mernokinfo_Bot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Data",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        SubjectCode = c.String(),
                        ClassCode = c.String(),
                        ZH = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Data");
        }
    }
}
