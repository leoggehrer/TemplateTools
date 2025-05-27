//@BaseCode

namespace TemplateTools.Logic.Generation
{
    using System.Reflection;
    using TemplateTools.Logic.Common;
    using TemplateTools.Logic.Contracts;
    using TemplateTools.Logic.Extensions;
    using TemplateTools.Logic.Models;

    /// <summary>
    /// This class provides many methods for generating program parts.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ItemGenerator"/> class.
    /// </remarks>
    /// <param name="solutionProperties">The solution properties.</param>
    internal abstract partial class ItemGenerator(ISolutionProperties solutionProperties) : GeneratorObject(solutionProperties)
    {
        #region properties
        /// <summary>
        /// Gets the properties of the item.
        /// </summary>
        protected abstract ItemProperties ItemProperties { get; }
        /// <summary>
        /// The separators used to split values in the valueSeparators array.
        /// </summary>
        private static readonly char[] valueSeparators = [','];
        #endregion properties

        #region queries
        /// <summary>
        /// Determines whether or not a given type can be created.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// Returns true if the type can be created, false otherwise.
        /// </returns>
        protected virtual bool CanCreate(Type type)
        {
            return true;
        }
        /// <summary>
        /// Determines whether a property can be created.
        /// </summary>
        /// <param name="propertyInfo">The property info of the property.</param>
        /// <returns>Returns true if the property can be created; otherwise, false.</returns>
        protected virtual bool CanCreate(PropertyInfo propertyInfo)
        {
            return QuerySetting<bool>(UnitType.General, ItemType.Property, propertyInfo.DeclaringName(), StaticLiterals.Generate, "True");
        }
        /// <summary>
        /// Determines whether a property can be copied.
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo object representing the property.</param>
        /// <returns>
        /// True if the property can be copied; otherwise, False.
        /// </returns>
        protected virtual bool CanCopyProperty(PropertyInfo propertyInfo)
        {
            return QuerySetting<bool>(UnitType.General, ItemType.Property, propertyInfo.DeclaringName(), StaticLiterals.Copy, "True");
        }
        /// <summary>
        /// Retrieves the view properties of the specified type.
        /// </summary>
        /// <param name="type">The type to get the view properties from.</param>
        /// <returns>An enumerable collection of PropertyInfo objects representing the view properties of the specified type.</returns>
        protected static IEnumerable<PropertyInfo> GetViewProperties(Type type)
        {
            var typeProperties = type.GetAllPropertyInfos();
            var viewProperties = typeProperties.Where(e => StaticLiterals.VersionProperties.Any(p => p.Equals(e.Name)) == false
                                                        && ItemProperties.IsListType(e.PropertyType) == false
                                                        && (e.PropertyType.IsEnum || e.PropertyType.IsValueType || e.PropertyType.IsPrimitive || ItemProperties.IsPrimitiveNullable(e) || e.PropertyType == typeof(string)));

            return viewProperties ?? [];
        }
        #endregion queries

