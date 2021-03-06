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

        public void Clear()
        {
            orderedProducts.Clear();

        }

        List<OrderedProductDto> orderedProducts = new List<OrderedProductDto>();
        private readonly AllDbContext dbContext;
        private readonly MapperService mapper;

        public int BasketCount => orderedProducts?.Count ?? 0;

        public ProductDto AddToBasket(ProductDto product)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(z => z.CreateMap<ProductDto, ProductDto>()));

            var instance = mapper.Map<ProductDto>(product);
            instance.IsInBasket = true;            
            orderedProducts.Add(new OrderedProductDto { ProductDto = instance, Count = 1 });
            return instance;
        }

        public ProductDto RemoveFromBasket(ProductDto product)
        {
            var removable = orderedProducts.Find(x => x.ProductDto.Id == product.Id);
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
            await Task.Run(() => orderedProducts = productDtos.Select(x => mapper.GetCopy(x)).ToList());
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

            var instance = mapper.GetCopy(dto);
            instance.Count--;
            return instance;
        }

        public IEnumerable<ProductDto> GetCatalog(string cName = null)
        {
            if (cName == null)
                return orderedProducts.Select(x => x.ProductDto);

            return orderedProducts.Select(x => x.ProductDto).Where(x => x.Category == cName);
        }

        public async Task<OrderedProductDto> UpdateSale(OrderedProductDto dto)
        {
            var instance = mapper.GetCopy(dto);
            var sale = await GetMaxCommonSale(dto.ProductDto.Id, dto.Count);
            instance.CommonSale = sale;

            return instance;
        }

        public BasketService(AllDbContext dbContext, MapperService mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
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

    }
}
