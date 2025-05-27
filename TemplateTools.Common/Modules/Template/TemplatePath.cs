//@BaseCode

using System.Reflection;

namespace TemplateTools.Common.Modules.Template
{
    /// <summary>
    /// Provides utility methods for working with template paths.
    /// </summary>
    public static partial class TemplatePath
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// </summary>
        /// <remarks>
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static TemplatePath()
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

        #region methods
        /// <summary>
        /// Retrieves the sub-paths within the specified start path.
        /// </summary>
        /// <param name="startPath">The starting path.</param>
        /// <returns>An array of sub-paths.</returns>
        public static string[] GetSubPaths(string startPath)
        {
            return QueryDirectoryStructure(startPath, n => n.Contains('.') == false, StaticLiterals.IgnoreFolderNames);
        }
        /// <summary>
        /// Retrieves an array of sub-paths within the specified start path, up to the specified maximum depth.
        /// </summary>
        /// <param name="startPath">The starting path from which to retrieve sub-paths.</param>
        /// <param name="maxDepth">The maximum depth of sub-paths to retrieve.</param>
        /// <returns>An array of sub-paths within the specified start path.</returns>
        public static string[] GetSubPaths(string startPath, int maxDepth)
        {
            return QueryDirectoryStructure(startPath, n => n.StartsWith($"{Path.DirectorySeparatorChar}.") == false, maxDepth, StaticLiterals.IgnoreFolderNames);
        }
        /// <summary>
        /// Retrieves an array of quick template projects from the specified starting path.
        /// </summary>
        /// <param name="startPath">The starting path to search for quick template projects.</param>
        /// <returns>An array of quick template projects.</returns>
        /// <remarks>
        /// Quick template projects are identified by their name starting with "QT" and will exclude
        /// directories with names "bin", "obj", and "node_modules" from the search results.
        /// </remarks>
        public static string[] GetTemplatePaths(string startPath)
        {
            return QueryDirectoryStructure(startPath, n => n.StartsWith("SE"), "bin", "obj", "node_modules");
        }
        /// <summary>
        /// Retrieves an array of string values representing the paths to SETemplate solutions within a specified directory.
        /// </summary>
        /// <param name="startPath">The starting directory path in which to search for SETemplate solutions.</param>
        /// <returns>
        /// An array of string value representing the paths to SETemplate solutions found in the specified directory.
        /// </returns>
        public static string[] GetTemplateSolutions(string startPath)
        {
            var result = new List<string>();
            var paths = GetTemplatePaths(startPath);

            foreach (var qtPath in paths)
            {
                var di = new DirectoryInfo(qtPath);

                if (di.GetFiles().Any(f => Path.GetExtension(f.Name).Equals(".sln", StringComparison.CurrentCultureIgnoreCase)))
                {
                    result.Add(qtPath);
                }
            }
            return [.. result];
        }
        /// <summary>
        /// Retrieves the directory structure of a specified path.
        /// </summary>
        /// <param name="path">The path to the root directory.</param>
        /// <param name="filter">The optional filter function to determine which directories to include.</param>
        /// <param name="excludeFolders">The optional list of folder names to exclude from the directory structure.</param>
        /// <returns>An array of strings representing the full paths of the directories in the directory structure.</returns>
        public static string[] QueryDirectoryStructure(string path, Func<string, bool>? filter, params string[] excludeFolders)
        {
            return QueryDirectoryStructure(path, filter, 3, excludeFolders);
        }

