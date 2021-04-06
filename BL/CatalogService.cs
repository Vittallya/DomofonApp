using AutoMapper;
using DAL;
using DAL.Dto;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class CatalogService
    {
        private readonly AllDbContext dbContext;

        public CatalogService(AllDbContext allDbContext)
        {
            this.dbContext = allDbContext;
        }

        List<ProductDto> products;
        public async Task Reload()
        {
            Mapper mapper = new Mapper(new MapperConfiguration(x => x.CreateMap<Product, ProductDto>()));

            await dbContext.Products.LoadAsync();

            var list = await dbContext.
                Products.
                AsNoTracking().
                ToListAsync();

            products = list.
                Select(x => mapper.Map<Product, ProductDto>(x)).
                ToList();
        }

        public ProductDto GetCopy(ProductDto product)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(z => z.CreateMap<ProductDto, ProductDto>()));

            var instance = mapper.Map<ProductDto>(product);
            return instance;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(string name = null)
        {
            if(products == null)
            {
                await Reload();
            }

            if(name != null)
                return products.Where(x => x.Name == name);

            return products;
        }

    }
}