        #region create constructors
        /// <summary>
        /// Creates a partial static constructor for a class with optional initialization statements.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <param name="initStatements">Optional initialization statements.</param>
        /// <returns>An enumerable collection of strings representing the lines of the partial static constructor.</returns>
        public virtual IEnumerable<string> CreatePartialStaticConstrutor(string className, IEnumerable<string>? initStatements = null)
        {
            var lines = new List<string>(CreateComment("Initializes the class (created by the generator)."))
            {
                $"static {className}()",
                "{",
                "ClassConstructing();"
            };
            if (initStatements != null)
            {
                foreach (var item in initStatements)
                {
                    lines.Add($"{item}");
                }
            }
            lines.Add($"ClassConstructed();");
            lines.Add("}");
            lines.AddRange(CreateComment("This method is called before the construction of the class."));
            lines.Add("static partial void ClassConstructing();");
            lines.AddRange(CreateComment("This method is called when the class is constructed."));
            lines.Add("static partial void ClassConstructed();");
            lines.Add(string.Empty);
            return lines;
        }
        /// <summary>
        /// Creates a partial constructor with the specified visibility, class name, argument list, and base constructor.
        /// </summary>
        /// <param name="visibility">The visibility of the constructor (e.g., public, private).</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="argumentList">The list of arguments for the constructor.</param>
        /// <param name="baseConstructor">The base constructor to be called.</param>
        /// <param name="initStatements">The list of initialization statements.</param>
        /// <param name="withPartials">Indicates whether to include partial methods for constructing and constructed.</param>
        /// <returns>An IEnumerable of strings representing the lines of code for the constructor.</returns>
        public virtual IEnumerable<string> CreatePartialConstrutor(string visibility, string className, string? argumentList = null, string? baseConstructor = null, IEnumerable<string>? initStatements = null, bool withPartials = true)
        {
            var lines = new List<string>(CreateComment($"Initializes a new instance of the <see cref=\"{className}\"/> class (created by the generator.)"))
            {
                $"{visibility} {className}({argumentList})",
            };

            if (string.IsNullOrEmpty(baseConstructor) == false)
                lines.Add($" : {baseConstructor}");

            lines.Add("{");
            lines.Add("Constructing();");
            if (initStatements != null)
            {
                foreach (var item in initStatements)
                {
                    lines.Add($"{item}");
                }
            }
            else
            {
                lines.Add(string.Empty);
            }
            lines.Add($"Constructed();");
            lines.Add("}");
            if (withPartials)
            {
                lines.AddRange(CreateComment("This method is called the object is being constraucted."));
                lines.Add("partial void Constructing();");
                lines.AddRange(CreateComment("This method is called when the object is constructed."));
                lines.Add("partial void Constructed();");
            }
            return lines;
        }
        /// <summary>
        /// Retrieves the properties of a type that are eligible for generation.
        /// </summary>
        /// <param name="type">The type whose properties are to be retrieved.</param>
        /// <returns>
        /// An enumerable collection of <see cref="PropertyInfo"/> objects representing the properties
        /// of the specified type that are eligible for generation.
        /// </returns>
        public static IEnumerable<PropertyInfo> GetGenerationProperties(Type type)
        {
            var typeProperties = type.GetProperties().Where(pi => pi.DeclaringType == type);
            var generationProperties = typeProperties.Where(e => StaticLiterals.VersionProperties.Any(p => p.Equals(e.Name)) == false
                                                                && ItemProperties.IsListType(e.PropertyType) == false
                                                                && (e.PropertyType.IsEnum 
                                                                    || e.PropertyType.IsValueType 
                                                                    || e.PropertyType.IsPrimitive
                                                                    || ItemProperties.IsPrimitiveArrayType(e)
                                                                    || ItemProperties.IsPrimitiveNullable(e) 
                                                                    || e.PropertyType == typeof(string)));
            return generationProperties ?? [];
        }
        #endregion create constructors

        #region create property
        /// <summary>
        /// Creates the property attributes for the given PropertyInfo.
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo object representing the property.</param>
        /// <param name="codeLines">The list of code lines to add the attributes to.</param>
        protected virtual void CreatePropertyAttributes(PropertyInfo propertyInfo, List<string> codeLines) { }
        /// <summary>
        /// Creates XML documentation for the method that retrieves attributes of a property.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> object representing the property.</param>
        /// <param name="codeLines">The list of string representing the code lines.</param>
        protected virtual void CreateGetPropertyAttributes(PropertyInfo propertyInfo, List<string> codeLines) { }
        /// <summary>
        /// Creates the set property attributes for the specified property.
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo object that represents the property.</param>
        /// <param name="codeLines">The list of code lines where the attributes will be added.</param>
        protected virtual void CreateSetPropertyAttributes(PropertyInfo propertyInfo, List<string> codeLines) { }
        /// <summary>
        /// Retrieves the default value for a given property.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> object representing the property.</param>
        /// <param name="defaultValue">A reference to the default value, which will be updated with the result.</param>
        protected virtual void GetPropertyDefaultValue(PropertyInfo propertyInfo, ref string defaultValue) { }

