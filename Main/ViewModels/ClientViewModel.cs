using BL;
using DAL.Dto;
using MVVM_Core;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Main.ViewModels
{
    public class ClientViewModel : BasePageViewModel
    {
        private readonly UserService userService;
        private readonly OrderService orderService;

        public ClientDto ClientDto { get; set; }

        public OrderDto SelectedOrder { get; set; }

        public ObservableCollection<OrderedProductDto> OrderedProducts { get; set; }
        public ObservableCollection<ServiceDto> OrderedServices { get; set; }

        public ClientViewModel(PageManager pageservice, UserService userService, OrderService orderService) : base(pageservice)
        {
            this.userService = userService;
            this.orderService = orderService;
            Init();
        }

        async void Init()
        {
            Orders = new ObservableCollection<OrderDto>(await orderService.GetAllOrders(userService.CurrentUser.Id));
            ClientDto = userService.CurrentUser;
        }

        public ICommand CancelOrder => new CommandAsync(async x =>
        {
            if(x is OrderDto dto && 
            MessageBox.Show("Отменить заказ?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int index = Orders.IndexOf(dto);
                Orders[index] = await orderService.CancelOrder(dto.Id);
            }
        });

        public bool IsLoadingVisible { get; set; }

        public bool IsServicesExist { get; set; }


        public bool IsExpanded { get; set; }

        public ICommand SelectOrder => new CommandAsync(async x =>
        {
            if (x is OrderDto dto)
            {

                IsLoadingVisible = true;
                SelectedOrder = dto;
                OrderedProducts = new ObservableCollection<OrderedProductDto>(
                    await orderService.GetOrderedProducts(dto.Id));
                OrderedServices = new ObservableCollection<ServiceDto>(
                    await orderService.GetOrderedServices(dto.Id));

                IsServicesExist = OrderedServices.Count > 0;

                IsLoadingVisible = false;
                IsExpanded = true;
            }
        });

        protected override void Back(object p)
        {
            pageservice.Back<Pages.CatalogPage>(DisappearAnimation.Default);
        }

        public ObservableCollection<OrderDto> Orders { get; set; }

        public override int PoolIndex => Rules.Pages.MainPool;
    }
}