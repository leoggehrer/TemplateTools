//@BaseCode

using System.Reflection;
using System.Text;

namespace TemplateTools.Logic.Generation
{
    /// <summary>
    /// Represents a class that contains properties and methods related to item properties.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ItemProperties"/> class.
    /// </remarks>
    /// <param name="solutionName">The name of the solution.</param>
    /// <param name="projectExtension">The extension of the project.</param>
    internal partial class ItemProperties(string solutionName, string projectExtension)
    {
        /// <summary>
        /// Gets the name of the solution.
        /// </summary>
        /// <value>
        /// A string representing the name of the solution.
        /// </value>
        public string SolutionName { get; } = solutionName;
        /// <summary>
        /// Gets the project file extension.
        /// </summary>
        /// <value>
        /// The project file extension.
        /// </value>
        public string ProjectExtension { get; } = projectExtension;
        /// <summary>
        /// Gets the namespace of the project.
        /// </summary>
        /// <value>
        /// The namespace of the project.
        /// </value>
        public string ProjectNamespace => $"{SolutionName}{ProjectExtension}";
        /// <summary>
        /// Gets the common project name.
        ///</summary>
        public string CommonProject => $"{SolutionName}{StaticLiterals.CommonExtension}";
        /// <summary>
        /// Gets the logic project name.
        ///</summary>
        public string LogicProject => $"{SolutionName}{StaticLiterals.LogicExtension}";

        #region solution properties
        /// <summary>
        /// Gets or sets an array of template project identifiers.
        /// </summary>
        /// <value>
        /// An array of template project identifiers.
        /// </value>
        public static string[] TemplateProjectIdentifiers => [.. CommonStaticLiterals.TemplateProjectExtensions.Select(e => e[1..])];
        #endregion solution properties

