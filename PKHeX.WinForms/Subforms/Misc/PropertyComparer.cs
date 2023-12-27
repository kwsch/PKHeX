using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace PKHeX.WinForms;

public sealed class PropertyComparer<T> : IComparer<T> where T : class
{
    private readonly IComparer comparer;
    private PropertyDescriptor propertyDescriptor;
    private int reverse;

    public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
    {
        propertyDescriptor = property;
        Type comparerForPropertyType = typeof(Comparer<>).MakeGenericType(property.PropertyType);
        var ci = comparerForPropertyType.InvokeMember("Default", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public, null, null, null);
        comparer = ci == null ? new Comparer(CultureInfo.InvariantCulture) : (IComparer) ci;
        SetListSortDirection(direction);
    }

    #region IComparer<T> Members

    public int Compare(T? x, T? y)
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
