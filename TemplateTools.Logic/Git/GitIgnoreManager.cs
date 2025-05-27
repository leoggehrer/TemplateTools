//@BaseCode

namespace TemplateTools.Logic.Git
{
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Text;
    /// <summary>
    /// Manages the .gitignore file for a given directory.
    /// </summary>
    public partial class GitIgnoreManager
    {
        #region Properties
        /// <summary>
        /// Represents the path to the .gitignore file.
        /// </summary>
        private static string GitIgnoreFile => ".gitignore";
        /// <summary>
        /// Gets the search patterns used for searching source files.
        /// </summary>
        /// <value>
        /// An array of strings representing the search patterns.
        /// </value>
        private static string[] SearchPatterns => StaticLiterals.SourceFileExtensions.Split('|');
        
        /// <summary>
        /// Gets or sets the beginning of a gitignore block in a SETemplate.
        /// </summary>
        private static string BeginGitIgnoreBlock => "#SETemplateStart";
        /// <summary>
        /// Gets the string value representing the end of a Git ignore block.
        /// </summary>
        /// <value>
        /// The string value "#SETemplateEnd" representing the end of a Git ignore block.
        /// </value>
        private static string EndGitIgnoreBlock => "#SETemplateEnd";
        #endregion Properties
        
        /// <summary>
        /// Removes all generated files from a git repository at the specified path.
        /// </summary>
        /// <param name="path">The path to the git repository.</param>
        public static void Run(string path)
        {
            var gitIgnoreResult = new List<string>();
            
            Parallel.ForEach(SearchPatterns, searchPattern =>
            {
                var sourceFiles = GetLabeledFiles(path, searchPattern, [StaticLiterals.GeneratedCodeLabel]);
                var gitIngoredFiles = GetGitIgnoredEntries(sourceFiles, path);
                
                lock (gitIgnoreResult)
                {
                    gitIgnoreResult.AddRange(gitIngoredFiles);
                }
            });
            WriteGitIgnoreFiles(gitIgnoreResult, path);
        }
        /// <summary>
        /// Deletes all generated files that are ignored from git.
        /// </summary>
        /// <param name="path">The path to the directory containing the files.</param>
        /// <returns>Void</returns>
        public static void DeleteIgnoreEntries(string path)
        {
            var files = Array.Empty<string>();
            
            WriteGitIgnoreFiles(files, path);
        }
        /// <summary>
        /// Retrieves a collection of files that contain specified labels, from a specified directory and its subdirectories.
        /// </summary>
        /// <param name="path">The directory path to search for files.</param>
        /// <param name="searchPattern">The search pattern to match against the names of the files.</param>
        /// <param name="labels">An array of labels to search for within the files.</param>
        /// <returns>A collection of file paths that contain at least one of the specified labels.</returns>
        private static ConcurrentBag<string> GetLabeledFiles(string path, string searchPattern, string[] labels)
        {
            var result = new ConcurrentBag<string>();
            var files = Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories);
            
            Parallel.ForEach(files, file =>
            {
                using var streamReader = new StreamReader(file, Encoding.Default);
                var line = streamReader.ReadLine();
                
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (labels.Any(l => line.Contains(l)))
                    {
                        result.Add(file);
                    }
                }
            });
            return result;
        }
        /// <summary>
        /// Retrieves the list of git ignored entries from the specified files and path.
        /// </summary>
        /// <param name="files">The collection of files to check.</param>
        /// <param name="path">The base path to remove from each file path.</param>
        /// <returns>A collection of git ignored entries.</returns>
        private static List<string> GetGitIgnoredEntries(IEnumerable<string> files, string path)
        {
            var result = new List<string>();
            
            foreach (var file in files)
            {
                var gitIgnoreFile = file.Replace(path, "");
                
                if (!string.IsNullOrWhiteSpace(gitIgnoreFile))
                {
                    result.Add(gitIgnoreFile.Replace(@"\", "/"));
                }
            }
            return result;
        }
        /// <summary>
        /// Writes the specified files to the gitignore file in the given path.
        /// </summary>
        /// <param name="files">The files to be written to the gitignore file.</param>
        /// <param name="path">The path where the gitignore file is located.</param>
        private static void WriteGitIgnoreFiles(IEnumerable<string> files, string path)
        {
            var gitIgnoreFilePath = Path.Combine(path, GitIgnoreFile);
            
            if (File.Exists(gitIgnoreFilePath))
            {
                var lines = File.ReadAllLines(gitIgnoreFilePath);
                var start = 0;
                var end = 0;
                
                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    
                    if (line == BeginGitIgnoreBlock)
                    {
                        start = i;
                    }
                    
                    if (start > 0 && line == EndGitIgnoreBlock)
                    {
                        end = i;
                    }
                }
                
                var result = new List<string>();
                
                if (end > start && start > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i < start || i > end)
                        {
                            result.Add(lines[i]);
                        }
                        else
                        {
                            i = end;
                            result.Add(BeginGitIgnoreBlock);
                            result.AddRange(files);
                            result.Add(EndGitIgnoreBlock);
                        }
                    }
                }
                else
                {
                    result.AddRange(lines);
                    result.Add(string.Empty);
                    result.Add(BeginGitIgnoreBlock);
                    result.AddRange(files);
                    result.Add(EndGitIgnoreBlock);
                }
                
                File.WriteAllLines(gitIgnoreFilePath, result);
                
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    var processInfo = new ProcessStartInfo();
                    var anyCommand = "git rm -r --cached .";
                    
                    processInfo.WorkingDirectory = path;
                    processInfo.UseShellExecute = true;
                    processInfo.FileName = @"C:\Windows\System32\cmd.exe";
                    processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    processInfo.Arguments = anyCommand;
                    Process.Start(processInfo);
                }
            }
        }
    }
}