        /// <summary>
        /// Creates a property based on the given type and property information.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>An enumerable collection of strings representing the generated property.</returns>
        public virtual IEnumerable<string> CreateProperty(Type type, PropertyInfo propertyInfo)
        {
            var handled = false;
            var result = new List<string>();

            BeforeCreateProperty(type, propertyInfo, result, ref handled);
            if (handled == false)
            {
                result.AddRange(CreateAutoProperty(type, propertyInfo));
            }
            AfterCreateProperty(type, propertyInfo, result);
            return result;
        }
        /// <summary>
        /// This method is called before creating a property in the specified type.
        /// </summary>
        /// <param name="type">The type where the property is being created.</param>
        /// <param name="propertyInfo">The property information being created.</param>
        /// <param name="codeLines">The list of code lines being generated for the property.</param>
        /// <param name="handled">A boolean indicating whether the operation has already been handled.</param>
        partial void BeforeCreateProperty(Type type, PropertyInfo propertyInfo, List<string> codeLines, ref bool handled);
        /// <summary>
        /// This method is called after a property is created in the specified type.
        /// </summary>
        /// <param name="type">The type containing the newly created property.</param>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> object representing the newly created property.</param>
        /// <param name="codeLines">The list of code lines for the property.</param>
        partial void AfterCreateProperty(Type type, PropertyInfo propertyInfo, List<string> codeLines);

        /// <summary>
        /// Creates an auto property for the specified type and property.
        /// </summary>
        /// <param name="type">The type to create the auto property for.</param>
        /// <param name="propertyInfo">The property information of the property.</param>
        /// <returns>An enumerable of strings representing the auto property.</returns>
        public virtual IEnumerable<string> CreateAutoProperty(Type type, PropertyInfo propertyInfo)
        {
            return CreateAutoProperty(type, propertyInfo, $"Property '{propertyInfo.Name}' generated by the generator.");
        }

        /// <summary>
        /// Creates an auto-generated property for a given type and property info.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="propertyInfo">The PropertyInfo object for the property.</param>
        /// <param name="comment">The comment to be included in the documentation.</param>
        /// <returns>An IEnumerable collection of strings representing the XML documentation for the method.</returns>
        public virtual IEnumerable<string> CreateAutoProperty(Type type, PropertyInfo propertyInfo, string comment)
        {
            var result = new List<string>();
            var defaultValue = GetDefaultValue(propertyInfo);
            var propertyType = GetPropertyType(propertyInfo);

            if (defaultValue.HasContent())
                defaultValue = defaultValue.Replace($"{StaticLiterals.TProperty}", propertyType.Replace("[]", string.Empty));

            var property = $"public {propertyType} {propertyInfo.Name} ";

            GetPropertyDefaultValue(propertyInfo, ref defaultValue);
            if (string.IsNullOrEmpty(defaultValue))
            {
                property += "{ get; set; }";
            }
            else
            {
                property += "{ get; set; }" + $" = {defaultValue};";
            }
            result.AddRange(CreateComment(string.Format(comment, propertyInfo.Name)));
            result.Add(property);
            return result;
        }
        /// Creates XML documentation for the method CreatePartialProperty.
        /// @param propertyInfo - A PropertyInfo object representing the property
        /// @return An IEnumerable<string> representing the generated XML documentation
        public virtual IEnumerable<string> CreatePartialProperty(PropertyInfo propertyInfo)
        {
            var result = new List<string>();
            var defaultValue = GetDefaultValue(propertyInfo);
            var propertyType = GetPropertyType(propertyInfo);
            var fieldType = GetPropertyType(propertyInfo);
            var fieldName = CreateFieldName(propertyInfo, "_");
            var paramName = CreateFieldName(propertyInfo, "_");

            if (defaultValue.HasContent())
                defaultValue = defaultValue.Replace($"{StaticLiterals.TProperty}", propertyType.Replace("[]", string.Empty));

            result.Add(string.Empty);
            result.AddRange(CreateComment(propertyInfo));
            CreatePropertyAttributes(propertyInfo, result);
            result.Add($"public {fieldType} {propertyInfo.Name}");
            result.Add("{");
            result.AddRange(CreatePartialGetProperty(propertyInfo));
            result.AddRange(CreatePartialSetProperty(propertyInfo));
            result.Add("}");

            GetPropertyDefaultValue(propertyInfo, ref defaultValue);
            result.Add(string.IsNullOrEmpty(defaultValue)
            ? $"private {fieldType} {fieldName}{(ItemProperties.IsListType(propertyInfo.PropertyType) ? " = new();" : ";")}"
            : $"private {fieldType} {fieldName} = {defaultValue};");

            result.Add($"partial void On{propertyInfo.Name}Reading();");
            result.Add($"partial void On{propertyInfo.Name}Changing(ref bool handled, {fieldType} value, ref {fieldType} {paramName});");
            result.Add($"partial void On{propertyInfo.Name}Changed();");
            return result;
        }
        /// <summary>
        /// Creates the XML documentation for the <see cref="CreatePartialGetProperty"/> method.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> object representing the property.</param>
        /// <returns>An <see cref="IEnumerable{String}"/> containing the created XML documentation.</returns>
        public virtual IEnumerable<string> CreatePartialGetProperty(PropertyInfo propertyInfo)
        {
            var result = new List<string>();
            var fieldName = CreateFieldName(propertyInfo, "_");

            CreateGetPropertyAttributes(propertyInfo, result);
            result.Add("get");
            result.Add("{");
            result.Add($"On{propertyInfo.Name}Reading();");
            result.Add($"return {fieldName};");
            result.Add("}");
            return result;
        }
        /// <summary>
        /// Creates a partial set property for a given PropertyInfo object.
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo object representing the property.</param>
        /// <returns>An IEnumerable of strings representing the lines of code for the partial set property.</returns>
        public virtual IEnumerable<string> CreatePartialSetProperty(PropertyInfo propertyInfo)
        {
            var result = new List<string>();
            var propName = propertyInfo.Name;
            var fieldName = CreateFieldName(propertyInfo, "_");

            CreateSetPropertyAttributes(propertyInfo, result);
            result.Add("set");
            result.Add("{");
            result.Add("bool handled = false;");
            result.Add($"On{propName}Changing(ref handled, value, ref {fieldName});");
            result.Add("if (handled == false)");
            result.Add("{");
            result.Add($"{fieldName} = value;");
            result.Add("}");
            result.Add($"OnPropertyChanged();");
            result.Add($"On{propName}Changed();");
            result.Add("}");
            return [.. result];
        }

