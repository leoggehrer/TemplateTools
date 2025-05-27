//@BaseCode

namespace TemplateTools.Logic
{
    /// <summary>
    /// Static class containing various literals used throughout the application.
    /// </summary>
    public static partial class StaticLiterals
    {
        #region Code-Generation
        /// <summary>
        /// Gets the generation file postfix.
        /// </summary>
        public static string GenerationPostFix => "Generation";
        /// <summary>
        /// Gets the name of the generated code file.
        /// </summary>
        public static string GeneratedCodeFileName => "_GeneratedCode.cs";
        /// <summary>
        /// Gets the custom file extension.
        /// </summary>
        public static string CustomFileExtension => ".custom";
        /// <summary>
        /// Gets or sets the source file extensions.
        /// </summary>
        public static string SourceFileExtensions => CommonStaticLiterals.SourceFileExtensions;
        /// <summary>
        /// Gets the file extension for C# files.
        /// </summary>
        public static string CSharpFileExtension => CommonStaticLiterals.CSharpFileExtension;
        /// <summary>
        /// Gets the file extension for typescript files.
        /// </summary>
        public static string TSFileExtension => CommonStaticLiterals.TSFileExtension;
        /// <summary>
        /// Gets the file extension for razor page files.
        /// </summary>
        public static string RazorFileExtension => CommonStaticLiterals.RazorFileExtension;
        /// <summary>
        /// Gets the code file extension for razor page files.
        /// </summary>
        public static string RazorCodeFileExtension => CommonStaticLiterals.RazorCodeFileExtension;
        /// <summary>
        /// Gets the HTML file extension for C# files.
        /// </summary>
        public static string CSharpHtmlFileExtension => $"{CommonStaticLiterals.CSharpFileExtension}html";
        /// <summary>
        /// Gets the HTML file extension for C# files.
        /// </summary>
        public static string PlantUmlFileExtension => CommonStaticLiterals.PlantUmlFileExtension;
        /// <summary>
        /// Gets the label for generated code.
        /// </summary>
        public static string GeneratedCodeLabel => CommonStaticLiterals.GeneratedCodeLabel;
        /// <summary>
        /// Gets or sets the customized and generated code label.
        /// </summary>
        public static string CustomizedAndGeneratedCodeLabel => CommonStaticLiterals.CustomizedAndGeneratedCodeLabel;
        
        /// <summary>
        /// Gets or sets the beginning label for custom imports in Angular.
        /// </summary>
        public static string CustomImportBeginLabel => "//@CustomImportBegin";
        ///<summary>
        /// Gets or sets the end label for custom imports in Angular.
        ///</summary>
        public static string CustomImportEndLabel => "//@CustomImportEnd";
        /// <summary>
        /// Gets or sets the Angular custom code begin label.
        /// </summary>
        public static string CustomCodeBeginLabel => "//@CustomCodeBegin";
        /// <summary>
        /// Gets the end label of custom code for Angular.
        /// </summary>
        public static string CustomCodeEndLabel => "//@CustomCodeEnd";
        
        /// <summary>
        /// Gets or sets the alias for the entity.
        /// </summary>
        public static string EntityAlias => "TEntity";
        /// <summary>
        /// Gets or sets the alias for the model.
        /// </summary>
        public static string ModelAlias => "TModel";
        #endregion Code-Generation

        #region Project Extensions
        /// <summary>
        /// Gets or sets the common extension for the property.
        /// </summary>
        public static string CommonExtension => CommonStaticLiterals.CommonExtension;
        /// <summary>
        /// Gets or sets the logic extension for the property.
        /// </summary>
        public static string LogicExtension => CommonStaticLiterals.LogicExtension;
        /// <summary>
        /// Gets the WebApiExtension property.
        /// This property represents the Web API extension.
        /// </summary>
        public static string WebApiExtension => CommonStaticLiterals.WebApiExtension;
        /// <summary>
        /// Gets the MVVM extension.
        /// </summary>
        public static string MVVMAppExtension => CommonStaticLiterals.MVVMAppExtension;
        /// <summary>
        /// Gets the angular extension used by the application.
        /// </summary>
        public static string AngularExtension => CommonStaticLiterals.AngularExtension;
        #endregion Project Extensions

