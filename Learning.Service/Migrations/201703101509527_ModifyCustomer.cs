namespace Learning.Service.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyCustomer : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Customers", "CustomerGroup_Id", "dbo.CustomerGroups");
            DropIndex("dbo.Customers", new[] { "CustomerGroup_Id" });
            AddColumn("dbo.Customers", "CustomerGroup_Id1", c => c.Int());
            AlterColumn("dbo.Customers", "CustomerGroup_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Customers", "CustomerGroup_Id1");
            AddForeignKey("dbo.Customers", "CustomerGroup_Id1", "dbo.CustomerGroups", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Customers", "CustomerGroup_Id1", "dbo.CustomerGroups");
            DropIndex("dbo.Customers", new[] { "CustomerGroup_Id1" });
            AlterColumn("dbo.Customers", "CustomerGroup_Id", c => c.Int());
            DropColumn("dbo.Customers", "CustomerGroup_Id1");
            CreateIndex("dbo.Customers", "CustomerGroup_Id");
            AddForeignKey("dbo.Customers", "CustomerGroup_Id", "dbo.CustomerGroups", "Id");
        }
    }
}
