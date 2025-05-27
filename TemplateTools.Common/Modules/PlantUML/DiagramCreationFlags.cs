//@BaseCode

namespace TemplateTools.Common.Modules.PlantUML
{
    public enum DiagramCreationFlags
    {
        Default = 0,
        EnumMembers = 1,
        ClassMembers = 2 * EnumMembers,
        InterfaceMembers = 2 * ClassMembers,
        
        TypeExtends = 2 * InterfaceMembers,
        InterfaceExtends = 2 * TypeExtends,
        
        ImplementedInterfaces = 2 * InterfaceExtends,
        
        ClassRelations = 2 * ImplementedInterfaces,
        InterfaceRelations = 2 * ClassRelations,
        
        All = EnumMembers | ClassMembers | InterfaceMembers | TypeExtends | InterfaceExtends | ImplementedInterfaces | ClassRelations | InterfaceRelations,
    }
}

