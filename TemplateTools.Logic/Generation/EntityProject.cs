//@BaseCode
namespace TemplateTools.Logic.Generation
{
    using System.Reflection;
    using TemplateTools.Logic.Contracts;
    /// <summary>
    /// Represents an internal sealed partial class for working with entity projects.
    /// </summary>
    internal sealed partial class EntityProject
    {
        /// <summary>
        /// Gets or sets the solution properties.
        /// </summary>
        /// <value>
        /// The solution properties.
        /// </value>
        public ISolutionProperties SolutionProperties { get; private set; }
        /// <summary>
        /// Gets the name of the project by combining the solution name and logic extension.
        /// </summary>
        /// <value>The name of the project.</value>
        public string ProjectName => $"{SolutionProperties.SolutionName}{SolutionProperties.LogicExtension}";
        /// <summary>
        /// Gets the full path of the project, combining the solution path and project name.
        /// </summary>
        /// <value>The full path of the project.</value>
        public string ProjectPath => Path.Combine(SolutionProperties.SolutionPath, ProjectName);
        
        /// <summary>
        /// Initializes a new instance of the EntityProject class.
        /// </summary>
        /// <param name="solutionProperties">The solution properties.</param>
        private EntityProject(ISolutionProperties solutionProperties)
        {
            SolutionProperties = solutionProperties;
        }
        /// <summary>
        /// Creates a new instance of the EntityProject class using the provided solution properties.
        /// </summary>
        /// <param name="solutionProperties">The solution properties to be used in creating the EntityProject instance.</param>
        /// <returns>A new instance of the EntityProject class.</returns>
        public static EntityProject Create(ISolutionProperties solutionProperties)
        {
            return new(solutionProperties);
        }
        
        private IEnumerable<Type>? assemblyTypes;
        ///<summary>
        /// Gets or sets the types in the assembly.
        ///</summary>
        ///<value>
        /// An IEnumerable of Type representing the collection of types in the assembly.
        ///</value>
        public IEnumerable<Type> AssemblyTypes
        {
            get
            {
                assemblyTypes = assemblyTypes ??= SolutionProperties.LogicAssemblyTypes;
                if (assemblyTypes == null)
                {
                    if (SolutionProperties.CompileLogicAssemblyFilePath.HasContent() && File.Exists(SolutionProperties.CompileLogicAssemblyFilePath))
                    {
                        var assembly = Assembly.LoadFile(SolutionProperties.CompileLogicAssemblyFilePath!);
                        
                        if (assembly != null)
                        {
                            try
                            {
                                assemblyTypes = assembly.GetTypes();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in {nameof(assemblyTypes)}: {ex.Message}");
                            }
                        }
                    }
                    if (assemblyTypes == null && SolutionProperties.LogicAssemblyFilePath.HasContent() && File.Exists(SolutionProperties.LogicAssemblyFilePath))
                    {
                        var assembly = Assembly.LoadFile(SolutionProperties.LogicAssemblyFilePath);
                        
                        if (assembly != null)
                        {
                            try
                            {
                                assemblyTypes = assembly.GetTypes();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in {nameof(assemblyTypes)}: {ex.Message}");
                            }
                        }
                    }
                }
                return assemblyTypes ?? [];
            }
        }
        
        /// <summary>
        /// Retrieves the collection of all enum types within the assembly.
        /// </summary>
        /// <value>
        /// An IEnumerable of Type objects that represent the enum types.
        /// </value>
        public IEnumerable<Type> EnumTypes => AssemblyTypes.Where(t => t.IsEnum && t.IsNested == false);
        /// <summary>
        /// Gets the interface types found within the assembly.
        /// </summary>
        /// <value>
        /// An enumerable collection of Type objects representing the interface types.
        /// </value>
        public IEnumerable<Type> InterfaceTypes => AssemblyTypes.Where(t => t.IsInterface);

