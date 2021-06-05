using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PKHeX.WinForms
{
    public abstract class SortableBindingList<T> : BindingList<T> where T : class
    {
        private readonly Dictionary<Type, PropertyComparer<T>> comparers;
        private bool isSorted;
        private ListSortDirection listSortDirection;
        private PropertyDescriptor? propertyDescriptor;

        protected SortableBindingList() : base(new List<T>())
        {
            comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        protected override bool SupportsSortingCore => true;

        protected override bool IsSortedCore => isSorted;

        protected override PropertyDescriptor? SortPropertyCore => propertyDescriptor;

        protected override ListSortDirection SortDirectionCore => listSortDirection;

        protected override bool SupportsSearchingCore => true;

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            List<T> itemsList = (List<T>)Items;

            Type propertyType = prop.PropertyType;
            if (!comparers.TryGetValue(propertyType, out var comparer))
            {
                comparer = new PropertyComparer<T>(prop, direction);
                comparers.Add(propertyType, comparer);
            }

            comparer.SetPropertyAndDirection(prop, direction);
            itemsList.Sort(comparer);

            propertyDescriptor = prop;
            listSortDirection = direction;
            isSorted = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            isSorted = false;
            propertyDescriptor = base.SortPropertyCore;
            listSortDirection = base.SortDirectionCore;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override int FindCore(PropertyDescriptor prop, object key)
        {
            int count = Count;
            for (int i = 0; i < count; ++i)
            {
                var obj = this[i];
                var val = prop.GetValue(obj);
                if (val?.Equals(key) == true)
                    return i;
            }

            return -1;
        }
    }
}