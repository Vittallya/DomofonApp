namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fillDb : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO [Services] VALUES (N'Доставка по Москве', 2900)");
            Sql("INSERT INTO [Services] VALUES (N'Доставка по России', 3990)");
            Sql("INSERT INTO [Services] VALUES (N'Монтаж домофона ', 3500)");
            Sql("INSERT INTO [Services] VALUES (N'Проводка ', 5500)");

            Sql("INSERT INTO [Products] VALUES (N'Домофон обычный','Visit', 6900,'1')");
            Sql("INSERT INTO [Products] VALUES (N'Видеодомофон обычный','SuperDom', 6900,'2')");
            Sql("INSERT INTO [Products] VALUES (N'Видеодомофон extra','Visit', 12500,'1')");
            Sql("INSERT INTO [Products] VALUES (N'Комплект видеодомофона Falcon Eye','ExtraPhones', 15400,'3')");
            Sql("INSERT INTO [Products] VALUES (N'Домофон выского качества','Global', 7600,'4')");
            Sql("INSERT INTO [Products] VALUES (N'Домофон Heavy','Visit', 5900,'5')");

            Sql("INSERT INTO [CommonSales] VALUES (1, 10, 5)");
            Sql("INSERT INTO [CommonSales] VALUES (1, 25, 10)");
            Sql("INSERT INTO [CommonSales] VALUES (2, 15, 10)");
            Sql("INSERT INTO [CommonSales] VALUES (4, 10, 4)");
            Sql("INSERT INTO [CommonSales] VALUES (5, 15, 10)");
            Sql("INSERT INTO [CommonSales] VALUES (5, 25, 15)");
        }
        
        public override void Down()
        {
        }
    }
}
