﻿using BL;
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
            pageservice.ChangePage<Pages.BasketPage>(DisappearAnimation.Default);
        });

        async void Init()
        {
            IsLoading = true;
            await Reload();
        }

        async Task Reload()
        {
            if (basketService.BasketCount > 0)
            {
                Products = new ObservableCollection<ProductDto>(await catalogService.GetProductsIncludeBasketAsync(basketService.GetCatalog()));
            }
            else
            {

                Products = new ObservableCollection<ProductDto>(await catalogService.GetProductsAsync());
            }
            IsLoading = false;
            OnPropertyChanged(nameof(BasketCount));
        }


        public override int PoolIndex => Rules.Pages.MainPool;
    }
}