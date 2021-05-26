﻿using BL;
using MVVM_Core;

namespace Main.ViewModels
{
    public class OrderResultViewModel : BasePageViewModel
    {
        private readonly OrderService orderService;
        private readonly BasketService basketService;
        private readonly ServicesService servicesService;

        public OrderResultViewModel(PageService pageservice, 
            OrderService orderService, BasketService basketService, ServicesService servicesService) : base(pageservice)
        {
            this.orderService = orderService;
            this.basketService = basketService;
            this.servicesService = servicesService;
            Init();
        }

        public string Message { get; set; }

        async void Init()
        {
            bool res = await orderService.ApplyOrder();
            Message = res ? "Заказ успешно оформлен! Наш менеджер свяжется с Вами в ближайшее время." : orderService.ErrorMessage;
            basketService.Clear();
            servicesService.Clear();
            pageservice.ClearAllPools();
        }


        protected override void Next(object p)
        {
            pageservice.ChangePage<Pages.CatalogPage>(PoolIndex, DisappearAnimation.Default);
        }

        public override int PoolIndex => Rules.Pages.MainPool;
    }
}