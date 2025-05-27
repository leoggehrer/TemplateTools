//@BaseCode

namespace TemplateTools.Logic.Generation
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;
    using TemplateTools.Logic.Common;
    using TemplateTools.Logic.Contracts;

    /// <summary>
    /// Represents a generator object.
    /// </summary>
    internal abstract partial class GeneratorObject
    {
        /// Initializes the static members of the GeneratorObject class.
        static GeneratorObject()
        {
            ClassConstructing();
            
            ClassConstructed();
        }
        /// <summary>
        /// Invoked when the class is being constructed.
        /// </summary>
        /// <remarks>
        /// This method is invoked during the class construction process, before the constructor is called.
        /// It can be used to perform any initialization or setup before the object is fully constructed.
        /// </remarks>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called upon the construction of the class.
        /// </summary>
        /// <remarks>
        /// Add any additional remarks about the method here.
        /// </remarks>
        /// <seealso cref="ClassName"/>
        static partial void ClassConstructed();
        
        #region properties
        /// <summary>
        /// Gets or sets the configuration of the application.
        /// </summary>
        public Configuration Configuration { get; init; }
        /// <summary>
        /// Gets or sets the solution properties.
        /// </summary>
        /// <value>
        /// The solution properties.
        /// </value>
        public ISolutionProperties SolutionProperties { get; init; }
        #endregion properties
        
        #region constructor
        ///<summary>
        /// Initializes a new instance of the GeneratorObject class with the specified solutionProperties.
        ///</summary>
        ///<param name="solutionProperties">The solution properties to be used.</param>
        public GeneratorObject(ISolutionProperties solutionProperties)
        {
            Constructing();
            SolutionProperties = solutionProperties;
            Configuration = new Configuration(solutionProperties);
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of an object.
        /// It is used for any initialization or setup tasks needed.
        /// </summary>
        /// <remarks>
        /// This method is partial and should be implemented in another part of the class.
        /// </remarks>
        /// <seealso cref="OtherMethod()" />
        /// <seealso cref="AnotherMethod(int)" />
        partial void Constructing();
        /// <summary>
        /// This method is called when the object is fully constructed and ready for use.
        /// </summary>
        partial void Constructed();
        #endregion constructor
        
        #region query settings
        /// <summary>
        /// Returns the value of a specific query generation setting.
        /// </summary>
        /// <param name="unitType">The type of unit.</param>
        /// <param name="itemType">The type of item.</param>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="valueName">The name of the value.</param>
        /// <param name="defaultValue">The default value if the setting is not found.</param>
        /// <returns>The value of the specified query generation setting.</returns>
        public string QueryGenerationSettingValue(string unitType, string itemType, string itemName, string valueName, string defaultValue)
        {
            return Configuration.QuerySettingValue(unitType, itemType, itemName, valueName, defaultValue);
        }
        /// <summary>
        /// Retrieves the value of a query generation setting based on specified parameters.
        /// </summary>
        /// <param name="unitType">The unit type used for the query generation setting.</param>
        /// <param name="itemType">The item type used for the query generation setting.</param>
        /// <param name="itemName">The name of the item used for the query generation setting.</param>
        /// <param name="valueName">The name of the value used for the query generation setting.</param>
        /// <param name="defaultValue">The default value to return if the queried setting is not found.</param>
        /// <returns>The value of the query generation setting.</returns>
        public string QueryGenerationSettingValue(UnitType unitType, ItemType itemType, string itemName, string valueName, string defaultValue)
        {
            return Configuration.QuerySettingValue(unitType, itemType, itemName, valueName, defaultValue);
        }
        #endregion query settings
        
        #region helpers
        
        #region comment-helpers
        /// <summary>
        /// This method generates a comment.
        /// </summary>
        /// <returns>A collection of comments</returns>
        protected virtual IEnumerable<string> CreateComment()
        {
            var result = new List<string>()
            {
                "/// <summary>",
                "/// Generated by the generator",
                "/// </summary>",
            };
            return result;
        }
        /// <summary>
        /// Creates a comment for the specified comment.
        /// </summary>
        /// <param name="comment">The comment text.</param>
        /// <returns>An enumerable collection of strings representing the generated comment.</returns>
        protected virtual IEnumerable<string> CreateComment(string comment)
        {
            var result = new List<string>()
            {
                "/// <summary>",
                $"/// {comment}",
                "/// </summary>",
            };
            return result;
        }
        /// <summary>
        /// Creates a comment for the specified type.
        /// </summary>
        /// <param name="type">The type to create a comment for.</param>
        /// <returns>A collection of strings representing the generated comment.</returns>
        protected virtual IEnumerable<string> CreateComment(Type type)
        {
            var result = new List<string>()
            {
                "/// <summary>",
                "/// Generated by the generator.",
                "/// </summary>",
            };
            return result;
        }
        /// <summary>
        /// Creates a comment for the given property information.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> object representing the property.</param>
        /// <returns>An <see cref="IEnumerable{String}"/> containing the comment for the property.</returns>
        protected virtual IEnumerable<string> CreateComment(PropertyInfo propertyInfo)
        {
            var result = new List<string>()
            {
                "/// <summary>",
                $"/// Property '{propertyInfo.Name}' generated by the generator.",
                "/// </summary>",
            };
            return result;
        }
        #endregion Comment-Helpers
        
        #region Property-Helpers
        /// <summary>
        /// Determines whether the specified property is a foreign key and retrieves the foreign key name.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="name">The name of the foreign key.</param>
        /// <returns><c>true</c> if the property is a foreign key; otherwise, <c>false</c>.</returns>
        public static bool HasForeignKeyDefined(PropertyInfo propertyInfo, out string name)
        {
            bool result;
            var attribute = propertyInfo.GetCustomAttribute<ForeignKeyAttribute>();

            if (attribute != default)
            {
                result = true;
                name = attribute.Name ?? propertyInfo.Name.Replace("Id", string.Empty);
            }
            else
            {
                result = false;
                name = string.Empty;
            }
            return result;
        }
        /// <summary>
        /// Checks if the specified property has the RequiredAttribute defined.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>True if the property has the RequiredAttribute defined; otherwise, false.</returns>
        public static bool HasRequiredDefined(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<RequiredAttribute>() != null;
        }
        /// <summary>
        /// Determines if a property has a maximum length defined using the MaxLengthAttribute.
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo object representing the property.</param>
        /// <param name="maxLength">The maximum length defined for the property.</param>
        /// <returns>True if the property has a maximum length defined, false otherwise.</returns>
        public static bool HasMaxLengthDefined(PropertyInfo propertyInfo, out int maxLength)
        {
            var result = false;
            
            maxLength = 0;
            if (propertyInfo.PropertyType == typeof(string))
            {
                var maxLengthAttribute = propertyInfo.GetCustomAttribute<MaxLengthAttribute>();
                
                if (maxLengthAttribute != null)
                {
                    maxLength = maxLengthAttribute.Length;
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// Checks if the specified property has a minimum length defined.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> object representing the property.</param>
        /// <param name="minLength">When this method returns, contains the minimum length defined for the property, if found; otherwise, 0.</param>
        /// <returns><c>true</c> if the property has a minimum length defined; otherwise, <c>false</c>.</returns>
        public static bool HasMinLengthDefined(PropertyInfo propertyInfo, out int minLength)
        {
            var result = false;
            
            minLength = 0;
            if (propertyInfo.PropertyType == typeof(string))
            {
                var minLengthAttribute = propertyInfo.GetCustomAttribute<MinLengthAttribute>();
                
                if (minLengthAttribute != null)
                {
                    minLength = minLengthAttribute.Length;
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// Determines whether the specified <see cref="PropertyInfo"/> object represents a reference property like a Id or ArtistId or MenuId_Parent.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> object to check.</param>
        /// <returns>
        ///   <c>true</c> if the property is a reference property; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsReferenceProperty(PropertyInfo propertyInfo)
        {
            var result = false;
            var idText = "Id";
            
            if (propertyInfo.Name.Length > 2 && propertyInfo.Name.EndsWith(idText))
            {
                result = true;
            }
            else if (propertyInfo.Name.Contains($"{idText}_"))
            {
                var idx = propertyInfo.Name.IndexOf($"{idText}_");
                
                if (idx > 0 && idx + idText.Length + 1 < propertyInfo.Name.Length)
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// Converts the property type.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <returns>The converted property type.</returns>
        protected virtual string ConvertPropertyType(string type) => type;
        /// <summary>
        /// This method analyzes a property and determines its type,
        /// considering if it is nullable and whether it is a reference property or not.
        /// It returns the type as a string, potentially appending a question mark to indicate nullability if necessary.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> object to check.</param>
        /// <returns>The type as a string.</returns>
        protected virtual string GetPropertyType(PropertyInfo propertyInfo)
        {
            var nullable = propertyInfo.IsNullable();
            var result = IsReferenceProperty(propertyInfo) ? StaticLiterals.IdType : propertyInfo.PropertyType.GetCodeDefinition();
            
            if (nullable && result.EndsWith('?') == false)
            {
                result += '?';
            }
            return ConvertPropertyType(result);
        }
        /// <summary>
        /// Diese Methode ermittelt den Feldnamen der Eigenschaft.
        /// </summary>
        /// <param name="propertyInfo">Das Eigenschaftsinfo-Objekt.</param>
        /// <param name="prefix">Prefix der dem Namen vorgestellt ist.</param>
        /// <returns>Der Feldname als Zeichenfolge.</returns>
        public static string CreateFieldName(PropertyInfo propertyInfo, string prefix)
        {
            return $"{prefix}{char.ToLower(propertyInfo.Name.First())}{propertyInfo.Name[1..]}";
        }
        /// <summary>
        /// Retrieves the default value for the specified property.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> object representing the property.</param>
        /// <returns>
        /// A <see cref="string"/> representing the default value for the property.
        /// </returns>
        public static string GetDefaultValue(PropertyInfo propertyInfo)
        {
            string result = string.Empty;
            
            if (propertyInfo.IsNullable() == false)
            {
                if (propertyInfo.PropertyType == typeof(string))
                {
                    result = "string.Empty";
                }
                else if (ItemProperties.IsArrayType(propertyInfo.PropertyType))
                {
                    result = $"[]";
                }
                else if (ItemProperties.IsListType(propertyInfo.PropertyType))
                {
                    result = "new()";
                }
            }
            return result;
        }
        /// <summary>
        /// Creates a parameter name for the given <see cref="PropertyInfo"/>.
        /// Parameter name is created by prefixing an underscore to the lowercase first character of the property name and appending the rest of the property name as it is.
        /// </summary>
        /// <param name="propertyInfo">The property information object.</param>
        /// <returns>The formatted parameter name.</returns>
        public static string CreateParameterName(PropertyInfo propertyInfo) => $"_{char.ToLower(propertyInfo.Name[0])}{propertyInfo.Name[1..]}";
        #endregion property-helpers
        #endregion helpers
    }
}

