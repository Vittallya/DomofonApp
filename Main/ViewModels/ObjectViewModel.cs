using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Main.ViewModels
{

    public interface IObjectViewModel
    {
        StackPanel Stack { get; }
    }

    public class ObjectViewModel<T>: IObjectViewModel
    {
        private readonly bool isEdit;

        public T Item { get; protected set; }
        public StackPanel Stack { get; private set; } = new StackPanel();

        public ObjectViewModel(T item, bool isEdit, bool autoGenerate = true)
        {
            Item = item;
            this.isEdit = isEdit;

            if (autoGenerate)
                GenerateFields();
        }

        public void AddTextBox(string name, Func<T, string> propName, DependencyProperty binding = null)
        {
            if (binding == null)
                binding = TextBox.TextProperty;

            var tb = new TextBox();
            tb.SetBinding(binding, propName(Item));

            AddToStakPanel(tb, name);            
        }

        public void AddDatePicker(string name, Func<T, string> propName, DependencyProperty binding = null)
        {
            if (binding == null)
                binding = DatePicker.SelectedDateProperty;

            var tb = new DatePicker();
            tb.SetBinding(binding, propName(Item));

            AddToStakPanel(tb, name);            
        }

        public void AddCombobox<TControl, TModel>(string name,
            IEnumerable<TModel> objects,
            Func<T, string> propValue, 
            Func<TModel, string> propValuePath, 
            Func<TModel, string> propDisplay)
            where TControl: Selector, new()
        {

            var combo = new TControl();
            combo.ItemsSource = objects;
            combo.DisplayMemberPath = propDisplay(default);
            combo.SelectedValuePath = propValuePath(default);
            combo.SetBinding(Selector.SelectedValueProperty, propValue(Item));

            AddToStakPanel(combo, name);            
        }


        public void AddControl<TControl>(string displName, Dictionary<DependencyProperty, Func<T, string>> binds, TControl control = null)
            where TControl: Control, new()
        {
            if (control == null)
                control = new TControl();

            if(binds != null)
            {
                foreach(var b in binds)
                {
                    control.SetBinding(b.Key, b.Value(Item));
                }
            }

            AddToStakPanel(control, displName);
        }

        void GenerateFields()
        {
            var type = typeof(T);

            var props = type.GetProperties();

            foreach(var pr in props)
            {
                if(pr.PropertyType == typeof(DateTime))
                {
                    AddDatePicker(pr.Name, x => pr.Name);
                    
                }
                else if(pr.PropertyType == typeof(string) || pr.PropertyType.IsPrimitive || pr.PropertyType.IsValueType)
                {
                    AddTextBox(pr.Name, x => pr.Name);                    
                }
            }

        }

        void AddToStakPanel(Control control, string displayName)
        {
            var stack = new StackPanel();
            stack.Margin = new Thickness(0, 5, 0, 5);

            var label = new TextBlock();
            label.Text = displayName;

            stack.Children.Add(label);
            stack.Children.Add(control);

            if (stack.Parent is StackPanel parent)
                parent.Children.Remove(stack);

            Stack.Children.Add(stack);
        }

    }
}
