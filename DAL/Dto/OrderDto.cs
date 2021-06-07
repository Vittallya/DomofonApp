using DAL.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Dto
{
    public class OrderDto: IDto
    {
        public int Id { get; set; }
        public int Count { get; set; }

        /// <summary>
        /// Оптовая скидка
        /// </summary>
        public string Address { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public double FullCost { get; set; }
        public double CommonSale { get; set; }

        public DateTimeOffset CreationDate { get; set; }

        public bool IsCanceled => OrderStatus == OrderStatus.Canceled;
        public double PersonalSale { get; set; }

        public ClientDto ClientDto { get; set; }

        public bool IsActive => OrderStatus == OrderStatus.Active;

        public string StatusStr
        {
            get
            {
                switch (OrderStatus)
                {
                    case OrderStatus.Completed: return "Завершен";
                    case OrderStatus.Canceled: return "Отменен";
                    case OrderStatus.CanceledByAdmin: return "Отменен администратором";
                }
                return "В обработке";
            }
        }
    }
}
