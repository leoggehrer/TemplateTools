//@BaseCode

namespace TemplateTools.Logic.Common
{
    [Flags]
    public enum UnitType : long
    {
        All,
        General,
        
        Common,
        Logic,
        WebApi,
        
        AspMvc,
        AngularApp,
        MVVMApp,
        ClientBlazorApp,
        ConApp,
        
        TemplateCodeGenerator,
        TemplateTool,
    }
}