        #region Items
        public static readonly string DbObjectName = "DbObject";

        public static readonly string EntityObjectName = "EntityObject";
        public static readonly string VersionEntityObjectName = "VersionEntityObject";
        public static readonly string EntitySetName = "EntitySet";

        public static readonly string ViewObjectName = "ViewObject";
        public static readonly string ViewSetName = "ViewSet";

        public static readonly string ContextAccessor = "ContextAccessor";
        public static readonly string GenericItemViewModel = "GenericItemViewModel";
        public static readonly string GenericItemsViewModel = "GenericItemsViewModel";

        public static readonly string IdentifiableName = "IIdentifiable";
        public static readonly string ContextContractName = "IContext";
        public static readonly string EntitySetContractName = "IEntitySet";
        public static readonly string ViewSetContractName = "IViewSet";

        public static readonly string GlobalUsingIdentifiableName = "CommonContracts.IIdentifiable";
        public static readonly string GlobalUsingVersionableName = "CommonContracts.IVersionable";
        #endregion Items

        #region Entity properties
        public static readonly string IdentityProperty = "Id";
        public static readonly string ExternalIdentityProperty = "Guid";
        public static readonly string RowVersionProperty = "RowVersion";

        public static readonly string[] IdentityProperties = [IdentityProperty, ExternalIdentityProperty];
        public static readonly string[] VersionProperties = [IdentityProperty, ExternalIdentityProperty, RowVersionProperty];
        public static string[] NoGenerationProperties => [.. IdentityProperties.Union(VersionProperties)];
        #endregion Entity properties
        
        #region Model properties
        public static readonly string IdType = nameof(IdType);

        public static readonly string ModelObjectName = "ModelObject";
        public static readonly string VersionModelObjectName = "VersionModelObject";
        public static readonly string ViewModelObjectName = "ViewModelObject";

        public static readonly string AngularModelObjectName = "IKeyModel";
        public static readonly string AngularVersionModelObjectName = "IVersionModel";
        public static readonly string AngularViewModelObjectName = "IViewModel";
        #endregion Model properties

        public static readonly Dictionary<string, string> ModelBaseClassMapping = new()
        {
            { EntityObjectName, ModelObjectName },
            { VersionEntityObjectName, VersionModelObjectName },
            { ViewObjectName, ViewModelObjectName },
        };
        public static readonly Dictionary<string, string> AngularBaseClassMapping = new()
        {
            { EntityObjectName, AngularModelObjectName },
            { VersionEntityObjectName, AngularVersionModelObjectName },
            { ViewObjectName, AngularViewModelObjectName },
        };

        #region Folders and Files
        /// <summary>
        /// Gets the folder path where entities are stored.
        /// </summary>
        public static string EntitiesFolder => "Entities";
        /// <summary>
        /// Gets the name of the folder where the models are stored.
        /// </summary>
        public static string ModelsFolder => "Models";
        /// <summary>
        /// Gets the name of the folder where the view models are stored.
        /// </summary>
        public static string ViewModelsFolder => "ViewModels";
        /// <summary>
        /// Gets or sets an array of module folders.
        /// </summary>
        public static string[] ModuleFolders => [EntitiesFolder, ModelsFolder];
        
        /// <summary>
        /// Gets the folder name for the account.
        /// </summary>
        public static string AccountFolder => "Account";
        /// <summary>
        /// Gets or sets the folder where log files are stored.
        /// </summary>
        public static string LoggingFolder => "Logging";
        /// <summary>
        /// Gets the folder name for revisions.
        /// </summary>
        public static string RevisionFolder => "Revision";
        /// <summary>
        /// Gets the folder name for contracts.
        /// </summary>
        public static string ContractsFolder => "Contracts";
        /// <summary>
        /// Gets the path of the DataContext folder.
        /// </summary>
        public static string DataContextFolder => "DataContext";
        /// <summary>
        /// Gets the path to the Controllers folder.
        /// </summary>
        public static string ControllersFolder => "Controllers";
        /// <summary>
        /// Gets the folder name for the diagrams.
        /// </summary>
        /// <value>
        /// The folder name for the diagrams.
        /// </value>
        public static string DiagramsFolder => "Diagrams";
        #endregion Folders and Files
        
