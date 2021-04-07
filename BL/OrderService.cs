using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DAL;
using DAL.Dto;
using DAL.Models;
using AutoMapper;
using System.Data.Entity;

namespace BL
{
    public class OrderService
    {
        private readonly AllDbContext dbContext;

        OrderDto _currentOrder;
        List<OrderedProductDto> _orderedProducts;

        public bool IsEdit { get; private set; }
        public bool IsStarted { get; private set; }

        public void Clear()
        {
            IsEdit = false;
            IsStarted = false;
            _currentOrder = null;
            _orderedProducts = null;
        }

        public OrderService(AllDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void StartOrder(IEnumerable<OrderedProductDto> orderedProducts)
        {
            IsEdit = false;        
            _orderedProducts = orderedProducts.ToList();
            _currentOrder = new OrderDto();
        }

        

        

        public OrderDto GetOrder()
        {
            Mapper mapper = new Mapper(new MapperConfiguration(x => x.CreateMap<OrderDto, OrderDto>()));

            var copy = mapper.Map<OrderDto>(_currentOrder);
            return copy;
        }

        public void SetupOrder()
        {

        }

        public async Task<IEnumerable<OrderDto>> GetAllOrders(int clientId)
        {
            await dbContext.Orders.LoadAsync();

            Mapper mapper = new Mapper(new MapperConfiguration(x => x.CreateMap<Order, OrderDto>()));

            var list = await dbContext.
                Orders.
                AsNoTracking().
                Where(x => x.ClientId == clientId).
                Select(y => mapper.Map<Order, OrderDto>(y)).
                ToListAsync();

            return list;
        }
    }
}
