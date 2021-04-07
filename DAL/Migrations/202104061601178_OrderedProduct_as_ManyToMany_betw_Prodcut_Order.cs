namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderedProduct_as_ManyToMany_betw_Prodcut_Order : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderProducts", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.OrderProducts", "Product_Id", "dbo.Products");
            DropIndex("dbo.OrderProducts", new[] { "Order_Id" });
            DropIndex("dbo.OrderProducts", new[] { "Product_Id" });
            CreateTable(
                "dbo.OrderedProducts",
                c => new
                    {
                        OrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        CommonSale = c.Double(nullable: false),
                        Count = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrderId, t.ProductId })
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId);
            
            DropColumn("dbo.Orders", "Count");
            DropColumn("dbo.Orders", "CommonSale");
            DropTable("dbo.OrderProducts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.OrderProducts",
                c => new
                    {
                        Order_Id = c.Int(nullable: false),
                        Product_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Order_Id, t.Product_Id });
            
            AddColumn("dbo.Orders", "CommonSale", c => c.Double(nullable: false));
            AddColumn("dbo.Orders", "Count", c => c.Int(nullable: false));
            DropForeignKey("dbo.OrderedProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderedProducts", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderedProducts", new[] { "ProductId" });
            DropIndex("dbo.OrderedProducts", new[] { "OrderId" });
            DropTable("dbo.OrderedProducts");
            CreateIndex("dbo.OrderProducts", "Product_Id");
            CreateIndex("dbo.OrderProducts", "Order_Id");
            AddForeignKey("dbo.OrderProducts", "Product_Id", "dbo.Products", "Id", cascadeDelete: true);
            AddForeignKey("dbo.OrderProducts", "Order_Id", "dbo.Orders", "Id", cascadeDelete: true);
        }
    }
}
