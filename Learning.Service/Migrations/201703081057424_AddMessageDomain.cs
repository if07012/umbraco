namespace Learning.Service.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMessageDomain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "PersonId", c => c.Int(nullable: false));
            AddColumn("dbo.Messages", "ContentMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "ContentMessage");
            DropColumn("dbo.Messages", "PersonId");
        }
    }
}
