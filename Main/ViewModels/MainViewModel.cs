using BL;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MVVM_Core;
using System.Threading.Tasks;
using DAL;
using System;
using Main.Services;
using DAL.Models;

namespace Main.ViewModels
{
    
    public class MainViewModel : BaseSliderViewModel
    {
        private readonly PageManager pageService;
        private readonly DbContextLoader contextLoader;
        private readonly ClientPipeHanlder pipeHanlder;
        private readonly EventBus eventBus;
        private readonly UpdateHandlerService handlerService;


        public bool IsErrorLoading { get; set; }

        public string ErrorMessage { get; set; }
        public string ErrorMessageDetail { get; set; }

        public MainViewModel(PageManager pageService, 
            DbContextLoader contextLoader, 
            ClientPipeHanlder pipeHanlder, 
            EventBus eventBus,
            Services.UpdateHandlerService handlerService)
        {
            this.pageService = pageService;
            this.contextLoader = contextLoader;
            this.pipeHanlder = pipeHanlder;
            this.eventBus = eventBus;
            this.handlerService = handlerService;
            pageService.PageChanged += PageService_PageChanged;
            

            Init();
        }

        public string LoadingText { get; set; } = "Загрузка бд...";

        async void Init()
        {
            pipeHanlder.Init("DomofonApp");
            pipeHanlder.UpdateCalled += PipeHanlder_UpdateCalled;

            IsLoaded = await contextLoader.LoadAsync<Product>();
            IsLoadingAnimation = false;

            if (IsLoaded)
            {
                pageService.ChangeNewPage<Pages.CatalogPage>(defaultAnim);
            }
            else
            {
                IsErrorLoading = true;
                ErrorMessage = contextLoader.Message;
                ErrorMessageDetail = contextLoader.MessageDetail;
            }
            
        }

        private void PipeHanlder_UpdateCalled(string msg)
        {
            handlerService.Handle(msg);
        }

        public bool IsLoaded { get; set; }

        public bool IsLoadingAnimation { get; set; } = true;

        public int Width { get; set; } = 800;
        public override Page CurrentPage { get; set; }
    }
}