        /// <summary>
        /// Creates a ViewModel property based on the provided PropertyInfo.
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo object representing the property.</param>
        /// <returns>An enumerable collection of strings representing the generated ViewModel property.</returns>
        public virtual IEnumerable<string> CreateViewModelProperty(PropertyInfo propertyInfo)
        {
            var result = new List<string>();
            var fieldType = GetPropertyType(propertyInfo);

            result.Add(string.Empty);
            result.AddRange(CreateComment(propertyInfo));
            CreatePropertyAttributes(propertyInfo, result);
            result.Add($"public {fieldType} {propertyInfo.Name}");
            result.Add("{");
            result.AddRange(CreateViewModelGetProperty(propertyInfo));
            result.AddRange(CreateViewModelSetProperty(propertyInfo));
            result.Add("}");

            return result;
        }
        /// <summary>  
        /// Creates the getter property for a ViewModel based on the provided PropertyInfo.  
        /// </summary>  
        /// <param name="propertyInfo">The PropertyInfo object representing the property.</param>  
        /// <returns>An enumerable collection of strings representing the generated getter property.</returns>  
        public virtual IEnumerable<string> CreateViewModelGetProperty(PropertyInfo propertyInfo)
        {
            var result = new List<string>();
            var propName = propertyInfo.Name;

            CreateGetPropertyAttributes(propertyInfo, result);
            result.Add("get");
            result.Add("{");
            result.Add($"return Model.{propName};");
            result.Add("}");
            return result;
        }
        /// <summary>
        /// Creates the setter property for a ViewModel based on the provided PropertyInfo.
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo object representing the property.</param>
        /// <returns>An enumerable collection of strings representing the generated setter property.</returns>
        public virtual IEnumerable<string> CreateViewModelSetProperty(PropertyInfo propertyInfo)
        {
            var result = new List<string>();
            var propName = propertyInfo.Name;

            CreateSetPropertyAttributes(propertyInfo, result);
            result.Add("set");
            result.Add("{");
            result.Add($"Model.{propName} = value;");
            result.Add($"OnPropertyChanged();");
            result.Add("}");
            return result;
        }
        #endregion create properties

