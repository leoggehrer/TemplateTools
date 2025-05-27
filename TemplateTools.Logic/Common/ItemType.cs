//@BaseCode

namespace TemplateTools.Logic.Common
{
    [Flags]
    public enum ItemType : ulong
    {
        #region contracts
        ContextContract,
        EntityContract,
        EntitySetContract,
        ViewContract,
        ViewSetContract,
        #endregion contracts

        #region entities
        EntitySet,
        ViewSet,
        #endregion entities

        #region models
        WebApiModel,
        WebApiEditModel,
        MVVMAppModel,
        MVVMAppEditModel,
        MVVVMAppItemViewModel,
        MVVVMAppItemsViewModel,
        #endregion models

        #region properties
        Property,
        ModelProperty,
        FilterProperty,
        InterfaceProperty,
        #endregion properties
        
        #region controllers
        EntityController,
        ViewController,
        ContextAccessor,
        #endregion controllers
        
        #region services
        DbContext,
        Service,
        #endregion services
        
        #region facades and factories
        Facade,
        
        Factory,
        FactoryControllerMethode,
        FactoryFacadeMethode,
        #endregion facades and factories
        
        #region angular
        TypeScriptEnum,
        TypeScriptModel,
        TypeScriptService,
        #endregion angular
        
        #region diagram
        EntityClassDiagram,
        #endregion diagram
        
        #region general
        Attribute,
        AllItems,
        Lambda,
        #endregion general
    }
}
