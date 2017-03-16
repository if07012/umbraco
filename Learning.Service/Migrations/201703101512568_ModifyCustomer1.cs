namespace Learning.Service.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyCustomer1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Customers", "CustomerGroup_Id1", "dbo.CustomerGroups");
            DropIndex("dbo.Customers", new[] { "CustomerGroup_Id1" });
            DropColumn("dbo.Customers", "CustomerGroup_Id1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customers", "CustomerGroup_Id1", c => c.Int());
            CreateIndex("dbo.Customers", "CustomerGroup_Id1");
            AddForeignKey("dbo.Customers", "CustomerGroup_Id1", "dbo.CustomerGroups", "Id");
        }
    }
}
