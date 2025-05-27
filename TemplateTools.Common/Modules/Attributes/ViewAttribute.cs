//@BaseCode
namespace TemplateTools.Common.Modules.Attributes
{
    /// <summary>
    /// An attribute used to mark a class as a view with a specified name and optional schema.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public partial class ViewAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAttribute"/> class with the specified view name.
        /// </summary>
        /// <param name="name">The name of the view.</param>
        public ViewAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the schema of the view.
        /// </summary>
        public string? Schema { get; set; }
    }
}
