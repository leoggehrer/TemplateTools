//@BaseCode
namespace TemplateTools.ConApp.Modules
{
    using TemplateTools.Common.Extensions;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using TemplateTools.Logic;

    /// <summary>
    /// Represents a partial internal class <see cref="Copier"/>.
    /// </summary>
    internal partial class Copier
    {
        #region Class-Constructors
        /// <summary>
        /// Represents the static constructor for the Copier class.
        /// </summary>
        static Copier()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// Represents a partial method that is called when a class is being constructed.
        /// </summary>
        /// <remarks>
        /// This method can be implemented by classes that are marked as partial and can be used to perform additional actions during the construction of the class.
        /// </remarks>
        /// <seealso cref="ClassConstructing()"/>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        /// <summary>
        /// Gets the separator character used in the application.
        /// </summary>
        /// <value>The separator character used in the application.</value>
        private static char Separator => ';';
        /// <summary>
        /// Gets or sets the logger action.
        /// </summary>
        /// <value>
        /// The logger action.
        /// </value>
        public static Action<string> Logger { get; set; } = s => System.Diagnostics.Debug.WriteLine(s);

        /// <summary>
        /// Gets the name of Dockerfile.
        /// </summary>
        /// <value>
        /// The name of Dockerfile.
        /// </value>
        private static string DockerfileName => "dockerfile";
        /// <summary>
        /// Gets the name of the Docker Compose file.
        /// </summary>
        /// <value>
        /// The name of the Docker Compose file as a string.
        /// </value>
        private static string DockerComposefileName => "docker-compose.yml";
        /// <summary>
        /// Gets or sets the array of file extensions used for replacement.
        /// </summary>
        /// <value>
        /// The array of file extensions.
        /// </value>
        private static string[] ReplaceExtensions { get; } =
        [
            ".asax",
            ".axaml",
            ".config",
            ".cs",
            ".cshtml",
            ".csproj",
            ".csv",
            ".css",
            ".esproj",
            ".html",
            ".js",
            ".less",
            ".manifest",
            ".md",
            ".razor",
            ".sln",
            ".template",
            ".tt",
            ".ts",
            ".txt",
            ".xaml",
            ".xml",
        ];
        /// <summary>
        /// Gets the array of files to be replaced.
        /// </summary>
        /// <value>
        /// An array of strings representing the names of the files to be replaced.
        /// </value>
        private static string[] ReplaceFiles { get; } =
        [
            "appsettings.json",
            "appsettings.Development.json",
            "launchSettings.json",
        ];
        /// <summary>
        /// Array of file extensions for project files.
        /// </summary>
        private static string[] ProjectExtensions { get; } =
        [
            ".asax",
            ".axaml",
            ".config",
            ".cs",
            ".cshtml",
            ".csproj",
            ".csv",
            ".css",
            ".esproj",
            ".html",
            ".ico",
            ".jpg",
            ".js",
            ".json",
            ".less",
            ".manifest",
            ".md",
            ".png",
            ".razor",
            ".scss",
            ".template",
            ".tt",
            ".ts",
            ".txt",
            ".xaml",
            ".xml",
        ];
        /// <summary>
        /// Gets the array of supported file extensions for the solution.
        /// </summary>
        /// <value>
        /// The supported file extensions.
        /// </value>
        private static string[] SolutionExtenions { get; } =
        [
            ".cd",
            ".cmd",
            ".csv",
            ".docx",
            ".drawio",
            ".html",
            ".jpg",
            ".json",
            ".md",
            ".pdf",
            ".png",
            ".razor",
            ".razor.cs",
            ".ts",
            ".txt",
            ".xlsx",
            ".yml",
        ];
        /// <summary>
        /// Gets or sets the list of extensions.
        /// </summary>
        /// <value>
        /// The list of extensions.
        /// </value>
        private List<string> Extensions { get; } = [];
        /// <summary>
        /// Gets the project GUIDs associated with the project.
        /// </summary>
        /// <remarks>
        /// This property represents a collection of unique identifiers that identify the project.
        /// </remarks>
        /// <value>
        /// A list of project GUIDs.
        /// </value>
        private List<string> ProjectGuids { get; } = [];

