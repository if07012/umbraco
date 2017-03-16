namespace Learning.Service.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyCustomer12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "CustomerGroupId", c => c.Int(nullable: false));
            DropColumn("dbo.Customers", "CustomerGroups_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customers", "CustomerGroups_Id", c => c.Int(nullable: false));
            DropColumn("dbo.Customers", "CustomerGroupId");
        }
    }
}
