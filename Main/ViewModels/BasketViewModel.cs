using BL;
using DAL.Dto;
using DAL.Models;
using Main.Events;
using MVVM_Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Main.ViewModels
{
    public class BasketViewModel : BasePageViewModel
    {
        private readonly BasketService basketService;
        private readonly CatalogService catalogService;
        private readonly ServicesService servicesService;
        private readonly OrderService orderService;
        private readonly UserService userService;
        private readonly EventBus eventBus;
        private readonly Timer timer = new Timer(5, 100);

        public ObservableCollection<OrderedProductDto> OrderedProducts { get; set; }

        public ObservableCollection<ServiceDto> IncludedServices { get; set; }

        public ObservableCollection<ServiceDto> NotIncludedServices { get; set; }

        public bool IsServicesIncluded => IncludedServices != null && IncludedServices.Count > 0;

        public bool IsPromtVisible { get; set; }


        protected override async void Back(object p)
        {
            await basketService.SetupFilledProducts(OrderedProducts);
            pageservice.Back<Pages.CatalogPage>(BackSlideAnim, true);
        }

        public BasketViewModel(PageManager pageservice, BasketService basketService, 
            CatalogService catalogService, ServicesService servicesService, 
            OrderService orderService, UserService userService, EventBus eventBus) : base(pageservice)
        {
            this.basketService = basketService;
            this.catalogService = catalogService;
            this.servicesService = servicesService;
            this.orderService = orderService;
            this.userService = userService;
            this.eventBus = eventBus;
            Init();
        }

        public ICommand AddService => new CommandAsync(async x =>
        {
            if(x is ServiceDto dto)
            {
                NotIncludedServices.Remove(dto);
                IncludedServices.Add(dto);
                await Calculate();
                OnPropertyChanged(nameof(IsServicesIncluded));
            }
            IsPromtVisible = false;
        }); 
        public ICommand RemoveService => new CommandAsync(async x =>
        {
            if(x is ServiceDto dto)
            {
                IncludedServices.Remove(dto);
                NotIncludedServices.Add(dto);
                await Calculate();
                OnPropertyChanged(nameof(IsServicesIncluded));
            }
        });

        public ICommand ClosePrompt => new Command(x =>
        {
            IsPromtVisible = false;
        });
        public ICommand OpenPrompt => new Command(x =>
        {
            IsPromtVisible = true;
        });

        async Task Update()
        {
            _isUpdate = true;
            await Calculate();
            await basketService.SetupFilledProducts(OrderedProducts);
            await servicesService.SetupUsedServices(IncludedServices);
            _isUpdate = false;
        }

        protected override async void Next(object p)
        {
            await orderService.SetupData(OrderedProducts, IncludedServices, FinalCost);

            if (userService.IsAutorized)
            {
                await OnRegister(new Events.ClientRegistered(userService.CurrentUser));
                pageservice.ChangeNewPage<Pages.OrderResultPage>(DisappearAnimation.Default);
            }
            else
            {

                pageservice.SetupNext<Pages.ClientRegisterPage, Pages.OrderResultPage>(DisappearAnimation.Default);
                pageservice.SetupNext<Pages.LoginPage, Pages.OrderResultPage>(DisappearAnimation.Default);
                eventBus.Subscribe<Events.ClientRegistered, BaseViewModel>(OnRegister);
                eventBus.Subscribe<Events.AccountEntered, BaseViewModel>(OnEnter);
                pageservice.ChangeNewPage<Pages.ClientRegisterPage>(DisappearAnimation.Default);
            }
        }

        private async Task OnEnter(AccountEntered arg)
        {
            orderService.SetupClient(arg.Id);
            eventBus.Describe<Events.ClientRegistered, BaseViewModel>();
        }

        private async Task OnRegister(ClientRegistered arg)
        {
            orderService.SetupClient(arg.User.Id);
            eventBus.Describe<Events.AccountEntered, BaseViewModel>();
        }


        async Task Reload()
        {
            await servicesService.ReloadAsync();

            OrderedProducts = new ObservableCollection<OrderedProductDto>(basketService.GetOrderProducts());
            NotIncludedServices = new ObservableCollection<ServiceDto>(servicesService.GetNotUsedServices());
            IncludedServices = new ObservableCollection<ServiceDto>(servicesService.GetUsedServices());
            await Calculate();
        }

        async void Init()
        {
            await Reload();
            timer.TimerCompletedAsync += async () => await Update();
        }

        private bool _isUpdate;

        public OrderedProductDto Selected { get; set; }

        public double FinalCost { get; set; }

        public double CommonCost { get; set; }
        public double CommonSaleValute { get; set; }

        async Task Calculate()
        {
            CommonCost = 0;
            FinalCost = 0;
            CommonSaleValute = 0;

            for (int i = 0; i < OrderedProducts.Count; i++)
            {
                var dto = OrderedProducts[i];
                var inst = await basketService.UpdateSale(dto);
                OrderedProducts[i] = inst;

                double saleValute = inst.Cost * inst.CommonSale / 100;
                CommonSaleValute += saleValute;
                FinalCost += inst.Cost - saleValute;
                CommonCost += inst.Cost;
            }

            foreach(var s in IncludedServices)
            {
                CommonCost += s.Cost;
                FinalCost += s.Cost;
            }
        }

        public ICommand DeleteFromBasket => new CommandAsync(async x =>
        {
            OrderedProducts.Remove(Selected);
            await Update();
            
        }, y => Selected != null);

        public ICommand MinusCommand => new Command(x =>
        {
            if(x is OrderedProductDto dto)
            {        
                int i = OrderedProducts.IndexOf(dto);
                OrderedProducts[i] = basketService.DecreaceCount(dto);

                if(!timer.IsStarted)
                    timer.StartTimer();
                timer.Reset();

            }
        }, y=> !_isUpdate);

        public ICommand PlusCommand => new Command(x =>
        {
            if (x is OrderedProductDto dto)
            {
                int i = OrderedProducts.IndexOf(dto);
                OrderedProducts[i] = basketService.IncreaceCount(dto);

                if (!timer.IsStarted)
                    timer.StartTimer();
                timer.Reset();
            }
        }, y => !_isUpdate);


        public override int PoolIndex => Rules.Pages.MainPool;
    }

    class Timer
    {
        private readonly int deltaTime;
        private int count;
        private int countTemp;
        public bool IsStarted { get; private set; }

        public event Action TimerCompleted;

        public event Func<Task> TimerCompletedAsync;

        public Timer(int count = 5, int deltaTime = 1000)
        {
            this.count = count;
            this.deltaTime = deltaTime;
        }

        public async void StartTimer()
        {
            Reset();
            IsStarted = true;

            while(countTemp > 0)
            {
                countTemp--;
                await Task.Delay(deltaTime);
            }
            IsStarted = false;

            OnTimerCompleted();
        }

        public void Reset()
        {
            countTemp = count;
        }


        private async void OnTimerCompleted()
        {
            TimerCompleted?.Invoke();
            await TimerCompletedAsync?.Invoke();
        }

    }
}