        internal static readonly string[] separator = ["=", ","];

        /// <summary>
        /// Copies projects from a source directory to a target directory and generates a template code based on the projects.
        /// </summary>
        /// <param name="sourceDirectory">The directory where the source projects are located.</param>
        /// <param name="targetDirectory">The directory where the copied projects and template code will be placed.</param>
        /// <param name="sourceProjects">The collection of project names to be copied and included in the template code.</param>
        public void Copy(string sourceDirectory, string targetDirectory, IEnumerable<string> sourceProjets)
        {
            if (string.IsNullOrWhiteSpace(sourceDirectory) == true)
                throw new ArgumentException(null, nameof(sourceDirectory));

            if (string.IsNullOrWhiteSpace(targetDirectory) == true)
                throw new ArgumentException(null, nameof(targetDirectory));

            Logger($"Source-Project: {sourceDirectory}");
            Logger($"Target-Directory: {targetDirectory}");

            if (sourceDirectory.Equals(targetDirectory) == false)
            {
                Logger("Running");
                var result = CreateTemplate(sourceDirectory, targetDirectory, sourceProjets);

                foreach (var ext in Extensions.OrderBy(i => i))
                {
                    System.Diagnostics.Debug.WriteLine($",\"{ext}\"");
                }

                Generator.DeleteGeneratedFiles(targetDirectory);

                if (result)
                {
                    Logger("Finished!");
                }
                else
                {
                    Logger("Not finished! There are some errors!");
                }
            }
        }

