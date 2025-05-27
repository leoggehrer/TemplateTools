//@BaseCode

namespace TemplateTools.Common
{
    /// <summary>
    /// Provides a collection of static literals.
    /// </summary>
    public static partial class StaticLiterals
    {
        /// Initializes the static literals.
        static StaticLiterals()
        {
            BeforeClassInitialize();
            TemplateProjects =
            [
            ];
            TemplateToolProjects =
            [
                "TemplateTools.Logic",
                "TemplateTools.ConApp",
            ];
            TemplateProjectExtensions =
            [
                CommonExtension,
                ConsoleExtension,
                CodeGenerationExtension,
                LogicExtension,
                LogicUnitTestExtension,
                WebApiExtension,
                MVVMAppExtension,
                AngularExtension,
            ];
            GenerationIgnoreFolders = IgnoreSubFolders;
            AfterClassInitialize();
        }
        /// <summary>
        /// This method is called before the class is initialized.
        /// </summary>
        static partial void BeforeClassInitialize();
        /// <summary>
        /// This method is called after the initialization of the class is completed.
        /// </summary>
        static partial void AfterClassInitialize();
        
        /// Gets the extension for solution files.
        /// @return The file extension for solution files as a string.
        public static string SolutionFileExtension => ".sln";
        /// <summary>
        /// Gets the file extension for project files.
        /// </summary>
        /// <value>
        /// The project file extension.
        /// </value>
        public static string ProjectFileExtension => ".csproj";

        #region Template project extensions
        /// <summary>
        /// Gets the extension for Common project..
        /// </summary>
        public static string CommonExtension => ".Common";
        /// <summary>
        /// Gets the extension for Console Application.
        /// </summary>
        public static string ConsoleExtension => ".ConApp";
        /// <summary>
        /// Gets the code generation extension.
        /// </summary>
        /// <value>The code generation extension.</value>
        public static string CodeGenerationExtension => ".CodeGenApp";
        /// <summary>
        /// Gets the logic extension.
        /// </summary>
        public static string LogicExtension => ".Logic";
        /// <summary>
        /// Gets the extension for Logic Unit Tests.
        /// </summary>
        public static string LogicUnitTestExtension => ".Logic.UnitTest";
        /// <summary>
        /// Gets the extension for WebApi files.
        /// </summary>
        public static string WebApiExtension => ".WebApi";
        /// <summary>
        /// Gets the MVVM extension for a WPF application.
        /// </summary>
        public static string MVVMAppExtension => ".MVVMApp";
        /// <summary>
        /// Gets the extension for Angular app files.
        /// </summary>
        /// <returns>The extension for Angular app files.</returns>
        public static string AngularExtension => ".AngularApp";
        #endregion Template project extensions
        
        /// <summary>
        /// Gets or sets the array of template projects.
        /// </summary>
        /// <value>
        /// An array of strings representing the template projects.
        /// </value>
        public static string[] TemplateProjects { get; private set; }
        /// Gets or sets the array of template tool projects.
        /// This property represents an array of strings that stores the names of template tool projects.
        /// The array is read-only and can only be set through the private setter.
        /// @returns An array of strings representing the names of template tool projects.
        public static string[] TemplateToolProjects { get; private set; }
        /// <summary>
        /// Gets the array of template project extensions.
        /// </summary>
        /// <value>
        /// The array of template project extensions.
        /// </value>
        public static string[] TemplateProjectExtensions { get; private set; }
        
