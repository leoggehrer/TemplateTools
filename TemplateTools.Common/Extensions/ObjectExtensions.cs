//@BaseCode

using System.Reflection;

namespace TemplateTools.Common.Extensions
{
    /// <summary>
    /// Provides extension methods for objects.
    /// </summary>
    public static partial class ObjectExtensions
    {
        #region Check arguments
        /// <summary>
        /// Checks if the specified argument is null and throws an <see cref="ArgumentNullException"/> if it is.
        /// </summary>
        /// <param name="source">The object to check.</param>
        /// <param name="argName">The name of the argument.</param>
        public static void CheckArgument(this object? source, string argName)
        {
            if (source == null)
            throw new ArgumentNullException(argName);
        }
        #endregion Check arguments
        
        #region Equal methods
        /// <summary>
        /// Compares the properties of two objects to determine if they are equal, excluding any properties specified to be ignored.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="other">The other object to compare with.</param>
        /// <param name="ignore">The names of the properties to ignore during the comparison.</param>
        /// <returns><c>true</c> if the properties of the two objects are equal; otherwise, <c>false</c>.</returns>
        public static bool AreEqualProperties(this object source, object other, params string[] ignore)
        {
            static bool IsSimpleType(PropertyInfo propertyInfo)
            {
                var result = false;
                var underlingType = propertyInfo.GetUnderlyingType();
                
                if (underlingType != null)
                {
                    result = underlingType.IsSimpleType();
                }
                return result;
            }
            var sourceType = source.GetType();
            var otherType = other.GetType();
            var result = sourceType == otherType && source == other;
            
            if (sourceType == otherType)
            {
                var ignoreList = new List<string>(ignore);
                var unequalProperties =
                from pi in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where pi != null && !ignoreList.Contains(pi.Name) && pi.GetIndexParameters().Length == 0 && IsSimpleType(pi)
                let sourceValue = sourceType.GetProperty(pi.Name)?.GetValue(source, null)
                let otherValue = otherType.GetProperty(pi.Name)?.GetValue(other, null)
                where sourceValue != otherValue && (sourceValue == null || !sourceValue.Equals(otherValue))
                select sourceValue;
                
                result = unequalProperties.Any() == false;
            }
            return result;
        }
        #endregion Equal methods
        
        #region CopyTo methods
        /// <summary>
        /// Copies the properties of the source object to a new instance of type T and returns it.
        /// </summary>
        /// <typeparam name="T">The type of the object to copy to. Must be a reference type and have a default constructor.</typeparam>
        /// <param name="source">The object to copy properties from.</param>
        /// <returns>A new instance of type T with properties copied from the source object.</returns>
        public static T CopyTo<T>(this object source) where T : class, new()
        {
            var target = new T();
            
            CopyProperties(target, source);
            return target;
        }
        /// <summary>
        /// Copies the properties from the source object to a new instance of type T, based on the provided filter function.
        /// </summary>
        /// <typeparam name="T">The type of the new instance.</typeparam>
        /// <param name="source">The source object to copy properties from.</param>
        /// <param name="filter">The filter function that determines which properties to copy.</param>
        /// <returns>A new instance of type T with copied properties from the source object.</returns>
        public static T CopyTo<T>(this object source, Func<string, bool> filter) where T : class, new()
        {
            var target = new T();
            
            CopyProperties(target, source, filter, null);
            return target;
        }
        /// <summary>
        /// Copies the properties of an object to a new instance of type T.
        /// </summary>
        /// <typeparam name="T">The type of the target object.</typeparam>
        /// <param name="source">The source object to copy properties from.</param>
        /// <param name="mapping">An optional mapping function to modify property names.</param>
        /// <returns>A new instance of type T with copied properties.</returns>
        public static T CopyTo<T>(this object source, Func<string, string>? mapping) where T : class, new()
        {
            var target = new T();
            
            CopyProperties(target, source, null, mapping);
            return target;
        }
        /// <summary>
        /// Copies the properties from the source object to a new instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to create and copy the properties to. Must be a reference type and have a parameterless constructor.</typeparam>
        /// <param name="source">The source object to copy properties from.</param>
        /// <param name="filter">Optional. A function that determines which properties should be copied based on their names.</param>
        /// <param name="mapping">Optional. A function that determines how property names should be mapped from the source object to the target object.</param>
        /// <returns>A new instance of the specified type with copied properties from the source object.</returns>
        public static T CopyTo<T>(this object source, Func<string, bool>? filter, Func<string, string>? mapping) where T : class, new()
        {
            var target = new T();
            
            CopyProperties(target, source, filter, mapping);
            return target;
        }
        
