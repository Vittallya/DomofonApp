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
using Main.Events;
using System.IO;

namespace Main.ViewModels
{
    public class CatalogViewModel : BasePageViewModel
    {
        private readonly CatalogService catalogService;
        private readonly EventBus eventBus;
        private readonly BasketService basketService;
        private readonly UserService userService;
        private readonly RegisterService registerService;

        public bool IsLoading { get; set; }

        public CatalogViewModel(PageManager pageservice, BL.CatalogService catalogService, 
            EventBus eventBus, BasketService basketService, UserService userService, RegisterService registerService) : base(pageservice)
        {
            this.catalogService = catalogService;
            this.eventBus = eventBus;
            this.basketService = basketService;
            this.userService = userService;
            this.registerService = registerService;
            Init();
         
        }

        public ObservableCollection<ProductDto> Products { get; set; }

        public ICommand AddToBasket => new Command(x =>
        {
            if (x is ProductDto product)
            {
                int i = Products.IndexOf(product);
                Products[i] = basketService.AddToBasket(product);
                OnPropertyChanged(nameof(BasketCount));
            }

        });

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
                OnPropertyChanged(nameof(BasketCount));

            }

        });

        public ICommand ToBasket => new Command(x =>
        {
            pageservice.ChangeNewPage<Pages.BasketPage>(DisappearAnimation.Default);
        });


        public bool IsAutorized { get; set; }

        public string ClientName { get; set; }

        async void Init()
        {            
            IsLoading = true;
            IsAutorized = userService.IsAutorized;
            ClientName = userService.CurrentUser?.Name;
            eventBus.Describe<Events.ClientRegistered, CatalogViewModel>();
            await Reload();
        }

        public ICommand ToLogin => new Command(x =>
        {
            pageservice.ChangeNewPage<Pages.LoginPage>(DisappearAnimation.Default);
            pageservice.SetupNext<Pages.LoginPage, Pages.CatalogPage>(BackSlideAnim);
        });

        public ICommand ToLoginAdmin => new Command(x =>
        {
            pageservice.ChangeNewPage<Pages.LoginAdminPage>(DisappearAnimation.Default);
        });

        public ICommand LogoutCommand => new Command(x =>
        {
            userService.Logout();
            pageservice.ReloadCurrentPage(DisappearAnimation.Default);
        });




        public ICommand ToRegister => new Command(x =>
        {
            registerService.IsRegisterRequiered = true;
            pageservice.SetupNext<Pages.ClientRegisterPage, Pages.CatalogPage>(BackSlideAnim);
            pageservice.SetupNext<Pages.LoginPage, Pages.CatalogPage>(BackSlideAnim);
            pageservice.ChangeNewPage<Pages.ClientRegisterPage>(DisappearAnimation.Default);
        });

        public ICommand ToProfileView => new Command(x =>
        {
            pageservice.ChangeNewPage<Pages.ClientPage>(DisappearAnimation.Default);
        });

        async Task Reload()
        {
            string catalog = File.ReadAllLines(Rules.Static.FileName)[0];

            await catalogService.Reload(x => $"{catalog}\\{x}");
            if (basketService.BasketCount > 0)
            {
                Products = new ObservableCollection<ProductDto>(catalogService.GetProductsIncludeBasketAsync(basketService.GetCatalog()));
            }
            else
            {

                Products = new ObservableCollection<ProductDto>(catalogService.GetProductsAsync());
            }
            IsLoading = false;
            OnPropertyChanged(nameof(BasketCount));
        }


        public override int PoolIndex => Rules.Pages.MainPool;
    }
}
