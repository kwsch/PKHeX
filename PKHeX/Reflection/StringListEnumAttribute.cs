using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace PKHeX.Reflection
{
    /// <summary>
    /// Attribute for a property whose value is of type <see cref="int"/> and represents a value in an enumeration defined by <see cref="Util.getStringList(string, string)"/>.
    /// </summary>
    public class StringListEnumAttribute : BatchEditAttribute
    {

        public StringListEnumAttribute(string listName)
        {
            this.ListName = listName;
        }

        public string ListName { get; set; }

        private string CurrentLanguage
        {
            get
            {
                // Get the language part of the current culture (so if the culture is "en-US", get only "en")
                return Thread.CurrentThread.CurrentCulture.Name.Substring(0, 2);
            }
        }

        public override Type[] SupportedTypes
        {
            get
            {
                return new Type[] { typeof(int) };
            }
        }

        public override bool IsValueEqual(object instance, PropertyInfo property, object value)
        {
            var isString = value.GetType() == typeof(string);
            var isInt = value.GetType() == typeof(int);
            var currentRawValue = (int)property.GetValue(instance, null);
            int actual;
            if (isInt)
            {
                // Input is an integer.
                return (int)value == currentRawValue;
            }
            else if ((isString && int.TryParse((string)value, out actual)))
            {
                // Input is an integer string.
                return actual == currentRawValue;
            }
            else if (isString)
            {
                // Input is a string from the list (hopefully)
                var list = Util.getStringList(ListName, CurrentLanguage);
                var currentValue = list[currentRawValue];
                return (string)value == currentValue;
            }
            else
            {
                // Input is something else.
                throw new InvalidCastException("The StringListEnumAttribute is only to be used on properties of type 'int'.");
            }
        }

        public override void SetValue(object instance, PropertyInfo property, object value)
        {
            var isString = value.GetType() == typeof(string);
            var isInt = value.GetType() == typeof(int);
            int parsed;
            if (isInt)
            {
                // Input is an integer.
                property.SetValue(instance, value, null);
            }
            else if ((isString && int.TryParse((string)value, out parsed)))
            {
                // Input is an integer string.
                property.SetValue(instance, parsed, null);
            }
            else if (isString)
            {
                // Input is a string from the list (hopefully)
                var list = Util.getStringList(ListName, CurrentLanguage);
                var targetValue = value as string;
                if (list.Contains(targetValue))
                {
                    property.SetValue(instance, Array.FindIndex(list, (string x) => x == targetValue), null);
                }
                else
                {
                    throw new EnumItemNotInListException(targetValue);
                }
            }
            else
            {
                // Input is something else.
                throw new InvalidCastException("The StringListEnumAttribute is only to be used on properties of type 'int'.");
            }
        }

        public override object GetValue(object instance, PropertyInfo property)
        {
            var list = Util.getStringList(ListName, CurrentLanguage);
            var value = (int)property.GetValue(instance, null);
            return list[value];
        }
    }
}