        #region Settings
        /// <summary>
        /// Gets the value "All" representing all items.
        /// </summary>
        public static string AllItems => "All";
        /// <summary>
        /// Represents the name of the TProperty property as a string.
        /// </summary>
        public static string TProperty => nameof(TProperty);
        /// <summary>
        /// Gets the name of the Copy property.
        /// </summary>
        public static string Copy => nameof(Copy);
        /// <summary>
        /// Gets the name of the property "Generate".
        /// </summary>
        public static string Generate => nameof(Generate);
        /// <summary>
        /// Gets the name of the Definition.
        /// </summary>
        public static string Definition => nameof(Definition);
        /// <summary>
        /// Represents the name of the HTML tag.
        /// </summary>
        public static string HTMLTag => nameof(HTMLTag);
        /// <summary>
        /// Gets the name of the PropertyNames property.
        /// </summary>
        public static string PropertyNames => nameof(PropertyNames);
        /// <summary>
        /// Gets the name of the get accessor.
        /// </summary>
        public static string GetAccessor => nameof(GetAccessor);
        /// <summary>
        /// Gets the string value of the set accessor.
        /// </summary>
        public static string SetAccessor => nameof(SetAccessor);
        /// <summary>
        /// Gets the name of the "Visibility" property.
        /// </summary>
        public static string Visibility => nameof(Visibility);
        /// <summary>
        /// Gets the name of the Attribute property.
        /// </summary>
        public static string Attribute => nameof(Attribute);
        /// <summary>
        /// Gets the name of the EntitySetGenericType property.
        /// </summary>
        public static string EntitySetGenericType => nameof(EntitySetGenericType);
        /// <summary>
        /// Gets the name of the EntitySetGenericType property.
        /// </summary>
        public static string ItemViewModelGenericType => nameof(ItemViewModelGenericType);
        /// <summary>
        /// Gets the name of the EntitySetGenericType property.
        /// </summary>
        public static string ItemsViewModelGenericType => nameof(ItemsViewModelGenericType);
        /// <summary>
        /// Gets the name of the ContractSetGenericType property.
        /// </summary>
        public static string ContractSetGenericType => nameof(ContractSetGenericType);
        /// <summary>
        /// Gets the name of the ControllerGenericType property.
        /// </summary>
        public static string ControllerGenericType => nameof(ControllerGenericType);
        /// <summary>
        /// Gets the name of the FacadeGenericType property.
        /// </summary>
        public static string FacadeGenericType => nameof(FacadeGenericType);
        /// <summary>
        /// Gets the name of the ServiceGenericType property.
        /// </summary>
        public static string ServiceGenericType => nameof(ServiceGenericType);
        
        /// <summary>
        /// Gets the name of the ViewTemplate property.
        /// </summary>
        public static string ViewTemplate => nameof(ViewTemplate);
        #endregion Settings
        
        #region Modules
        /// <summary>
        /// Gets the name of the Account property.
        /// </summary>
        /// <value>
        /// The name of the Account property.
        /// </value>
        public static string Account => nameof(Account);
        /// <summary>
        /// Gets the string representation of the 'Access' property name.
        /// </summary>
        public static string Access => nameof(Access);
        /// <summary>
        /// Gets the name of the Logging property.
        /// </summary>
        /// <value>
        /// The name of the Logging property.
        /// </value>
        public static string Logging => nameof(Logging);
        /// <summary>
        /// Gets the name of the Revision property.
        /// </summary>
        /// <value>The name of the Revision property.</value>
        public static string Revision => nameof(Revision);
        /// <summary>
        /// Gets the name of the secure identity.
        /// </summary>
        /// <value>
        /// The secure identity name.
        /// </value>
        public static string SecureIdentity => nameof(SecureIdentity);
        /// <summary>
        /// Gets the login session property name.
        /// </summary>
        /// <value>The login session property name.</value>
        public static string LoginSession => nameof(LoginSession);
        #endregion Modules
    }
}