        /// <summary>
        /// Copies the properties from the source object to the target object.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        public static void CopyTo(this object source, object target)
        {
            CopyProperties(target, source);
        }
        /// <summary>
        /// Copies properties from the source object to the target object, based on the provided filter.
        /// </summary>
        /// <param name="source">The source object from which to copy properties.</param>
        /// <param name="target">The target object to which properties are copied.</param>
        /// <param name="filter">A filter function to specify which properties to copy based on their name.</param>
        /// <remarks>
        /// This method copies properties from the source object to the target object using reflection.
        /// The filter function is used to determine which properties should be copied based on their name.
        /// </remarks>
        public static void CopyTo(this object source, object target, Func<string, bool> filter)
        {
            CopyProperties(target, source, filter, null);
        }
        /// <summary>
        /// Copies the properties from the source object to the target object using the provided property name mapping function.
        /// </summary>
        /// <param name="source">The source object to copy properties from.</param>
        /// <param name="target">The target object to copy properties to.</param>
        /// <param name="mapping">The property name mapping function to customize the property names before copying.</param>
        /// <remarks>This method is an extension method for the Object class.</remarks>
        /// <exception cref="ArgumentNullException">Thrown if either the source or target object is null.</exception>
        public static void CopyTo(this object source, object target, Func<string, string> mapping)
        {
            CopyProperties(target, source, null, mapping);
        }
        /// <summary>
        /// Copies the properties from the source object to the target object using filtering and mapping functions.
        /// </summary>
        /// <param name="source">The source object from which to copy the properties.</param>
        /// <param name="target">The target object where the properties will be copied to.</param>
        /// <param name="filter">A function that determines whether a property should be copied or not.</param>
        /// <param name="mapping">A function that maps the property names between the source and target objects.</param>
        /// <remarks>
        /// <para>
        /// This method allows for copying properties from one object to another using filtering and mapping functions.
        /// The <paramref name="filter"/> function is used to determine whether a property should be copied or not,
        /// and the <paramref name="mapping"/> function is used to map the property names between the source and target objects.
        /// </para>
        /// <para>
        /// The <paramref name="source"/> object is the object from which the properties will be copied,
        /// and the <paramref name="target"/> object is the object where the properties will be copied to.
        /// </para>
        /// </remarks>
        public static void CopyTo(this object source, object target, Func<string, bool> filter, Func<string, string> mapping)
        {
            CopyProperties(target, source, filter, mapping);
        }
        #endregion CobyTo methods
        
