﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Admin.Core
{
    public class ColumnBuilder<T>
    {
        private List<Column> _cols;

        public Column[] GetColumns()
        {
            return _cols?.ToArray();
        }

        public void AutoGenerate()
        {
            var type = typeof(T);

            var props = type.GetProperties();
            _cols = new List<Column>(props.Length);


            foreach (var prop in props)
            {
                string name = prop.Name;
                AddColumn(prop, name);
            }
        }

        private void AddColumn(PropertyInfo pr, string name)
        {
            if (pr.PropertyType.IsPrimitive || pr.PropertyType.IsValueType || pr.PropertyType == typeof(string))
            {
                if (_cols == null)
                    _cols = new List<Column>();

                _cols.Add(new Column(name, pr.Name));
            }
        }


        public void AddColumn<TValue>(Expression<Func<T, TValue>> func, string name = null)
        {
            string propName = Helper.GetPropertyName(func);
            var type = typeof(T);
            AddColumn(type.GetProperty(propName), name);
        }

        internal void Clear()
        {
            _cols?.Clear();
        }
    }
}