        /// <summary>
        /// Creates a template in the target directory based on the source solution directory and a list of source projects.
        /// </summary>
        /// <param name="sourceSolutionPath">The directory of the source solution.</param>
        /// <param name="targetSolutionPath">The directory where the template will be created.</param>
        /// <param name="sourceProjets">The list of source projects.</param>
        /// <returns>True if the template creation is successful, otherwise false.</returns>
        private bool CreateTemplate(string sourceSolutionPath, string targetSolutionPath, IEnumerable<string> sourceProjets)
        {
            if (Directory.Exists(targetSolutionPath) == false)
            {
                Directory.CreateDirectory(targetSolutionPath);
            }

            var sourceFolderName = new DirectoryInfo(sourceSolutionPath).Name;
            var targetFolderName = new DirectoryInfo(targetSolutionPath).Name;

            CopySolutionStructure(sourceSolutionPath, targetSolutionPath, sourceProjets);

            foreach (var directory in Directory.GetDirectories(sourceSolutionPath, "*", SearchOption.AllDirectories))
            {
                var subFolder = directory.Replace(sourceSolutionPath, string.Empty);

                if (CommonStaticLiterals.IgnoreFolders.Any(i => subFolder.EndsWith(i) || subFolder.Contains(i)) == false
                                                             && sourceProjets.Any(i => subFolder.EndsWith(i)))
                {
                    subFolder = subFolder.Replace(sourceFolderName, targetFolderName);

                    CopyProjectDirectoryWorkFiles(directory, sourceSolutionPath, targetSolutionPath);
                }
            }
            return true;
        }
        /// <summary>
        /// Copies the structure of a solution from the source solution directory to the target solution directory,
        /// including the solution file, solution files, and project files.
        /// </summary>
        /// <param name="sourceSolutionPath">The directory of the source solution</param>
        /// <param name="targetSolutionPath">The directory of the target solution</param>
        /// <param name="sourceProjects">The collection of source project files</param>
        /// <remarks>
        /// This method copies the solution file from the source solution directory to the target solution directory
        /// and adjusts the necessary paths and filenames. It also copies all solution files and project files
        /// from the source solution directory to the target solution directory.
        /// </remarks>
        private void CopySolutionStructure(string sourceSolutionPath, string targetSolutionPath, IEnumerable<string> sourceProjects)
        {
            var sourceSolutionFilePath = TemplatePath.GetSolutionFilePath(sourceSolutionPath);

            if (string.IsNullOrEmpty(sourceSolutionFilePath) == false)
            {
                var sourceSolutionName = Path.GetFileNameWithoutExtension(sourceSolutionFilePath);
                var targetSolutionFolder = new DirectoryInfo(targetSolutionPath).Name;
                var targetSolutionFilePath = Path.Combine((string)targetSolutionPath, $"{targetSolutionFolder}{CommonStaticLiterals.SolutionFileExtension}");

                CopySolutionFile(sourceSolutionFilePath, targetSolutionFilePath, sourceSolutionName, targetSolutionFolder, sourceProjects);
                CopySolutionFiles(sourceSolutionPath, targetSolutionPath);
                CopySolutionProjectFiles(sourceSolutionPath, targetSolutionPath, sourceProjects);
            }
        }
        /// <summary>
        /// Splits the full text of a TagInfo object into an array of strings.
        /// </summary>
        /// <param name="tag">The TagInfo object containing the full text to be split.</param>
        /// <returns>An array of strings containing the split sections of the full text.</returns>
        private static string[] SplitProjectEntry(TagInfo tag)
        {
            var result = new List<string>();
            var removeItems = new[] { " ", "\t" };
            var data = tag.FullText.RemoveAll(removeItems)
                          .Split($"{Environment.NewLine}")
                          .Where(e => e.HasContent());

            result.AddRange(data);
            return [.. result];
        }
        /// <summary>
        /// Checks if the given entry items represent a solution entry.
        /// </summary>
        /// <param name="entryItems">The collection of entry items to check.</param>
        /// <returns>True if the entry items represent a solution entry; otherwise, false.</returns>
        private static bool IsSolutionEntry(IEnumerable<string> entryItems)
        {
            return entryItems.Count() > 1 && entryItems.ElementAt(1).StartsWith("ProjectSection(SolutionItems)");
        }
        /// <summary>
        /// Converts the solution entry items to a formatted IEnumerable of strings.
        /// </summary>
        /// <param name="entryItems">The IEnumerable of string entry items to convert.</param>
        /// <returns>The formatted IEnumerable of strings.</returns>
        private static List<string> ConvertSolutionEntry(IEnumerable<string> entryItems)
        {
            var result = new List<string>();
            var items = entryItems.ToArray();

            result.Add(items[0]);
            result.Add($"\t{items[1]}");

            for (int i = 2; i < items.Length - 1; i++)
            {
                var item = items[i];

                if (item.Contains('='))
                {
                    var data = item.Split("=");

                    if (SolutionExtenions.Any(e => e.Equals(Path.GetExtension(data[0]))))
                    {
                        result.Add($"\t\t{item}");
                    }
                    else
                    {
                        result.Add(item);
                    }
                }
                else
                {
                    result.Add("\t" + item);
                }
            }
            result.Add(items[^1]);
            return result;
        }
        /// <summary>
        /// Converts project entry items based on specified parameters.
        /// </summary>
        /// <param name="entryItems">The collection of entry items to convert.</param>
        /// <param name="sourceSolutionName">The name of the source solution.</param>
        /// <param name="targetSolutionName">The name of the target solution.</param>
        /// <param name="sourceProjects">The collection of source projects.</param>
        /// <returns>The converted project entry items.</returns>
        private static List<string> ConvertProjectEntry(IEnumerable<string> entryItems, string sourceSolutionName, string targetSolutionName, IEnumerable<string> sourceProjects)
        {
            var result = new List<string>();
            var items = entryItems.ToArray();
            var regex = new Regex(sourceSolutionName, RegexOptions.IgnoreCase);

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];

