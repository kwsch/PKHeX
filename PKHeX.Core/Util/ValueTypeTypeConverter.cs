using System;
using System.Collections;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Used for allowing a struct to be mutated in a PropertyGrid.
    /// </summary>
    public class ValueTypeTypeConverter : ExpandableObjectConverter
    {
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) => true;

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            object boxed = context.PropertyDescriptor.GetValue(context.Instance);
            foreach (DictionaryEntry entry in propertyValues)
            {
                var pi = context.PropertyDescriptor.PropertyType.GetProperty(entry.Key.ToString());
                if (pi?.CanWrite == true)
                    pi.SetValue(boxed, Convert.ChangeType(entry.Value, pi.PropertyType), null);
            }
            return boxed;
        }
    }
}
