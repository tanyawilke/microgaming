namespace FinanceRequest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class confirmationcodelength : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RequestModels", "User", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.RequestModels", "ConfirmationCode", c => c.String(maxLength: 50));
            CreateIndex("dbo.RequestModels", "User");
            AddForeignKey("dbo.RequestModels", "User", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequestModels", "User", "dbo.AspNetUsers");
            DropIndex("dbo.RequestModels", new[] { "User" });
            AlterColumn("dbo.RequestModels", "ConfirmationCode", c => c.String(maxLength: 10));
            DropColumn("dbo.RequestModels", "User");
        }
    }
}
