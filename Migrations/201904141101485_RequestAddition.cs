namespace FinanceRequest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequestAddition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RequestModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Charity = c.String(nullable: false),
                        PlayItForward = c.Boolean(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubmissionDate = c.DateTime(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RequestModels");
        }
    }
}
