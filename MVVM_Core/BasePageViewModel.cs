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
        protected readonly PageService pageservice;

        public abstract int PoolIndex { get; }

        protected virtual ISliderAnimation BackSlideAnim => DisappearAnimation.Default;

        protected virtual void Back()
        {
            pageservice.Back(PoolIndex, BackSlideAnim);
        }

        protected virtual void Next()
        {

        }

        public ICommand BackCommand => new Command(x =>
        {
            Back();
        });

        public ICommand NextCommand => new Command(x =>
        {
            Next();
        });

        public BasePageViewModel(PageService pageservice)
        {
            this.pageservice = pageservice;
        }
    }
}
