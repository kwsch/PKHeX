using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace PKHeX.Avalonia.Controls;

/// <summary>
/// An <see cref="ObservableCollection{T}"/> with sorting support for use with DataGrid columns.
/// Replaces the WinForms SortableBindingList pattern.
/// </summary>
public class SortableList<T> : ObservableCollection<T>
{
    /// <summary>
    /// Sorts the collection by the specified property name and direction.
    /// </summary>
    public void Sort(string propertyName, ListSortDirection direction = ListSortDirection.Ascending)
    {
        var prop = typeof(T).GetProperty(propertyName)
            ?? throw new ArgumentException($"Property '{propertyName}' not found on {typeof(T).Name}");

        var sorted = direction == ListSortDirection.Ascending
            ? this.OrderBy(x => prop.GetValue(x))
            : this.OrderByDescending(x => prop.GetValue(x));

        var items = sorted.ToList();
        ClearItems();
        foreach (var item in items)
            Add(item);
    }
}