        #region create contracts
        /// <summary>
        /// Creates an entity contract for the specified type.
        /// </summary>
        /// <param name="type">The type for which the entity contract is created.</param>
        /// <param name="unitType">The unit type.</param>
        /// <param name="itemType">The item type.</param>
        /// <param name="filter">An optional filter function to determine which properties to copy.</param>
        /// <returns>A <see cref="GeneratedItem"/> representing the created entity contract.</returns>
        protected virtual GeneratedItem CreateEntityContract(Type type, UnitType unitType, ItemType itemType, Func<PropertyInfo, bool>? filter = null)
        {
            var baseInterface = ItemProperties.GetEntityBaseInterface(type);
            var itemName = ItemProperties.CreateContractName(type);
            var fileName = $"{itemName}{StaticLiterals.CSharpFileExtension}";
            var visibility = ItemProperties.GetDefaultVisibility(type);
            var itemFullName = ItemProperties.CreateFullContractType(type);
            var itemNamespace = ItemProperties.CreateFullNamespace(type, StaticLiterals.ContractsFolder);
            var typeProperties = type.GetProperties().Where(pi => pi.DeclaringType == type);
            var filteredProperties = GetGenerationProperties(type).Where(filter ?? (p => true));
            var generatedProperties = new List<PropertyInfo>();

            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = itemFullName,
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateSubFilePath(type, fileName, StaticLiterals.ContractsFolder),
            };

            visibility = QuerySetting<string>(unitType, itemType, type, StaticLiterals.Visibility, visibility);

            result.AddRange(CreateComment(type));
            result.Add($"{visibility} partial interface {itemName} : {baseInterface}");
            result.Add("{");
            foreach (var propertyInfo in filteredProperties)
            {
                if (CanCreate(propertyInfo)
                    && QuerySetting<bool>(unitType, ItemType.InterfaceProperty, propertyInfo.DeclaringName(), StaticLiterals.Generate, "True"))
                {
                    var getAccessor = string.Empty;
                    var setAccessor = string.Empty;
                    var propertyType = GetPropertyType(propertyInfo);

                    if (propertyInfo.CanRead
                        && QuerySetting<bool>(unitType, ItemType.InterfaceProperty, propertyInfo.DeclaringName(), StaticLiterals.GetAccessor, "True"))
                    {
                        getAccessor = "get;";
                    }
                    if (propertyInfo.CanWrite
                        && QuerySetting<bool>(unitType, ItemType.InterfaceProperty, propertyInfo.DeclaringName(), StaticLiterals.SetAccessor, "True"))
                    {
                        setAccessor = "set;";
                    }
                    result.Add($"{propertyType} {propertyInfo.Name}" + " { " + $"{getAccessor} {setAccessor}" + " } ");
                    generatedProperties.Add(propertyInfo);
                }
            }

            // Added copy properties method
            result.AddRange(CreateContractCopyProperties(type, itemName, pi => generatedProperties.Contains(pi)));

            result.Add("}");
            result.EnvelopeWithANamespace(itemNamespace);
            result.FormatCSharpCode();
            return result;
        }
        #endregion create contracts

