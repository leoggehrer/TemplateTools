//@BaseCode
using TemplateTools.ConApp.Modules;

namespace TemplateTools.ConApp.Apps
{
    /// <summary>
    /// Represents an application for comparing and synchronizing source code files between a source path and multiple target paths.
    /// </summary>
    internal partial class SynchronizationApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes static members of the ComparisonApp class.
        /// </summary>
        /// <remarks>
        /// This constructor sets up the initial values for the static properties and arrays used in the ComparisonApp class.
        /// </remarks>
        static SynchronizationApp()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called just before the class constructor is called.
        /// </summary>
        /// <remarks>
        /// This method can be used to perform any necessary initialization or setup
        /// logic specific to the class being constructed, before the constructor
        /// is executed.
        /// </remarks>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region Instance-Constructors
        /// <summary>
        /// Represents an application for performing comparisons.
        /// </summary>
        public SynchronizationApp()
        {
            Constructing();
            CodeSolutionPath = SolutionPath;
            SourceLabels = [CommonStaticLiterals.BaseCodeLabel, CommonStaticLiterals.BaseCodeLabel];
            TargetPaths = [];
            AddTargetPaths = [];
            TargetLabels = [CommonStaticLiterals.CodeCopyLabel, CommonStaticLiterals.BaseCodeLabel];
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

        #region properties
        /// <summary>
        /// Gets or sets the source path.
        /// </summary>
        private string CodeSolutionPath { get; set; }
        /// <summary>
        /// Gets or sets the target paths.
        /// </summary>
        /// <value>
        /// An array of strings representing the target paths.
        /// </value>
        private string[] TargetPaths { get; set; }
        /// <summary>
        /// Gets or sets the target paths to be added.
        /// </summary>
        /// <value>
        /// An array of strings representing the target paths to be added.
        /// </value>
        private string[] AddTargetPaths { get; set; }
        /// <summary>
        /// Gets an array of search patterns used for searching source files.
        /// </summary>
        /// <value>
        /// An array of search patterns.
        /// </value>
        private static string[] SearchPatterns => CommonStaticLiterals.SourceFileExtensions.Split('|');
        /// <summary>
        /// Gets or sets the source labels.
        /// </summary>
        /// <value>
        /// The source labels.
        /// </value>
        private string[] SourceLabels { get; set; }
        /// <summary>
        /// Gets or sets the target labels.
        /// </summary>
        /// <value>
        /// An array of strings representing the target labels.
        /// </value>
        private string[] TargetLabels { get; set; }
        #endregion Properties

        #region overrides
        /// <summary>
        /// Creates an array of menu items for the application menu.
        /// </summary>
        /// <returns>An array of MenuItem objects representing the menu items.</returns>
        protected override MenuItem[] CreateMenuItems()
        {
            var handled = false;
            var targetPaths = new List<string>();
            var mnuIdx = 0;
            var menuItems = new List<MenuItem>()
            {
                new()
                {
                    Key = "-----",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Path", "Change the source solution path"),
                    Action = (self) =>
                    {
                        CodeSolutionPath = ChangeTemplateSolutionPath(CodeSolutionPath, MaxSubPathDepth, ReposPath);
                    }
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Add path", "Add a target path"),
                    Action = (self) => AddTargetPath(),
                },
                new()
                {
                    Key = "-----",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
            };
            BeforeGetTargetPaths(ReposPath, targetPaths, ref handled);
            if (handled == false)
            {
                targetPaths.AddRange(TemplatePath.GetTemplateSolutions(ReposPath));
                targetPaths.AddRange(AddTargetPaths);
            }
            else
            {
                targetPaths.AddRange(AddTargetPaths);
            }
            targetPaths.Remove(CodeSolutionPath);
            AfterGetTargetPaths(ReposPath, targetPaths);

            TargetPaths = [.. targetPaths.Distinct().Order()];

            foreach (var path in TargetPaths)
            {
                menuItems.Add(new()
                {
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = "a",
                    Text = ToLabelText("Synchronize with", $"{path.Replace(ReposPath, ".")}", 19, ' '),
                    Action = (self) =>
                    {
                        var targetPath = self.Params["path"]?.ToString() ?? string.Empty;

                        new PartialSynchronizationApp(CodeSolutionPath, targetPath).Run([]);
                    },
                    Params = new() { { "path", path } },
                });
            }
            return [.. menuItems.Union(CreateExitMenuItems())];
        }
        /// <summary>
        /// Prints the header for the PlantUML application.
        /// </summary>
        protected override void PrintHeader()
        {
            List<KeyValuePair<string, object>> headerParams = [new("Balance labels(s):", string.Empty)];
            for (int i = 0; i < SourceLabels.Length && i < TargetLabels.Length; i++)
            {
                headerParams.Add(new($"  {SourceLabels[i],-15} =>", TargetLabels[i]));
            }
            headerParams.Add(new("Source code path:", CodeSolutionPath));

            base.PrintHeader("Template Synchronization", [.. headerParams]);
        }
        #endregion overrides

        #region app methods
        /// <summary>
        /// Adds a target path to the list of target paths.
        /// </summary>
        private void AddTargetPath()
        {
            PrintLine();
            Print("Add path: ");

            var input = Console.ReadLine();

            if (Directory.Exists(input))
            {
                AddTargetPaths = [.. AddTargetPaths.Union([input])];
            }
        }
        /// <summary>
        /// Balances the solutions between a source path and a target path using the specified labels and search patterns.
        /// </summary>
        /// <param name="sourcePath">The path of the source.</param>
        /// <param name="sourceLabels">The labels of the source.</param>
        /// <param name="targetPath">The path of the target.</param>
        /// <param name="targetLabels">The labels of the target.</param>
        /// <param name="searchPatterns">The search patterns to use.</param>
        private static void BalancingSolution(string sourcePath, string[] sourceLabels, string targetPath, string[] targetLabels, string[] searchPatterns)
        {
            StartProgressBar();
            Synchronizer.BalancingPath(sourcePath, sourceLabels, targetPath, targetLabels, searchPatterns);
            StopProgressBar();
        }
        #endregion app methods

        #region partial methods
        /// <summary>
        /// Represents a method that is called before the GetTargetPaths method is executed.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPaths">The list of target paths.</param>
        /// <param name="handled">A reference to a boolean value indicating whether the method has been handled.</param>
        static partial void BeforeGetTargetPaths(string sourcePath, List<string> targetPaths, ref bool handled);
        /// <summary>
        /// This method is called after getting the target paths for a source path.
        /// </summary>
        /// <param name="sourcePath">The source path for which the target paths were retrieved.</param>
        /// <param name="targetPaths">The list of target paths corresponding to the provided source path.</param>
        /// <remarks>
        /// This method allows performing additional tasks or modifications after obtaining the target paths
        /// for a given source path. It can be overridden in derived classes to customize the behavior.
        /// </remarks>
        static partial void AfterGetTargetPaths(string sourcePath, List<string> targetPaths);
        #endregion partial methods
    }
}
