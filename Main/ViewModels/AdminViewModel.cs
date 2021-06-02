using MVVM_Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using BL;
using System.Collections.ObjectModel;
using DAL.Models;
using DAL.Dto;
using System.Windows.Input;
using System.Windows;
using System.IO;
using System.Data.Entity;
using Main.Windows;
using MVVM_Core.Validation;

namespace Main.ViewModels
{
    public class AdminViewModel : BasePageViewModel
    {
        private readonly AllDbContext dbContext;
        private readonly Validator validator;
        private readonly FileBrowserService fileBrowser;
        private readonly MapperService mapper;
        private Window _window;

        public ObservableCollection<ProductDto> Products { get; set; }
        public ObservableCollection<CommonSale> Sales { get; set; }
        public ObservableCollection<DAL.Models.Order> Orders { get; set; }
        public ObservableCollection<ServiceDto> Services { get; set; }

        private ObjectViewModel<ProductDto> _productVm;
        private ObjectViewModel<CommonSale> _saleVm;
        private ObjectViewModel<ServiceDto> _serviceVm;
        private string _imageCatalog;
        private string selectedDir;
        private bool hasChanges;

        public AdminViewModel(PageManager pageservice,
                              AllDbContext dbContext,
                              Validator validator,
                              FileBrowserService fileBrowser,
                              MapperService mapper) : base(pageservice)
        {
            this.dbContext = dbContext;
            this.validator = validator;
            this.fileBrowser = fileBrowser;
            this.mapper = mapper;
            Init();
        }

        public object Item { get; set; }
        public object Param1 { get; set; }
        public bool IsEdit { get; set; }

        Func<Task> _invoker;

        public async Task ReloadProducts()
        {
            await dbContext.Products.LoadAsync();
            var list = await dbContext.Products.AsNoTracking().ToListAsync();

            Products = new ObservableCollection<ProductDto>(
                list.Select(x =>
                {
                    ProductDto n = mapper.MapTo<Product, ProductDto>(x);
                    n.ImageFullPath = $"{DefalutImageCatalog}\\{x.ImagePath}";
                    return n;
                }));
        }

        public async Task ReloadServices()
        {
            await dbContext.Services.LoadAsync();
            var list = await dbContext.Services.AsNoTracking().ToListAsync();

            Services = new ObservableCollection<ServiceDto>(
                list.Select(x => mapper.MapTo<Service, ServiceDto>(x)));
        }

        public async Task ReloadSales()
        {
            await dbContext.CommonSales.Include(x => x.Product).LoadAsync();
            var list = await dbContext.CommonSales.AsNoTracking().ToListAsync();
            Sales = new ObservableCollection<CommonSale>(list);
        }

        private async void Init()
        {
            DefalutImageCatalog = File.ReadAllLines(Rules.Static.FileName)[0];
            hasChanges = false;
            await ReloadProducts();
            await ReloadServices();
            await ReloadSales();
        }

        #region Товар
        public ICommand AddProduct => new Command(x =>
        {
            validator.Clear();
            Product product = new Product();
            Item = product;

            validator.ForProperty(() => product.Cost, "Стоимость").MoreEqualThan(0);
            validator.ForProperty(() => product.Manufacturer, "Производитель").NotEmpty();
            validator.ForProperty(() => product.Name, "Название").NotEmpty();
            IsEdit = false;

            _invoker = async () =>
            {
                dbContext.Products.Add(product);
                await dbContext.SaveChangesAsync();

                var dto = mapper.MapTo<Product, ProductDto>(product);
                dto.ImageFullPath = $"{DefalutImageCatalog}\\{product.ImagePath}";
                Products.Add(dto);
            };

            ShowWindow<ProductWindow>();
        });

