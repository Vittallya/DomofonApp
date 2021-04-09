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

        public DateTimeOffset CreationDate { get; set; }

        public bool IsCanceled => OrderStatus == OrderStatus.Canceled;
        public double PersonalSale { get; set; }
    }
}
