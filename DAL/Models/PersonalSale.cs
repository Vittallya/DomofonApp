using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PersonalSale
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public double SaleValue { get; set; }
        public bool IsActual { get; set; }
        public virtual Client Client { get; set; }
        public virtual Product Product { get; set; }
    }
}
