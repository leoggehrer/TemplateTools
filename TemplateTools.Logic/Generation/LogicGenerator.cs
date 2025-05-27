//@BaseCode
namespace TemplateTools.Logic.Generation
{
    using System.Reflection;
    using TemplateTools.Logic.Contracts;
    using TemplateTools.Logic.Models;

    /// <summary>
    /// Represents a logic generator that is responsible for generating logic-related code.
    /// </summary>
    internal sealed partial class LogicGenerator : ModelGenerator
    {
        #region fields
        private ItemProperties? _itemProperties;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets or sets the ItemProperties for the current instance.
        /// </summary>
        protected override ItemProperties ItemProperties => _itemProperties ??= new ItemProperties(SolutionProperties.SolutionName, StaticLiterals.LogicExtension);
        /// <summary>
        /// Gets or sets a value indicating whether to generate the database context.
        /// </summary>
        public bool GenerateDbContext { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to generate the entity contracts.
        /// </summary>
        public bool GenerateEntityContracts { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to generate the entity sets.
        /// </summary>
        public bool GenerateEntitySets { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to generate the entity set contracts.
        /// </summary>
        public bool GenerateEntitySetContracts { get; set; }
        #endregion properties

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicGenerator"/> class.
        /// </summary>
        /// <param name="solutionProperties">The solution properties.</param>
        public LogicGenerator(ISolutionProperties solutionProperties) : base(solutionProperties)
        {
            var generateAll = QuerySetting<string>(Common.ItemType.AllItems, StaticLiterals.AllItems, StaticLiterals.Generate, "True");

            GenerateDbContext = QuerySetting<bool>(Common.ItemType.DbContext, StaticLiterals.AllItems, StaticLiterals.Generate, generateAll);
            GenerateEntityContracts = QuerySetting<bool>(Common.ItemType.EntityContract, StaticLiterals.AllItems, StaticLiterals.Generate, generateAll);
            GenerateEntitySets = QuerySetting<bool>(Common.ItemType.EntitySet, StaticLiterals.AllItems, StaticLiterals.Generate, generateAll);
            GenerateEntitySetContracts = QuerySetting<bool>(Common.ItemType.EntitySetContract, StaticLiterals.AllItems, StaticLiterals.Generate, generateAll);
        }
        #endregion constructors

        #region generations
        /// <summary>
        /// Generates all the required items for the application.
        /// </summary>
        /// <returns>An enumerable list of generated items.</returns>
        public IEnumerable<IGeneratedItem> GenerateAll()
        {
            var result = new List<IGeneratedItem>();

            result.AddRange(CreateEntityContracts());
            result.AddRange(ConnectEntitiesWithContract());
            result.AddRange(CreateEntitySetContracts());
            result.AddRange(CreateEntitySets());

            result.AddRange(ConnectViewsWithContract());
            result.AddRange(CreateViewSetContracts());
            result.AddRange(CreateViewSets());

            result.Add(CreateDbContext());
            result.Add(CreateDbContextContract(Common.UnitType.Logic, Common.ItemType.ContextContract));
            return result;
        }

        /// <summary>
        ///   Determines whether the specified type should generate default values.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is not a generation entity; otherwise, <c>false</c>.
        /// </returns>
        private static bool GetGenerateDefault(Type type)
        {
            return EntityProject.IsSystemEntity(type) || EntityProject.IsCustomEntity(type);
        }

        /// <summary>
        /// Creates a database context for the project.
        /// </summary>
        /// <returns>The generated item representing the database context.</returns>
        private GeneratedItem CreateDbContext()
        {
            var entityProject = EntityProject.Create(SolutionProperties);
            var dataContextNamespace = $"{ItemProperties.ProjectNamespace}.{StaticLiterals.DataContextFolder}";
            var result = new GeneratedItem(Common.UnitType.Logic, Common.ItemType.DbContext)
            {
                FullName = $"{dataContextNamespace}.ProjectDbContext",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = $"DataContext{Path.DirectorySeparatorChar}ProjectDbContext{StaticLiterals.GenerationPostFix}{StaticLiterals.CSharpFileExtension}",
            };
            result.AddRange(CreateComment());
            result.Add($"partial class ProjectDbContext");
            result.Add("{");

            result.Add("#region properties");
            foreach (var type in entityProject.AllEntityTypes)
            {
                var defaultValue = (GenerateDbContext && GetGenerateDefault(type)).ToString();

                if (QuerySetting<bool>(Common.ItemType.DbContext, type, StaticLiterals.Generate, defaultValue))
                {
                    var entityType = ItemProperties.GetModuleSubType(type);
                    var entitySubType = $"{StaticLiterals.EntitiesFolder}.{ItemProperties.CreateSubTypeFromEntity(type)}";
                    var entitySetName = ItemProperties.CreateEntitySetName(type);
                    var contractSetName = ItemProperties.CreateContractSetName(type);
                    var contractSubNamespace = ItemProperties.CreateSubNamespaceFromEntity(type, StaticLiterals.ContractsFolder);
                    var contractSetSubType = $"{contractSubNamespace}.{contractSetName}";
                    var dbSetName = $"Db{type.Name}Set";
                    var dbSetType = $"DbSet<{entityType}>";

                    result.AddRange(CreateComment(type));
                    result.Add($"private {dbSetType} {dbSetName}" + "{ get; set; }");

                    result.AddRange(CreateComment(type));
                    result.Add($"public {contractSetSubType} {entitySetName} => ({contractSetSubType})GetEntitySet<{entitySubType}>();");
                }
            }

            foreach (var type in entityProject.AllViewTypes)
            {
                var defaultValue = (GenerateDbContext && GetGenerateDefault(type)).ToString();

                if (QuerySetting<bool>(Common.ItemType.DbContext, type, StaticLiterals.Generate, defaultValue))
                {
                    var entityType = ItemProperties.GetModuleSubType(type);
                    var entitySubType = $"{StaticLiterals.EntitiesFolder}.{ItemProperties.CreateSubTypeFromEntity(type)}";
                    var viewSetName = ItemProperties.CreateViewSetName(type);
                    var contractSetName = ItemProperties.CreateContractSetName(type);
                    var contractSubNamespace = ItemProperties.CreateSubNamespaceFromEntity(type, StaticLiterals.ContractsFolder);
                    var contractSetSubType = $"{contractSubNamespace}.{contractSetName}";
                    var dbSetName = $"Db{type.Name}Set";
                    var dbSetType = $"DbSet<{entityType}>";

                    result.AddRange(CreateComment(type));
                    result.Add($"private {dbSetType} {dbSetName}" + "{ get; set; }");

                    result.AddRange(CreateComment(type));
                    result.Add($"public {contractSetSubType} {viewSetName} => ({contractSetSubType})GetViewSet<{entitySubType}>();");
                }
            }

            result.Add("#endregion properties");
            result.Add(string.Empty);

            result.Add("#region partial methods");
            result.AddRange(CreateComment());
            result.Add($"partial void GetGeneratorDbSet<E>(ref DbSet<E>? dbSet, ref bool handled) where E : Entities.{StaticLiterals.DbObjectName}");
            result.Add("{");

            bool first = false;

            foreach (var type in entityProject.AllEntityTypes.Union(entityProject.AllViewTypes))
            {
                var defaultValue = (GenerateDbContext && GetGenerateDefault(type)).ToString();

                if (QuerySetting<bool>(Common.ItemType.DbContext, type, StaticLiterals.Generate, defaultValue))
                {
                    var entityType = ItemProperties.GetModuleSubType(type);

                    result.Add($"{(first ? "else " : string.Empty)}if (typeof(E) == typeof({entityType}))");
                    result.Add("{");
                    result.Add($"dbSet = Db{type.Name}Set as DbSet<E>;");
                    result.Add("handled = true;");
                    result.Add("}");
                    first = true;
                }
            }
            result.Add("}");
            result.AddRange(CreateComment());
            result.Add($"partial void GetGeneratorEntitySet<E>(ref EntitySet<E>? entitySet, ref bool handled) where E : Entities.{StaticLiterals.EntityObjectName}, new()");
            result.Add("{");

            first = false;

            foreach (var type in entityProject.AllEntityTypes)
            {
                var defaultValue = (GenerateDbContext && GetGenerateDefault(type)).ToString();

                if (QuerySetting<bool>(Common.ItemType.DbContext, type, StaticLiterals.Generate, defaultValue))
                {
                    var entityType = ItemProperties.GetModuleSubType(type);
                    var subNamespace = ItemProperties.CreateSubNamespaceFromType(type).Replace(StaticLiterals.EntitiesFolder, StaticLiterals.DataContextFolder);
                    var entitySubType = $"{StaticLiterals.EntitiesFolder}.{ItemProperties.CreateSubTypeFromEntity(type)}";
                    var entitySetName = ItemProperties.CreateEntitySetName(type);
                    var entitySetType = $"{subNamespace}.{entitySetName}";
                    var dbSetName = $"Db{type.Name}Set";

                    result.Add($"{(first ? "else " : string.Empty)}if (typeof(E) == typeof({entityType}))");
                    result.Add("{");
                    result.Add($"entitySet = new {entitySetType}(this,{dbSetName}) as {StaticLiterals.EntitySetName}<E>;");
                    result.Add("handled = true;");
                    result.Add("}");
                    first = true;
                }
            }
            result.Add("}");

            result.AddRange(CreateComment());
            result.Add($"partial void GetGeneratorViewSet<E>(ref ViewSet<E>? viewSet, ref bool handled) where E : Entities.{StaticLiterals.ViewObjectName}, new()");
            result.Add("{");

            first = false;

            foreach (var type in entityProject.AllViewTypes)
            {
                var defaultValue = (GenerateDbContext && GetGenerateDefault(type)).ToString();

                if (QuerySetting<bool>(Common.ItemType.DbContext, type, StaticLiterals.Generate, defaultValue))
                {
                    var entityType = ItemProperties.GetModuleSubType(type);
                    var subNamespace = ItemProperties.CreateSubNamespaceFromType(type).Replace(StaticLiterals.EntitiesFolder, StaticLiterals.DataContextFolder);
                    var entitySubType = $"{StaticLiterals.EntitiesFolder}.{ItemProperties.CreateSubTypeFromEntity(type)}";
                    var viewSetName = ItemProperties.CreateViewSetName(type);
                    var viewSetType = $"{subNamespace}.{viewSetName}";
                    var dbSetName = $"Db{type.Name}Set";

                    result.Add($"{(first ? "else " : string.Empty)}if (typeof(E) == typeof({entityType}))");
                    result.Add("{");
                    result.Add($"viewSet = new {viewSetType}(this,{dbSetName}) as {StaticLiterals.ViewSetName}<E>;");
                    result.Add("handled = true;");
                    result.Add("}");
                    first = true;
                }
            }
            result.Add("}");

            result.AddRange(CreateComment());
            result.Add($"static partial void OnViewModelCreating(ModelBuilder modelBuilder)");
            result.Add("{");

            foreach (var type in entityProject.AllViewTypes)
            {
                var defaultValue = (GenerateDbContext && GetGenerateDefault(type)).ToString();

                if (QuerySetting<bool>(Common.ItemType.DbContext, type, StaticLiterals.Generate, defaultValue))
                {
                    var viewAttribute = type.GetCustomAttribute<CommonModules.Attributes.ViewAttribute>();

                    if (viewAttribute != null)
                    {
                        var viewName = viewAttribute.Name.HasContent() ? viewAttribute.Name : type.Name.CreatePluralWord();
                        var viewShema = viewAttribute.Schema;
                        var entitySubType = $"{StaticLiterals.EntitiesFolder}.{ItemProperties.CreateSubTypeFromEntity(type)}";

                        result.Add($"modelBuilder.Entity<{entitySubType}>()");
                        if (viewShema.HasContent())
                        {
                            result.Add($".ToView(\"{viewName}\", \"{viewShema}\")");
                        }
                        else
                        {
                            result.Add($".ToView(\"{viewName}\")");
                        }
                        result.Add($".HasNoKey();");
                    }
                }
            }

            result.Add("}");
            result.Add("#endregion partial methods");

            result.Add("}");
            result.EnvelopeWithANamespace(dataContextNamespace);
            result.FormatCSharpCode();
            return result;
        }
        /// <summary>
        /// Creates entity contracts.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        private List<GeneratedItem> CreateEntityContracts()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.AllEntityTypes.Where(t => EntityProject.IsSystemEntity(t)))
            {
                var itemtype = Common.ItemType.EntityContract;
                var defaultValue = (GenerateEntityContracts && GetGenerateDefault(type)).ToString();

                if (CanCreate(type) && QuerySetting<bool>(itemtype, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(CreateEntityContract(type, Common.UnitType.Logic, itemtype));
                }
            }
            return result;
        }

        /// <summary>
        /// Creates entity contracts.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        private List<GeneratedItem> ConnectEntitiesWithContract()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.AllEntityTypes)
            {
                var itemtype = Common.ItemType.EntityContract;
                var defaultValue = (GenerateEntityContracts && GetGenerateDefault(type)).ToString();

                if (CanCreate(type) && QuerySetting<bool>(itemtype, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(ConnectEntityWithContract(type, Common.UnitType.Logic, itemtype));
                }
            }
            return result;
        }
        /// <summary>
        /// Creates view contracts.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        private List<GeneratedItem> ConnectViewsWithContract()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.AllViewTypes)
            {
                var itemtype = Common.ItemType.ViewContract;
                var defaultValue = (GenerateEntityContracts && GetGenerateDefault(type)).ToString();

                if (CanCreate(type) && QuerySetting<bool>(itemtype, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(ConnectEntityWithContract(type, Common.UnitType.Logic, itemtype));
                }
            }
            return result;
        }

        /// <summary>
        /// Creates an entity contract for the specified type, unit type, and item type.
        /// </summary>
        /// <param name="type">The type of the entity contract.</param>
        /// <param name="unitType">The unit type of the entity contract.</param>
        /// <param name="itemType">The item type of the entity contract.</param>
        /// <returns>The generated entity contract item.</returns>
        private GeneratedItem ConnectEntityWithContract(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var subNamespace = ItemProperties.CreateSubNamespaceFromType(type);
            var itemName = ItemProperties.CreateEntityName(type);
            var subPath = ItemProperties.CreateSubPathFromType(type);
            var fileName = $"{itemName}{StaticLiterals.GenerationPostFix}{StaticLiterals.CSharpFileExtension}";
            var entityNamespace = $"{ItemProperties.ProjectNamespace}.{subNamespace}";
            var contractType = ItemProperties.CreateFullContractType(type);
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = $"{entityNamespace}.{itemName}",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = $"{subPath}{Path.DirectorySeparatorChar}{fileName}",
            };
            result.AddRange(CreateComment(type));
            result.Add($"partial class {itemName} : {contractType}");
            result.Add("{");
            result.Add("}");
            result.EnvelopeWithANamespace(entityNamespace);
            result.FormatCSharpCode();
            return result;
        }

        /// <summary>
        /// Creates entity sets.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        private List<GeneratedItem> CreateEntitySets()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.AllEntityTypes)
            {
                var itemType = Common.ItemType.EntitySet;
                var defaultValue = (GenerateEntitySets && GetGenerateDefault(type)).ToString();

                if (CanCreate(type) && QuerySetting<bool>(itemType, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(CreateEntitySet(type, Common.UnitType.Logic, itemType));
                }
            }
            return result;
        }

        /// <summary>
        /// Creates view sets.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        private List<GeneratedItem> CreateViewSets()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.AllViewTypes)
            {
                var itemType = Common.ItemType.ViewSet;
                var defaultValue = (GenerateEntitySets && GetGenerateDefault(type)).ToString();

                if (CanCreate(type) && QuerySetting<bool>(itemType, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(CreateViewSet(type, Common.UnitType.Logic, itemType));
                }
            }
            return result;
        }

        /// <summary>
        /// Creates an entity set for the specified type, unit type, and item type.
        /// </summary>
        /// <param name="type">The type of the entity set.</param>
        /// <param name="unitType">The unit type of the entity set.</param>
        /// <param name="itemType">The item type of the entity set.</param>
        /// <returns>The generated entity set item.</returns>
        private GeneratedItem CreateEntitySet(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var itemName = ItemProperties.CreateEntitySetName(type);
            var contractSetName = ItemProperties.CreateContractSetName(type);
            var contractSubNamespace = ItemProperties.CreateSubNamespaceFromEntity(type, StaticLiterals.ContractsFolder);
            var fileName = $"{itemName}{StaticLiterals.CSharpFileExtension}";
            var entitySetGenericType = QuerySetting<string>(itemType, StaticLiterals.AllItems, StaticLiterals.EntitySetGenericType, StaticLiterals.EntitySetName);
            var entityType = ItemProperties.GetModuleSubType(type);
            var contractType = ItemProperties.CreateFullContractType(type);
            var dataContextSubNamespace = ItemProperties.CreateSubNamespaceFromEntity(type, StaticLiterals.DataContextFolder);
            var dataContextNamespace = $"{ItemProperties.ProjectNamespace}.{dataContextSubNamespace}";
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = $"{dataContextNamespace}.{itemName}",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateSubFilePath(type, fileName, StaticLiterals.DataContextFolder),
            };

            entitySetGenericType = QuerySetting<string>(itemType, type, StaticLiterals.EntitySetGenericType, entitySetGenericType);
            result.Add($"using TEntity = {entityType};");
            result.Add($"using TContract = {contractType};");
            result.AddRange(CreateComment(type));
            result.Add($"internal sealed partial class {itemName} : {entitySetGenericType}<TEntity>, {contractSubNamespace}.{contractSetName}");
            result.Add("{");

            result.AddRange(CreatePartialStaticConstrutor(itemName));
            result.AddRange(CreatePartialConstrutor("public", itemName, $"ProjectDbContext context, DbSet<TEntity> dbSet", "base(context, dbSet)", null, true));

            result.AddRange(CreateCopyProperties("protected override", "TEntity", "TContract", "target", "source"));
            result.Add("}");
            result.EnvelopeWithANamespace(dataContextNamespace);
            result.FormatCSharpCode();
            return result;
        }

        /// <summary>
        /// Creates an entity set for the specified type, unit type, and item type.
        /// </summary>
        /// <param name="type">The type of the entity set.</param>
        /// <param name="unitType">The unit type of the entity set.</param>
        /// <param name="itemType">The item type of the entity set.</param>
        /// <returns>The generated entity set item.</returns>
        private GeneratedItem CreateViewSet(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var itemName = ItemProperties.CreateViewSetName(type);
            var contractSetName = ItemProperties.CreateContractSetName(type);
            var fileName = $"{itemName}{StaticLiterals.CSharpFileExtension}";
            var viewSetGenericType = QuerySetting<string>(itemType, StaticLiterals.AllItems, StaticLiterals.EntitySetGenericType, StaticLiterals.ViewSetName);
            var entityType = ItemProperties.GetModuleSubType(type);
            var contractType = ItemProperties.CreateFullContractType(type);
            var contractSubNamespace = ItemProperties.CreateSubNamespaceFromEntity(type, StaticLiterals.ContractsFolder);
            var dataContextSubNamespace = ItemProperties.CreateSubNamespaceFromEntity(type, StaticLiterals.DataContextFolder);
            var dataContextNamespace = $"{ItemProperties.ProjectNamespace}.{dataContextSubNamespace}";
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = $"{dataContextNamespace}.{itemName}",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateSubFilePath(type, fileName, StaticLiterals.DataContextFolder),
            };

            viewSetGenericType = QuerySetting<string>(itemType, type, StaticLiterals.EntitySetGenericType, viewSetGenericType);
            result.Add($"using TEntity = {entityType};");
            result.Add($"using TContract = {contractType};");
            result.AddRange(CreateComment(type));
            result.Add($"internal sealed partial class {itemName} : {viewSetGenericType}<TEntity>, {contractSubNamespace}.{contractSetName}");
            result.Add("{");

            result.AddRange(CreatePartialStaticConstrutor(itemName));
            result.AddRange(CreatePartialConstrutor("public", itemName, $"ProjectDbContext context, DbSet<TEntity> dbSet", "base(context, dbSet)", null, true));

            result.AddRange(CreateCopyProperties("protected override", "TEntity", "TContract", "target", "source"));
            result.Add("}");
            result.EnvelopeWithANamespace(dataContextNamespace);
            result.FormatCSharpCode();
            return result;
        }

