using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MVVM_Core.Components
{
    class PageComponent
    {

        internal PageComponent(Type fromPage, Type toPage, ISliderAnimation animation, HistoryAdd toHistory)
        {
            FromPage = fromPage;
            ToPage = toPage;
            Animation = animation;
            HistoryAdd = toHistory;
        }

        public Type FromPage { get; }
        public Type ToPage { get; }
        public ISliderAnimation Animation { get; }
        public HistoryAdd HistoryAdd { get; }
    }
}
