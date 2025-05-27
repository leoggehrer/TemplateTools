//@BaseCode

namespace TemplateTools.Logic
{
    using System.Collections.Concurrent;
    using System.Text;
    using TemplateTools.Logic.Contracts;

    /// <summary>
    /// A static partial class that contains methods for generating code based on a solution.
    /// </summary>
    public static partial class Generator
    {
        #region properties
        /// <summary>
        /// Gets or sets the logging console.
        /// </summary>
        public static Action<string> LoggingConsole {  get; set; } = msg => System.Diagnostics.Debug.WriteLine(msg);
        #endregion  properties

        /// <summary>
        /// Generates a collection of <see cref="IGeneratedItem"/> objects based on the provided solution path.
        /// </summary>
        /// <param name="solutionPath">The file path of the solution.</param>
        /// <returns>A collection of <see cref="IGeneratedItem"/> objects.</returns>
        public static IEnumerable<IGeneratedItem> Generate(string solutionPath)
        {
            ISolutionProperties solutionProperties = SolutionProperties.Create(solutionPath);

            return Generate(solutionProperties);
        }
        /// <summary>
        /// Generates the necessary components based on the given solution properties.
        /// </summary>
        /// <param name="solutionProperties">The solution properties used for generating the components.</param>
        /// <returns>An IEnumerable of IGeneratedItem.</returns>
        public static IEnumerable<IGeneratedItem> Generate(ISolutionProperties solutionProperties)
        {
            var result = new ConcurrentBag<IGeneratedItem>();
            var configuration = new Configuration(solutionProperties);
            var tasks = new List<Task>();

            #region Common
            if (configuration.QuerySettingValue<bool>(Common.UnitType.Logic.ToString(), StaticLiterals.AllItems, StaticLiterals.AllItems, "Generate", "True"))
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var generator = new Generation.CommonGenerator(solutionProperties);
                    var generatedItems = new List<IGeneratedItem>();

                    WriteLogging("Create Common-Components...");
                    generatedItems.AddRange(generator.GenerateAll());
                    result.AddRangeSafe(generatedItems);
                }));
            }
            #endregion Common

            #region Logic
            if (configuration.QuerySettingValue<bool>(Common.UnitType.Logic.ToString(), StaticLiterals.AllItems, StaticLiterals.AllItems, "Generate", "True"))
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var generator = new Generation.LogicGenerator(solutionProperties);
                    var generatedItems = new List<IGeneratedItem>();

                    WriteLogging("Create Logic-Components...");
                    generatedItems.AddRange(generator.GenerateAll());
                    result.AddRangeSafe(generatedItems);
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var generator = new Generation.DiagramGenerator(solutionProperties);
                    var generatedItems = new List<IGeneratedItem>();

                    WriteLogging("Create Entity-Diagrams...");
                    generatedItems.AddRange(generator.GenerateEntityDiagrams());
                    result.AddRangeSafe(generatedItems);
                }));
            }
            #endregion Logic

            #region WebApiApp
            if (configuration.QuerySettingValue<bool>(Common.UnitType.WebApi.ToString(), StaticLiterals.AllItems, StaticLiterals.AllItems, "Generate", "True"))
            {
                var generator = new Generation.WebApiGenerator(solutionProperties);

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var generatedItems = new List<IGeneratedItem>();

                    WriteLogging("Create WebApi-Components...");
                    generatedItems.AddRange(generator.GenerateAll());
                    result.AddRangeSafe(generatedItems);
                }));
            }
            #endregion WebApiApp

            #region MVVMApp
            if (configuration.QuerySettingValue<bool>(Common.UnitType.MVVMApp.ToString(), StaticLiterals.AllItems, StaticLiterals.AllItems, "Generate", "True"))
            {
                var generator = new Generation.MVVMGenerator(solutionProperties);

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var generatedItems = new List<IGeneratedItem>();

                    WriteLogging("Create WebApi-Components...");
                    generatedItems.AddRange(generator.GenerateAll());
                    result.AddRangeSafe(generatedItems);
                }));
            }
            #endregion MVVMApp

            #region AngularApp
            if (configuration.QuerySettingValue<bool>(Common.UnitType.AngularApp.ToString(), StaticLiterals.AllItems, StaticLiterals.AllItems, "Generate", "True"))
            {
                var generator = new Generation.AngularGenerator(solutionProperties);

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var generatedItems = new List<IGeneratedItem>();

                    WriteLogging("Create Angular-Components...");
                    generatedItems.AddRange(generator.GenerateAll());
                    result.AddRangeSafe(generatedItems);
                }));
            }
            #endregion AngularApp

            Task.WaitAll([.. tasks]);
            return result;
        }

        /// <summary>
        /// Determines whether the specified type is an entity.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// <c>true</c> if the specified type is an entity; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEntity(Type type)
        {
            return type.IsClass
                   && type.IsNested == false
                   && type.Namespace != null
                   && type.Namespace!.Contains($".{StaticLiterals.EntitiesFolder}")
                   && type.GetBaseTypes().FirstOrDefault(t => t.Name.Equals(StaticLiterals.EntityObjectName)) != null;
        }

        /// <summary>
        /// Determines whether the specified type is a view.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// <c>true</c> if the specified type is a view; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsView(Type type)
        {
            return type.IsClass
                   && type.IsNested == false
                   && type.Namespace != null
                   && type.Namespace!.Contains($".{StaticLiterals.EntitiesFolder}")
                   && type.GetBaseTypes().FirstOrDefault(t => t.Name.Equals(StaticLiterals.ViewObjectName)) != null;
        }

        /// <summary>
        /// Deletes all generated files in the specified source path.
        /// </summary>
        /// <param name="sourcePath">The path to the source files.</param>
        public static void DeleteGeneratedFiles(string sourcePath)
        {
            var solutionProperties = SolutionProperties.Create(sourcePath);
            var configuration = new Configuration(solutionProperties);

            foreach (var searchPattern in StaticLiterals.SourceFileExtensions.Split("|"))
            {
                var deleteFiles = GetGeneratedFiles(sourcePath, searchPattern, [StaticLiterals.GeneratedCodeLabel]);

                foreach (var file in deleteFiles)
                {
                    var isTemplateProjectFile = solutionProperties.IsTemplateProjectFile(file);

                    if (isTemplateProjectFile)
                    {
                        var projectName = solutionProperties.GetProjectNameFromFile(file); ;
                        var defaultValue = configuration.QuerySettingValue(projectName, StaticLiterals.AllItems, StaticLiterals.AllItems, "Delete", true.ToString());
                        var canDelete = configuration.QuerySettingValue<bool>(projectName, "File", Path.GetFileName(file), "Delete", defaultValue);

                        if (canDelete)
                        {
                            if (Generation.FileHandler.IsCustomizeableFile(file))
                            {
                                Generation.FileHandler.SaveCustomParts(file);
                            }
                            File.Delete(file);
                        }
                    }
                    else if (Path.GetExtension(file).Equals(StaticLiterals.PlantUmlFileExtension, StringComparison.CurrentCultureIgnoreCase))
                    {
                        File.Delete(file);
                    }
                }
            }
            var defines = Preprocessor.ProjectFile.ReadDefinesInProjectFiles(sourcePath);

            Preprocessor.ProjectFile.SwitchDefine(defines, Preprocessor.ProjectFile.GeneratedCodePrefix, Preprocessor.ProjectFile.OffPostfix);
            Preprocessor.ProjectFile.WriteDefinesInProjectFiles(sourcePath, defines);
            Preprocessor.PreprocessorCommentHelper.SetPreprocessorDefineCommentsInFiles(sourcePath, defines);
        }
        /// <summary>
        /// Retrieves a collection of generated files located in the specified path that match the given search pattern, excluding files in ignored folders.
        /// </summary>
        /// <param name="path">The root directory path to search for generated files.</param>
        /// <param name="searchPattern">The search pattern to match against the file names.</param>
        /// <param name="labels">An array of labels used to filter the files based on their contents.</param>
        /// <returns>A IEnumerable of string representing the paths of the generated files.</returns>
        private static List<string> GetGeneratedFiles(string path, string searchPattern, string[] labels)
        {
            var result = new List<string>();

            foreach (var file in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                                          .Where(f => CommonStaticLiterals.GenerationIgnoreFolders.Any(e => f.Contains(e)) == false))
            {
                var lines = File.ReadAllLines(file, Encoding.Default);

                if (lines.Length > 0 && labels.Any(l => lines.First().Contains(l)))
                {
                    result.Add(file);
                }
            }
            return result;
        }
        /// <summary>
        /// Cleans directories within the specified path by removing the directories with the names "ViewTemplates" and "Diagrams".
        /// </summary>
        /// <param name="path">The path of the parent directory to clean.</param>
        /// <remarks>
        /// This method is a convenience wrapper around the overloaded CleanDirectories method
        /// which allows excluding additional directory names from being cleaned.
        /// </remarks>
        public static void CleanDirectories(string path)
        {
            CleanDirectories(path, ["ViewTemplates", "Diagrams"], []);
        }
        /// <summary>
        /// Cleans directories by removing specific files and empty directories.
        /// </summary>
        /// <param name="path">The root directory path.</param>
        /// <param name="excludeFolders">An array of folders to exclude from deletion.</param>
        /// <param name="dropFolders">An array of folders to force deletion regardless of their content.</param>
        public static void CleanDirectories(string path, string[] excludeFolders, string[] dropFolders)
        {
            static int CleanDirectories(DirectoryInfo dirInfo, string[] excludeFolders, params string[] dropFolders)
            {
                int result = 0;

                try
                {
                    result = dirInfo.GetFiles()
                                    .Count(f => f.Name.Equals(".DS_Store", StringComparison.CurrentCultureIgnoreCase) == false);

                    foreach (var item in dirInfo.GetDirectories())
                    {
                        int fileCount = CleanDirectories(item, dropFolders);

                        try
                        {
                            if (fileCount == 0 && excludeFolders.Any(ef => item.FullName.EndsWith(ef)) == false)
                            {
                                item.Delete(true);
                            }
                            else if ((dropFolders.FirstOrDefault(df => item.FullName.EndsWith(df))) != null)
                            {
                                fileCount = 0;
                                item.Delete(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error in {System.Reflection.MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
                        }
                        result += fileCount;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in {System.Reflection.MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
                }
                return result;
            }

            CleanDirectories(new DirectoryInfo(path), excludeFolders, dropFolders);
        }
        /// <summary>
        /// Writes the specified message to the console.
        /// </summary>
        /// <param name="message">The string that is output to the console.</param>
        public static void WriteLogging(string message)
        {
            LoggingConsole?.Invoke(message);
        }
    }
}

