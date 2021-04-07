using BL;
using DAL.Dto;
using DAL.Models;
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
        private readonly Timer timer = new Timer(5, 100);

        protected override void Back()
        {
            pageservice.ChangePage<Pages.CatalogPage>(DisappearAnimation.Default);
        }

        public BasketViewModel(PageService pageservice, BasketService basketService) : base(pageservice)
        {
            this.basketService = basketService;
            Init();
        }


        async Task Update()
        {
            await Calculate();
            await basketService.SetupFilledProducts(OrderedProducts);
        }

        public ObservableCollection<OrderedProductDto> OrderedProducts { get; set; }

        async Task Reload()
        {
            OrderedProducts = new ObservableCollection<OrderedProductDto>(basketService.GetOrderProducts());
            await Calculate();
        }

        async void Init()
        {
            await Reload();
            timer.TimerCompletedAsync += async () => await Update();
        }

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
        });

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
        });


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