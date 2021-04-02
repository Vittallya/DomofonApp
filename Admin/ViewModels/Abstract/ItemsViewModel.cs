using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DAL;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using Admin.Services;
using System.Data.Entity;
using Admin.Components;
using System.Reflection;
using Admin.Events;

namespace Admin.ViewModels
{
    public abstract class ItemsViewModel<T> : BasePageViewModel, IItemsViewModel where T: class, new()
    {
        protected readonly PageService pageservice;
        protected readonly AllDbContext dbContext;
        protected readonly FieldsGenerator fieldsGenerator;
        protected readonly CloneItemsSerivce cloneItems;
        protected readonly EventBus eventBus;
        PropertyInfo idProp = typeof(T).GetProperty("Id");


        public virtual async Task<bool> CheckBeforeAdd(T item)
        {
            return true;
        }

        protected virtual async Task OnEdit()
        {

        }
        protected virtual async Task OnAdd()
        {

        }

        protected ItemsViewModel(PageService pageservice, 
            AllDbContext dbContext, 
            Services.FieldsGenerator fieldsGenerator,
            CloneItemsSerivce cloneItems, EventBus eventBus) : base(pageservice)
        {
            this.pageservice = pageservice;
            this.dbContext = dbContext;
            this.fieldsGenerator = fieldsGenerator;
            this.cloneItems = cloneItems;
            this.eventBus = eventBus;
            Init();

            
        }

        async void Init()
        {
            GenerateBindingVIew();

            if (fieldsGenerator.IsLastDetail)
            {
                await FromDetailsPage(fieldsGenerator.IsEdit, fieldsGenerator.Item as T);
                fieldsGenerator.Clear();
            }
            await LoadItems();

        }

        private void GenerateBindingVIew()
        {
            var list = BindingList.
                Where(x => x.PropertyType != PropertyType.OuterPropertyIdNonVisible).
                ToList();

            GridView.Columns.Clear();
            foreach (var bind in list)
            {
                GridViewColumn item = new GridViewColumn();

                item.DisplayMemberBinding = new Binding(bind.BindingPath);
                item.Header = bind.DisplayName;
                GridView.Columns.Add(item);
            }
        }

        public override int PoolIndex { get; } = 1;

        public T SelectedItem { get; set; }
        public ObservableCollection<T> Items { get; set; }

        public async Task Remove(T item)
        {
            dbContext.Set<T>().Remove(item);
            await dbContext.SaveChangesAsync();
            await OnDbChanged();
        }
        public async Task Add(T item)
        {
            dbContext.Set<T>().Add(item);
            await dbContext.SaveChangesAsync();
            await OnAdd();
            await OnDbChanged();
        }
        protected async Task Edit(T item)
        {
            var id = idProp.GetValue(item);
            T copy = await dbContext.Set<T>().FindAsync(id);

            cloneItems.Clone(item, copy);
            dbContext.Entry<T>(copy).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
            await OnEdit();
            await OnDbChanged(id);
        }

        protected virtual async Task OnDbChanged(object id = null)
        {
            await eventBus.Publish(new UpdatePipe(typeof(T), id));
        }

        public ICommand RemoveCommand => new CommandAsync( async x =>
        {
            await Remove(SelectedItem);
            await LoadItems();

        }, x => SelectedItem != null);

        public ICommand EditCommand => new Command(x =>
        {            
            ToDetailsPage(true);
        }, x => SelectedItem != null);

        public ICommand AddCommand => new Command(x =>
        {            
            ToDetailsPage(false);
        });
        public ICommand UpdateCommand => new CommandAsync(async x =>
        {
            await LoadItems();

        });

        protected virtual async Task LoadItems()
        {
            await dbContext.Set<T>().LoadAsync();
            Items = new ObservableCollection<T>(dbContext.Set<T>());
        }

        protected virtual async Task FromDetailsPage(bool isEdit, T item)
        {
            if (isEdit)
            {
                await Edit(item);
            }
            else if(await CheckBeforeAdd(item))
            {                
                await Add(item);
            }
        }

        protected virtual async void ToDetailsPage(bool isEdit)
        {
            T item = isEdit ? SelectedItem : new T();

            await fieldsGenerator.Generate(item, BindingList, isEdit);
            Locator.SetDetailsViewModel<T>();
            pageservice.ChangePage<Pages.ItemDetailPage>(PoolIndex, DisappearAndToSlideAnim.ToLeft);
        }


        public GridView GridView { get; set; } = new GridView();

        public StackPanel StackPanel { get; set; } = new StackPanel();

        public abstract BindingComponent[] BindingList { get; }
    }
}
