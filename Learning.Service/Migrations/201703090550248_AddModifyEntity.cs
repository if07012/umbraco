namespace Learning.Service.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModifyEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "CreateDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Messages", "LastUpdated", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "LastUpdated");
            DropColumn("dbo.Messages", "CreateDate");
        }
    }
}
