//@BaseCode

namespace TemplateTools.ConApp
{
    public abstract partial class ConsoleApplication : CommonTool.ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="ConsoleApplication"/> class.
        /// </summary>
        /// <remarks>
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static ConsoleApplication()
        {
            ClassConstructing();
            var reposPath = Path.Combine(SourcePath, "repos");
            if (Directory.Exists(reposPath))
            {
                ReposPath = reposPath;
            }
            if (string.IsNullOrEmpty(SolutionPath))
            {
                SolutionPath = TemplatePath.GetSolutionPathByExecution();
            }
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

        #region Instance-Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleApplication"/> class.
        /// </summary>
        public ConsoleApplication()
        {
            Constructing();
            MaxSubPathDepth = 1;
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of the object.
        /// </summary>
        partial void Constructing();
        /// <summary>
        /// This method is called when the object is constructed.
        /// </summary>
        partial void Constructed();
        #endregion Instance-Constructors

        #region Helpers
        protected static string ReposPath { get; set; } = SourcePath;
        /// <summary>
        /// Retrieves a collection of source code files from a given directory path.
        /// </summary>
        /// <param name="path">The root directory path where the search will begin.</param>
        /// <param name="searchPattern">The search pattern used to filter the files.</param>
        /// <returns>A collection of file paths that match the search pattern and contain the specified label.</returns>
        protected static List<string> GetFilesByExtension(string path, string searchPattern)
        {
            var result = new List<string>();
            var files = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                                 .Where(f => CommonStaticLiterals.GenerationIgnoreFolders.Any(e => f.Contains(e)) == false)
                                 .OrderBy(i => i);

            result.AddRange(files);
            return result;
        }
        #endregion Helpers
    }
}
