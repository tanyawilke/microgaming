namespace FinanceRequest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnToRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RequestModels", "ConfirmationCode", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RequestModels", "ConfirmationCode");
        }
    }
}
