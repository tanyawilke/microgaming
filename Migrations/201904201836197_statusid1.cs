namespace FinanceRequest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class statusid1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.RequestModels", name: "Status_Id", newName: "StatusId");
            RenameIndex(table: "dbo.RequestModels", name: "IX_Status_Id", newName: "IX_StatusId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.RequestModels", name: "IX_StatusId", newName: "IX_Status_Id");
            RenameColumn(table: "dbo.RequestModels", name: "StatusId", newName: "Status_Id");
        }
    }
}
