using BL;
using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DAL.Models;
using DAL.Dto;
using System.Windows.Input;
using AutoMapper;

namespace Main.ViewModels
{
    public class CatalogViewModel : BasePageViewModel
    {
        private readonly CatalogService catalogService;
        private readonly EventBus eventBus;
        private readonly BasketService basketService;

        public bool IsLoading { get; set; }

        public CatalogViewModel(PageService pageservice, BL.CatalogService catalogService, EventBus eventBus, BasketService basketService) : base(pageservice)
        {
            this.catalogService = catalogService;
            this.eventBus = eventBus;
            this.basketService = basketService;
            Init();
         
        }

        public ObservableCollection<ProductDto> Products { get; set; }

        public ICommand AddToBasket => new Command(x =>
        {
            if(x is ProductDto product)
            {
                int i = Products.IndexOf(product);
                Products[i] = basketService.AddToBasket(product);
                OnPropertyChanged("BasketCount");
            }
            
        });

        List<ProductDto> list = new List<ProductDto>();

        public int BasketCount
        {
            get
            {
                var value = basketService.BasketCount;
                BasketHasProducts = value > 0;
                return basketService.BasketCount;
            }
        }

        public bool BasketHasProducts { get; set; }

        public ICommand RemoveFromBasket => new Command(x =>
        {
            if (x is ProductDto product)
            {
                int i = Products.IndexOf(product);
                Products[i] = basketService.RemoveFromBasket(product);
                OnPropertyChanged("BasketCount");

            }

        });


        private void Products_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("Коллекция изменилась");
        }

        async void Init()
        {
            IsLoading = true;
            await Reload();
            Products.CollectionChanged += Products_CollectionChanged;
        }

        async Task Reload()
        {
            Products = new ObservableCollection<ProductDto>(await catalogService.GetProductsAsync());
            IsLoading = false;
        }


        public override int PoolIndex => Rules.Pages.MainPool;
    }
}