        #region copy properties
        /// <summary>
        /// Copies the value of the specified property from the given object to this object.
        /// </summary>
        /// <param name="copyType">The type of object to copy the property from.</param>
        /// <param name="propertyInfo">The PropertyInfo object representing the property to copy.</param>
        /// <returns>A string representation of the copied property assignment.</returns>
        protected virtual string CopyProperty(string copyType, PropertyInfo propertyInfo)
        {
            return $"{propertyInfo.Name} = other.{propertyInfo.Name};";
        }
        /// <summary>
        /// Creates the source code for copy properties of the specified types.
        /// </summary>
        /// <param name="modifiers">The modifiers of the copy properties method.</param>
        /// <param name="entityType">The type whose properties will be copied.</param>
        /// <param name="contractType">The type whose properties will be copied.</param>
        /// <param name="targetName">The name of the target parameter.</param>
        /// <param name="sourceName">The name of the source parameter.</param>
        /// <returns>
        /// An enumerable collection of strings representing the generated copy properties method.
        /// </returns>
        protected virtual IEnumerable<string> CreateCopyProperties(string modifiers, string entityType, string contractType, string targetName, string sourceName)
        {
            var result = new List<string>();

            result.AddRange(CreateComment($"Copies the properties from the source <see cref=\"{entityType}\"/> to the target <see cref=\"{entityType}\"/>."));
            result.Add($"/// <param name=\"{targetName}\">The target <see cref=\"{entityType}\"/>.</param>");
            result.Add($"/// <param name=\"{sourceName}\">The source <see cref=\"{entityType}\"/>.</param>");
            result.Add($"{modifiers} void CopyProperties({entityType} {targetName}, {entityType} {sourceName})");
            result.Add("{");
            result.Add($"{targetName}.CheckArgument(nameof({targetName}));");
            result.Add($"{sourceName}.CheckArgument(nameof({sourceName}));");
            result.Add("bool handled = false;");
            result.Add($"BeforeCopyProperties({targetName}, {sourceName}, ref handled);");
            result.Add("if (handled == false)");
            result.Add("{");

            result.Add($"if ({targetName} is {contractType} itarget");
            result.Add($"    && {sourceName} is {contractType} isource)");
            result.Add("{");
            result.Add("itarget.CopyProperties(isource);");
            result.Add("}");

            result.Add("}");
            result.Add($"AfterCopyProperties({targetName}, {sourceName});");
            result.Add("}");

            result.AddRange(CreateComment("This method is called before copying the properties of another object to the current instance."));
            result.Add($"/// <param name=\"{targetName}\">The object to copy the properties from.</param>");
            result.Add($"/// <param name=\"{sourceName}\">The object in which the properties are to be copied.</param>");
            result.Add("/// <param name=\"handled\">A boolean value that indicates whether the method has been handled.</param>");
            result.Add($"partial void BeforeCopyProperties({entityType} {targetName}, {entityType} {sourceName}, ref bool handled);");

            result.AddRange(CreateComment("This method is called after copying properties from another instance of the class."));
            result.Add($"/// <param name=\"{targetName}\">The object to copy the properties from.</param>");
            result.Add($"/// <param name=\"{sourceName}\">The object in which the properties are to be copied.</param>");
            result.Add($"partial void AfterCopyProperties({entityType} {targetName}, {entityType} {sourceName});");
            return result.Where(l => string.IsNullOrEmpty(l) == false);
        }
        /// <summary>
        /// Creates a copy of the properties of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="visibility">The visibility of the copy properties method.</param>
        /// <param name="type">The type whose properties will be copied.</param>
        /// <param name="copyType">The type to which the properties will be copied.</param>
        /// <param name="filter">An optional filter function to determine which properties to copy.</param>
        /// <returns>
        /// An enumerable collection of strings representing the generated copy properties method.
        /// </returns>
        public virtual IEnumerable<string> CreateContractCopyProperties(Type type, string copyType, Func<PropertyInfo, bool>? filter = null)
        {
            var result = new List<string>();
            var baseInterface = ItemProperties.GetEntityBaseInterface(type);
            var typeProperties = type.GetProperties().Where(pi => pi.DeclaringType == type);
            var filteredProperties = typeProperties.Where(filter ?? (p => true));

            result.AddRange(CreateComment("Copies the properties of another object to this instance."));
            result.Add("/// <param name=\"other\">The object to copy the properties from.</param>");
            result.Add($"void CopyProperties({copyType} other)");
            result.Add("{");
            result.Add("bool handled = false;");
            result.Add("BeforeCopyProperties(other, ref handled);");
            result.Add("if (handled == false)");
            result.Add("{");

            if (baseInterface == default)
            {
                result.Add("other.CheckArgument(nameof(other));");
            }
            else
            {
                result.Add($"(({baseInterface})this).CopyProperties(other);");
            }

            foreach (var item in filteredProperties)
            {
                if (item.CanWrite && item.CanRead && CanCreate(item) && CanCopyProperty(item))
                {
                    result.Add(CopyProperty(copyType, item));
                }
            }

            result.Add("}");
            result.Add("AfterCopyProperties(other);");
            result.Add("}");

            result.AddRange(CreateComment("This method is called before copying the properties of another object to the current instance."));
            result.Add("/// <param name=\"other\">The object to copy the properties from.</param>");
            result.Add("/// <param name=\"handled\">A boolean value that indicates whether the method has been handled.</param>");
            result.Add($"partial void BeforeCopyProperties({copyType} other, ref bool handled);");

            result.AddRange(CreateComment("This method is called after copying properties from another instance of the class."));
            result.Add("/// <param name=\"other\">The other instance of the class from which properties were copied.</param>");
            result.Add($"partial void AfterCopyProperties({copyType} other);");
            return result.Where(l => string.IsNullOrEmpty(l) == false);
        }
        #endregion copy properties

