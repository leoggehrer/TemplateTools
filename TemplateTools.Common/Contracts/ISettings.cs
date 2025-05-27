//@BaseCode
namespace TemplateTools.Common.Contracts
{
    public partial interface ISettings
    {
        string? this[string key] { get; }
    }
}
