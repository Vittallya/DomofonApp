using DAL.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Dto
{
    public class OrderedProductDto
    {
        
        public double Cost => Count * ProductDto.Cost;

        public double SaleCost => Cost - (Cost * CommonSale / 100);

        public int Count { get; set; }
        public double CommonSale { get; set; }
        public virtual ProductDto ProductDto { get; set; }
    }
}