        #region CopyFrom methods
        /// <summary>
        /// Copies the properties from the source object to the target object.
        /// </summary>
        /// <param name="target">The target object to copy the properties to.</param>
        /// <param name="source">The source object to copy the properties from.</param>
        /// <remarks>If the source object is null, no properties will be copied.</remarks>
        public static void CopyFrom(this object target, object source)
        {
            if (source != null)
            {
                CopyProperties(target, source);
            }
        }
        /// <summary>
        /// Copies the properties from the source object to the target object based on the specified filter.
        /// </summary>
        /// <param name="target">The target object to copy properties to.</param>
        /// <param name="source">The source object to copy properties from.</param>
        /// <param name="filter">The filter function to determine which properties to copy.</param>
        public static void CopyFrom(this object target, object source, Func<string, bool> filter)
        {
            if (source != null)
            {
                CopyProperties(target, source, filter, null);
            }
        }
        /// <summary>
        /// Copies properties from the source object to the target object using the provided mapping function.
        /// </summary>
        /// <param name="target">The target object to copy properties to.</param>
        /// <param name="source">The source object to copy properties from.</param>
        /// <param name="mapping">The mapping function to map property names between the source and target objects.</param>
        public static void CopyFrom(this object target, object source, Func<string, string> mapping)
        {
            if (source != null)
            {
                CopyProperties(target, source, null, mapping);
            }
        }
        /// <summary>
        /// Copies properties from the source object to the target object based on the provided filter and mapping functions.
        /// </summary>
        /// <param name="target">The object to copy properties to.</param>
        /// <param name="source">The object to copy properties from.</param>
        /// <param name="filter">A function that determines if a property should be copied based on its name.</param>
        /// <param name="mapping">A function that maps the property name from the source object to the target object.</param>
        public static void CopyFrom(this object target, object source, Func<string, bool> filter, Func<string, string> mapping)
        {
            if (source != null)
            {
                CopyProperties(target, source, filter, mapping);
            }
        }
        #endregion CopyFrom methods
        
