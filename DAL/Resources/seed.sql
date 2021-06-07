USE [DomofonDb]
GO

SET IDENTITY_INSERT [Services] ON;

INSERT INTO [Services]([Id],[Name],[Cost]) VALUES 
	(1, N'Доставка по Москве', 2900),
	(2, N'Доставка по России', 3990),
	(3, N'Монтаж домофона ', 3500),
	(4, N'Проводка ', 5500);

SET IDENTITY_INSERT [Services] OFF;


SET IDENTITY_INSERT [Categories] ON;

INSERT INTO [Categories]([Id],[Name]) VALUES 
	(1, N'Голосовые домофоны'),
	(2, N'Видеодомофоны'),
	(3, N'Домофоны со сканером лица' ),
	(4, N'Видедомофоны со встроенной сигнализацией');

SET IDENTITY_INSERT [Categories] OFF;

SET IDENTITY_INSERT [Products] ON;

INSERT INTO [Products]([Id],[Name], [Manufacturer], [Cost], [ImagePath], [StorageCount], [Category]) VALUES 
						(1,N'Домофон обычный','Visit', 6900,'1.png', 123, N'Голосовые домофоны'),
						(2,N'Видеодомофон обычный','SuperDom', 6900,'2.png', 435, N'Видеодомофоны'),
						(3,N'Видеодомофон extra','Visit', 12500,'3.png', 344, N'Видеодомофоны'),
						(4,N'Комплект видеодомофона Falcon Eye','ExtraPhones', 15400,'4.png', 543, N'Видедомофоны со встроенной сигнализацией'),
						(5,N'Домофон выского качества','Global', 7600,'5.png', 190, N'Домофоны со сканером лица'),
						(6,N'Домофон Heavy','Visit', 5900,'6.png', 567, N'Голосовые домофоны');

SET IDENTITY_INSERT [Products] OFF;

INSERT INTO [CommonSales] VALUES 
			(1, 10, 5),
			(1, 25, 10),
			(2, 15, 10),
			(4, 10, 4),
			(5, 15, 10),
			(5, 25, 15);