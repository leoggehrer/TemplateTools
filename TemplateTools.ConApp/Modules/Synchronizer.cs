//@BaseCode
using System.Text;

namespace TemplateTools.ConApp.Modules
{
    public partial class Synchronizer
    {
        /// <summary>
        /// Balances the solutions between a source path and a target path using the specified labels and search patterns.
        /// </summary>
        /// <param name="sourcePath">The path of the source.</param>
        /// <param name="sourceLabels">The labels of the source.</param>
        /// <param name="targetPath">The path of the target.</param>
        /// <param name="targetLabels">The labels of the target.</param>
        /// <param name="searchPatterns">The search patterns to use.</param>
        public static void BalancingPath(string sourcePath, string[] sourceLabels, string targetPath, string[] targetLabels, string[] searchPatterns)
        {
            BalancingPaths(sourcePath, sourceLabels, [targetPath], targetLabels, searchPatterns);
        }
        /// <summary>
        /// Balances the solutions by deleting target labeled files and copying source labeled files.
        /// </summary>
        /// <param name="sourcePath">The path of the source directory.</param>
        /// <param name="sourceLabels">An array of source labels.</param>
        /// <param name="targetPaths">An IEnumerable containing the target paths.</param>
        /// <param name="targetLabels">An array of target labels.</param>
        /// <remarks>
        /// This method deletes all target labeled files in the targetPaths and then copies all source labeled files from the sourcePath to the targetPaths.
        /// </remarks>
        public static void BalancingPaths(string sourcePath, string[] sourceLabels, IEnumerable<string> targetPaths, string[] targetLabels, string[] searchPatterns)
        {
            var sourcePathExists = Directory.Exists(sourcePath);

            if (sourcePathExists && sourceLabels.Length == targetLabels.Length)
            {
                var targetPathsExists = new List<string>();

                foreach (var path in targetPaths)
                {
                    if (Directory.Exists(path))
                    {
                        targetPathsExists.Add(path);
                    }
                }
                for (int i = 0; i < sourceLabels.Length; i++)
                {
                    var sourceLabel = sourceLabels[i];
                    var targetLabel = targetLabels[i];

                    // Delete all target labeled files
                    foreach (var targetPath in targetPathsExists)
                    {
                        foreach (var searchPattern in searchPatterns)
                        {
                            var targetCodeFiles = GetSourceCodeFiles(targetPath, searchPattern, targetLabel);

                            foreach (var targetCodeFile in targetCodeFiles)
                            {
                                File.Delete(targetCodeFile);
                            }
                        }
                    }
                    // Copy all source labeled files
                    foreach (var searchPattern in searchPatterns)
                    {
                        var sourceCodeFiles = GetSourceCodeFiles(sourcePath, searchPattern, sourceLabel);

                        foreach (var targetPath in targetPathsExists)
                        {
                            foreach (var sourceCodeFile in sourceCodeFiles)
                            {
                                SynchronizeSourceCodeFile(sourceCodeFile, targetPath, sourceLabel, targetLabel);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Copies a source code file from the source path to the target path and synchronizes it
        /// with the specified labels.
        /// </summary>
        /// <param name="sourceFilePath">The path of the source code file to be synchronized.</param>
        /// <param name="targetPath">The path of the target directory where the file will be synchronized.</param>
        /// <param name="sourceLabel">The label associated with the source code file.</param>
        /// <param name="targetLabel">The label associated with the target code file.</param>
        /// <returns>True if the synchronization is successful, otherwise false.</returns>
        private static bool SynchronizeSourceCodeFile(string sourceFilePath, string targetPath, string sourceLabel, string targetLabel)
        {
            var result = false;
            var canCopy = true;
            var isProjectTargetPath = targetPath == TemplatePath.GetDirectoryFromPath(targetPath, ".csproj");
            var isSolutionTargetPath = isProjectTargetPath == false && targetPath == TemplatePath.GetDirectoryFromPath(targetPath, ".sln");
            var isSolutionTargetSubPath = isSolutionTargetPath == false && TemplatePath.IsSolutionPath(targetPath);
            var sourcePath = Path.GetDirectoryName(sourceFilePath) ?? string.Empty;
            var sourceSolutionFilePath = TemplatePath.FindSolutionFilePath(sourcePath);
            var sourceSolutionName = Path.GetFileNameWithoutExtension(sourceSolutionFilePath);
            var targetSolutionFilePath = TemplatePath.FindSolutionFilePath(targetPath);
            var targetSolutionName = Path.GetFileNameWithoutExtension(targetSolutionFilePath);
            var targetFilePath = default(string);

            if (TemplatePath.IsProjectFilePath(sourceFilePath))
            {
                var sourceSubFilePath = TemplatePath.GetProjectSubFilePath(sourceFilePath);

                if (isProjectTargetPath)
                {
                    var targetProjectPath = default(string);
                    var sourceProjectName = TemplatePath.GetProjectNameFromPath(sourceFilePath);
                    var targetProjectName = TemplatePath.GetProjectNameFromPath(targetPath);

                    if (sourceProjectName.Equals(targetProjectName.Replace(targetSolutionName, sourceSolutionName)))
                    {
                        targetProjectPath = targetPath;
                        targetFilePath = Path.Combine(targetProjectPath, sourceSubFilePath);
                    }
                    else
                    {
                        canCopy = false;
                    }
                }
                else if (isSolutionTargetPath)
                {
                    var targetProjectName = default(string);
                    var targetProjectPath = default(string);
                    var sourceProjectName = TemplatePath.GetProjectNameFromPath(sourceFilePath);

                    targetProjectName = sourceProjectName.Replace(sourceSolutionName, targetSolutionName);
                    targetProjectPath = Path.Combine(targetPath, targetProjectName);
                    targetFilePath = Path.Combine(targetProjectPath, sourceSubFilePath);
                }
                else
                {
                    canCopy = false;
                }
            }
            else if (TemplatePath.IsAngularPath(targetPath) && TemplatePath.IsAngularFilePath(sourceFilePath))
            {
                var sourceSubFilePath = TemplatePath.GetProjectSubFilePath(sourceFilePath, ".esproj");

                targetFilePath = Path.Combine(targetPath, sourceSubFilePath);
            }
            else if (TemplatePath.IsSolutionFilePath(sourceFilePath))
            {
                if (isProjectTargetPath)
                {
                    canCopy = false;
                }
                else if (isSolutionTargetPath)
                {
                    var sourceSubFilePath = TemplatePath.GetSolutionSubFilePath(sourceFilePath);

                    targetFilePath = Path.Combine(targetPath, sourceSubFilePath);
                }
                else if (isSolutionTargetSubPath)
                {
                    var sourceSubFilePath = TemplatePath.GetSolutionSubFilePath(sourceFilePath);

                    targetFilePath = Path.Combine(targetPath, sourceSubFilePath);
                }
                else
                {
                    canCopy = false;
                }
            }
            else
            {
                var fileName = Path.GetFileName(sourceFilePath);

                targetFilePath = Path.Combine(targetPath, fileName);

                if (Directory.Exists(targetPath) == false)
                {
                    Directory.CreateDirectory(targetPath);
                }
                canCopy = File.Exists(targetFilePath) == false;
            }

            if (canCopy && targetFilePath.HasContent())
            {
                var path = Path.GetDirectoryName(targetFilePath);

                if (string.IsNullOrEmpty(path) == false && Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                if (File.Exists(targetFilePath))
                {
                    var lines = File.ReadAllLines(targetFilePath, Encoding.Default);

                    canCopy = false;
                    if (lines.Length != 0 && lines.First().Contains(targetLabel))
                    {
                        canCopy = true;
                    }
                }
                if (canCopy && string.IsNullOrEmpty(targetFilePath) == false)
                {
                    var cpyLines = new List<string>();
                    var srcLines = File.ReadAllLines(sourceFilePath, Encoding.Default)
                                       .Select(i => i.Replace(sourceSolutionName, targetSolutionName));
                    var srcFirst = srcLines.FirstOrDefault();

                    if (srcFirst != null)
                    {
                        cpyLines.Add(srcFirst.Replace(sourceLabel, targetLabel));
                    }
                    cpyLines.AddRange(File.ReadAllLines(sourceFilePath, Encoding.Default)
                            .Skip(1)
                            .Select(i => i.Replace(sourceSolutionName, targetSolutionName)));
                    File.WriteAllLines(targetFilePath, [.. cpyLines], Encoding.UTF8);
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves a collection of source code files from a given directory path.
        /// </summary>
        /// <param name="path">The root directory path where the search will begin.</param>
        /// <param name="searchPattern">The search pattern used to filter the files.</param>
        /// <param name="label">The label used to identify relevant files.</param>
        /// <returns>A collection of file paths that match the search pattern and contain the specified label.</returns>
        private static List<string> GetSourceCodeFiles(string path, string searchPattern, string label)
        {
            var result = new List<string>();
            var files = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                                 .Where(f => CommonStaticLiterals.GenerationIgnoreFolders.Any(e => f.Contains(e)) == false)
                                 .OrderBy(i => i);

            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file, Encoding.Default);

                if (lines.Length != 0 && lines.First().Contains(label))
                {
                    result.Add(file);
                }
                //System.Diagnostics.Debug.WriteLine($"{file}");
            }
            return result;
        }
    }
}
