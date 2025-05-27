//@BaseCode

namespace TemplateTools.Logic
{
    using System.Text;
    using TemplateTools.Logic.Common;
    using TemplateTools.Logic.Contracts;

    /// <summary>
    /// Represents the static writer class.
    /// </summary>
    public static partial class Writer
    {
        #region Class-Constructors
        /// <summary>
        /// Represents the static writer class.
        /// </summary>
        static Writer()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called before the class is constructed.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region fields
        private static readonly string[] InfoText =
        [
            "/*****************************************************************************************",
            "  Please note that this file is regenerated each time it is generated",
            "  and all your changes will be overwritten in this file.",
            "  If you still want to make changes, you can do this in 2 ways:",
            "  ",
            "  1. Use a 'partial class name' according to the following pattern:",
            "  ",
            "  #if GENERATEDCODE_ON",
            "  namespace_name {",
            "    partial class ClassName",
            "    {",
            "      partial void BeforeExecute(ref bool handled)",
            "      {",
            "        //... do something",
            "        handled = true;",
            "      }",
            "    }",
            "   }",
            "  #endif",
            "  ",
            "  2. Change the label //@GeneratedCode to //@CustomizedCode, for example.",
            "     Alternatively, you can also remove the label or give it a different name.",
            "*****************************************************************************************/",
        ];
        #endregion fields

        #region properties
        /// <summary>
        /// Gets or sets the logging console.
        /// </summary>
        public static Action<string> LoggingConsole { get; set; } = msg => System.Diagnostics.Debug.WriteLine(msg);
        /// <summary>
        /// Gets or sets a value indicating whether the data should be written to the group file.
        /// </summary>
        /// <value>
        /// True if the data should be written to the group file; otherwise, false.
        /// </value>
        public static bool WriteToGroupFile { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether the information header should be written.
        /// </summary>
        /// <value>
        /// True if the information header should be written; otherwise, false.
        /// </value>
        public static bool WriteInfoHeader { get; set; } = true;
        #endregion  properties

        /// <summary>
        /// Writes various components to the specified solution path based on the solution properties and generated items.
        /// </summary>
        /// <param name="solutionPath">The path of the solution.</param>
        /// <param name="solutionProperties">The solution properties.</param>
        /// <param name="generatedItems">The collection of generated items.</param>
        public static void WriteAll(string solutionPath, ISolutionProperties solutionProperties, IEnumerable<IGeneratedItem> generatedItems)
        {
            var tasks = new List<Task>();

            #region WriteCommonComponents
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.CommonProjectName);
                var projectName = solutionProperties.GetProjectNameFromPath(projectPath);

                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Common && e.ItemType == ItemType.EntityContract));

