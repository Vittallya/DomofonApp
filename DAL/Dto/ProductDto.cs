using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Dto
{
    public class ProductDto: IDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }

        public double Cost { get; set; }
        public string ImageFullPath { get; set; }
        public bool IsInBasket { get; set; }
    }
}
