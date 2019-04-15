namespace FinanceRequest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StatusAddition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StatusModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.RequestModels", "Status_Id", c => c.Int());
            CreateIndex("dbo.RequestModels", "Status_Id");
            AddForeignKey("dbo.RequestModels", "Status_Id", "dbo.StatusModels", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequestModels", "Status_Id", "dbo.StatusModels");
            DropIndex("dbo.RequestModels", new[] { "Status_Id" });
            DropColumn("dbo.RequestModels", "Status_Id");
            DropTable("dbo.StatusModels");
        }
    }
}
