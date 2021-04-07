using AutoMapper;
using DAL;
using DAL.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class BasketService
    {
        List<OrderedProductDto> orderedProducts = new List<OrderedProductDto>();
        private readonly AllDbContext dbContext;

        public int BasketCount => orderedProducts.Count;

        public ProductDto AddToBasket(ProductDto product)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(z => z.CreateMap<ProductDto, ProductDto>()));

            var instance = mapper.Map<ProductDto>(product);
            instance.IsInBasket = true;            
            orderedProducts.Add(new OrderedProductDto { Product = instance, Count = 1 });
            return instance;
        }

        public ProductDto RemoveFromBasket(ProductDto product)
        {
            var removable = orderedProducts.Find(x => x.Product.Id == product.Id);
            orderedProducts.Remove(removable);

            Mapper mapper = new Mapper(new MapperConfiguration(z => z.CreateMap<ProductDto, ProductDto>()));
            var instance = mapper.Map<ProductDto>(product);
            instance.IsInBasket = false;
            return instance;
        }

        public IEnumerable<OrderedProductDto> GetOrderProducts()
        {         
            return orderedProducts;
        }

        public async Task SetupFilledProducts(IEnumerable<OrderedProductDto> productDtos)
        {
            await Task.Run(() => orderedProducts = productDtos.Select(x => GetCopy(x)).ToList());
        }

        public OrderedProductDto IncreaceCount(OrderedProductDto dto)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(z => z.CreateMap<OrderedProductDto, OrderedProductDto>()));
            var instance = mapper.Map<OrderedProductDto>(dto);
            instance.Count++;
            return instance;
        }
        public OrderedProductDto DecreaceCount(OrderedProductDto dto)
        {
            if (dto.Count <= 1)
                return dto;

            var instance = GetCopy(dto);
            instance.Count--;
            return instance;
        }

        public IEnumerable<ProductDto> GetCatalog()
        {
            return orderedProducts.Select(x => x.Product);
        }

        public async Task<OrderedProductDto> UpdateSale(OrderedProductDto dto)
        {
            var instance = GetCopy(dto);
            var sale = await GetMaxCommonSale(dto.Product.Id, dto.Count);
            instance.CommonSale = sale;

            return instance;
        }

        public BasketService(AllDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<double> GetMaxCommonSale(int productId, int count)
        {
            await dbContext.CommonSales.LoadAsync();

            var sale = await dbContext.CommonSales.
                Where(x => x.ProductId == productId && x.StartCount <= count).
                OrderByDescending(x => x.StartCount).
                FirstOrDefaultAsync();

            return sale?.SaleValue ?? 0;
        }


        public T GetCopy<T>(T obj)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(z => z.CreateMap<T, T>()));
            return mapper.Map<T>(obj);
        }


        double GetCost(int count, double unitCost)
        {
            return count * unitCost;
        }
    }
}
