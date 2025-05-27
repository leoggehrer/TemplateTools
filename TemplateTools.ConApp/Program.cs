namespace TemplateTools.ConApp
{
    internal partial class Program
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// </summary>
        /// <remarks>
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static Program()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called during the construction of the class.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// Represents a method that is called when a class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        static void Main(string[] args)
        {
            var app = new Apps.ToolsApp();

            app.Run(args);
        }
    }
}
