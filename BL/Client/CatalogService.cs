using AutoMapper;
using DAL;
using DAL.Dto;
using DAL.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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

        

        public async Task Reload(Func<string, string> pathGetter)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(x => x.CreateMap<Product, ProductDto>()));

            await dbContext.Products.LoadAsync();

            var list = await dbContext.
                Products.
                Where(x => x.StorageCount > 0).
                AsNoTracking().
                ToListAsync();

            products = list.
                Select(x => 
                {
                    var inst = mapper.Map<Product, ProductDto>(x);
                    inst.ImageFullPath = pathGetter?.Invoke(x.ImagePath);
                    return inst;
                    }).
                ToList();
        }

        public T GetCopy<T>(T product)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(z => z.CreateMap<T, T>()));

            var instance = mapper.Map<T>(product);
            return instance;
        }

        public IEnumerable<ProductDto> GetProducts(string cName = null)
        {
            if(cName != null)
                return products.Where(x => x.Category == cName);

            return products;
        }

        public IEnumerable<ProductDto> GetProductsIncludeBasket(IEnumerable<ProductDto> dtos, string cName = null)
        {
            var collection = (GetProducts(cName)).Except(dtos, new IdComparer()).OfType<ProductDto>();
            return collection.Union(dtos);
        }

        public IEnumerable<string> GetCategories()
        {
            return products.Select(x => x.Category).Distinct();
        }
    }

    class IdComparer : IEqualityComparer<IDto>
    {
        public bool Equals(IDto x, IDto y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(IDto obj)
        {
            return obj.Id;
        }
    }

}
