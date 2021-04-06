namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class start : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Phone = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Login = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.CommonSales",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        StartCount = c.Int(nullable: false),
                        SaleValue = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Manufacturer = c.String(),
                        Cost = c.Double(nullable: false),
                        ImagePath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        Count = c.Int(nullable: false),
                        FullCost = c.Double(nullable: false),
                        CreationDate = c.DateTimeOffset(nullable: false, precision: 7),
                        CommonSale = c.Double(nullable: false),
                        PersonalSale = c.Double(nullable: false),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Cost = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PersonalSales",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        SaleValue = c.Double(nullable: false),
                        IsActual = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ClientId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ServiceOrders",
                c => new
                    {
                        Service_Id = c.Int(nullable: false),
                        Order_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Service_Id, t.Order_Id })
                .ForeignKey("dbo.Services", t => t.Service_Id, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.Order_Id, cascadeDelete: true)
                .Index(t => t.Service_Id)
                .Index(t => t.Order_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonalSales", "ProductId", "dbo.Products");
            DropForeignKey("dbo.PersonalSales", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.ServiceOrders", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.ServiceOrders", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.Orders", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Orders", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.CommonSales", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Profiles", "Id", "dbo.Clients");
            DropIndex("dbo.ServiceOrders", new[] { "Order_Id" });
            DropIndex("dbo.ServiceOrders", new[] { "Service_Id" });
            DropIndex("dbo.PersonalSales", new[] { "ProductId" });
            DropIndex("dbo.PersonalSales", new[] { "ClientId" });
            DropIndex("dbo.Orders", new[] { "ClientId" });
            DropIndex("dbo.Orders", new[] { "ProductId" });
            DropIndex("dbo.CommonSales", new[] { "ProductId" });
            DropIndex("dbo.Profiles", new[] { "Id" });
            DropTable("dbo.ServiceOrders");
            DropTable("dbo.PersonalSales");
            DropTable("dbo.Services");
            DropTable("dbo.Orders");
            DropTable("dbo.Products");
            DropTable("dbo.CommonSales");
            DropTable("dbo.Profiles");
            DropTable("dbo.Clients");
        }
    }
}
