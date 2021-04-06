using AutoMapper;
using DAL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class BasketService
    {
        List<ProductDto> products = new List<ProductDto>();

        public int BasketCount => products.Count;

        public ProductDto AddToBasket(ProductDto product)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(z => z.CreateMap<ProductDto, ProductDto>()));

            var instance = mapper.Map<ProductDto>(product);
            instance.IsInBasket = true;
            products.Add(instance);
            return instance;
        }

        public ProductDto RemoveFromBasket(ProductDto product)
        {
            products.Remove(product);
            Mapper mapper = new Mapper(new MapperConfiguration(z => z.CreateMap<ProductDto, ProductDto>()));
            var instance = mapper.Map<ProductDto>(product);
            instance.IsInBasket = false;
            return instance;
        }

        public IEnumerable<ProductDto> GetAllProducts()
        {
            return products;
        }
    }
}
