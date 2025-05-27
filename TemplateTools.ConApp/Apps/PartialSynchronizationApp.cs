//@BaseCode

using TemplateTools.ConApp.Modules;

namespace TemplateTools.ConApp.Apps
{
    public partial class PartialSynchronizationApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes static members of the ComparisonApp class.
        /// </summary>
        /// <remarks>
        /// This constructor sets up the initial values for the static properties and arrays used in the ComparisonApp class.
        /// </remarks>
        static PartialSynchronizationApp()
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
        public PartialSynchronizationApp(string sourceCodePath, string targetCodePath)
        {
            Constructing();
            PageSize = 20;
            SourceCodePath = sourceCodePath;
            SourceLabels = [CommonStaticLiterals.BaseCodeLabel, CommonStaticLiterals.BaseCodeLabel];
            TargetCodePath = targetCodePath;
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
        private string SourceCodePath { get; set; }
        /// <summary>
        /// Gets or sets the path of the target code.
        /// </summary>
        private string TargetCodePath { get; set; }
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
        /// Executes the necessary actions before the main execution of the program.
        /// </summary>
        protected override void BeforeExecution()
        {
            PrintHeader();
            StartProgressBar();
        }
        /// <summary>
        /// Performs actions after the execution of the method.
        /// </summary>
        protected override void AfterExecution()
        {
            StopProgressBar();
        }
        /// <summary>
        /// Creates an array of menu items for the application menu.
        /// </summary>
        /// <returns>An array of MenuItem objects representing the menu items.</returns>
        protected override MenuItem[] CreateMenuItems()
        {
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
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText($"{MaxSubPathDepth}", "Change max sub path depth"),
                    Action = (self) => ChangeMaxSubPathDepth(),
                },
                new()
                {
                    Key = "-----",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
            };

            var targetPaths = Directory.GetDirectories(TargetCodePath)
                                       .Where(d => d.Contains($"{Path.DirectorySeparatorChar}.") == false)
                                       .OrderBy(d => d)
                                       .ToArray();

            menuItems.AddRange(CreatePageMenuItems(ref mnuIdx, targetPaths, (item, menuItem) =>
            {
                menuItem.OptionalKey = "a";
                menuItem.Text = ToLabelText("Synchronize with", $"{item.Replace(TargetCodePath, string.Empty)}");
                menuItem.Action = (self) =>
                {
                    var targetPath = self.Params["targetpath"]?.ToString() ?? string.Empty;

                    BalancingPath(SourceCodePath, SourceLabels, targetPath, TargetLabels, SearchPatterns);
                };
                menuItem.Params = new() { { "targetpath", item } };
            }));
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
            headerParams.Add(new(new string('-', 32), string.Empty));
            headerParams.Add(new("Source code path:", SourceCodePath));
            headerParams.Add(new("Target code path:", TargetCodePath));

            base.PrintHeader("Template Partial Synchronization", [.. headerParams]);
        }
        #endregion overrides

        #region App methods
        /// <summary>
        /// Balances the solutions between a source path and a target path using the specified labels and search patterns.
        /// </summary>
        /// <param name="sourcePath">The path of the source.</param>
        /// <param name="sourceLabels">The labels of the source.</param>
        /// <param name="targetPath">The path of the target.</param>
        /// <param name="targetLabels">The labels of the target.</param>
        /// <param name="searchPatterns">The search patterns to use.</param>
        private static void BalancingPath(string sourcePath, string[] sourceLabels, string targetPath, string[] targetLabels, string[] searchPatterns)
        {
            PrintLine($"Balancing {targetPath}...");
            Synchronizer.BalancingPath(sourcePath, sourceLabels, targetPath, targetLabels, searchPatterns);
        }
        #endregion App methods
    }
}
