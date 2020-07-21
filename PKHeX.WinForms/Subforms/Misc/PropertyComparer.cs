using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace PKHeX.WinForms
{
    public sealed class PropertyComparer<T> : IComparer<T>
    {
        private readonly IComparer comparer;
        private PropertyDescriptor propertyDescriptor;
        private int reverse;

        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
        {
            propertyDescriptor = property;
            Type comparerForPropertyType = typeof(Comparer<>).MakeGenericType(property.PropertyType);
            comparer = (IComparer)comparerForPropertyType.InvokeMember("Default", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public, null, null, null);
            SetListSortDirection(direction);
        }

        #region IComparer<T> Members

        public int Compare(T x, T y)
        {
            return reverse * comparer.Compare(propertyDescriptor.GetValue(x), propertyDescriptor.GetValue(y));
        }

        #endregion

        private void SetPropertyDescriptor(PropertyDescriptor descriptor)
        {
            propertyDescriptor = descriptor;
        }

        private void SetListSortDirection(ListSortDirection direction)
        {
            reverse = direction == ListSortDirection.Ascending ? 1 : -1;
        }

        public void SetPropertyAndDirection(PropertyDescriptor descriptor, ListSortDirection direction)
        {
            SetPropertyDescriptor(descriptor);
            SetListSortDirection(direction);
        }
    }
}