        /// <summary>
        /// Gets the collection of view types, within the assembly.
        /// </summary>
        public IEnumerable<Type> AllViewTypes => AssemblyTypes.Where(t => t.IsClass
                                                                         && t.IsAbstract == false
                                                                         && t.IsNested == false
                                                                         && t.Namespace != null
                                                                         && t.Namespace!.Contains($".{StaticLiterals.EntitiesFolder}")
                                                                         && t.GetBaseTypes().FirstOrDefault(t => t.Name.Equals(StaticLiterals.ViewObjectName)) != null);

        /// <summary>
        /// Gets the collection of entity types, within the assembly.
        /// </summary>
        public IEnumerable<Type> AllEntityTypes => AssemblyTypes.Where(t => t.IsClass
                                                                         && t.IsAbstract == false
                                                                         && t.IsNested == false
                                                                         && t.Namespace != null
                                                                         && t.Namespace!.Contains($".{StaticLiterals.EntitiesFolder}")
                                                                         && t.GetBaseTypes().FirstOrDefault(t => t.Name.Equals(StaticLiterals.EntityObjectName)) != null);

        /// <summary>
        /// Gets the collection of entity types, excluding certain types, within the assembly.
        /// </summary>
        public IEnumerable<Type> EntityTypes => AllEntityTypes.Where(t => t.FullName!.Contains($"{StaticLiterals.EntitiesFolder}.{StaticLiterals.AccountFolder}.") == false
                                                                       && t.FullName!.Contains($"{StaticLiterals.EntitiesFolder}.{StaticLiterals.LoggingFolder}.") == false
                                                                       && t.FullName!.Contains($"{StaticLiterals.EntitiesFolder}.{StaticLiterals.LoggingFolder}.") == false);

        /// <summary>
        /// Determines if the specified <paramref name="type"/> is an account entity.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="type"/> is an account entity; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAccountEntity(Type type)
        {
            var result = type.FullName!.EndsWith($".{StaticLiterals.Account}.Identity")
            || type.FullName!.EndsWith($".{StaticLiterals.Account}.IdentityXRole")
            || type.FullName!.EndsWith($".{StaticLiterals.Account}.LoginSession")
            || type.FullName!.EndsWith($".{StaticLiterals.Account}.Role")
            || type.FullName!.EndsWith($".{StaticLiterals.Account}.SecureIdentity")
            || type.FullName!.EndsWith($".{StaticLiterals.Account}.User");
            
            return result;
        }
        /// <summary>
        /// Determines whether the specified type is an access entity.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is an access entity; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAccessEntity(Type type)
        {
            return type.FullName!.EndsWith($".{StaticLiterals.Access}.AccessRule");
        }
        /// <summary>
        /// Determines whether the specified type is a logging entity.
        /// </summary>
        /// <param name="type">
        /// The type to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified type is a logging entity; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLoggingEntity(Type type)
        {
            return type.FullName!.EndsWith($".{StaticLiterals.Logging}.ActionLog");
        }
        /// <summary>
        /// Checks if the specified type is a revision entity.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is a revision entity; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRevisionEntity(Type type)
        {
            return type.FullName!.EndsWith($".{StaticLiterals.Revision}.History");
        }
        ///<summary>
        ///Checks if the given type is a system entity.
        ///</summary>
        ///<param name="type">The type to check.</param>
        ///<returns>True if the type is a system entity, false otherwise.</returns>
        public static bool IsSystemEntity(Type type)
        {
            return IsAccountEntity(type) || IsAccessEntity(type) || IsLoggingEntity(type) || IsRevisionEntity(type);
        }
        ///<summary>
        ///Checks if the given type is a custom entity.
        ///</summary>
        ///<param name="type">The type to check.</param>
        ///<returns>True if the type is a custom entity, false otherwise.</returns>
        public static bool IsCustomEntity(Type type)
        {
            return IsSystemEntity(type) == false;
        }
    }
}