        /// <summary>
        /// Queries the directory structure starting from the specified path and returns an array of directory paths that match the specified criteria.
        /// </summary>
        /// <param name="path">The root path from which to start querying the directory structure.</param>
        /// <param name="filter">An optional filter function to apply to each directory name. Only directories that satisfy the filter will be included in the result.</param>
        /// <param name="maxDeep">The maximum depth of the directory structure to query.</param>
        /// <param name="excludeFolders">An array of folder names to exclude from the result.</param>
        /// <returns>An array of directory paths that match the specified criteria.</returns>
        public static string[] QueryDirectoryStructure(string path, Func<string, bool>? filter, int maxDeep, params string[] excludeFolders)
        {
            static void GetDirectoriesWithoutHidden(Func<string, bool>? filter, DirectoryInfo directoryInfo, List<string> list, int maxDeep, int deep, params string[] excludeFolders)
            {
                try
                {
                    if (directoryInfo.Attributes.HasFlag(FileAttributes.Hidden) == false)
                    {
                        if ((filter == null || filter(directoryInfo.Name)))
                        {
                            list.Add(directoryInfo.FullName);
                        }
                        if (deep < maxDeep)
                        {
                            foreach (var di in directoryInfo.GetDirectories())
                            {
                                if (excludeFolders.Any(e => e.Equals(di.Name, StringComparison.CurrentCultureIgnoreCase)) == false)
                                {
                                    GetDirectoriesWithoutHidden(filter, di, list, maxDeep, deep + 1, excludeFolders);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
                }
            }
            var result = new List<string>();
            var directoryInfo = new DirectoryInfo(path);

            GetDirectoriesWithoutHidden(filter, directoryInfo, result, Math.Max(maxDeep, 0), 0, excludeFolders);
            return [.. result];
        }

        /// <summary>
        /// Retrieves an array of parent paths for quick template projects starting from a specified path, as well as any additional paths to include.
        /// </summary>
        /// <param name="startPath">The starting path to search for quick template projects.</param>
        /// <param name="includePaths">The additional paths to include in the search.</param>
        /// <returns>An array of parent paths for quick template projects.</returns>
        /// <remarks>
        /// The method retrieves quick template projects using the GetSETemplateProjects method with the specified start path and includes the additional paths provided in the includePaths parameter.
        /// The method then determines the parent directory for each quick template project path and ensures that there are no duplicate or nested paths in the result.
        /// The resulting parent paths are ordered alphabetically and returned as an array.
        /// </remarks>
        public static string[] GetTemplateParentPaths(string startPath, params string[] includePaths)
        {
            var result = new List<string>();
            var qtProjects = GetTemplatePaths(startPath).Union(includePaths).ToArray();
            var qtPaths = qtProjects.Select(p => GetParentDirectory(p))
                                    .Distinct()
                                    .OrderBy(p => p);

            foreach (var qtPath in qtPaths)
            {
                if (result.Any(x => qtPath.Length > x.Length && qtPath.Contains(x)) == false)
                {
                    result.Add(qtPath);
                }
            }
            return [.. result];
        }
        /// <summary>
        /// Returns the parent directory of the specified path.
        /// </summary>
        /// <param name="path">The path for which to retrieve the parent directory.</param>
        /// <returns>
        /// The full path of the parent directory of the specified path if it exists; otherwise, the original path is returned.
        /// </returns>
        /// <remarks>
        /// This method uses the <see cref="Directory.GetParent(string)"/> method to retrieve the parent directory of the specified path.
        /// It returns the full path of the parent directory if it exists; otherwise, it returns the original path.
        /// </remarks>
        public static string GetParentDirectory(string path)
        {
            var result = Directory.GetParent(path);

            return result != null ? result.FullName : path;
        }

        /// <summary>
        /// Retrieves the solution path by examining the execution path.
        /// </summary>
        /// <returns>The directory path of the solution file if found; otherwise, an empty string.</returns>
        public static string GetSolutionPathByExecution()
        {
            var result = string.Empty;
            var executionPath = AppDomain.CurrentDomain.BaseDirectory;

            // Search for the solution file in the current directory and parent directories
            var solutionFile = FindSolutionFilePath(executionPath);

            if (solutionFile != null)
            {
                result = Path.GetDirectoryName(solutionFile) ?? string.Empty;
            }
            return result;
        }

        /// <summary>
        /// Determines whether the specified path is a solution path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the specified path is a solution path; otherwise, <c>false</c>.</returns>
        public static bool IsSolutionPath(string path)
        {
            return path.HasContent() && GetSolutionItemDataFromPath(path, ".sln").Name.HasContent();
        }
        /// <summary>
        /// Determines whether the specified file path is a solution file path.
        /// </summary>
        /// <param name="filePath">The file path to check.</param>
        /// <returns><c>true</c> if the file path is a solution file path; otherwise, <c>false</c>.</returns>
        public static bool IsSolutionFilePath(string filePath)
        {
            var path = Path.GetDirectoryName(filePath);

            return IsSolutionPath(path ?? string.Empty);
        }
        /// <summary>
        /// Finds the solution file path by traversing up the directory tree from the specified directory.
        /// </summary>
        /// <param name="directory">The starting directory to search for the solution file.</param>
        /// <returns>The full path of the solution file if found; otherwise, an empty string.</returns>
        public static string FindSolutionFilePath(string directory)
        {
            List<string> solutionFiles = new();

            // Traverse through all directories upwards until the root folder is reached
            while (directory.HasContent())
            {
                // Search for a .sln file in the current directory
                solutionFiles.AddRange(Directory.GetFiles(directory, "*.sln"));

                // Move to the parent directory
                directory = Directory.GetParent(directory)?.FullName!;
            }
            return solutionFiles.Count > 0 ? solutionFiles.Last() : string.Empty;
        }
        /// <summary>
        /// Retrieves the solution name from the given file path.
        /// </summary>
        /// <param name="path">The path of the solution file.</param>
        /// <returns>The name of the solution file.</returns>
        public static string GetSolutionNameFromPath(string path)
        {
            return GetSolutionItemDataFromPath(path, ".sln").Name;
        }
        /// <summary>
        /// Gets the solution file path from the specified source solution path.
        /// </summary>
        /// <param name="sourcePath">The directory of the source solution.</param>
        /// <returns>The file path of the solution file if found; otherwise, an empty string.</returns>
        public static string GetSolutionFilePath(string sourcePath)
        {
            return Directory.GetFiles(sourcePath, $"*{StaticLiterals.SolutionFileExtension}", SearchOption.TopDirectoryOnly)
                            .FirstOrDefault() ?? string.Empty;
        }
        /// <summary>
        /// Gets the name of the solution file from the specified source solution path.
        /// </summary>
        /// <param name="sourcePath">The directory of the source solution.</param>
        /// <returns>The name of the solution file if found; otherwise, an empty string.</returns>
        public static string GetSolutionFileName(string sourcePath)
        {
            return Path.GetFileName(GetSolutionFilePath(sourcePath));
        }
        /// <summary>
        /// Gets the name of the solution from the specified source solution path.
        /// </summary>
        /// <param name="sourcePath">The directory of the source solution.</param>
        /// <returns>The name of the solution file if found; otherwise, an empty string.</returns>
        public static string GetSolutionName(string sourcePath)
        {
            return Path.GetFileNameWithoutExtension(GetSolutionFileName(sourcePath));
        }
        /// <summary>
        /// Determines whether the specified path is a project path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the specified path is a project path; otherwise, <c>false</c>.</returns>
        public static bool IsProjectPath(string path)
        {
            return path.HasContent() && GetSolutionItemDataFromPath(path, ".csproj").Name.HasContent();
        }
        /// <summary>
        /// Determines whether the given file path is a project file path.
        /// </summary>
        /// <param name="filePath">The file path to check.</param>
        /// <returns><c>true</c> if the file path is a project file path; otherwise, <c>false</c>.</returns>
        public static bool IsProjectFilePath(string filePath)
        {
            var path = Path.GetDirectoryName(filePath);

            return IsProjectPath(path ?? string.Empty);
        }

        /// <summary>
        /// Determines whether the specified path belongs to an Angular project.
        /// </summary>
        /// <param name="filePath">The file path to check.</param>
        /// <returns><c>true</c> if the file path is part of an Angular project; otherwise, <c>false</c>.</returns>
        public static bool IsAngularPath(string path)
        {
            var result = false;

            if (path.HasContent())
            {
                var projectName = GetSolutionItemDataFromPath(path!, ".esproj").Name;

                result = projectName.EndsWith(StaticLiterals.AngularExtension);
            }
            return result;
        }

        /// <summary>
        /// Determines whether the specified file path belongs to an Angular project.
        /// </summary>
        /// <param name="filePath">The file path to check.</param>
        /// <returns><c>true</c> if the file path is part of an Angular project; otherwise, <c>false</c>.</returns>
        public static bool IsAngularFilePath(string filePath)
        {
            var result = false;
            var path = Path.GetDirectoryName(filePath);

            if (path.HasContent())
            {
                var projectName = GetSolutionItemDataFromPath(path!, ".esproj").Name;

                result = projectName.EndsWith(StaticLiterals.AngularExtension);
            }
            return result;
        }

        /// <summary>
        /// Gets the project name from the given path.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>The project name.</returns>
        public static string GetProjectNameFromPath(string path)
        {
            return GetSolutionItemDataFromPath(path, ".csproj").Name;
        }

        /// <summary>
        /// Gets the sub file path by extracting the file path relative to the .csproj file within the solution.
        /// </summary>
        /// <param name="filePath">The full file path.</param>
        /// <returns>The sub file path relative to the .csproj file within the solution.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the filePath is null.</exception>
        public static string GetProjectSubFilePath(string filePath)
        {
            return GetProjectSubFilePath(filePath, ".csproj");
        }

        /// <summary>
        /// Gets the sub file path by extracting the file path relative to the specified project file extension within the solution.
        /// </summary>
        /// <param name="filePath">The full file path.</param>
        /// <param name="fileExtension">The file extension of the project file (e.g., ".csproj", ".esproj").</param>
        /// <returns>
        /// The sub file path relative to the project file within the solution.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is null.</exception>
        public static string GetProjectSubFilePath(string filePath, string fileExtension)
        {
            var subPath = GetSolutionItemDataFromPath(filePath, fileExtension).SubPath;
            var result = filePath!.Replace(subPath, string.Empty);

            if (result.StartsWith(Path.DirectorySeparatorChar))
            {
                result = result[1..];
            }
            return result;
        }

        /// <summary>
        /// Gets the sub file path of a solution file.
        /// </summary>
        /// <param name="filePath">The full file path.</param>
        /// <returns>The sub file path.</returns>
        public static string GetSolutionSubFilePath(string filePath)
        {
            var subPath = GetSolutionItemDataFromPath(filePath, ".sln").SubPath;
            var result = filePath!.Replace(subPath, string.Empty);

            if (result.StartsWith(Path.DirectorySeparatorChar))
            {
                result = result[1..];
            }
            return result;
        }

        /// <summary>
        /// Retrieves the solution item data from the given path and item extension.
        /// </summary>
        /// <param name="path">The path to search for the solution item.</param>
        /// <param name="itemExtension">The extension of the solution item.</param>
        /// <returns>A tuple containing the name and subpath of the solution item.</returns>
        public static (string Name, string SubPath) GetSolutionItemDataFromPath(string path, string itemExtension)
        {
            var itemName = string.Empty;
            var subPath = path.StartsWith(Path.DirectorySeparatorChar) ? Path.DirectorySeparatorChar.ToString() : string.Empty;
            var itemsEnumerator = path.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
                                      .GetEnumerator();

            while (itemName.IsNullOrEmpty() && itemsEnumerator.MoveNext())
            {
                subPath = Path.Combine(subPath, itemsEnumerator.Current.ToString()!);

                var files = Directory.GetFiles(subPath, $"*{itemExtension}");

                if (files.Length != 0)
                {
                    itemName = itemsEnumerator.Current.ToString() ?? string.Empty;
                }
            }
            return (itemName, subPath);
        }

        /// <summary>
        /// Retrieves the path from the given path by checking for the presence of a file with the specified extension.
        /// </summary>
        /// <param name="path">The original path.</param>
        /// <param name="checkFileExtension">The file extension to check for.</param>
        /// <returns>
        /// The path up to the directory where the first file with the specified extension is found,
        /// or an empty string if no such file is found.
        /// </returns>
        public static string GetDirectoryFromPath(string path, string checkFileExtension)
        {
            var result = string.Empty;
            var checkPath = path.StartsWith(Path.DirectorySeparatorChar) ? Path.DirectorySeparatorChar.ToString() : string.Empty;
            var data = path.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < data.Length && result == string.Empty; i++)
            {
                checkPath = Path.Combine(checkPath, data[i]);

                var files = Directory.GetFiles(checkPath, $"*{checkFileExtension}");

                if (files.Length != 0)
                {
                    result = checkPath;
                }
            }
            return result;
        }
        /// <summary>
        /// Retrieves the directory name from a given path by checking for a specific file extension.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <param name="checkFileExtension">The file extension to check for.</param>
        /// <returns>
        /// The directory name that contains the file with the specified extension, or an empty string if no such file is found.
        /// </returns>
        public static string GetFolderNameFromPath(string path, string checkFileExtension)
        {
            var result = string.Empty;
            var checkPath = path.StartsWith(Path.DirectorySeparatorChar) ? Path.DirectorySeparatorChar.ToString() : string.Empty;
            var data = path.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < data.Length && result == string.Empty; i++)
            {
                checkPath = Path.Combine(checkPath, data[i]);

                var files = Directory.GetFiles(checkPath, $"*{checkFileExtension}");

                if (files.Length != 0)
                {
                    result = data[i];
                }
            }
            return result;
        }

        /// <summary>
        /// Recursively deletes files and empty directories from the specified path,
        /// excluding directories specified in the dropFolders parameter.
        /// </summary>
        /// <param name="path">The root path from where the cleaning operation begins.</param>
        /// <param name="dropFolders">The array of folder names to exclude from deletion.</param>
        public static void CleanDirectories(string path, params string[] dropFolders)
        {
            static int CleanDirectories(DirectoryInfo dirInfo, params string[] dropFolders)
            {
                int result = 0;

                try
                {
                    result = dirInfo.GetFiles().Length;
                    foreach (var item in dirInfo.GetDirectories())
                    {
                        int fileCount = CleanDirectories(item, dropFolders);

                        try
                        {
                            if (fileCount == 0)
                            {
                                item.Delete();
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
            CleanDirectories(new DirectoryInfo(path), dropFolders);
        }
        #endregion methods

        #region CLI argument methods
        /// <summary>
        /// Opens the solution folder in Windows Explorer.
        /// </summary>
        /// <param name="solutionPath">The path of the solution folder.</param>
        public static void OpenSolutionFolder(string solutionPath)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    WorkingDirectory = solutionPath,
                    FileName = "explorer",
                    Arguments = solutionPath,
                    CreateNoWindow = true,
                });
            }
        }
        #endregion CLI argument methods
    }
}
