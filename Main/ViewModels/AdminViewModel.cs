using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Admin;
using System.Windows.Controls;
using DAL;
using System.Collections.ObjectModel;
using DAL.Models;
using DAL.Dto;
using System.Windows.Input;
using System.Windows;

namespace Main.ViewModels
{
    public class AdminViewModel : BasePageViewModel
    {
        private readonly AllDbContext dbContext;

        private Window _window;

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<CommonSale> Sales { get; set; }
        public ObservableCollection<DAL.Models.Order> Orders { get; set; }
        public ObservableCollection<Service> Services { get; set; }

        private ObjectViewModel<Product> _productVm;
        private ObjectViewModel<CommonSale> _saleVm;
        private ObjectViewModel<Service> _serviceVm;

        public AdminViewModel(PageManager pageservice, AllDbContext dbContext) : base(pageservice)
        {
            this.dbContext = dbContext;

            

        }




    }
}