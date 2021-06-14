using MVVM_Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MVVM_Core
{
    public enum HistoryAdd
    {
        None, Usual, Temp
    }


    public class PageManager
    {
        public event Action<Page, ISliderAnimation> PageChanged;

        public void ChangeNewPage<TPage>(ISliderAnimation anim) where TPage : Page, new()
        {
            ChangeNewPage(typeof(TPage), anim);
        }



        public void ChangeNewPage(Type pageType, ISliderAnimation anim)
        {
            Page page = Activator.CreateInstance(pageType) as Page;
            AddToHistory(page);
            OnChangePage(page, anim);
        }


        private void AddToHistory(Page page)
        {
            int index = _history.IndexOf(page);

            if (index > -1)
            {
                _history[index] = page;
            }
            else
            {
                _history.Add(page);
            }
        }


        public Page CurrentPage { get; private set; }

        public void OnChangePage<TPage>(TPage target, ISliderAnimation anim) where TPage : Page, new()
        {
            CurrentPage = target;
            PageChanged?.Invoke(target, anim);
        }

        private List<PageComponent> invokers = new List<PageComponent>();


        public void SetupNext<FromTPage, ToPage>(ISliderAnimation animation, HistoryAdd historyAdd = HistoryAdd.Usual) where FromTPage : Page, new()
            where ToPage : Page, new()
        {
            var type = typeof(FromTPage);

            int index = invokers.FindIndex(x => x.FromPage == type);

            var component = new PageComponent(type, typeof(ToPage), animation, historyAdd);

            if (index == -1)
            {
                invokers.Add(component);
            }

            else
            {
                invokers[index] = component;
            }

        }

        public void GoToNextCheckPoint()
        {
            throw new NotImplementedException();
        }

        public bool IsCurrentPageTemp { get; private set; }

        public void ChangeNewPageTemp<TPage>(ISliderAnimation anim) where TPage : Page, new()
        {
            TPage page = new TPage();
            _temp.Add(page);
            IsCurrentPageTemp = true;
            OnChangePage(page, anim);
        }
        public void BackTemp<TPage>(ISliderAnimation anim, bool reload = false) where TPage: Page, new()
        {
            if (_temp.Count > 1) 
            {
                var target = _temp.FindIndex(x => x.GetType() == typeof(TPage));

                if (target > -1)
                {
                    BackTo(anim, reload, target, _temp);
                }
            }
        }

        public void BackTemp(ISliderAnimation anim, bool reload = false)
        {
            if (_temp.Count > 1) 
            {
                var target = _temp.Count - 2;
                BackTo(anim, reload, target, _temp);
            }
        }


        public int NextCheckPoint { get; private set; }
        public int PreviewCheckPoint { get; private set; }

        public void SetupNextCheckPointCurrent()
        {
            NextCheckPoint = _history.IndexOf(CurrentPage);
        }

        public void SetupNextCheckPoint<T>()
        {
            throw new NotImplementedException();
        }

        public void SetupPreviewCheckPointCurrent()
        {
            PreviewCheckPoint = _history.IndexOf(CurrentPage);
        }

        public void SetupPreviewCheckPoint<T>()
        {
            throw new NotImplementedException();
        }
        public void GoToPreviewCheckPoint(ISliderAnimation backSlideAnim, bool v)
        {
            throw new NotImplementedException();
        }



        public void ReloadCurrentPage(ISliderAnimation animation)
        {
            CurrentPage = Activator.CreateInstance(CurrentPage.GetType()) as Page;
            OnChangePage(CurrentPage, animation);

        }


        public bool Next()
        {
            var curntType = CurrentPage.GetType();

            var comp = invokers.FirstOrDefault(x => x.FromPage == curntType);

            if (comp != null)
            {
                ChangeNewPage(comp.ToPage, comp.Animation);
                invokers.RemoveAll(x => x.ToPage == comp.ToPage);
                return true;
            }
            return false;
        }


        List<Page> _history = new List<Page>();

        List<Page> _temp = new List<Page>();

        public void Back(ISliderAnimation anim = null, bool needReload = false)
        {
            if (_history.Count > 1)
            {
                BackTo(anim, needReload, _history.Count - 2, _history);
            }            
        }



        public void Back<TPage>(ISliderAnimation anim = null, bool needReload = false) where TPage : Page, new()
        {
            if (_history.Count > 1)
            {
                var target = _history.FindIndex(x => x.GetType() == typeof(TPage));

                if (target > -1)
                    BackTo(anim, needReload, target, _history);
                else
                    throw new ArgumentException("");
            }
        }

        private void BackTo(ISliderAnimation anim, bool needReload, int targetIndex, List<Page> history)
        {
            if (needReload)
            {
                history[targetIndex] = Activator.CreateInstance(history[targetIndex].GetType()) as Page;
            }

            
            history.RemoveRange(targetIndex + 1, _history.Count - targetIndex - 1);
            OnChangePage(history[targetIndex], anim);
        }

        public void ClearNext()
        {
            invokers.Clear();
        }


        public void ClearHistory()
        {
            _history.Clear();
        }
        public void ClearHistoryAndChangeTo<TPage>(ISliderAnimation animation = null) where TPage : Page, new()
        {
            ClearHistory();
            ChangeNewPage<TPage>(animation);
        }

    }
}