        /// <summary>
        /// Creates view set contracts.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        private List<GeneratedItem> CreateViewSetContracts()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.AllViewTypes)
            {
                var itemType = Common.ItemType.ViewSetContract;
                var defaultValue = (GenerateEntitySetContracts && GetGenerateDefault(type)).ToString();

                if (CanCreate(type) && QuerySetting<bool>(itemType, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(CreateViewSetContract(type, Common.UnitType.Logic, itemType));
                }
            }
            return result;
        }
        private GeneratedItem CreateViewSetContract(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var itemName = ItemProperties.CreateContractSetName(type);
            var fileName = $"{itemName}{StaticLiterals.CSharpFileExtension}";
            var visibility = ItemProperties.GetDefaultVisibility(type);
            var entitySubType = $"{StaticLiterals.EntitiesFolder}.{ItemProperties.CreateSubTypeFromEntity(type)}";
            var subNamespace = ItemProperties.CreateSubNamespaceFromEntity(type, StaticLiterals.ContractsFolder);
            var contractNamespace = $"{ItemProperties.ProjectNamespace}.{subNamespace}";
            var contractSetGenericType = QuerySetting<string>(itemType, StaticLiterals.AllItems, StaticLiterals.ContractSetGenericType, StaticLiterals.ViewSetContractName);
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = $"{contractNamespace}.{itemName}",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateSubFilePath(type, fileName, StaticLiterals.ContractsFolder),
            };
            result.AddRange(CreateComment(type));
            result.Add($"{visibility} partial interface {itemName} : {contractSetGenericType}<{entitySubType}>");
            result.Add("{");
            result.Add("}");
            result.EnvelopeWithANamespace(contractNamespace);
            result.FormatCSharpCode();
            return result;
        }

