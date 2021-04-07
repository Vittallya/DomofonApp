using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DAL.Models
{
    public class OrderedProduct
    {
        [Key]
        [Column(Order =1)]
        public int OrderId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int ProductId { get; set; }
        /// <summary>
        /// Оптовая скидка
        /// </summary>
        public double CommonSale { get; set; }
        public int Count { get; set; }
        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }
    }
}