        /// Gets or sets the array of folders that should be ignored during generation.
        /// This property is used to specify the folders that should not be included when generating something.
        /// @returns The array of folders to be ignored during generation.
        /// @since 1.0.0
        public static string[] GenerationIgnoreFolders { get; private set; }
        /// Gets the label for generated code.
        /// @returns The label for generated code.
        public static string GeneratedCodeLabel => "@" + "GeneratedCode";
        /// <summary>
        /// Gets the label for customized and generated code.
        /// </summary>
        /// <returns>The label for customized and generated code.</returns>
        public static string CustomizedAndGeneratedCodeLabel => "@" + "CustomAndGeneratedCode";
        /// <summary>
        /// Represents the label to ignore.
        /// </summary>
        public static string IgnoreLabel => "@" + "Ignore";
        /// <summary>
        /// Gets the label for the base code.
        /// </summary>
        /// <value>A string representing the label for base code.</value>
        public static string BaseCodeLabel => "@" + "BaseCode";
        /// <summary>
        /// Gets a label for code copy.
        /// </summary>
        /// <value>A string representing the label for code copy.</value>
        public static string CodeCopyLabel => "@" + "CodeCopy";
        /// <summary>
        /// Gets the file extension for C# files.
        /// </summary>
        /// <returns>The file extension for C# files, which is '.cs'.</returns>
        public static string CSharpFileExtension => ".cs";
        /// <summary>
        /// Gets the file extension for typescript files.
        /// </summary>
        /// <returns>The file extension for C# files, which is '.cs'.</returns>
        public static string TSFileExtension => ".ts";
        /// <summary>
        /// Gets the file extension for razor page files.
        /// </summary>
        /// <returns>The file extension for razor page files, which is '.razor'.</returns>
        public static string RazorFileExtension => ".razor";
        /// <summary>
        /// Gets the code file extension for razor page files.
        /// </summary>
        /// <returns>The code file extension for razor page files, which is '.razor.cs'.</returns>
        public static string RazorCodeFileExtension => ".razor.cs";
        /// <summary>
        /// Gets the plant uml file extension.
        /// </summary>
        /// <returns>The plant uml, which is '.puml'.</returns>
        public static string PlantUmlFileExtension => ".puml";
        /// <summary>
        /// Gets the extensions of source files.
        /// </summary>
        /// <value>
        /// The extensions of source files in the format "*.css|*.cs|*.ts|*.cshtml|*.razor|*.razor.cs|*.template".
        /// </value>
        public static string SourceFileExtensions => "*.cs|*.ts|*.css|*.cshtml|*.razor|*.razor.cs|*.template|*.puml|*.axaml";
        
        /// <summary>
        /// Gets the maximum page size.
        /// </summary>
        /// <returns>The maximum page size as an integer.</returns>
        public static int MaxPageSize => 500;
        
        #region Folders and Files
        /// <summary>
        /// Gets the list of folders to be ignored.
        /// </summary>
        /// <value>
        /// An array of strings representing the folders to be ignored.
        /// </value>
        public static string[] IgnoreFolders { get; } =
        [
            $"{Path.DirectorySeparatorChar}.angular",
            $"{Path.DirectorySeparatorChar}.angular{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}.vs",
            $"{Path.DirectorySeparatorChar}.vs{Path.DirectorySeparatorChar}",
            //$"{Path.DirectorySeparatorChar}.vscode",
            //$"{Path.DirectorySeparatorChar}.vscode{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}.git",
            $"{Path.DirectorySeparatorChar}.git{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}bin",
            $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}obj",
            $"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}node_modules",
            $"{Path.DirectorySeparatorChar}node_modules{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}Migrations",
            $"{Path.DirectorySeparatorChar}Migrations{Path.DirectorySeparatorChar}",
        ];
        /// <summary>
        /// Array of folder file paths to be ignored.
        /// </summary>
        public static string[] IgnoreFolderNames { get; } =
        [
            $".angular",
            $".vs",
            $".vscode",
            $".git",
            $".github",
            $"bin",
            $"obj",
            $"node_modules",
            $"Migrations",
        ];
        /// <summary>
        /// Array of folder file paths to be ignored.
        /// </summary>
        public static string[] IgnoreSubFolders { get; } =
        [
            $"{Path.DirectorySeparatorChar}.angular{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}.vs{Path.DirectorySeparatorChar}",
            //$"{Path.DirectorySeparatorChar}.vscode{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}.git{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}node_modules{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}Migrations{Path.DirectorySeparatorChar}",
        ];
        #endregion Folders and Files
    }
}