        public ICommand EditProduct => new Command(x =>
        {
            if(x is ProductDto dto)
            {
                validator.Clear();

                int index = Products.IndexOf(dto);

                var product = mapper.MapTo<ProductDto, Product>(dto);
                Item = product;
                Param1 = product.ImagePath;

                validator.ForProperty(() => product.Cost, "Стоимость").MoreEqualThan(0);
                validator.ForProperty(() => product.Manufacturer, "Производитель").NotEmpty();
                validator.ForProperty(() => product.Name, "Название").NotEmpty();
                IsEdit = true;

                _invoker = async () =>
                {

                    dto = mapper.MapTo<Product, ProductDto>(product);
                    dto.ImageFullPath = $"{DefalutImageCatalog}\\{product.ImagePath}";

                    Products[index] = dto;

                    dbContext.Entry(await dbContext.Products.FindAsync(product.Id)).State = EntityState.Detached;
                    dbContext.Entry(product).State = EntityState.Modified;

                    await dbContext.SaveChangesAsync();
                    await ReloadSales();
                };

                ShowWindow< ProductWindow>();
            }

            
        });

        public ICommand DeleteProduct => new CommandAsync(async x =>
        {

            if(x is ProductDto dto)
            {
                dbContext.Products.Remove(await dbContext.Products.FindAsync(dto.Id));
                await dbContext.SaveChangesAsync(); 
                await ReloadSales();

                Products.Remove(dto);
            }
        });
        #endregion

        #region Услуга
        public ICommand AddService => new Command(x =>
        {
            validator.Clear();
            Service item = new Service();
            Item = item;

            validator.ForProperty(() => item.Cost, "Стоимость").MoreEqualThan(0);
            validator.ForProperty(() => item.Name, "Название").NotEmpty();
            IsEdit = false;

            _invoker = async () =>
            {
                dbContext.Services.Add(item);
                await dbContext.SaveChangesAsync();

                var dto = mapper.MapTo<Service, ServiceDto>(item);
                Services.Add(dto);
            };
            ShowWindow<ServiceWindow>();
        });

        public ICommand EditService => new Command(x =>
        {
            if(x is ServiceDto dto)
            {
                validator.Clear();

                int index = Services.IndexOf(dto);

                var item = mapper.MapTo<ServiceDto, Service>(dto);
                Item = item;

                validator.ForProperty(() => item.Cost, "Стоимость").MoreEqualThan(0);
                validator.ForProperty(() => item.Name, "Название").NotEmpty();
                IsEdit = true;

                _invoker = async () =>
                {

                    dto = mapper.MapTo<Service, ServiceDto>(item);

                    Services[index] = dto;

                    dbContext.Entry(await dbContext.Services.FindAsync(item.Id)).State = EntityState.Detached;
                    dbContext.Entry(item).State = EntityState.Modified;

                    await dbContext.SaveChangesAsync();
                };

                ShowWindow<ServiceWindow>();
            }

            
        });

        public ICommand DeleteService => new CommandAsync(async x =>
        {

            if(x is ServiceDto dto)
            {
                dbContext.Services.Remove(await dbContext.Services.FindAsync(dto.Id));
                await dbContext.SaveChangesAsync();
                Services.Remove(dto);
            }
        });

#endregion

        public ICommand AddSale => new Command(x =>
        {
            validator.Clear();
            CommonSale item = new CommonSale();
            Item = item;

            validator.ForProperty(() => item.SaleValue, "Скидка").MoreEqualThan(0);
            validator.ForProperty(() => item.ProductId, "Товар").MoreThan(0, "Товар должен быть указан");
            validator.ForProperty(() => item.StartCount, "Количество товаров").MoreEqualThan(0);

            Param1 = Products;

            IsEdit = false;

            _invoker = async () =>
            {
                dbContext.CommonSales.Add(item);
                await dbContext.SaveChangesAsync();
                await dbContext.Entry(item).Reference(y => y.Product).LoadAsync();
                Sales.Add(item);
            };

            ShowWindow<SaleWindow>();
        });

