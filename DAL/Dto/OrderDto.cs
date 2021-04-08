﻿using DAL.Interfaces;
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
    }
}
