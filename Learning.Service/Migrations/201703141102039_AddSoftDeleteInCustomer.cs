namespace Learning.Service.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSoftDeleteInCustomer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "IsDelete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "IsDelete");
        }
    }
}
