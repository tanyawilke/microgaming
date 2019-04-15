namespace FinanceRequest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttachmentModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AttachmentModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        File = c.String(nullable: false, maxLength: 400),
                        RequestId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RequestModels", t => t.RequestId, cascadeDelete: true)
                .Index(t => t.RequestId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AttachmentModels", "RequestId", "dbo.RequestModels");
            DropIndex("dbo.AttachmentModels", new[] { "RequestId" });
            DropTable("dbo.AttachmentModels");
        }
    }
}
