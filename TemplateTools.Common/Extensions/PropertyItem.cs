//@BaseCode

using System.Reflection;

namespace TemplateTools.Common.Extensions
{
    /// <summary>
    /// Represents an item that encapsulates information about a property.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="PropertyItem"/> class with the specified <see cref="PropertyInfo"/>.
    /// </remarks>
    /// <param name="propertyInfo">The <see cref="PropertyInfo"/> object.</param>
    public partial class PropertyItem(PropertyInfo propertyInfo)
    {

        /// <summary>
        /// Determines if the property type is a string.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the property type is a string; otherwise, <c>false</c>.
        /// </returns>
        public bool IsStringType => PropertyInfo.PropertyType == typeof(string);
        
        /// <summary>
        /// Gets a value indicating whether the underlying property type is an array.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the property type is an array; otherwise, <c>false</c>.
        /// </returns>
        public bool IsArrayType => PropertyInfo.PropertyType.IsArray;
        
        /// <summary>
        /// Determines whether the property type is a complex type.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the property type is a complex type; otherwise, <c>false</c>.
        /// </returns>
        public bool IsComplexType => PropertyInfo.PropertyType.GetTypeInfo().IsValueType == false;
        /// Gets the type that declares the current property.
        /// @returns The Type object representing the type that declares the current property, or null if the property is global.
        public Type? DeclaringType => PropertyInfo.DeclaringType;
        
        /// <summary>
        /// Determines if the property can be read.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the property can be read; otherwise, <c>false</c>.
        /// </returns>
        public bool CanRead => PropertyInfo.CanRead;
        /// <summary>
        /// Determines if the associated property can be read and if the get accessor of the property is public.
        /// </summary>
        /// <returns>
        /// True if the property can be read and the get accessor is public; otherwise, false.
        /// </returns>
        public bool CanReadAndIsPublic => PropertyInfo.CanRead && PropertyInfo.GetGetMethod(true)!.IsPublic;
        /// <summary>
        /// Gets a value indicating whether the property can be written to.
        /// </summary>
        /// <returns>
        /// True if the property can be written to; otherwise, false.
        /// </returns>
        public bool CanWrite => PropertyInfo.CanWrite;
        /// <summary>
        /// Checks if the write access is enabled and the property's set method is public.
        /// </summary>
        /// <returns>
        /// True if the write access is enabled and the property's set method is public; otherwise, false.
        /// </returns>
        public bool CanWriteAndIsPublic => PropertyInfo.CanWrite && PropertyInfo.GetSetMethod(true)!.IsPublic;

        /// <summary>
        /// Gets or sets the PropertyInfo instance of the property.
        /// </summary>
        /// <value>
        /// The PropertyInfo instance of the property.
        /// </value>
        public PropertyInfo PropertyInfo { get; private set; } = propertyInfo;
        /// <summary>
        /// Gets or sets the property items dictionary.
        /// </summary>
        /// <value>The property items dictionary.</value>
        public Dictionary<string, PropertyItem> PropertyItems { get; private set; } = [];
    }
}

