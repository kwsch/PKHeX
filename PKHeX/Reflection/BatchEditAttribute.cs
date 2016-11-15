using System;
using System.Reflection;

namespace PKHeX.Reflection
{
    /// <summary>
    /// Base class for an attribute of a property providing special behavior in the Batch Editor.
    /// </summary>
    public abstract class BatchEditAttribute: Attribute
    {

        /// <summary>
        /// Array of types the current implementation of BatchEditAttribute supports.
        /// </summary>
        public abstract Type[] SupportedTypes { get; }

        /// <summary>
        /// Determines whether or not the value of the property is equivalent to the provided value.
        /// </summary>
        /// <param name="instance">Instance of the object containing the given property.</param>
        /// <param name="property">The property this attribute marks.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>A boolean indicating whether or not the value of the property is equivalent to the provided value.</returns>
        public abstract bool IsValueEqual(object instance, PropertyInfo property, object value);

        /// <summary>
        /// Sets the value of the property to the given value.
        /// </summary>
        /// <param name="instance">Instance of the object containing the given property.</param>
        /// <param name="property">The property this attribute marks.</param>
        /// <param name="value">The value to set.</param>
        public abstract void SetValue(object instance, PropertyInfo property, object value);

        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        /// <param name="instance">Instance of the object containing the given property.</param>
        /// <param name="property">The property this attribute marks.</param>
        public abstract object GetValue(object instance, PropertyInfo property);
    }
}
