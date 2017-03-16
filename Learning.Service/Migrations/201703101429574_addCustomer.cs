namespace Learning.Service.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCustomer : DbMigration
    {        
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        SurName = c.String(),
                        Height = c.Int(nullable: false),
                        Weight = c.Double(nullable: false),
                        BirthDate = c.DateTime(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        CustomerGroup_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomerGroups", t => t.CustomerGroup_Id)
                .Index(t => t.CustomerGroup_Id);
            
            CreateTable(
                "dbo.Vehicles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                        VehicleModel_Id = c.Int(),
                        Customer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VehicleModels", t => t.VehicleModel_Id)
                .ForeignKey("dbo.Customers", t => t.Customer_Id)
                .Index(t => t.VehicleModel_Id)
                .Index(t => t.Customer_Id);
            
            CreateTable(
                "dbo.VehicleModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Vehicles", "Customer_Id", "dbo.Customers");
            DropForeignKey("dbo.Vehicles", "VehicleModel_Id", "dbo.VehicleModels");
            DropForeignKey("dbo.Customers", "CustomerGroup_Id", "dbo.CustomerGroups");
            DropIndex("dbo.Vehicles", new[] { "Customer_Id" });
            DropIndex("dbo.Vehicles", new[] { "VehicleModel_Id" });
            DropIndex("dbo.Customers", new[] { "CustomerGroup_Id" });
            DropTable("dbo.VehicleModels");
            DropTable("dbo.Vehicles");
            DropTable("dbo.Customers");
            DropTable("dbo.CustomerGroups");
        }
    }
}
