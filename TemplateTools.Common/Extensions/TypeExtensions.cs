//@BaseCode

using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace TemplateTools.Common.Extensions
{
    /// <summary>
    /// Provides extension methods for types.
    /// </summary>
    public static partial class TypeExtensions
    {
        private const char GenericSpecialChar = '`';
        private const string GenericSeparator = ", ";

        /// <summary>
        /// Returns the clean name of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get the clean name for</param>
        /// <returns>The clean name of the specified <paramref name="type"/></returns>
        public static string GetCleanName(this Type type)
        {
            var name = type.Name;

            if (type.IsGenericType)
            {
                name = name[..name.IndexOf(GenericSpecialChar)];
            }
            return name;
        }
        /// <summary>
        /// Generates a code definition for a given Type.
        /// </summary>
        /// <param name="type">The Type to generate code definition for.</param>
        /// <returns>A string representing the code definition of the Type.</returns>
        public static string GetCodeDefinition(this Type type)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("{0}.{1}", type.Namespace, type.GetCleanName());
            if (type.IsGenericType)
            {
                var names = from genericArg in type.GetGenericArguments()
                            select GetCodeDefinition(genericArg);
                sb.Append('<');
                sb.Append(string.Join(GenericSeparator, [.. names]));
                sb.Append('>');
            }
            if (sb.ToString().StartsWith("System.Nullable<")
            && sb.ToString().EndsWith('>'))
            {
                sb.Replace("System.Nullable<", string.Empty);
                sb.Replace(">", string.Empty);

                sb.Append('?');
            }
            return sb.ToString();
        }
        /// <summary>
        /// Is a helper class for some methods of type extensions.
        /// </summary>
        public class Extend
        {
            /// <summary>
            /// Gets or sets the type of the object.
            /// </summary>
            public Type? Type { get; set; }
            /// <summary>
            /// Gets or sets a list of Extend objects.
            /// </summary>
            public List<Extend> Extends { get; set; } = [];
        }
        /// <summary>
        /// Checks if the specified type is an interface or not.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="argName">The name of the argument being checked.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="type"/> is not an interface.</exception>
        public static void CheckInterface(this Type? type, string argName)
        {
            ArgumentNullException.ThrowIfNull(type);

            if (type.IsInterface == false)
            throw new ArgumentException($"The parameter '{argName}' must be an interface.");
        }
        /// <summary>
        /// Determines whether the specified type is a nullable type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the specified type is a nullable type; otherwise, false.</returns>
        public static bool IsNullableType(this Type type)
        {
            return type.IsValueType == false || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Determines whether the specified type is a nullable value type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the specified type is a nullable value type; otherwise, <c>false</c>.</returns>
        public static bool IsNullableValueType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GetGenericArguments()[0].IsValueType;
        }
        /// <summary>
        /// Determines whether the specified type is a generic collection type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the type is a generic collection type; otherwise, false.</returns>
        public static bool IsGenericCollectionType(this Type type)
        {
            return type.FullName!.StartsWith($"{nameof(System)}.{nameof(System.Collections)}.{nameof(System.Collections.Generic)}");
        }
        /// <summary>
        /// Determine whether a type is simple (String, Decimal, DateTime, etc)
        /// or complex (i.e. custom class with public properties and methods).
        /// </summary>
        public static bool IsSimpleType(this Type type)
        {
            var checkType = type;
            
            if (type.IsNullableValueType())
            {
                checkType = Nullable.GetUnderlyingType(type);
            }
            
            return checkType!.IsValueType ||
            checkType!.IsPrimitive ||
            new[]
            {
                typeof(String),
                typeof(Decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
            }.Contains(checkType) ||
            (Convert.GetTypeCode(checkType) != TypeCode.Object);
        }
        
        /// <summary>
        /// Returns the underlying type of the specified MemberInfo.
        /// </summary>
        /// <param name="member">The MemberInfo to get the underlying type from.</param>
        /// <returns>
        /// The underlying Type of the specified MemberInfo, or null if the MemberInfo
        /// does not have an underlying type.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the input MemberInfo is not of type EventInfo, FieldInfo, MethodInfo,
        /// or PropertyInfo.
        /// </exception>
        public static Type? GetUnderlyingType(this MemberInfo member)
        {
            return member.MemberType switch
            {
                MemberTypes.Event => ((EventInfo)member).EventHandlerType,
                MemberTypes.Field => ((FieldInfo)member).FieldType,
                MemberTypes.Method => ((MethodInfo)member).ReturnType,
                MemberTypes.Property => ((PropertyInfo)member).PropertyType,
                _ => throw new ArgumentException("Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"),
            };
        }
        /// <summary>
        /// Determines whether the specified type is a numeric type.
        /// </summary>
        /// <param name="type">The type to check for numericness.</param>
        /// <returns>true if the specified type is numeric; otherwise, false.</returns>
        public static bool IsNumericType(this Type type)
        {
            var result = false;
            var checkType = type;
            
            if (type.IsNullableValueType())
            {
                checkType = Nullable.GetUnderlyingType(type);
            }
            
            switch (Type.GetTypeCode(checkType))
            {
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                result = true;
                break;
            }
            return result;
        }
        ///<summary>
        /// Checks if the specified type is a floating-point type.
        ///</summary>
        ///<param name="type">The type to check for floating-point.</param>
        ///<returns>True if the type is a floating-point type, otherwise false.</returns>
        public static bool IsFloatingPointType(this Type type)
        {
            var result = false;
            var checkType = type;
            
            if (type.IsNullableValueType())
            {
                checkType = Nullable.GetUnderlyingType(type);
            }
            
            switch (Type.GetTypeCode(checkType))
            {
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                result = true;
                break;
            }
            return result;
        }
        /// <summary>
        /// Gets the source type name for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to get the source type name for.</param>
        /// <returns>The source type name.</returns>
        public static string GetSourceTypeName(this Type type)
        {
            var result = type.Name;
            
            if (type.IsGenericType)
            {
                result = result.Substring(0, "`");
                result += "<";
                for (int i = 0; i < type.GetGenericArguments().Length; i++)
                {
                    if (i > 0)
                    {
                        result += ", ";
                    }
                    result += GetSourceTypeName(type.GetGenericArguments()[i]);
                }
                result += ">";
            }
            return result;
        }
        
        /// <summary>
        /// Returns a nested structure representing the interface hierarchy of the specified type.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> to retrieve the interface hierarchy for.</param>
        /// <returns>The <see cref="Extend"/> structure representing the interface hierarchy of the specified type.</returns>
        public static Extend GetInterfaceHierarchy(this Type type)
        {
            var result = new Extend { Type = type };
            
            static void GetInterfaceHierarchyRec(Extend root)
            {
                root.Extends = [.. root.Type!.GetAllInterfaces().Select(e => new Extend { Type = e })];
                foreach (var item in root.Extends)
                {
                    GetInterfaceHierarchyRec(item);
                }
            }
            
            if (type.IsInterface)
            {
                GetInterfaceHierarchyRec(result);
            }
            return result;
        }
        /// <summary>
        /// Retrieves the class hierarchy for the specified type.
        /// </summary>
        /// <param name="type">The type for which the class hierarchy should be retrieved.</param>
        /// <returns>An IEnumerable of Type representing the class hierarchy.</returns>
        public static IEnumerable<Type> GetClassHierarchy(this Type type)
        {
            var result = new List<Type>();
            
            if (type.IsClass)
            {
                var run = type.BaseType;
                
                result.Add(type);
                while (run != null && run.Name.Equals("object", StringComparison.CurrentCultureIgnoreCase) == false)
                {
                    result.Add(run);
                    run = run.BaseType;
                }
            }
            return result;
        }
        /// <summary>
        /// Retrieves a collection of all interfaces implemented or inherited by the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to retrieve the interfaces.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Type"/> representing the interfaces implemented or inherited by the specified <see cref="Type"/>.</returns>
        public static IEnumerable<Type> GetAllInterfaces(this Type type)
        {
            static void GetBaseInterfaces(Type type, List<Type> interfaces)
            {
                var run = type;
                
                while (run != null)
                {
                    foreach (var item in run.GetInterfaces())
                    {
                        if (interfaces.Contains(item) == false)
                        {
                            interfaces.Add(item);
                        }
                        GetBaseInterfaces(item, interfaces);
                    }
                    run = run.BaseType;
                }
            }
            var result = new List<Type>();
            
            GetBaseInterfaces(type, result);
            return result;
        }
        /// <summary>
        /// Retrieves all interfaces that are directly declared by the specified type.
        /// </summary>
        /// <param name="type">The type for which to retrieve the declared interfaces.</param>
        /// <returns>An enumerable collection of Type objects representing the interfaces directly declared by the specified type.</returns>
        public static IEnumerable<Type> GetDeclaredInterfaces(this Type type)
        {
            var run = type.BaseType;
            IEnumerable<Type> result = type.GetInterfaces();
            
            foreach (var item in result)
            {
                result = result.Except(item.GetAllInterfaces());
            }
            while (run != null)
            {
                result = result.Except(run.GetInterfaces());
                foreach (var item in result)
                {
                    result = result.Except(item.GetAllInterfaces());
                }
                run = run.BaseType;
            }
            return result;
        }
        /// <summary>
        /// Gets the relations of the specified type.
        /// </summary>
        /// <param name="type">The type to get the relations for.</param>
        /// <returns>An enumerable collection of type relations.</returns>
        public static IEnumerable<Type> GetRelations(this Type type)
        {
            return type.GetRelations(0);
        }
        /// <summary>
        /// Retrieves a collection of types that are related to the specified type, up to a given maximum depth.
        /// </summary>
        /// <param name="type">The type for which to retrieve the relations.</param>
        /// <param name="maxDeep">The maximum depth to traverse while retrieving relations.</param>
        /// <returns>A collection of types that are related to the specified type.</returns>
        /// <remarks>
        /// The relations include types that are found as fields within the specified type and its nested types.
        /// The search is performed recursively up to the given maximum depth.
        /// ValueType, String, and non-class/interface generic arguments are excluded from the results.
        /// </remarks>
        public static IEnumerable<Type> GetRelations(this Type type, int maxDeep)
        {
            static IEnumerable<Type> GetRelationsRec(Type type, List<Type> relations, int maxDeep, int deep)
            {
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                
                foreach (var item in type.GetFields(bindingFlags))
                {
                    if (item.FieldType.IsValueType == false)
                    {
                        if (item.FieldType.IsGenericType
                        && item.FieldType.GetGenericArguments().Any(e => (e.IsClass || e.IsInterface) && e != typeof(string)))
                        {
                            relations.Add(item.FieldType);
                            if (maxDeep < deep)
                            {
                                GetRelationsRec(item.FieldType, relations, maxDeep, deep + 1);
                            }
                        }
                        else if ((item.FieldType.IsClass || item.FieldType.IsInterface) && item.FieldType != typeof(string))
                        {
                            relations.Add(item.FieldType);
                            if (maxDeep < deep)
                            {
                                GetRelationsRec(item.FieldType, relations, maxDeep, deep + 1);
                            }
                        }
                    }
                }
                return relations;
            }
            var result = new List<Type>();
            
            return GetRelationsRec(type, result, maxDeep, 0);
        }
        /// <summary>
        /// Retrieves a collection of base types for the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to retrieve the base types for.</param>
        /// <returns>An enumerable collection of base types.</returns>
        /// <remarks>
        /// This method recursively traverses the inheritance hierarchy of the specified <see cref="Type"/>
        /// and retrieves all the base types, starting from the immediate base type up to the root base type.
        /// </remarks>
        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            var result = new List<Type>();
            var run = type.BaseType;
            
            while (run != null)
            {
                result.Add(run);
                run = run.BaseType;
            }
            return result;
        }
        /// <summary>
        /// Retrieves all base interfaces implemented by the specified type and its parent types.
        /// </summary>
        /// <param name="type">The type for which to retrieve base interfaces.</param>
        /// <returns>
        /// An enumerable collection of Type objects representing the base interfaces implemented by the specified type and its parent types.
        /// </returns>
        /// <remarks>
        /// This extension method recursively traverses the type hierarchy to retrieve all base interfaces.
        /// It ensures that duplicate interfaces are not included in the result.
        /// </remarks>
        public static IEnumerable<Type> GetBaseInterfaces(this Type type)
        {
            static void GetBaseInterfaces(Type type, List<Type> interfaces)
            {
                foreach (var item in type.GetInterfaces())
                {
                    if (interfaces.Contains(item) == false)
                    {
                        interfaces.Add(item);
                    }
                    GetBaseInterfaces(item, interfaces);
                }
            }
            var result = new List<Type>();
            
            GetBaseInterfaces(type, result);
            return result;
        }
        /// <summary>
        /// Retrieves all fields of a class, including inherited fields.
        /// </summary>
        /// <param name="type">The type to retrieve the fields from.</param>
        /// <returns>An enumerable collection of FieldInfo objects representing the fields.</returns>
        public static IEnumerable<FieldInfo> GetAllClassFields(this Type type)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var result = new List<FieldInfo>();
            
            if (type.IsClass)
            {
                var run = type.BaseType;
                
                result.AddRange(type.GetFields(bindingFlags));
                while (run != null && run.Name.Equals("object", StringComparison.CurrentCultureIgnoreCase) == false)
                {
                    result.AddRange(run.GetFields(bindingFlags));
                    run = run.BaseType;
                }
            }
            return result;
        }
        
        /// <summary>
        /// Retrieves the specified interface property by name from the given type.
        /// </summary>
        /// <param name="type">The type to search for the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>
        /// The <see cref="PropertyInfo"/> object representing the specified interface property,
        /// or <c>null</c> if the property is not found.
        /// </returns>
        public static PropertyInfo? GetInterfaceProperty(this Type? type, string name)
        {
            type.CheckArgument(nameof(type));
            
            return type?.GetAllInterfacePropertyInfos().SingleOrDefault(p => p.Name.Equals(name));
        }
        
        /// <summary>
        /// Retrieves all the properties of a given type object and returns them as a dictionary.
        /// </summary>
        /// <param name="typeObject">The type object for which to retrieve the properties.</param>
        /// <returns>A dictionary containing the properties of the type object.</returns>
        public static Dictionary<string, PropertyItem> GetAllTypeProperties(this Type typeObject)
        {
            var propertyItems = new Dictionary<string, PropertyItem>();
            
            GetAllTypePropertiesRec(typeObject, propertyItems);
            return propertyItems;
        }
        /// <summary>
        /// Recursively retrieves all properties from a specified type and adds them to a dictionary.
        /// </summary>
        /// <param name="typeObject">The type of object to retrieve properties from.</param>
        /// <param name="propertyItems">The dictionary to store the retrieved properties.</param>
        private static void GetAllTypePropertiesRec(Type typeObject, Dictionary<string, PropertyItem> propertyItems)
        {
            if (typeObject.BaseType != null
            && typeObject.BaseType != typeof(object))
            {
                GetAllTypePropertiesRec(typeObject.BaseType, propertyItems);
            }
            
            foreach (var property in typeObject.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                               .OrderBy(p => p.MetadataToken))
            {
                var propertyItem = new PropertyItem(property);
                
                if (propertyItems.ContainsKey(property.Name) == false)
                {
                    propertyItems.Add(property.Name, propertyItem);
                }
                else
                {
                    propertyItems[property.Name] = propertyItem;
                }
                // Bei rekursiven Strukturen kommt es hier zu einem Absturz!!!
                //if (propertyItem.IsComplexType)
                //{
                    //    GetAllTypePropertiesRec(propertyItem.PropertyInfo.PropertyType, propertyItem.PropertyItems);
                //}
            }
        }
        
        /// <summary>
        /// Retrieves all PropertyInfos for the specified Type.
        /// </summary>
        /// <param name="type">The Type to retrieve PropertyInfos from.</param>
        /// <returns>An IEnumerable collection of PropertyInfo objects representing the properties of the specified Type.</returns>
        public static IEnumerable<PropertyInfo> GetAllPropertyInfos(this Type type)
        {
            return type.IsInterface == false ? type.GetAllTypePropertyInfos() : type.GetAllInterfacePropertyInfos();
        }
        /// <summary>
        /// Retrieves all the public instance properties of the specified type.
        /// </summary>
        /// <param name="type">The type to retrieve the properties from.</param>
        /// <returns>An enumeration of public instance PropertyInfo objects representing the properties of the specified type.</returns>
        public static IEnumerable<PropertyInfo> GetAllTypePropertyInfos(this Type type)
        {
            type.CheckArgument(nameof(type));
            
            var result = new List<PropertyInfo>();
            
            foreach (var item in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                result.Add(item);
            }
            return result;
        }
        
        /// <summary>
        /// Retrieves all public properties defined in an interface or its inherited interfaces for a given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get the interface properties from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="PropertyInfo"/> that represents the interface properties.</returns>
        public static IEnumerable<PropertyInfo> GetAllInterfacePropertyInfos(this Type type)
        {
            var result = new List<PropertyInfo>();
            
            if (type.GetTypeInfo().IsInterface)
            {
                GetAllInterfacePropertyInfosRec(type, result);
            }
            else
            {
                foreach (var item in type.GetInterfaces())
                {
                    GetAllInterfacePropertyInfosRec(item, result);
                }
            }
            return result;
        }
        /// <summary>
        /// Recursively retrieves all property information from a given type and its interfaces,
        /// collecting them in a provided list.
        /// </summary>
        /// <param name="type">The type to retrieve property information from.</param>
        /// <param name="properties">The list to collect the property information in.</param>
        private static void GetAllInterfacePropertyInfosRec(Type type, List<PropertyInfo> properties)
        {
            foreach (var item in type.GetProperties())
            {
                if (properties.Find(p => p.Name.Equals(item.Name)) == null)
                {
                    properties.Add(item);
                }
            }
            foreach (var item in type.GetInterfaces())
            {
                GetAllInterfacePropertyInfosRec(item, properties);
            }
        }
        
        /// <summary>
        /// Determines if the specified type is a generic type that matches the given generic type definition.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="genericType">The generic type definition to compare against.</param>
        /// <returns>true if the specified type is a generic type that matches the given generic type definition; otherwise, false.</returns>
        public static bool IsGenericTypeOf(this Type type, Type genericType)
        {
            Type? instanceType = type;
            
            while (instanceType != null)
            {
                if (instanceType.IsGenericType && instanceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
                instanceType = instanceType.BaseType;
            }
            return false;
        }
        
        /// <summary>
        /// Returns the declaring name of the given PropertyInfo.
        /// The declaring name is formatted as "{DeclaringType.Name}.{Name}".
        /// </summary>
        /// <param name="source">The PropertyInfo instance.</param>
        /// <returns>The declaring name of the PropertyInfo.</returns>
        public static string DeclaringName(this PropertyInfo source) => $"{source.DeclaringType!.Name}.{source.Name}";
        /// <summary>
        /// Determines whether the specified <see cref="PropertyInfo"/> is nullable.
        /// </summary>
        /// <param name="source">The <see cref="PropertyInfo"/> to check.</param>
        /// <returns><c>true</c> if the <see cref="PropertyInfo"/> is nullable; otherwise, <c>false</c>.</returns>
        public static bool IsNullable(this PropertyInfo source) => IsNullableHelper(source.PropertyType, source.DeclaringType, source.CustomAttributes);
        /// <summary>
        /// Determines whether the specified field is nullable.
        /// </summary>
        /// <param name="source">The field info object.</param>
        /// <returns>true if the field is nullable; otherwise, false.</returns>
        public static bool IsNullable(this FieldInfo source) => IsNullableHelper(source.FieldType, source.DeclaringType, source.CustomAttributes);
        /// <summary>
        /// Checks if the given <see cref="ParameterInfo"/> is nullable.
        /// </summary>
        /// <param name="source">The <see cref="ParameterInfo"/> to check for nullable.</param>
        /// <returns>True if the <paramref name="source"/> is nullable, false otherwise.</returns>
        public static bool IsNullable(this ParameterInfo source) => IsNullableHelper(source.ParameterType, source.Member, source.CustomAttributes);
        /// <summary>
        /// Determines whether the specified member type is nullable or not.
        /// </summary>
        /// <param name="memberType">The type of the member.</param>
        /// <param name="declaringType">The declaring type of the member. Can be null.</param>
        /// <param name="customAttributes">The custom attributes applied to the member.</param>
        /// <returns>true if the member type is nullable; otherwise, false.</returns>
        private static bool IsNullableHelper(Type memberType, MemberInfo? declaringType, IEnumerable<CustomAttributeData> customAttributes)
        {
            if (memberType.IsValueType)
            return Nullable.GetUnderlyingType(memberType) != null;
            
            var nullable = customAttributes.FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
            
            if (nullable != null && nullable.ConstructorArguments.Count == 1)
            {
                var attributeArgument = nullable.ConstructorArguments[0];
                if (attributeArgument.ArgumentType == typeof(byte[]))
                {
                    var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value!;
                    
                    if (args.Count > 0 && args[0].ArgumentType == typeof(byte))
                    {
                        return (byte)args[0].Value! == 2;
                    }
                }
                else if (attributeArgument.ArgumentType == typeof(byte))
                {
                    return (byte)attributeArgument.Value! == 2;
                }
            }
            
            for (var type = declaringType; type != null; type = type.DeclaringType)
            {
                var context = type.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
                
                if (context != null &&
                context.ConstructorArguments.Count == 1 &&
                context.ConstructorArguments[0].ArgumentType == typeof(byte))
                {
                    return (byte)context.ConstructorArguments[0].Value! == 2;
                }
            }
            // Couldn't find a suitable attribute
            return false;
        }
    }
}

