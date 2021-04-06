using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{

    /// <summary>
    /// Оптовые скидки
    /// </summary>
    public class CommonSale
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        /// <summary>
        /// Количество товаров, начиная с которого будет действовать скидка
        /// </summary>
        public int StartCount { get; set; }

        public double SaleValue { get; set; }

        public virtual Product Product { get; set; }
    }
}
