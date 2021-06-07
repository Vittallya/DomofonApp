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
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

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


        void CheckFile()
        {
            if (!File.Exists(Rules.Static.FileName))
            {
                File.WriteAllText(Rules.Static.FileName, Properties.Resources.DefaultImageCatalog);
            }

            string path = File.ReadAllLines(Rules.Static.FileName)[0];

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                var imgs = new BitmapImage[]
                {
                    App.Current.MainWindow.FindResource("1") as BitmapImage,
                    App.Current.MainWindow.FindResource("2") as BitmapImage,
                    App.Current.MainWindow.FindResource("3") as BitmapImage,
                    App.Current.MainWindow.FindResource("4") as BitmapImage,
                    App.Current.MainWindow.FindResource("5") as BitmapImage,
                    App.Current.MainWindow.FindResource("6") as BitmapImage,
                };


                foreach(BitmapImage image in imgs)
                {
                    string name = Path.GetFileName(image.UriSource.OriginalString);

                    using(FileStream fs = new FileStream(Path.Combine(path, name), FileMode.CreateNew, FileAccess.Write))
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();

                        string ext = Path.GetExtension(name);


                        switch (ext)
                        {
                            case "jpeg": encoder = new JpegBitmapEncoder(); break;
                            case "jpg": encoder = new JpegBitmapEncoder(); break;
                            case "bmp": encoder = new BmpBitmapEncoder(); break;
                        }

                        encoder.Frames.Add(BitmapFrame.Create(image));
                        encoder.Save(fs);
                    }

                }
            }
        }

        async void Init()
        {
            ////IsLoaded = true;
            //pipeHanlder.Init("DomofonApp");
            //pipeHanlder.UpdateCalled += PipeHanlder_UpdateCalled;

            CheckFile();

            IsLoaded = await contextLoader.LoadAsync<Product>();
            IsLoadingAnimation = false;

            if (IsLoaded)
            {
                //pageService.ChangeNewPage<Pages.CatalogPage>(defaultAnim);
                pageService.ChangeNewPage<Pages.AdminPage>(defaultAnim);
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
