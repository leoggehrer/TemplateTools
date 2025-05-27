//@BaseCode

#if IDINT_ON
global using IdType = System.Int32;
#elif IDLONG_ON
    global using IdType = System.Int64;
#elif IDGUID_ON
    global using IdType = System.Guid;
#else
global using IdType = System.Int32;
#endif
global using Common = TemplateTools.Common;
global using CommonModules = TemplateTools.Common.Modules;
global using TemplateTools.Common.Extensions;
global using CommonStaticLiterals = TemplateTools.Common.StaticLiterals;
global using TemplatePath = TemplateTools.Common.Modules.Template.TemplatePath;
