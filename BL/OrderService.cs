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
        private IEnumerable<Service> _services;
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
                var inst = mapper.MapTo<ServiceDto, Service>(x);
                return inst;
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
            _currentOrder.FullCost = _fullCost;
            // при редактировании ???
            _currentOrder.OrderedProducts = _orderedProducts.ToList();

            _currentOrder.Services = _services.ToList();
            
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
                Where(x => x.ClientId == clientId).
                ToListAsync();

            return list.Select(y => mapper.MapTo<Order, OrderDto>(y));
        }
    }
}
