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
        private readonly MapperService mapper;
        Order _currentOrder;
        private IEnumerable<OrderedProduct> _orderedProducts;
        private IEnumerable<int> _services;
        private int _clientId;
        private double _fullCost;

        public bool IsEdit { get; private set; }
        public bool IsStarted { get; private set; }

        public void Clear()
        {
            IsEdit = false;
            IsStarted = false;
            _currentOrder = null;
            _orderedProducts = null;
            _services = null;
        }

        public OrderService(AllDbContext dbContext, MapperService mapperService)
        {
            this.dbContext = dbContext;
            this.mapper = mapperService;
        }

        public async Task SetupData(IEnumerable<OrderedProductDto> orderedProducts, IEnumerable<ServiceDto> services,
            double fullCost, int? orderId = null)
        {
            _fullCost = fullCost;
            IsEdit = orderId.HasValue;
            _currentOrder = IsEdit ? (await dbContext.Orders.FindAsync(orderId.Value)) : new Order();

            _orderedProducts = orderedProducts.
                Select(x =>
                {
                    var inst = mapper.MapTo<OrderedProductDto, OrderedProduct>(x);
                    inst.ProductId = x.ProductDto.Id;
                    inst.Order = _currentOrder;
                    return inst;
                }).ToList(); 

            _services = services.Select(x =>
            {
                //var inst = mapper.MapTo<ServiceDto, Service>(x);
                //inst.Orders = new List<Order> { _currentOrder };  
                //return inst;
                return x.Id;
            }).ToList();

        }

        public void SetupClient(int clientId)
        {
            _clientId = clientId;
        }

        public void SetupOrderData(OrderDto order)
        {
            int id = _currentOrder.Id;
            _currentOrder = mapper.MapTo<OrderDto, Order>(order);
            _currentOrder.Id = id;
        }


        public async Task<bool> ApplyOrder()
        {
            await dbContext.Services.LoadAsync();
            _currentOrder.FullCost = _fullCost;
            // при редактировании ???
            _currentOrder.OrderedProducts = _orderedProducts.ToList();


            foreach(var op in _orderedProducts)
            {
                var p = await dbContext.Products.FindAsync(op.ProductId);
                p.StorageCount -= op.Count;
                dbContext.Entry(p).State = EntityState.Modified;
            }

            if (!IsEdit)
            {
                _currentOrder.CreationDate = DateTimeOffset.Now;
                _currentOrder.OrderStatus = OrderStatus.Active;
                _currentOrder.ClientId = _clientId;
                dbContext.Orders.Add(_currentOrder);
                
            }
            else
            {
                //Удаление старых
                dbContext.OrderedProducts.RemoveRange(
                    dbContext.OrderedProducts.Where(x => x.OrderId == _currentOrder.Id));

                dbContext.Entry(_currentOrder).State = EntityState.Modified;
            }
            foreach (var sId in _services)
            {
                var serv = await dbContext.Services.FindAsync(sId);
                serv.Orders = new List<Order> { _currentOrder };
                dbContext.Entry(serv).State = EntityState.Modified;
            }

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        public string ErrorMessage { get; private set; }

        public async Task<IEnumerable<OrderDto>> GetAllOrders(int clientId)
        {
            await dbContext.Orders.LoadAsync();

            var list = await dbContext.
                Orders.
                AsNoTracking().
                Where(x => x.ClientId == clientId && x.OrderStatus == OrderStatus.Active).
                OrderBy(x => x.CreationDate).
                ToListAsync();

            return list.Select(y => 
            { 
                var inst = mapper.MapTo<Order, OrderDto>(y);
                return inst;
            });
        }

        public async Task<IEnumerable<ServiceDto>> GetOrderedServices(int orderId)
        {
            await dbContext.Orders.Include(x => x.Services).LoadAsync();

            var order = await dbContext.Orders.FindAsync(orderId);

            var list = order.Services;

            return list.Select(y =>
            {
                var inst = mapper.MapTo<Service, ServiceDto>(y);
                return inst;
            });
        }

        public async Task<IEnumerable<OrderedProductDto>> GetOrderedProducts(int orderId)
        {
            await dbContext.OrderedProducts.LoadAsync();
            await dbContext.CommonSales.LoadAsync();
            var order = await dbContext.Orders.FindAsync(orderId);

            var list = await dbContext.
                OrderedProducts.
                AsNoTracking().
                Where(x => x.OrderId == orderId).
                ToListAsync();


            return list.Select(y =>
            {

                var sales = dbContext.CommonSales.Where(x => x.ProductId == y.ProductId).ToList();

                var inst = mapper.MapTo<OrderedProduct, OrderedProductDto>(y);                
                inst.ProductDto = mapper.MapTo<Product, ProductDto>(y.Product);
                inst.CommonSale = sales.
                    Where(x => x.StartCount <= inst.Count).
                    OrderByDescending(x => x.SaleValue).
                    FirstOrDefault()?.SaleValue ?? 0;

                return inst;
            });
        }

        public async Task<OrderDto> CancelOrder(int orderId)
        {
            await dbContext.Orders.LoadAsync();
            var order = await dbContext.Orders.FindAsync(orderId);

            order.OrderStatus = OrderStatus.Canceled;
            dbContext.Entry(order).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
            return mapper.MapTo<Order, OrderDto>(order);
        }
    }
}