        #region CopyProperties methods
        /// <summary>
        /// Copies the properties from the source object to the target object.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="source">The source object.</param>
        public static void CopyProperties(object target, object source)
        {
            CopyProperties(target, source, null, null);
        }
        /// <summary>
        /// Copies the properties from the source object to the target object.
        /// </summary>
        /// <param name="target">The target object to copy the properties to.</param>
        /// <param name="source">The source object to copy the properties from.</param>
        /// <param name="filter">An optional filter function to selectively copy properties. Property names are passed as input
        /// to the filter function, and only properties for which the function returns true will be copied.</param>
        /// <param name="mapping">An optional mapping function that can be used to transform property names before copying.
        /// Property names are passed as input to the mapping function, and the transformed property names will be used
        /// when copying properties from source to target.</param>
        /// <remarks>
        /// This method uses reflection to retrieve property information from the target and source objects and
        /// copies the values from the source properties to the corresponding target properties.
        /// The <paramref name="filter"/> function can be used to specify whether certain properties should be copied or ignored.
        /// The <paramref name="mapping"/> function can be used to provide custom mapping of property names.
        /// </remarks>
        public static void CopyProperties(object target, object source, Func<string, bool>? filter, Func<string, string>? mapping)
        {
            Dictionary<string, PropertyItem> targetPropertyInfos = target.GetType().GetAllTypeProperties();
            Dictionary<string, PropertyItem> sourcePropertyInfos = source.GetType().GetAllTypeProperties();
            
            SetPropertyValues(target, source, filter, mapping, targetPropertyInfos, sourcePropertyInfos);
        }
        #endregion CopyProperties methods
        
#pragma warning disable IDE0060 // Remove unused parameter
        /// <summary>
        /// Calculates the hash code for the given objects.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="items">The objects to calculate the hash code for.</param>
        /// <returns>The calculated hash code.</returns>
        public static int CalculateHashCode(this object source, params object?[] items)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            static int CalculateHashCodeRec(object[] values)
            {
                var result = 0;
                
                if (values.Length == 1)
                {
                    result = HashCode.Combine(values[0]);
                }
                else if (values.Length == 2)
                {
                    result = HashCode.Combine(values[0], values[1]);
                }
                else if (values.Length == 3)
                {
                    result = HashCode.Combine(values[0], values[1], values[2]);
                }
                else if (values.Length == 4)
                {
                    result = HashCode.Combine(values[0], values[1], values[2], values[3]);
                }
                else if (values.Length == 5)
                {
                    result = HashCode.Combine(values[0], values[1], values[2], values[3], values[4]);
                }
                else if (values.Length == 6)
                {
                    result = HashCode.Combine(values[0], values[1], values[2], values[3], values[4], values[5]);
                }
                else if (values.Length == 7)
                {
                    result = HashCode.Combine(values[0], values[1], values[2], values[3], values[4], values[5], values[6]);
                }
                else if (values.Length == 8)
                {
                    result = HashCode.Combine(values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7]);
                }
                else
                {
                    result = HashCode.Combine(values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7]) + CalculateHashCodeRec([.. values.Skip(8).Take(values.Length - 8)]);
                }
                return result;
            }
            var noneNullValues = items.Where(i => i != null).ToArray();
            
            return CalculateHashCodeRec(noneNullValues!);
        }
        /// <summary>
        /// Sets the property values of a target object using the corresponding values from a source object based on the specified criteria.
        /// </summary>
        /// <param name="target">The target object to set the property values on.</param>
        /// <param name="source">The source object to get the property values from.</param>
        /// <param name="filter">A function used to filter the properties to be copied, defaults to copying all properties if not specified.</param>
        /// <param name="mapping">A function used to map the property names between the source and target objects, defaults to using the same property names if not specified.</param>
        /// <param name="targetPropertyInfos">A dictionary containing the property information of the target object.</param>
        /// <param name="sourcePropertyInfos">A dictionary containing the property information of the source object.</param>
        /// <remarks>
        /// This method sets the property values of the <paramref name="target"/> object based on the corresponding property values from the <paramref name="source"/> object.
        /// The properties to be copied can be filtered using the <paramref name="filter"/> function, which takes in the name of a property and returns true if the property should be copied.
        /// The property names can be mapped between the source and target objects using the <paramref name="mapping"/> function, which takes in the name of a property and returns the mapped name to be used in the target object.
        /// The property information of the target and source objects should be provided in the <paramref name="targetPropertyInfos"/> and <paramref name="sourcePropertyInfos"/> dictionaries, respectively.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="targetPropertyInfos"/> or <paramref name="sourcePropertyInfos"/> is null.</exception>
        private static void SetPropertyValues(object target, object source, Func<string, bool>? filter, Func<string, string>? mapping, Dictionary<string, PropertyItem> targetPropertyInfos, Dictionary<string, PropertyItem> sourcePropertyInfos)
        {
            filter ??= (n => true);
            mapping ??= (n => n);
            foreach (KeyValuePair<string, PropertyItem> propertyItemTarget in targetPropertyInfos)
            {
                if (sourcePropertyInfos.TryGetValue(mapping(propertyItemTarget.Value.PropertyInfo.Name), out var propertyItemSource))
                {
                    if (propertyItemSource.PropertyInfo.PropertyType == propertyItemTarget.Value.PropertyInfo.PropertyType
                    && ((propertyItemSource.CanReadAndIsPublic && propertyItemTarget.Value.CanWrite)
                    || (propertyItemSource.DeclaringType == propertyItemTarget.Value.DeclaringType && propertyItemSource.CanRead && propertyItemTarget.Value.CanWrite))
                    && (filter(propertyItemTarget.Value.PropertyInfo.Name)))
                    {
                        if (propertyItemSource.IsStringType)
                        {
                            object? value = propertyItemSource.PropertyInfo.GetValue(source);
                            
                            propertyItemTarget.Value.PropertyInfo.SetValue(target, value);
                        }
                        else if (propertyItemSource.IsArrayType)
                        {
                            object? value = propertyItemSource.PropertyInfo.GetValue(source);
                            
                            propertyItemTarget.Value.PropertyInfo.SetValue(target, value);
                        }
                        else if (propertyItemSource.PropertyInfo.PropertyType.IsValueType
                        && propertyItemTarget.Value.PropertyInfo.PropertyType.IsValueType)
                        {
                            object? value = propertyItemSource.PropertyInfo.GetValue(source);
                            
                            propertyItemTarget.Value.PropertyInfo.SetValue(target, value);
                        }
                        else if (propertyItemSource.IsComplexType)
                        {
                            object? value = propertyItemSource.PropertyInfo.GetValue(source);
                            
                            propertyItemTarget.Value.PropertyInfo.SetValue(target, value);
                        }
                    }
                }
            }
        }
    }
}