                    WriteLogging("Write Common-Contracts...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.CommonProjectName);
                var projectName = solutionProperties.GetProjectNameFromPath(projectPath);

                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Common && e.ItemType == ItemType.ViewContract));

                    WriteLogging("Write Common-Contracts...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            })));
            #endregion WriteCommonComponents

            #region WriteLogicComponents
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.EntityContract));

                    WriteLogging("Write Logic-Connect-Entity-Contracts...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.ViewContract));

                    WriteLogging("Write Logic-Connect-View-Contracts...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.EntitySetContract));

                    WriteLogging("Write Logic-Entity-Set-Contracts...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.EntitySet));

                    WriteLogging("Write Logic-Entity-Sets...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.ViewSetContract));

                    WriteLogging("Write Logic-View-Set-Contracts...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.ViewSet));

                    WriteLogging("Write Logic-View-Sets...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                var projectName = solutionProperties.GetProjectNameFromPath(projectPath);

                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.DbContext));

                    WriteLogging("Write Logic-DataContext...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            })));
            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.LogicProjectName);
                var projectName = solutionProperties.GetProjectNameFromPath(projectPath);

                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.ContextContract));

                    WriteLogging("Write Logic-Context-Contracts...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            })));

            tasks.Add(Task.Factory.StartNew((Action)(() =>
            {
                var projectPath = Path.Combine(solutionPath, StaticLiterals.DiagramsFolder);

                if (Directory.Exists(projectPath) == false)
                {
                    Directory.CreateDirectory(projectPath);
                }

                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where<IGeneratedItem>((Func<IGeneratedItem, bool>)(e => e.UnitType == UnitType.Logic && e.ItemType == ItemType.EntityClassDiagram));

                    WriteLogging("Write Logic-Entity-Diagrams...");
                    WriteItems(projectPath, writeItems, false);
                }
            })));
            #endregion WriteLogicComponents

            #region WriteWebApiComponents
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.WebApiProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where(e => e.UnitType == UnitType.WebApi && (e.ItemType == ItemType.WebApiModel || e.ItemType == ItemType.WebApiEditModel));

                    WriteLogging("Write WebApi-Models...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.WebApiProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where(e => e.UnitType == UnitType.WebApi && e.ItemType == ItemType.EntityController);

                    WriteLogging("Write WebApi-Entity-Controllers...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.WebApiProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where(e => e.UnitType == UnitType.WebApi && e.ItemType == ItemType.ViewController);

                    WriteLogging("Write WebApi-View-Controllers...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.WebApiProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where(e => e.UnitType == UnitType.WebApi && e.ItemType == ItemType.ContextAccessor);

                    WriteLogging("Write WebApi-ContextAccessor...");
                    WriteItems(projectPath, writeItems, WriteToGroupFile);
                }
            }));
            #endregion WriteWebApiComponents

            #region WriteMVVMComponents
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.MVVMAppProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where(e => e.UnitType == UnitType.MVVMApp && e.ItemType == ItemType.MVVMAppModel);

                    WriteLogging("Write MVVM-Models...");
                    WriteItems(projectPath, writeItems, false);
                }
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.MVVMAppProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where(e => e.UnitType == UnitType.MVVMApp && e.ItemType == ItemType.MVVVMAppItemViewModel);

                    WriteLogging("Write MVVM-Item-ViewModels...");
                    WriteItems(projectPath, writeItems, false);
                }
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.MVVMAppProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where(e => e.UnitType == UnitType.MVVMApp && e.ItemType == ItemType.MVVVMAppItemsViewModel);

                    WriteLogging("Write MVVM-Items-ViewModels...");
                    WriteItems(projectPath, writeItems, false);
                }
            }));
            #endregion WriteMVVMComponents

            #region WriteAngularComponents
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.AngularAppProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where(e => e.UnitType == UnitType.AngularApp && (e.ItemType == ItemType.TypeScriptEnum));

                    WriteLogging("Write Angular-Enums...");
                    WriteItems(projectPath, writeItems, false);
                }
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.AngularAppProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where(e => e.UnitType == UnitType.AngularApp && (e.ItemType == ItemType.TypeScriptModel));

                    WriteLogging("Write Angular-Models...");
                    WriteItems(projectPath, writeItems, false);
                }
            }));
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var projectPath = Path.Combine(solutionPath, solutionProperties.AngularAppProjectName);
                if (Directory.Exists(projectPath))
                {
                    var writeItems = generatedItems.Where(e => e.UnitType == UnitType.AngularApp && (e.ItemType == ItemType.TypeScriptService));

                    WriteLogging("Write Angular-Services...");
                    WriteItems(projectPath, writeItems, false);
                }
            }));
            #endregion WriteAngularComponents

            Task.WaitAll([.. tasks]);

            var defines = Preprocessor.ProjectFile.ReadDefinesInProjectFiles(solutionPath);

            Preprocessor.ProjectFile.SwitchDefine(defines, Preprocessor.ProjectFile.GeneratedCodePrefix, Preprocessor.ProjectFile.OnPostfix);
            Preprocessor.ProjectFile.WriteDefinesInProjectFiles(solutionPath, defines);
            Preprocessor.PreprocessorCommentHelper.SetPreprocessorDefineCommentsInFiles(solutionPath, defines);
        }

        #region write methods
        /// <summary>
        /// Writes the generated items to either a group file or individual code files.
        /// </summary>
        /// <param name="projectPath">The path of the project where the generated items will be written to.</param>
        /// <param name="generatedItems">The collection of generated items to be written.</param>
        /// <param name="writeToGroupFile">A boolean value indicating whether the generated items should be written to a group file or not.</param>
        internal static void WriteItems(string projectPath, IEnumerable<IGeneratedItem> generatedItems, bool writeToGroupFile)
        {
            if (writeToGroupFile)
            {
                WriteGeneratedCodeFile(projectPath, StaticLiterals.GeneratedCodeFileName, generatedItems);
            }
            else
            {
                WriteCodeFiles(projectPath, generatedItems);
            }
        }
        /// <summary>
        /// Writes the generated code file.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="generatedItems">The collection of generated items.</param>
        /// <remarks>
        /// This method checks if the generatedItems collection is not empty. It then determines the minimum common subpath from the generated items' subfile paths and combines it with the project path and given file name to create the full file path. Finally, it calls the overloaded WriteGeneratedCodeFile method with the full file path and the generated items.
        /// </remarks>
        internal static void WriteGeneratedCodeFile(string projectPath, string fileName, IEnumerable<IGeneratedItem> generatedItems)
        {
            if (generatedItems.Any())
            {
                var subPaths = generatedItems.Select(e => Path.GetDirectoryName(e.SubFilePath));
                var subPathItems = subPaths.Select(e => e!.Split(Path.DirectorySeparatorChar));
                var intersect = subPathItems.Any() ? subPathItems.ElementAt(0) : Array.Empty<string>().Select(e => e);

                for (int i = 1; i < subPathItems.Count(); i++)
                {
                    intersect = intersect.Intersect(subPathItems.ElementAt(i));
                }

                var minSubPath = string.Join(Path.DirectorySeparatorChar, intersect);
                var fullFilePath = Path.Combine(projectPath, minSubPath, fileName);

                WriteGeneratedCodeFile(fullFilePath, generatedItems);
            }
        }
        /// <summary>
        /// Writes generated code to a file at the specified file path.
        /// </summary>
        /// <param name="fullFilePath">The full file path where the generated code file should be written.</param>
        /// <param name="generatedItems">An IEnumerable of IGeneratedItem containing the generated items to be written.</param>
        /// <remarks>
        /// This method iterates through each item in the generatedItems collection, retrieves the source code of each item,
        /// and writes it to the file specified by the fullFilePath parameter. If the lines of source code exist, the directory
        /// of the fullFilePath is checked, and if it does not exist, it is created. The generated code is then written to the file.
        /// </remarks>
        internal static void WriteGeneratedCodeFile(string fullFilePath, IEnumerable<IGeneratedItem> generatedItems)
        {
            var lines = new List<string>();
            var directory = Path.GetDirectoryName(fullFilePath);

            foreach (var item in generatedItems)
            {
                lines.AddRange(item.SourceCode);
            }

            if (lines.Count != 0)
            {
                var sourceLines = new List<string>(lines);

                if (string.IsNullOrEmpty(directory) == false && Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }

                sourceLines.Insert(0, $"//{StaticLiterals.GeneratedCodeLabel}");
                File.WriteAllLines(fullFilePath, sourceLines);
            }
        }

        /// <summary>
        /// Writes code files to the specified project path.
        /// </summary>
        /// <param name="projectPath">The path of the project where the code files are to be written.</param>
        /// <param name="generatedItems">The collection of generated items representing the code files.</param>
        /// <returns>Returns nothing.</returns>
        internal static void WriteCodeFiles(string projectPath, IEnumerable<IGeneratedItem> generatedItems)
        {
            foreach (var item in generatedItems)
            {
                var sourceLines = new List<string>(item.SourceCode);
                var filePath = Path.Combine(projectPath, item.SubFilePath);

                if (item.FileExtension == StaticLiterals.CSharpHtmlFileExtension)
                {
                    sourceLines.Insert(0, $"@*{StaticLiterals.GeneratedCodeLabel}*@");
                }
                else if (item.FileExtension == StaticLiterals.RazorFileExtension)
                {
                    sourceLines.Insert(0, $"@*{StaticLiterals.GeneratedCodeLabel}*@");
                }
                else if (item.FileExtension == StaticLiterals.RazorCodeFileExtension)
                {
                    sourceLines.Insert(0, $"//{StaticLiterals.GeneratedCodeLabel}");
                }
                else if (item.FileExtension == StaticLiterals.PlantUmlFileExtension)
                {
                    sourceLines.Insert(0, $"@startuml {item.FullName}");
                    sourceLines.Insert(0, $"//{StaticLiterals.GeneratedCodeLabel}");
                    sourceLines.Add("@enduml");
                }
                else if (item.FileExtension == StaticLiterals.TSFileExtension)
                {
                    sourceLines.Insert(0, $"//{StaticLiterals.GeneratedCodeLabel}");
                }
                else
                {
                    if (WriteInfoHeader)
                    {
                        var index = 0;

                        foreach (var info in InfoText)
                        {
                            sourceLines.Insert(index++, info);
                        }
                    }
                    sourceLines.Insert(0, $"//{StaticLiterals.GeneratedCodeLabel}");
                }
                WriteCodeFile(filePath, sourceLines);
            }
        }
        /// <summary>
        /// Writes the specified source code to a file.
        /// </summary>
        /// <param name="sourceFilePath">The path of the file to be written.</param>
        /// <param name="source">The collection of strings representing the source code to be written.</param>
        internal static void WriteCodeFile(string sourceFilePath, IEnumerable<string> source)
        {
            var canCreate = true;
            var sourcePath = Path.GetDirectoryName(sourceFilePath);
            var customFilePath = Generation.FileHandler.CreateCustomFilePath(sourceFilePath);
            var generatedCode = StaticLiterals.GeneratedCodeLabel;

            if (File.Exists(sourceFilePath))
            {
                var lines = File.ReadAllLines(sourceFilePath);
                var header = lines.FirstOrDefault(l => l.Contains(StaticLiterals.GeneratedCodeLabel)
                                                    || l.Contains(StaticLiterals.CustomizedAndGeneratedCodeLabel));

                if (header != null)
                {
                    File.Delete(sourceFilePath);
                }
                else
                {
                    canCreate = false;
                }
            }
            else if (string.IsNullOrEmpty(sourcePath) == false
                     && Directory.Exists(sourcePath) == false)
            {
                Directory.CreateDirectory(sourcePath);
            }

            if (canCreate && source.Any())
            {
                File.WriteAllLines(sourceFilePath, source, Encoding.UTF8);
            }

            if (File.Exists(customFilePath))
            {
                File.Delete(customFilePath);
            }
        }
        /// <summary>
        /// Writes the specified message to the console.
        /// </summary>
        /// <param name="message">The string that is output to the console.</param>
        internal static void WriteLogging(string message)
        {
            LoggingConsole?.Invoke(message);
        }
        #endregion write methods
    }
}

