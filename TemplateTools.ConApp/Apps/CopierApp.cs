//@BaseCode

using TemplateTools.Logic;

namespace TemplateTools.ConApp.Apps
{
    /// <summary>
    /// Represents an application for copying template solutions to a target solution.
    /// </summary>
    public partial class CopierApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// This is the static constructor for the CopierApp class.
        /// </summary>
        /// <remarks>
        /// This constructor is responsible for initializing the static members of the CopierApp class.
        /// </remarks>
        static CopierApp()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called when the class is being constructed.
        /// </summary>
        /// <remarks>
        /// This is a partial method and must be implemented in a partial class.
        /// </remarks>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the path of the source solution.
        /// </summary>
        private string SourceSolutionPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the target solution path.
        /// </summary>
        private string TargetSolutionSubPath { get; set; } = Directory.GetParent(SolutionPath)?.FullName ?? string.Empty;
        /// <summary>
        /// Gets or sets the name of the target solution.
        /// </summary>
        private string TargetSolutionName { get; set; } = "TargetSolution";
        #endregion Properties

        #region overrides
        /// <summary>
        /// Creates an array of menu items for the application menu.
        /// </summary>
        /// <returns>An array of MenuItem objects representing the menu items.</returns>
        protected override MenuItem[] CreateMenuItems()
        {
            var mnuIdx = 0;
            var menuItems = new List<MenuItem>
            {
                new()
                {
                    Key = "---",
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
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Source path", "Change the source solution path"),
                    Action = (self) => SourceSolutionPath = ChangeTemplateSolutionPath(SourceSolutionPath, MaxSubPathDepth, ReposPath),
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Target path", "Change the target solution path"),
                    Action = (self) => TargetSolutionSubPath = SelectOrChangeToSubPath(TargetSolutionSubPath, MaxSubPathDepth, ReposPath),
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Target name", "Change the target solution name"),
                    Action = (self) => ChangeTargetSolutionName(),
                },

                new()
                {
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },

                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Start", "Start copy process"),
                    Action = (self) => CopySolution(),
                },
            };
            return [.. menuItems.Union(CreateExitMenuItems())];
        }

        /// <summary>
        /// Prints the header for the PlantUML application.
        /// </summary>
        /// <param name="sourcePath">The path of the solution.</param>
        protected override void PrintHeader()
        {
            var solutionProperties = SolutionProperties.Create(SourceSolutionPath);
            var sourceSolutionName = solutionProperties.SolutionName;
            var sourceLabel = $"'{sourceSolutionName}' from:";
            var targetLabel = $"'{TargetSolutionName}' to:";

            List<KeyValuePair<string, object>> headerParams =
            [
                new(sourceLabel, SourceSolutionPath),
                new("  -> copy ->  ", string.Empty),
                new(targetLabel, Path.Combine(TargetSolutionSubPath, TargetSolutionName)),
            ];

            base.PrintHeader("Template Copier", [.. headerParams]);
        }
        /// <summary>
        /// Performs any necessary setup or initialization before running the application.
        /// </summary>
        /// <param name="args">The command-line arguments passed to the application.</param>
        protected override void BeforeRun(string[] args)
        {
            TargetSolutionName = "TargetSolution";
            base.BeforeRun(args);
        }
        #endregion overrides

        #region app methods
        /// <summary>
        /// Changes the target solution name based on user input.
        /// </summary>
        private void ChangeTargetSolutionName()
        {
            PrintLine();
            Print("Enter target solution name: ");
            var name = ReadLine();

            if (string.IsNullOrEmpty(name) == false)
            {
                TargetSolutionName = name;
            }
        }
        /// <summary>
        /// Copies the source solution to the target solution path, including all template projects.
        /// </summary>
        private void CopySolution()
        {
            var copier = new Modules.Copier();
            var targetSolutionPath = Path.Combine(TargetSolutionSubPath, TargetSolutionName);
            var solutionProperties = SolutionProperties.Create(SourceSolutionPath);
            var allSourceProjectNames = solutionProperties.AllTemplateProjectNames;

            PrintHeader();
            StartProgressBar();
            PrintLine($"Copying '{solutionProperties.SolutionName}' to '{TargetSolutionName}'...");
            copier.Copy(SourceSolutionPath, targetSolutionPath, allSourceProjectNames);
            StopProgressBar();

            TemplatePath.OpenSolutionFolder(targetSolutionPath);
        }
        #endregion app methods
    }
}