        #region item names
        /// <summary>
        /// Creates the entity name from the type.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <returns>The entity name.</returns>
        public static string CreateEntityName(Type type) => type.Name;
        /// <summary>
        /// Generates the typescript model name from the type.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <returns>The typescript model name.</returns>
        public static string CreateTSModelName(Type type) => $"I{type.Name}";
        /// <summary>
        /// Generates the typescript property name from the property info.
        /// </summary>
        /// <param name="type">The property info object.</param>
        /// <returns>The typescript property name.</returns>
        public static string CreateTSPropertyName(PropertyInfo propertyInfo)
        {
            var sb = new StringBuilder();
            var name = propertyInfo.Name;

            for (int i = 0; i < name.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append(char.ToLower(name[i]));
                }
                else if ((i + 1) < name.Length
                         && char.IsUpper(name[i]) && char.IsUpper(name[i + 1]))
                {
                    sb.Append(char.ToLower(name[i]));
                }
                else
                {
                    sb.Append(name[i]);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Creates the contract name for a given type.
        /// </summary>
        /// <param name="type">The type for which the model contract name needs to be created.</param>
        /// <returns>The model contract name in the format "I{type.Name}".</returns>
        public static string CreateContractName(Type type) => $"I{type.Name}";
        /// <summary>
        /// Creates the entity set name from the type.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <returns>The entity set name.</returns>
        public static string CreateEntitySetName(Type type) => $"{CreateEntityName(type)}Set";
        /// <summary>
        /// Creates the view set name from the type.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <returns>The view set name.</returns>
        public static string CreateViewSetName(Type type) => $"{CreateEntityName(type)}Set";
        /// <summary>
        /// Creates the contract set name from the type.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <returns>The contracts set name.</returns>
        public static string CreateContractSetName(Type type) => $"I{type.Name}Set";

        /// <summary>
        /// Creates a model name based on the specified type.
        /// </summary>
        /// <param name="type">The type to create the model name for.</param>
        /// <returns>The name of the model.</returns>
        public static string CreateModelName(Type type) => type.Name;
        /// <summary>
        /// Creates the name of the edit model based on the provided type.
        /// </summary>
        /// <param name="type">The type used to create the edit model name.</param>
        /// <returns>The edit model name.</returns>
        public static string CreateEditModelName(Type type)
        {
            return $"{CreateModelName(type)}Edit";
        }
        /// <summary>
        /// Creates the item name of the view model based on the provided type.
        /// </summary>
        /// <param name="type">The type used to create the view model name.</param>
        /// <returns>The view model name.</returns>
        public static string CreateItemViewModelName(Type type)
        {
            return $"{CreateModelName(type)}ViewModel";
        }
        /// <summary>
        /// Creates the items name of the view model based on the provided type.
        /// </summary>
        /// <param name="type">The type used to create the view model name.</param>
        /// <returns>The view model name.</returns>
        public static string CreateItemsViewModelName(Type type)
        {
            return $"{CreateModelName(type).CreatePluralWord()}ViewModel";
        }
        #endregion item names

        #region entity items
        /// <summary>
        /// Diese Methode ermittelt den Sub-Typ aus einem Entity-Type (eg. App.Type).
        /// </summary>
        /// <param name="type">Entity-Typ</param>
        /// <returns>Sub-Typ</returns>
        public static string CreateSubTypeFromEntity(Type type)
        {
            var entityName = CreateEntityName(type);
            var namespaceItems = CreateNamespaceItems(type, StaticLiterals.EntitiesFolder).Skip(1);

            return $"{string.Join('.', namespaceItems)}{(namespaceItems.Any() ? "." : string.Empty)}{entityName}";
        }
        /// <summary>
        /// Diese Methode ermittelt den Teilnamensraum von einem Entity-Type.
        /// </summary>
        /// <param name="type">Typ von welchem der Teilnamensraum ermittelt wird.</param>
        /// <param name="replaceFolder">Ersetzt den entities folder.</param>
        /// <returns>Teil-Namensraum</returns>
        public static string CreateSubNamespaceFromEntity(Type type, string replaceFolder)
        {
            var namespaceItems = type.Namespace?.Split('.').Skip(2);
            var result = string.Join('.', namespaceItems!);

            return result.Replace(StaticLiterals.EntitiesFolder, replaceFolder);
        }
        #endregion entity items

        #region controller items
        /// <summary>
        /// Creates a controller name for the given type.
        /// </summary>
        /// <param name="type">The type used to create the controller name.</param>
        /// <returns>The controller name.</returns>
        public static string CreateControllerName(Type type)
        {
            return $"{type.Name.CreatePluralWord()}";
        }
        /// <summary>
        /// Creates the name of the controller class based on the given type.
        /// </summary>
        /// <param name="type">The type for which the controller class name is to be created.</param>
        /// <returns>The controller class name.</returns>
        public static string CreateControllerClassName(Type type)
        {
            return $"{CreateControllerName(type)}Controller";
        }
        ///<summary>
        /// Creates the fully qualified name of the controller type.
        ///</summary>
        ///<param name="type">The type of the controller.</param>
        ///<returns>The fully qualified name of the controller type.</returns>
        public string CreateControllerType(Type type)
        {
            return $"{CreateControllerNamespace(type)}.{CreateControllerClassName(type)}";
        }
        /// <summary>
        /// Creates the controller namespace for a given type.
        /// </summary>
        /// <param name="type">The type of the controller.</param>
        /// <returns>The fully qualified controller namespace.</returns>
        public string CreateControllerNamespace(Type type)
        {
            return $"{ProjectNamespace}.{CreateControllerSubNamespace(type)}";
        }
        /// <summary>
        /// Creates the sub-namespace for the controller based on the specified type.
        /// </summary>
        /// <param name="type">The type used to create the sub-namespace.</param>
        /// <returns>The sub-namespace for the controller.</returns>
        public string CreateControllerSubNamespace(Type type)
        {
            var namespaceItems = CreateModuleSubNamespaceItems(type, StaticLiterals.ControllersFolder);

            return $"{string.Join('.', namespaceItems)}";
        }
        /// <summary>
        /// Creates the subpath for controllers based on the given type, post-fix, and file extension.
        /// </summary>
        /// <param name="type">The type to create the subpath from.</param>
        /// <param name="postFix">The post-fix to append to the subpath.</param>
        /// <param name="fileExtension">The file extension to include in the subpath.</param>
        /// <returns>The created subpath for controllers.</returns>
        public string CreateControllersSubPathFromType(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(CreateControllerSubNamespace(type).Replace(".", Path.DirectorySeparatorChar.ToString()), $"{CreateControllerClassName(type)}{postFix}{fileExtension}");
        }
        #endregion controller items

        #region type items
        /// <summary>
        /// Creates the model type based on the specified type.
        /// </summary>
        /// <param name="type">The type used to create the model type.</param>
        /// <returns>The fully qualified model type in the format 'ProjectNamespace.ModelSubType'.</returns>
        public string CreateModelType(Type type)
        {
            return $"{ProjectNamespace}.{CreateModelSubType(type)}";
        }
        /// <summary>
        /// Creates the model subtype for the specified type.
        /// </summary>
        /// <param name="type">The type to create the subtype for.</param>
        /// <returns>The fully qualified model subtype name.</returns>
        public string CreateModelSubType(Type type)
        {
            var subNamespace = CreateModelSubNamespace(type);

            return $"{subNamespace}.{CreateModelName(type)}";
        }

        /// <summary>
        /// Constructs the namespace for the model based on the given type.
        /// </summary>
        /// <param name="type">The type used to create the model namespace.</param>
        /// <returns>The fully qualified model namespace.</returns>
        public string CreateModelNamespace(Type type)
        {
            return $"{ProjectNamespace}.{CreateModelSubNamespace(type)}";
        }
        /// <summary>
        /// Creates a model sub path based on the provided type, postFix, and fileExtension.
        /// </summary>
        /// <param name="type">The type of the model.</param>
        /// <param name="postFix">The post fix to be appended to the model's name.</param>
        /// <param name="fileExtension">The extension of the file.</param>
        /// <returns>The created model sub path.</returns>
        public string CreateModelSubPath(Type type, string postFix, string fileExtension)
        {
            return Path.Combine(CreateModelSubNamespace(type).Replace(".", Path.DirectorySeparatorChar.ToString()), $"{type.Name}{postFix}{fileExtension}");
        }
        /// <summary>
        /// Converts the full name of an entity type to the full name of a model type.
        /// </summary>
        /// <param name="typeFullname">The full name of the entity type.</param>
        /// <returns>The full name of the corresponding model type.</returns>
        public string ConvertEntityToModelType(string typeFullname)
        {
            var result = typeFullname;
            var entitiesFolder = $".{StaticLiterals.EntitiesFolder}.";
            var modelsFolder = $".{StaticLiterals.ModelsFolder}.";

            if (result.Contains(entitiesFolder))
            {
                result = result.Replace(entitiesFolder, modelsFolder);
                result = result.Replace(StaticLiterals.LogicExtension, ProjectExtension);
            }
            return result;
        }
        /// <summary>
        /// This method creates the model sub namespace from a project type.
        /// For example:
        ///     FullName SETemplate.Logic.Entities.Base.Artist becomes SubName Models.Base.Artist.
        /// </summary>
        /// <param name="type">The Type from which the subnamespace is created.</param>
        /// <returns>The subnamespace as a string.</returns>
        public string CreateModelSubNamespace(Type type)
        {
            var result = string.Empty;

            if (IsModelType(type))
            {
                var namespaceItems = CreateModuleSubNamespaceItems(type, StaticLiterals.ModelsFolder);

                result = string.Join('.', namespaceItems);
            }
            else if (IsEntityType(type))
            {
                var namespaceItems = CreateModuleSubNamespaceItems(type, StaticLiterals.ModelsFolder);

                result = string.Join('.', namespaceItems);
            }
            else if (IsViewType(type))
            {
                var namespaceItems = CreateModuleSubNamespaceItems(type, StaticLiterals.ModelsFolder);

                result = string.Join('.', namespaceItems);
            }
            return result;
        }
        /// <summary>
        /// Gets the default visibility for the specified type.
        /// </summary>
        /// <param name="type">The type for which to get the default visibility.</param>
        /// <returns>
        /// A string representing the default visibility of the type.
        /// Returns "internal" if the type is a system entity; otherwise, returns "public".
        /// </returns>
        public static string GetDefaultVisibility(Type type)
        {
            return EntityProject.IsSystemEntity(type) ? "internal" : "public";
        }

        /// <summary>
        /// Retrieves the base interface for the specified entity type.
        /// </summary>
        /// <param name="type">The type of the entity for which the base interface is to be determined.</param>
        /// <returns>
        /// A string representing the base interface of the entity type.
        /// If the entity type has a base type that is not a standard entity or versioned entity,
        /// the contract name of the base type is returned. Otherwise, a default global identifiable name is returned.
        /// </returns>
        public static string GetEntityBaseInterface(Type type)
        {
            var baseType = type.BaseType;
            string result;

            if (baseType != null && baseType.Name.Equals(StaticLiterals.EntityObjectName))
            {
                result = $"{StaticLiterals.GlobalUsingIdentifiableName}";
            }
            else if (baseType != null && baseType.Name.Equals(StaticLiterals.VersionEntityObjectName))
            {
                result = $"{StaticLiterals.GlobalUsingVersionableName}";
            }
            else if (baseType != null)
            {
                result = $"{CreateContractName(baseType)}";
            }
            else
            {
                result = $"{StaticLiterals.GlobalUsingIdentifiableName}";
            }

            return result;
        }
        #endregion type items

        #region logic items
        /// <summary>
        /// Creates the full contract type for the given type.
        /// </summary>
        /// <param name="type">The type for which the full contract type needs to be created.</param>
        /// <returns>The full contract type as a string.</returns>
        public string CreateFullContractType(Type type)
        {
            return EntityProject.IsSystemEntity(type) ? CreateFullLogicContractType(type)
                                                      : CreateFullCommonContractType(type);
        }
        /// <summary>
        /// Creates the full namespace for the specified type.
        /// </summary>
        /// <param name="type">The type for which the namespace is created.</param>
        /// <param name="preItems">Optional array of pre-defined namespace items.</param>
        /// <returns>The full namespace.</returns>
        public string CreateFullNamespace(Type type, params string[] preItems)
        {
            return EntityProject.IsSystemEntity(type) ? CreateFullLogicNamespace(type, preItems)
                                                      : CreateFullCommonNamespace(type, preItems);
        }

        ///<summary>
        /// Creates the full common contract type for the given type.
        ///</summary>
        ///<param name="type">The type for which the full common model contract type needs to be created.</param>
        ///<returns>The full logic model contract type.</returns>
        public string CreateFullCommonContractType(Type type)
        {
            return CreateFullCommonType(type, CreateContractName(type), StaticLiterals.ContractsFolder);
        }
        /// <summary>
        /// Creates the full common type by combining the common namespace, type name, and any optional pre-items.
        /// </summary>
        /// <param name="type">The type of the logic.</param>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="preItems">Optional pre-items to be included in the full logic type.</param>
        /// <returns>The full logic type as a string.</returns>
        public string CreateFullCommonType(Type type, string typeName, params string[] preItems)
        {
            return $"{CreateFullCommonNamespace(type, preItems)}.{typeName}";
        }
        /// <summary>
        /// Creates the full common namespace for the specified type.
        /// </summary>
        /// <param name="type">The type for which the logic namespace is created.</param>
        /// <param name="preItems">Optional array of pre-defined namespace items.</param>
        /// <returns>The full logic namespace.</returns>
        public string CreateFullCommonNamespace(Type type, params string[] preItems)
        {
            var namespaceItems = CreateModuleSubNamespaceItems(type, preItems);

            return $"{CommonProject}.{string.Join('.', namespaceItems)}";
        }

        ///<summary>
        /// Creates the full common contract type for the given type.
        ///</summary>
        ///<param name="type">The type for which the full common model contract type needs to be created.</param>
        ///<returns>The full logic model contract type.</returns>
        public string CreateFullLogicContractType(Type type)
        {
            return CreateFullLogicType(type, CreateContractName(type), StaticLiterals.ContractsFolder);
        }
        /// <summary>
        /// Creates the full common type by combining the common namespace, type name, and any optional pre-items.
        /// </summary>
        /// <param name="type">The type of the logic.</param>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="preItems">Optional pre-items to be included in the full logic type.</param>
        /// <returns>The full logic type as a string.</returns>
        public string CreateFullLogicType(Type type, string typeName, params string[] preItems)
        {
            return $"{CreateFullLogicNamespace(type, preItems)}.{typeName}";
        }
        /// <summary>
        /// Creates the full common namespace for the specified type.
        /// </summary>
        /// <param name="type">The type for which the logic namespace is created.</param>
        /// <param name="preItems">Optional array of pre-defined namespace items.</param>
        /// <returns>The full logic namespace.</returns>
        public string CreateFullLogicNamespace(Type type, params string[] preItems)
        {
            var namespaceItems = CreateModuleSubNamespaceItems(type, preItems);

            return $"{LogicProject}.{string.Join('.', namespaceItems)}";
        }
        #endregion logic items

        #region subType items
        /// <summary>
        /// Returns the subtype of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which the subtype needs to be determined.</param>
        /// <returns>The subtype of the specified <paramref name="type"/>.</returns>
        public string GetSubType(Type type)
        {
            return GetSubType(type.FullName!);
        }
        /// <summary>
        /// Retrieves the sub-type from the given type's full name, by filtering out specific elements.
        /// </summary>
        /// <param name="typeFullName">The full name of the type.</param>
        /// <returns>The sub-type extracted from the full name.</returns>
        public string GetSubType(string typeFullName)
        {
            var ignore = false;
            var result = new List<string>();
            var typeItems = typeFullName.Split('.');

            for (int i = 0; i < typeItems.Length; i++)
            {
                if (typeItems[i].Equals(SolutionName))
                {
                    ignore = true;
                }
                else if (TemplateProjectIdentifiers.Any(e => e.Equals(typeItems[i])))
                {
                    ignore = true;
                }
                else if (StaticLiterals.ModuleFolders.Any(e => e.Equals(typeItems[i])))
                {
                    ignore = true;
                }
                else
                {
                    ignore = false;
                }
                if (ignore == false)
                {
                    result.Add(typeItems[i]);
                }
            }
            return string.Join('.', result);
        }

        /// <summary>
        /// Retrieves the module subtype for the specified type.
        /// </summary>
        /// <param name="type">The type to retrieve the module subtype for.</param>
        /// <returns>The module subtype.</returns>
        public string GetModuleSubType(Type type)
        {
            return GetModuleSubType(type.FullName!);
        }
        /// <summary>
        /// Retrieves the module sub-type from the given type full name.
        /// </summary>
        /// <param name="typeFullName">The full name of the type.</param>
        /// <returns>The module sub-type as a string.</returns>
        public string GetModuleSubType(string typeFullName)
        {
            var ignore = false;
            var result = new List<string>();
            var typeItems = typeFullName.Split('.');

            for (int i = 0; i < typeItems.Length; i++)
            {
                if (typeItems[i].Equals(SolutionName))
                {
                    ignore = true;
                }
                else if (TemplateProjectIdentifiers.Any(e => e.Equals(typeItems[i])))
                {
                    ignore = true;
                }
                else
                {
                    ignore = false;
                }
                if (ignore == false)
                {
                    result.Add(typeItems[i]);
                }
            }
            return string.Join('.', result);
        }
        #endregion subType items

        #region path
        /// <summary>
        /// Creates a sub file path.
        /// </summary>
        /// <param name="type">The type used to create the module sub namespace items.</param>
        /// <param name="fileName">The file name to be appended to the sub path.</param>
        /// <param name="preItems">Optional additional items to be included in the sub path.</param>
        /// <returns>The sub file path.</returns>
        public string CreateSubFilePath(Type type, string fileName, params string[] preItems)
        {
            var namespaceItems = CreateModuleSubNamespaceItems(type, preItems);

            return $"{string.Join(Path.DirectorySeparatorChar, namespaceItems!.Union([fileName]))}";
        }

        /// <summary>
        /// Diese Methode ermittelt den Teilnamensraum von einem Typ.
        /// </summary>
        /// <param name="type">Typ von welchem der Teilnamensraum ermittelt wird.</param>
        /// <returns>Teil-Namensraum</returns>
        public static string CreateSubNamespaceFromType(Type type)
        {
            var namespaceItems = type.Namespace?.Split('.').Skip(2);

            return string.Join('.', namespaceItems!);
        }

        /// <summary>
        /// Creates a subpath from a given type's namespace.
        /// </summary>
        /// <param name="type">The type whose namespace will be used to create the subpath.</param>
        /// <returns>A string representing the subpath created from the type's namespace.</returns>
        public static string CreateSubPathFromType(Type type)
        {
            var namespaceItems = type.Namespace?.Split('.').Skip(2);

            return string.Join(Path.DirectorySeparatorChar, namespaceItems!);
        }

        ///<summary>
        /// Creates a collection of namespace items based on the provided type and cache item.
        ///</summary>
        ///<param name="type">The type used to obtain the namespace.</param>
        ///<param name="catcheItem">The cache item used to determine where to start collecting the namespace items.</param>
        ///<returns>
        /// Returns an enumerable collection of strings representing the namespace items.
        /// If the cache item is found, the collection will start from that item.
        /// If the cache item is not found, the collection will contain all the namespace items in the type's namespace.
        ///</returns>
        public static IEnumerable<string> CreateNamespaceItems(Type type, string catcheItem)
        {
            var catched = false;
            var result = new List<string>();
            var namespaceItems = type.Namespace?.Split('.');

            for (int i = 0; i < namespaceItems?.Length; i++)
            {
                if (namespaceItems[i].Equals(catcheItem))
                {
                    catched = true;
                }
                if (catched)
                {
                    result.Add(namespaceItems[i]);
                }
            }
            return catched ? result : namespaceItems!;
        }
        /// <summary>
        ///    Creates a collection of sub-namespace items for a given type.
        /// </summary>
        /// <param name="type">The type for which to create the sub-namespace items.</param>
        /// <param name="preItems">Optional pre-existing items that will be included in the result.</param>
        /// <returns>
        ///    An enumerable collection of string values representing the sub-namespace items.
        /// </returns>
        /// <remarks>
        ///    This method analyzes the namespace of the given type and creates a collection
        ///    of sub-namespace items, excluding certain items based on defined conditions.
        ///    The resulting collection is a combination of pre-existing items (if any) and
        ///    the dynamically generated sub-namespace items.
        /// </remarks>
        public IEnumerable<string> CreateModuleSubNamespaceItems(Type type, params string[] preItems)
        {
            var ignore = false;
            var result = new List<string>();
            var namespaceItems = type.Namespace?.Split('.');

            for (int i = 0; i < namespaceItems?.Length; i++)
            {
                if (namespaceItems[i].Equals(SolutionName))
                {
                    ignore = true;
                }
                else if (TemplateProjectIdentifiers.Any(e => e.Equals(namespaceItems[i])))
                {
                    ignore = true;
                }
                else if (StaticLiterals.ModuleFolders.Any(e => e.Equals(namespaceItems[i])))
                {
                    ignore = true;
                }
                else
                {
                    ignore = false;
                }
                if (ignore == false)
                {
                    result.Add(namespaceItems[i]);
                }
            }
            return preItems.Union(result);
        }
        #endregion paths

        #region type infos
        /// <summary>
        /// Determines whether the given property is a nullable primitive type.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>True if the property is a nullable primitive type; otherwise, false.</returns>
        public static bool IsPrimitiveNullable(PropertyInfo propertyInfo)
        {
            var result = propertyInfo.PropertyType.IsPrimitive
                      && propertyInfo.PropertyType.IsNullableType();

            //if (result)
            //{
            //    result = propertyInfo.PropertyType.GetGenericArguments()[0].IsPrimitive;
            //}
            return result;
        }
        /// <summary>
        /// Determines whether the specified type is an array type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is an array; otherwise, <c>false</c>.
        /// </returns>
        /// /
        public static bool IsArrayType(Type type)
        {
            return type.IsArray;
        }
        /// <summary>
        /// Determines whether the specified type is a list type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the type is a list type; otherwise, false.</returns>
        public static bool IsListType(Type type)
        {
            return type.FullName!.StartsWith("System.Collections.Generic.List")
                || type.FullName!.StartsWith("System.Collections.Generic.IList");
        }
        /// <summary>
        /// Determines whether the given type is an entity type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// <c>true</c> if the given type is an entity type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEntityType(Type type)
        {
            return type.GetBaseTypes().FirstOrDefault(t => t.Name.Equals(StaticLiterals.EntityObjectName)) != null;
        }
        /// <summary>
        /// Determines whether the given type is an view type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// <c>true</c> if the given type is an entity type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsViewType(Type type)
        {
            return type.GetBaseTypes().FirstOrDefault(t => t.Name.Equals(StaticLiterals.ViewObjectName)) != null;
        }
        /// <summary>
        /// Checks if the specified type is a List type containing entities.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>True if the type is a List type containing entities, otherwise false.</returns>
        public static bool IsEntityListType(Type type)
        {
            var result = false;

            if (type.IsGenericType && IsListType(type))
            {
                var genericType = type.GetGenericArguments()[0];

                result = IsEntityType(genericType);
            }
            return result;
        }
        /// <summary>
        /// Checks if the specified type is a Array type containing entities.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>True if the type is a Array type containing entities, otherwise false.</returns>
        public static bool IsEntityArrayType(Type type)
        {
            var result = false;

            if (IsArrayType(type))
            {
                var arrayType = type.GetElementType();

                result = arrayType != default && IsEntityType(arrayType);
            }
            return result;
        }
        /// <summary>
        /// Determines whether the specified property is an array of primitive types.
        /// </summary>
        /// <param name="propertyInfo">The property information to check.</param>
        /// <returns>
        /// <c>true</c> if the property is an array and its element type is a primitive type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrimitiveArrayType(PropertyInfo propertyInfo)
        {
            return IsPrimitiveArrayType(propertyInfo.PropertyType);
        }
        /// <summary>
        /// Determines whether the specified type is an array of primitive types.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// <c>true</c> if the specified type is an array and its element type is a primitive type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrimitiveArrayType(Type type)
        {
            return IsArrayType(type) && type.GetElementType()!.IsPrimitive;
        }
        /// <summary>
        /// Determines whether the specified Type is a model type.
        /// </summary>
        /// <param name="type">The Type to be checked.</param>
        /// <returns>True if the specified Type is a model type; otherwise, False.</returns>
        public static bool IsModelType(Type type)
        {
            return type.GetBaseTypes().FirstOrDefault(t => t.Name.Equals(StaticLiterals.ModelObjectName)) != null;
        }
        #endregion type infos
    }
}

