using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM_Core;
using BL;
using DAL;
using System.Windows;
using DAL.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Admin.ViewModels
{
    [Singleton]
    public class MainViewModel: BaseSliderViewModel
    {
        private readonly DbContextLoader loader;
        private readonly PageService pageService;
        private readonly EventBus eventBus;
        private readonly ServerPipeHandler serverPipeHandler;

        public bool LoadingContext { get; set; } = true;

        public MainViewModel(DbContextLoader loader, PageService pageService, EventBus eventBus, 
            BL.ServerPipeHandler serverPipeHandler)
        {
            this.loader = loader;
            this.pageService = pageService;
            this.eventBus = eventBus;
            this.serverPipeHandler = serverPipeHandler;
            pageService.PageChanged += PageService_PageChanged;

            init();
        }

        public ICommand Test => new Command(x =>
        {
            
        });

        public ICommand AutosCommand => new Command(x =>
        {
            //Locator.SetItemsViewModel<AutosViewModel>();
            pageService.ClearHistoryByPool(1);
            pageService.ChangePage<Pages.ItemsPage>(1, DisappearAndToSlideAnim.Default);
        });
        public ICommand FinesCommand => new Command(x =>
        {
            //Locator.SetItemsViewModel<FinesViewModel>();
            pageService.ClearHistoryByPool(1);
            pageService.ChangePage<Pages.ItemsPage>(1, DisappearAndToSlideAnim.Default);
        });

        public ICommand ParkingCommand => new Command(x =>
        {
            //Locator.SetItemsViewModel<ParkingViewModel>();
            pageService.ClearHistoryByPool(1);
            pageService.ChangePage<Pages.ItemsPage>(1, DisappearAndToSlideAnim.Default);
        });
        public ICommand EvacCommand => new Command(x =>
        {
            //Locator.SetItemsViewModel<EvacuationsViewModel>();
            pageService.ClearHistoryByPool(1);
            pageService.ChangePage<Pages.ItemsPage>(1, DisappearAndToSlideAnim.Default);
        });
        public ICommand DeclarationCommand => new Command(x =>
        {
            //Locator.SetItemsViewModel<DeclarationsViewModel>();
            pageService.ClearHistoryByPool(1);
            pageService.ChangePage<Pages.ItemsPage>(1, DisappearAndToSlideAnim.Default);
        });


        public override Page CurrentPage { get; set; }

        async Task Update(Events.UpdatePipe updatePipe)
        {
            serverPipeHandler.Send(updatePipe.GetString());
        }

        async void init()
        {
            eventBus.Subscribe<Events.UpdatePipe, MainViewModel>(Update, false);

            serverPipeHandler.Init();
            try
            {
                await loader.LoadAsync<Client>();
                LoadingContext = false;
            }
            catch(Exception ex)
            {
                MessageBoxResult res = MessageBox.Show(ex.Message, "", MessageBoxButton.OK);

                if (res == MessageBoxResult.OK || res == MessageBoxResult.Cancel)
                {
                    
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
