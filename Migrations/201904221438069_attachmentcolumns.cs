namespace FinanceRequest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class attachmentcolumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AttachmentModels", "ContentType", c => c.String(maxLength: 100));
            AddColumn("dbo.AttachmentModels", "Content", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AttachmentModels", "Content");
            DropColumn("dbo.AttachmentModels", "ContentType");
        }
    }
}