        public ICommand EditSale => new Command(x =>
        {
            if(x is CommonSale orig)
            {
                validator.Clear();

                int index = Sales.IndexOf(orig);

                orig.Product = null;

                var item = mapper.GetCopy(orig);
                Item = item;

                validator.ForProperty(() => item.SaleValue, "Скидка").MoreEqualThan(0);
                validator.ForProperty(() => item.ProductId, "Товар").MoreThan(0, "Товар должен быть указан");
                validator.ForProperty(() => item.StartCount, "Количество товаров").MoreEqualThan(0);

                Param1 = Products;

                IsEdit = true;

                _invoker = async () =>
                {
                    dbContext.Entry(await dbContext.CommonSales.FindAsync(item.Id)).State = EntityState.Detached;
                    dbContext.Entry(item).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                    Sales[index] = item;

                };

                ShowWindow<SaleWindow>();
            }

            
        });

        public ICommand DeleteSale => new CommandAsync(async x =>
        {

            if(x is CommonSale item)
            {
                dbContext.CommonSales.Remove(await dbContext.CommonSales.FindAsync(item.Id));
                await dbContext.SaveChangesAsync();
                Sales.Remove(item);
            }
        });

        public ICommand OpenFileDialogCommand => new Command(x =>
        {

            fileBrowser.GetImageFile(DefalutImageCatalog, out string path);
            (Item as Product).ImagePath = Path.GetFileName(path);
            Param1 = (Item as Product).ImagePath;

        });

        void ShowWindow<TWindow>()
            where TWindow: Window, new()
        {
            _window = new TWindow();
            _window.DataContext = this;
            _window.ShowDialog();
        }


        public ICommand AcceptCommand => new CommandAsync(async x =>
        {
            if (validator.IsCorrect)
            {
                _window.DialogResult = true;
                await _invoker?.Invoke();
                Param1 = null;
            }
            else
            {
                MessageBox.Show(validator.ErrorMessage);
            }

        });


        protected override void Back(object param)
        {
            pageservice.Back<Pages.CatalogPage>(BackSlideAnim, true);
        }

        public ICommand AcceptPath => new Command(x =>
        {
            try
            {
                if (!Directory.Exists(DefalutImageCatalog))
                {
                    Directory.CreateDirectory(DefalutImageCatalog);
                }

                File.WriteAllText(Rules.Static.FileName, DefalutImageCatalog);
                hasChanges = false;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }, y => hasChanges);

        public ObservableCollection<string> PathVariants { get; set; } = new ObservableCollection<string>();


        public bool IsVariantsVis { get; set; }


        void GetVariants(string path)
        {
            IsVariantsVis = false;

            if (path == null || path.Length == 0 || (path.Length <= 2 && path.IndexOf('\\') == -1))
            {
                var drives = DriveInfo.GetDrives().Select(x => x.Name);

                if(path.Length > 0)
                {
                    drives = drives.Where(x => x.ToLower().Contains(path.ToLower()));
                }

                IsVariantsVis = true;
                PathVariants = new ObservableCollection<string>(drives);
            }
            //C:\\
            else if ( path.IndexOf('\\') == 2 )
            {

                if(path[path.Length - 1] == '\\' && Directory.Exists(path))
                {

                    try
                    {
                        var dirs = Directory.GetDirectories(path).Select(x => x + '\\');
                        IsVariantsVis = true;
                        PathVariants = new ObservableCollection<string>(dirs);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
                else
                {
                    try
                    {
                        string pathBefore = Path.GetDirectoryName(path);

                        var dirs = Directory.GetDirectories(pathBefore).Select(x => x + '\\');

                        var a = dirs.Where(x => x.ToLower().Contains(path.ToLower()));
                        IsVariantsVis = true;
                        PathVariants = new ObservableCollection<string>(a);
                    }
                    catch { }
                }

            }


        }

        public ICommand SetVariant => new Command(x =>
        {
            DefalutImageCatalog = x?.ToString();
        });


        public string DefalutImageCatalog
        {
            get => _imageCatalog;
            set
            {
                if (value == _imageCatalog) return;
                GetVariants(value);
                _imageCatalog = value;
                hasChanges = true;
                OnPropertyChanged(nameof(DefalutImageCatalog));
            }


        }


    }
}