        #region create equals and hashcode
        /// <summary>
        /// Creates an XML documentation for the method <c>CreateEquals</c>.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> parameter representing the type.</param>
        /// <param name="otherType">The <see cref="System.String"/> parameter representing the other type.</param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{System.String}"/> containing the XML documentation for the method.</returns>
        public virtual IEnumerable<string> CreateEquals(Type type, string otherType)
        {
            var result = new List<string>();
            var counter = 0;
            var generationProperties = GetGenerationProperties(type);

            if (generationProperties.Any())
            {
                result.AddRange(CreateComment("Determines whether the specified object is equal to the current object."));
                result.Add("/// <param name=\"obj\">The object to compare with the current object.</param>");
                result.Add($"public override bool Equals(object? obj)");
                result.Add("{");
                result.Add("bool result = false;");
                result.Add($"if (obj is {otherType} other)");
                result.Add("{");

                foreach (var pi in generationProperties)
                {
                    if (pi.CanRead && CanCreate(pi) && pi.IsNavigationProperties() == false)
                    {
                        var codeLine = counter == 0 ? "result = base.Equals(other) && " : "    && ";

                        if (pi.PropertyType.IsValueType)
                        {
                            codeLine += $"{pi.Name} == other.{pi.Name}";
                        }
                        else
                        {
                            codeLine += $"IsEqualsWith({pi.Name}, other.{pi.Name})";
                        }
                        result.Add(codeLine);
                        counter++;
                    }
                }

                if (counter > 0)
                {
                    result[^1] = $"{result[^1]};";
                }
                result.Add("}");
                result.Add("return result;");
                result.Add("}");
            }
            return result;
        }
        /// <summary>
        /// Creates the XML documentation for the <see cref="CreateGetHashCode"/> method.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to create the GetHashCode method for.</param>
        /// <returns>An <see cref="IEnumerable{string}"/> containing the XML documentation.</returns>
        public virtual IEnumerable<string> CreateGetHashCode(Type type)
        {
            var result = new List<string>();

            var braces = 0;
            var counter = 0;
            var codeLine = string.Empty;
            var generationProperties = GetGenerationProperties(type);

            if (generationProperties.Any())
            {
                result.AddRange(CreateComment("Serves as the default hash function."));
                result.Add($"public override int GetHashCode()");
                result.Add("{");

                foreach (var pi in generationProperties)
                {
                    if (pi.CanRead && CanCreate(pi) && pi.IsNavigationProperties() == false)
                    {
                        if (counter == 0)
                        {
                            braces++;
                            codeLine = "this.CalculateHashCode(base.GetHashCode(), ";
                        }
                        else
                        {
                            codeLine += ", ";
                        }
                        codeLine += pi.Name;
                        counter++;
                    }
                }
                for (int i = 0; i < braces; i++)
                {
                    codeLine += ")";
                }

                if (counter > 0)
                {
                    result.Add($"return {codeLine};");
                }
                else
                {
                    result.Add($"return base.GetHashCode();");
                }
                result.Add("}");
            }
            return result;
        }
        #endregion create equals and hashcode

