using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.Models;
using DAL.Dto;
using System.Data.Entity;
using AutoMapper;

namespace BL
{
    public class ServicesService
    {
        private readonly AllDbContext dbContext;
        private IEnumerable<ServiceDto> usedServices;
        private IEnumerable<ServiceDto> allServices;

        public bool ServicesSelected => usedServices != null;

        public ServicesService(AllDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task ReloadAsync()
        {
            await dbContext.Services.LoadAsync();

            var list = dbContext.Services.AsNoTracking().ToList();

            Mapper mapper = new Mapper(new MapperConfiguration(z => z.CreateMap<Service, ServiceDto>()));

            allServices = list.Select(x => mapper.Map<Service, ServiceDto>(x));
        }

        public IEnumerable<ServiceDto> GetAllServices()
        {
            return allServices;
        }

        public IEnumerable<ServiceDto> GetNotUsedServices()
        {
            if (!ServicesSelected)
                return GetAllServices();

            var list = (allServices).Except(usedServices, new IdComparer()).Cast<ServiceDto>();

            return list;
        }

        public IEnumerable<ServiceDto> GetUsedServices()
        {
            return usedServices ?? new List<ServiceDto>();
        }

        public async Task SetupUsedServices(IEnumerable<ServiceDto> services)
        {
            await Task.Run(() => usedServices = services.Select(x => GetCopy(x)));
        }

        public T GetCopy<T>(T obj)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(z => z.CreateMap<T, T>()));
            return mapper.Map<T>(obj);
        }
    }
}
