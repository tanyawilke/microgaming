namespace FinanceRequest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class amendrequest : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.RequestModels", "User", "dbo.AspNetUsers");
            //DropIndex("dbo.RequestModels", new[] { "User" });
            //DropColumn("dbo.RequestModels", "User");
        }

        public override void Down()
        {
            AddColumn("dbo.RequestModels", "User", c => c.String(maxLength: 128));
            CreateIndex("dbo.RequestModels", "User");
            AddForeignKey("dbo.RequestModels", "User", "dbo.AspNetUsers", "Id");
        }
    }
}
