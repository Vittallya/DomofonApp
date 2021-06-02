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
using System.IO;

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
        private string _imageCatalog;
        private string selectedDir;

        public AdminViewModel(PageManager pageservice, AllDbContext dbContext) : base(pageservice)
        {
            this.dbContext = dbContext;



        }

        public ObservableCollection<string> PathVariants { get; set; } = new ObservableCollection<string>();


        public bool IsVariantsVis { get; set; }


        void GetVariants(string path)
        {
            IsVariantsVis = false;


            if (path.Length > 0 && path.Length < 3 && path.LastIndexOf('\\') == -1)
            {
                var drives = DriveInfo.GetDrives().Select(x => x.Name);
                IsVariantsVis = true;
                PathVariants = new ObservableCollection<string>(drives);
            }
            else if (path.LastIndexOf('\\') > -1)
            {
                var dirs = Directory.GetDirectories(path);
                IsVariantsVis = true;
                PathVariants = new ObservableCollection<string>(dirs);

            }


        }

        public string SelectedDir { get => selectedDir; set => selectedDir = value; }


        public string DefalutImageCatalog
        {
            get => _imageCatalog;
            set
            {
                if (value == _imageCatalog) return;
                GetVariants(value);
                _imageCatalog = value;
                OnPropertyChanged(nameof(DefalutImageCatalog));
            }


        }


    }
}