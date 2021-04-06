using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Service
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Cost { get; set; }

        /// <summary>
        /// Многие-ко-многим
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; }
    }
}