        #region query settings
        /// <summary>
        /// Retrieves the query settings for a given unit type, item type, item name, sub-item names, and setting name.
        /// </summary>
        /// <param name="unitType">The unit type.</param>
        /// <param name="itemType">The item type.</param>
        /// <param name="itemName">The item name.</param>
        /// <param name="subItemNames">The sub-item names.</param>
        /// <param name="settingName">The setting name.</param>
        /// <returns>A dictionary containing the query settings.</returns>
        protected Dictionary<string, string> QuerySettings(string unitType, string itemType, string itemName, string[] subItemNames, string settingName)
        {
            var result = new Dictionary<string, string>();
            var settingNameValues = QuerySetting<string>(unitType, itemType, itemName, settingName, string.Empty)
                              .Split(valueSeparators, StringSplitOptions.RemoveEmptyEntries)
                              .ToList();

            foreach (var subItemName in subItemNames)
            {
                var fullItemName = $"{itemName}.{subItemName}";

                foreach (var value in QuerySetting<string>(unitType, itemType, fullItemName, settingName, string.Empty)
                                     .Split(valueSeparators, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (settingNameValues.Contains(value) == false)
                    {
                        settingNameValues.Add(value);
                    }
                }
            }
            foreach (var settingNameValue in settingNameValues)
            {
                var settingValue = QuerySetting<string>(unitType, itemType, itemName, settingNameValue, string.Empty);

                if (result.ContainsKey(settingNameValue) == false)
                {
                    result.Add(settingNameValue, settingValue.Replace(',', ';'));
                }
                else
                {
                    result[settingNameValue] = settingValue.Replace(',', ';');
                }
            }
            foreach (var subItemName in subItemNames)
            {
                var fullItemName = $"{itemName}.{subItemName}";

                foreach (var settingNameValue in settingNameValues)
                {
                    var settingValue = result.TryGetValue(settingNameValue, out string? value) ? value : string.Empty;

                    settingValue = QuerySetting<string>(unitType, itemType, itemName, settingNameValue, settingValue);
                    settingValue = QuerySetting<string>(unitType, itemType, fullItemName, settingNameValue, settingValue);
                    if (result.ContainsKey(settingNameValue) == false)
                    {
                        result.Add(settingNameValue, settingValue.Replace(',', ';'));
                    }
                    else
                    {
                        result[settingNameValue] = settingValue.Replace(',', ';');
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Queries the setting value of type <typeparamref name="T"/> based on the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of the setting value to query.</typeparam>
        /// <param name="unitType">The unit type.</param>
        /// <param name="itemType">The item type.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="valueName">The name of the value to query.</param>
        /// <param name="defaultValue">The default value to use if the query fails.</param>
        /// <returns>The queried setting value of type <typeparamref name="T"/>.</returns>
        protected T QuerySetting<T>(UnitType unitType, ItemType itemType, PropertyInfo propertyInfo, string valueName, string defaultValue)
        {
            return QuerySetting<T>(unitType.ToString(), itemType.ToString(), propertyInfo.DeclaringName(), valueName, defaultValue);
        }
        /// <summary>
        /// Generic type parameter representing the return type of the QuerySetting method.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        protected T QuerySetting<T>(UnitType unitType, ItemType itemType, Type type, string valueName, string defaultValue)
        {
            return QuerySetting<T>(unitType.ToString(), itemType.ToString(), ItemProperties.CreateSubTypeFromEntity(type), valueName, defaultValue);
        }
        /// <summary>
        /// Queries the setting of type T.
        /// </summary>
        /// <typeparam name="T">The type of the setting.</typeparam>
        /// <param name="unitType">The unit type.</param>
        /// <param name="itemType">The item type.</param>
        /// <param name="itemName">The item name.</param>
        /// <param name="valueName">The value name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The queried setting of type T.</returns>
        protected T QuerySetting<T>(UnitType unitType, ItemType itemType, string itemName, string valueName, string defaultValue)
        {
            return QuerySetting<T>(unitType.ToString(), itemType.ToString(), itemName, valueName, defaultValue);
        }
        /// <summary>
        /// Generic type parameter representing the return type of the QuerySetting method.
        /// </summary>
        /// <typeparam name="T">The type of the value to be returned.</typeparam>
        protected T QuerySetting<T>(string unitType, string itemType, string itemName, string valueName, string defaultValue)
        {
            T result;

            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(unitType, itemType, $"{itemName}", valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }
        #endregion query settings
    }
}