        /// <summary>
        /// Creates entity set contracts.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        private List<GeneratedItem> CreateEntitySetContracts()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.AllEntityTypes)
            {
                var itemType = Common.ItemType.EntitySet;
                var defaultValue = (GenerateEntitySetContracts && GetGenerateDefault(type)).ToString();

                if (CanCreate(type) && QuerySetting<bool>(itemType, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(CreateEntitySetContract(type, Common.UnitType.Logic, itemType));
                }
            }
            return result;
        }
        private GeneratedItem CreateEntitySetContract(Type type, Common.UnitType unitType, Common.ItemType itemType)
        {
            var itemName = ItemProperties.CreateContractSetName(type);
            var fileName = $"{itemName}{StaticLiterals.CSharpFileExtension}";
            var visibility = ItemProperties.GetDefaultVisibility(type);
            var entitySubType = $"{StaticLiterals.EntitiesFolder}.{ItemProperties.CreateSubTypeFromEntity(type)}";
            var subNamespace = ItemProperties.CreateSubNamespaceFromEntity(type, StaticLiterals.ContractsFolder);
            var contractNamespace = $"{ItemProperties.ProjectNamespace}.{subNamespace}";
            var contractSetGenericType = QuerySetting<string>(itemType, StaticLiterals.AllItems, StaticLiterals.ContractSetGenericType, StaticLiterals.EntitySetContractName);
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = $"{contractNamespace}.{itemName}",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = ItemProperties.CreateSubFilePath(type, fileName, StaticLiterals.ContractsFolder),
            };
            result.AddRange(CreateComment(type));
            result.Add($"{visibility} partial interface {itemName} : {contractSetGenericType}<{entitySubType}>");
            result.Add("{");
            result.Add("}");
            result.EnvelopeWithANamespace(contractNamespace);
            result.FormatCSharpCode();
            return result;
        }

        /// <summary>
        /// Creates a context contract for the specified unit type and item type.
        /// </summary>
        /// <param name="unitType">The unit type of the context contract.</param>
        /// <param name="itemType">The item type of the context contract.</param>
        /// <returns>The generated context contract item.</returns>
        private GeneratedItem CreateDbContextContract(Common.UnitType unitType, Common.ItemType itemType)
        {
            var entityProject = EntityProject.Create(SolutionProperties);
            var itemName = StaticLiterals.ContextContractName;
            var contractNamespace = $"{ItemProperties.ProjectNamespace}.{StaticLiterals.ContractsFolder}";
            var subPath = $"{StaticLiterals.ContractsFolder}";
            var fileName = $"{itemName}{StaticLiterals.GenerationPostFix}{StaticLiterals.CSharpFileExtension}";
            var result = new GeneratedItem(unitType, itemType)
            {
                FullName = $"{contractNamespace}.{itemName}",
                FileExtension = StaticLiterals.CSharpFileExtension,
                SubFilePath = Path.Combine(subPath, fileName),
            };
            result.AddRange(CreateComment());
            result.Add($"partial interface {itemName}");
            result.Add("{");

            foreach (var type in entityProject.EntityTypes)
            {
                var defaultValue = (GenerateDbContext && GetGenerateDefault(type)).ToString();

                if (QuerySetting<bool>(Common.ItemType.DbContext, type, StaticLiterals.Generate, defaultValue))
                {
                    var entitySetName = ItemProperties.CreateEntitySetName(type);
                    var entitySubType = $"{StaticLiterals.EntitiesFolder}.{ItemProperties.CreateSubTypeFromEntity(type)}";
                    var contractSetName = ItemProperties.CreateContractSetName(type);
                    var contractSubNamespace = ItemProperties.CreateSubNamespaceFromEntity(type, StaticLiterals.ContractsFolder);
                    var contractSetSubType = $"{contractSubNamespace}.{contractSetName}";

                    result.AddRange(CreateComment(type));
                    result.Add($"{contractSetSubType} {entitySetName}" + "{ get; }");
                }
            }

            foreach (var type in entityProject.AllViewTypes)
            {
                var defaultValue = (GenerateDbContext && GetGenerateDefault(type)).ToString();

                if (QuerySetting<bool>(Common.ItemType.DbContext, type, StaticLiterals.Generate, defaultValue))
                {
                    var viewSubType = $"{StaticLiterals.EntitiesFolder}.{ItemProperties.CreateSubTypeFromEntity(type)}";
                    var viewSetName = ItemProperties.CreateViewSetName(type);
                    var contractSetName = ItemProperties.CreateContractSetName(type);
                    var contractSubNamespace = ItemProperties.CreateSubNamespaceFromEntity(type, StaticLiterals.ContractsFolder);
                    var contractSetSubType = $"{contractSubNamespace}.{contractSetName}";

                    result.AddRange(CreateComment(type));
                    result.Add($"{contractSetSubType} {viewSetName}" + "{ get; }");
                }
            }

            result.Add("}");
            result.EnvelopeWithANamespace(contractNamespace);
            result.FormatCSharpCode();
            return result;
        }
        #endregion generations

        #region query settings
        /// <summary>
        /// Queries a setting value and converts it to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to which the setting value will be converted.</typeparam>
        /// <param name="itemType">The common item type.</param>
        /// <param name="type">The type of the setting value.</param>
        /// <param name="valueName">The name of the setting value.</param>
        /// <param name="defaultValue">The default value to use if the setting value cannot be queried or converted.</param>
        /// <returns>
        /// The queried setting value converted to the specified type. If the setting value cannot be queried or converted,
        /// the default value will be returned.
        /// </returns>
        /// <remarks>
        /// If an exception occurs during the query or conversion process, the default value will be returned
        /// and the error message will be written to the debug output.
        /// </remarks>
        private T QuerySetting<T>(Common.ItemType itemType, Type type, string valueName, string defaultValue)
        {
            T result;

            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(Common.UnitType.Logic, itemType, ItemProperties.CreateSubTypeFromEntity(type), valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }
        /// <summary>
        /// Executes a query to retrieve a setting value and returns the result as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to which the setting value should be converted.</typeparam>
        /// <param name="itemType">The type of item to query for the setting value.</param>
        /// <param name="itemName">The name of the item to query for the setting value.</param>
        /// <param name="valueName">The name of the value to query for.</param>
        /// <param name="defaultValue">The default value to return if the query fails or the value cannot be converted.</param>
        /// <returns>The setting value as the specified type.</returns>
        private T QuerySetting<T>(Common.ItemType itemType, string itemName, string valueName, string defaultValue)
        {
            T result;

            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(Common.UnitType.Logic, itemType, itemName, valueName, defaultValue), typeof(T));
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

