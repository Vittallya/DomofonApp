using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVM_Core
{
    public abstract class BasePageViewModel: BaseViewModel
    {
        protected readonly PageManager pageservice;

        public virtual int PoolIndex { get; }

        protected virtual ISliderAnimation BackSlideAnim => DisappearAnimation.Default;

        protected virtual void Back(object param)
        {
            pageservice.Back(BackSlideAnim);
        }

        protected virtual void Next(object param)
        {
            pageservice.Next();
        }

        protected virtual async Task BackAsync(object param)
        {
            pageservice.Back(BackSlideAnim);
        }

        protected virtual async Task NextAsync(object param)
        {
            pageservice.Next();
        }

        public ICommand BackCommand => new Command(x =>
        {
            Back(x);
        });

        public ICommand NextCommand => new Command(x =>
        {
            Next(x);
        });

        public ICommand BackCommandAsync => new CommandAsync(async x =>
        {
            await BackAsync(x);
        });

        public ICommand NextCommandAsync => new CommandAsync(async x =>
        {
            await NextAsync(x);
        });

        public BasePageViewModel(PageManager pageservice)
        {
            this.pageservice = pageservice;
        }
    }
}