                if (item.StartsWith("Project("))
                {
                    var data = item.Split(separator, StringSplitOptions.None);

                    if (data.Length > 1 && sourceProjects.Any(e => e.Equals(data[1].RemoveAll("\""))))
                    {
                        result.Add("Project(\"{" + Guid.NewGuid().ToString().ToUpper() + "}\") = ");
                        for (int j = 1; j < data.Length; j++)
                        {
                            result[^1] = $"{result[^1]}{(j > 1 ? ", " : string.Empty)}" + $"{regex.Replace(data[j], targetSolutionName)}";
                        }
                        result.Add("EndProject");
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Copies the solution file from the source path to the target path,
        /// while updating the project entries and global sections.
        /// </summary>
        /// <param name="solutionSourceFilePath">The path of the source solution file.</param>
        /// <param name="targetSolutionFilePath">The path of the target solution file.</param>
        /// <param name="sourceSolutionName">The name of the source solution.</param>
        /// <param name="targetSolutionName">The name of the target solution.</param>
        /// <param name="sourceProjects">The collection of source project names.</param>
        /// <remarks>
        /// This method reads the contents of the source solution file and performs the following steps:
        /// 1. Extracts the project tags from the source text.
        /// 2. Appends the text before the first project tag to the target text.
        /// 3. Iterates over each project tag and converts the solution entries and project entries.
        /// 4. Appends the converted target lines to the target text.
        /// 5. Extracts the global tags from the remaining source text after the last project tag.
        /// 6. Appends the global tags to the target text.
        /// 7. Writes the target text to the target solution file.
        /// </remarks>
        /// <exception cref="FileNotFoundException">Thrown when the source solution file is not found.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the target directory is not found.</exception>
        /// <exception cref="IOException">Thrown when there is an error reading from or writing to the files.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the access to the files is denied.</exception>
        /// <exception cref="ArgumentNullException">Thrown when one or more parameters are null.</exception>
        /// <exception cref="ArgumentException">Thrown when one or more parameters are empty or invalid.</exception>
        private static void CopySolutionFile(string solutionSourceFilePath, string targetSolutionFilePath, string sourceSolutionName, string targetSolutionName, IEnumerable<string> sourceProjects)
        {
            var targetText = new StringBuilder();
            var targetLines = new List<string>();
            var sourceText = File.ReadAllText(solutionSourceFilePath, Encoding.Default);
            var projectTags = sourceText.GetAllTags(["Project(", $"EndProject{Environment.NewLine}"]);

            if (projectTags.Any())
            {
                targetText.Append(sourceText[..projectTags.First().StartTagIndex]);
                foreach (var tag in projectTags)
                {
                    var entryItems = SplitProjectEntry(tag);

                    if (IsSolutionEntry(entryItems))
                    {
                        targetLines.AddRange(ConvertSolutionEntry(entryItems));
                    }
                    else // it is a project entry
                    {
                        targetLines.AddRange(ConvertProjectEntry(entryItems, sourceSolutionName, targetSolutionName, sourceProjects));
                    }
                }
                targetText.AppendLine(targetLines.ToText());

                var globalTags = sourceText[projectTags.Last().EndIndex..]
                                                       .GetAllTags("GlobalSection(", "EndGlobalSection");

                targetText.AppendLine("Global");
                foreach (var tag in globalTags)
                {
                    if (tag.FullText.Contains("GlobalSection(ProjectConfigurationPlatforms) = postSolution"))
                    {
                        var data = tag.FullText.Split(Environment.NewLine);

                        if (data.Length != 0)
                            targetText.AppendLine($"\t{data[0]}");

                        for (int i = 1; i < data.Length - 1; i++)
                        {
                            var guid = data[i].Partialstring("{", "}");

                            if (targetLines.Any(e => e.Contains(guid)))
                            {
                                targetText.AppendLine($"{data[i]}");
                            }
                        }

                        if (data.Length != 0)
                            targetText.AppendLine($"{data[^1]}");
                    }
                    else
                    {
                        targetText.Append('\t');
                        targetText.AppendLine(tag.FullText);
                    }
                }
                targetText.AppendLine("EndGlobal");
            }
            File.WriteAllText(targetSolutionFilePath, targetText.ToString(), Encoding.Default);
        }
        /// <summary>
        /// Copies solution files from source solution directory to target solution directory.
        /// </summary>
        /// <param name="sourceSolutionPath">The directory of the source solution.</param>
        /// <param name="targetSolutionPath">The directory of the target solution.</param>
        private void CopySolutionFiles(string sourceSolutionPath, string targetSolutionPath)
        {
            var sourceSolutionFilePath = TemplatePath.GetSolutionFilePath(sourceSolutionPath);
            var targetSolutionFolder = new DirectoryInfo(targetSolutionPath).Name;
            var sourceSolutionName = Path.GetFileNameWithoutExtension(sourceSolutionFilePath);

            foreach (var sourceFile in new DirectoryInfo(sourceSolutionPath).GetFiles("*", SearchOption.TopDirectoryOnly)
                                                                            .Where(f => SolutionExtenions.Any(e => e.Equals(f.Extension, StringComparison.CurrentCultureIgnoreCase))))
            {
                var targetFilePath = CreateTargetFilePath(sourceFile.FullName, sourceSolutionPath, targetSolutionPath);

                CopyFile(sourceFile.FullName, targetFilePath, sourceSolutionName, targetSolutionFolder);
            }
        }

        /// <summary>
        /// Copies the project files from the source solution directory to the target solution directory.
        /// </summary>
        /// <param name="sourceSolutionPath">The source solution directory path.</param>
        /// <param name="targetSolutionPath">The target solution directory path.</param>
        /// <param name="sourceProjects">An enumerable collection of source project names.</param>
        /// <remarks>
        /// This method iterates through the source solution directory and its subdirectories to find project files.
        /// It then checks if the directory name of a project file ends with any of the provided source project names.
        /// If it does, the method creates a target file path using the original source file path and the target solution directory.
        /// The file is then copied from the source file path to the target file path, and the project GUIDs are replaced.
        /// </remarks>
        private void CopySolutionProjectFiles(string sourceSolutionPath, string targetSolutionPath, IEnumerable<string> sourceProjects)
        {
            var projectFilePath = string.Empty;
            var sourceSolutionName = TemplatePath.GetSolutionName(sourceSolutionPath);
            var sourceSolutionFilePath = TemplatePath.GetSolutionFilePath(sourceSolutionPath);
            var targetSolutionFolder = new DirectoryInfo(targetSolutionPath).Name;

            foreach (var sourceFile in new DirectoryInfo(sourceSolutionPath).GetFiles($"*{CommonStaticLiterals.ProjectFileExtension}", SearchOption.AllDirectories))
            {
                var directoryName = sourceFile.DirectoryName?.ToLower();

                if (sourceProjects.Any(e => directoryName != null && directoryName.EndsWith(e.ToLower())))
                {
                    var targetFilePath = CreateTargetFilePath(sourceFile.FullName, sourceSolutionPath, targetSolutionPath);

                    CopyFile(sourceFile.FullName, targetFilePath, sourceSolutionName, targetSolutionFolder);
                }
            }
            if (string.IsNullOrEmpty(projectFilePath) == false)
            {
                ProjectGuids.AddRange(ReplaceProjectGuids(projectFilePath));
            }
        }
        /// <summary>
        /// Copies the work files from a source directory to a target directory within the specified solution directories.
        /// </summary>
        /// <param name="sourceDirectory">The source directory from which to copy the work files.</param>
        /// <param name="sourceSolutionDirectory">The source solution directory.</param>
        /// <param name="targetSolutionDirectory">The target solution directory.</param>
        private void CopyProjectDirectoryWorkFiles(string sourceDirectory, string sourceSolutionDirectory, string targetSolutionDirectory)
        {
            var projectFilePath = string.Empty;
            var sourceSolutionFilePath = TemplatePath.GetSolutionFilePath(sourceSolutionDirectory);
            var sourceSolutionName = TemplatePath.GetSolutionName(sourceSolutionDirectory);
            var targetSolutionFolder = new DirectoryInfo(targetSolutionDirectory).Name;
            var sourceFiles = new DirectoryInfo(sourceDirectory).GetFiles("*", SearchOption.AllDirectories)
                                                                .Where(f => CommonStaticLiterals.IgnoreSubFolders.Any(i => f.FullName.Contains(i, StringComparison.CurrentCultureIgnoreCase)) == false
                                                                         && (f.Name.Equals("dockerfile", StringComparison.CurrentCultureIgnoreCase) || ProjectExtensions.Any(i => i.Equals(Path.GetExtension(f.Name)))));

            foreach (var sourceFile in sourceFiles)
            {
                var targetFilePath = CreateTargetFilePath(sourceFile.FullName, sourceSolutionDirectory, targetSolutionDirectory);

                CopyFile(sourceFile.FullName, targetFilePath, sourceSolutionName, targetSolutionFolder);
            }
        }
        /// <summary>
        /// Copies a file from a source file path to a target file path.
        /// The method also performs various operations based on the file type and filenames.
        /// </summary>
        /// <param name="sourceFilePath">The full path of the source file to be copied.</param>
        /// <param name="targetFilePath">The full path where the copied file will be saved.</param>
        /// <param name="sourceSolutionName">The name of the source solution.</param>
        /// <param name="targetSolutionName">The name of the target solution.</param>
        /// <returns>void</returns>
        private void CopyFile(string sourceFilePath, string targetFilePath, string sourceSolutionName, string targetSolutionName)
        {
            var fileName = Path.GetFileName(sourceFilePath);
            var extension = Path.GetExtension(sourceFilePath);

            if (Extensions.SingleOrDefault(i => i.Equals(extension, StringComparison.CurrentCultureIgnoreCase)) == null)
            {
                Extensions.Add(extension);
            }

            if (sourceFilePath.EndsWith(DockerfileName, StringComparison.CurrentCultureIgnoreCase))
            {
                var sourceLines = File.ReadAllLines(sourceFilePath, Encoding.Default);
                var targetLines = sourceLines.Select(l => l.Replace(sourceSolutionName, targetSolutionName));

                WriteAllLines(targetFilePath, [.. targetLines], Encoding.Default);
            }
            else if (sourceFilePath.EndsWith(DockerComposefileName, StringComparison.CurrentCultureIgnoreCase))
            {
                var sourceLines = File.ReadAllLines(sourceFilePath, Encoding.Default);
                var targetLines = sourceLines.Select(l => l.Replace(sourceSolutionName, targetSolutionName))
                                             .Select(l => l.Replace(sourceSolutionName.ToLower(), targetSolutionName.ToLower()));

                WriteAllLines(targetFilePath, [.. targetLines], Encoding.Default);
            }
            else if (ReplaceFiles.Any(f => f.Equals(fileName, StringComparison.CurrentCultureIgnoreCase))
            || ReplaceExtensions.Any(i => i.Equals(extension, StringComparison.CurrentCultureIgnoreCase)))
            {
                var targetLines = new List<string>();
                var sourceLines = File.ReadAllLines(sourceFilePath, Encoding.Default);
                var regex = new Regex(sourceSolutionName, RegexOptions.IgnoreCase);

                if (sourceFilePath.EndsWith("BlazorApp.csproj"))
                {
                    for (int i = 0; i < sourceLines.Length; i++)
                    {
                        var sourceLine = sourceLines[i];

                        if (sourceLine.TrimStart().StartsWith("<UserSecretsId>"))
                        {
                            sourceLine = $"    <UserSecretsId>{Guid.NewGuid()}</UserSecretsId>";
                            sourceLines[i] = sourceLine;
                        }
                    }
                }

                if (sourceLines.Length == 0
                    || (sourceLines.Length != 0
                        && sourceLines.First().Contains(CommonStaticLiterals.IgnoreLabel) == false
                        && sourceLines.First().Contains(CommonStaticLiterals.GeneratedCodeLabel) == false))
                {
                    foreach (var sourceLine in sourceLines)
                    {
                        var targetLine = regex.Replace(sourceLine, targetSolutionName);

                        targetLine = targetLine.Replace(CommonStaticLiterals.BaseCodeLabel, CommonStaticLiterals.CodeCopyLabel);
                        targetLines.Add(targetLine);
                    }
                    WriteAllLines(targetFilePath, [.. targetLines], Encoding.UTF8);
                }
            }
            else if (File.Exists(targetFilePath) == false)
            {
                CopyFile(sourceFilePath, targetFilePath);
            }
        }

        /// <summary>
        /// Replaces the project GUIDs in the XML file specified by the file path.
        /// </summary>
        /// <param name="filePath">The path of the XML file to modify.</param>
        /// <returns>An array of strings containing the original project GUIDs and their corresponding new GUIDs.</returns>
        private static string[] ReplaceProjectGuids(string filePath)
        {
            var result = new List<string>();
            var xml = new XmlDocument();

            xml.Load(filePath);

            if (xml.DocumentElement != null)
            {
                foreach (XmlNode node in xml.DocumentElement.ChildNodes)
                {
                    // first node is the url ... have to go to nexted loc node
                    foreach (XmlNode item in node)
                    {
                        if (item.Name.Equals("ProjectGuid") == true)
                        {
                            string newGuid = Guid.NewGuid().ToString().ToUpper();

                            result.Add($"{item.InnerText}{Separator}{newGuid}");
                            item.InnerText = "{" + newGuid + "}";
                        }
                    }
                }
            }
            xml.Save(filePath);
            return [.. result];
        }
        /// <summary>
        /// Creates the target file path based on the source file path and solution directories.
        /// </summary>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceSolutionPath">The source solution directory.</param>
        /// <param name="targetSolutionPath">The target solution directory.</param>
        /// <returns>The target file path.</returns>
        private static string CreateTargetFilePath(string sourceFilePath, string sourceSolutionPath, string targetSolutionPath)
        {
            var result = targetSolutionPath;
            var sourceSolutionFilePath = TemplatePath.GetSolutionFilePath(sourceSolutionPath);
            var sourceSolutionName = Path.GetFileNameWithoutExtension(sourceSolutionFilePath);
            var targetSolutionFolder = new DirectoryInfo(targetSolutionPath).Name;
            var subSourceFilePath = sourceFilePath.Replace(sourceSolutionPath, string.Empty);

            foreach (var item in subSourceFilePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrEmpty(item) == false)
                {
                    result = Path.Combine(result, item.Replace(sourceSolutionName, targetSolutionFolder));
                }
            }
            return result;
        }

        /// <summary>
        /// Ensures the existence of a directory at the specified path.
        /// </summary>
        /// <param name="path">The string representing the directory path.</param>
        private static void EnsureExistsDirectory(string path)
        {
            if (path != null && Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }
        ///<summary>
        /// Copies a file from a source path to a target path.
        ///</summary>
        ///<param name="sourceFilePath">The path of the file to be copied.</param>
        ///<param name="targetFilePath">The path where the file should be copied to.</param>
        ///<remarks>
        /// This method ensures that the target directory exists before copying the file. If the directory does not exist, it will create it.
        ///</remarks>
        private static void CopyFile(string sourceFilePath, string targetFilePath)
        {
            var directory = Path.GetDirectoryName(targetFilePath);

            if (directory != null)
            {
                EnsureExistsDirectory(directory);
            }
            File.Copy(sourceFilePath, targetFilePath);
        }
        /// <summary>
        /// Writes all lines to a specified file.
        /// </summary>
        /// <param name="filePath">The path to the file to write the lines to.</param>
        /// <param name="lines">An enumerable collection of strings representing the lines to write.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <remarks>
        /// If the directory does not exist, it will be created before writing the lines to the file.
        /// </remarks>
        private static void WriteAllLines(string filePath, IEnumerable<string> lines, Encoding encoding)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (directory != null)
            {
                EnsureExistsDirectory(directory);
            }
            File.WriteAllLines(filePath, [.. lines], encoding);
        }
    }
}

