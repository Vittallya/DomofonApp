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

        Order _currentOrder;
        Product _currentProduct;

        public bool IsEdit { get; private set; }
        public bool IsStarted { get; private set; }

        public void Clear()
        {
            IsEdit = false;
            IsStarted = false;
            _currentOrder = null;
            _currentProduct = null;
        }

        public OrderService(AllDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public OrderDto GetOrder()
        {
            Mapper mapper = new Mapper(new MapperConfiguration(x => x.CreateMap<Order, OrderDto>()));

            var dto = mapper.Map<Order, OrderDto>(_currentOrder);
            return dto;
        }

        public async Task BeginEditing(int orderId)
        {
            IsEdit = true;
            _currentOrder = await dbContext.Orders.FindAsync(orderId);
        }
        public async Task BeginNewOrder(int productId)
        {
            _currentProduct = await dbContext.Products.FindAsync(productId);
            IsEdit = false;

            _currentOrder = new Order { ProductId = productId };
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
