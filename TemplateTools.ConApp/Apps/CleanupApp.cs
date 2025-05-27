//@BaseCode
using System.IO;

namespace TemplateTools.ConApp.Apps
{
    /// <summary>
    /// Represents an internal class used for cleaning up directories.
    /// </summary>
    public partial class CleanupApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static CleanupApp()
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

        #region Instance-Constructors
        /// <summary>
        /// Represents a cleanup application.
        /// </summary>
        public CleanupApp()
        {
            Constructing();
            PageSize = 15;
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of the object.
        /// </summary>
        partial void Constructing();
        /// <summary>
        /// This method is called after the object is constructed.
        /// </summary>
        partial void Constructed();
        #endregion Instance-Constructors

        #region properties
        /// <summary>
        /// The folders to be dropped during cleanup.
        /// </summary>
        private static readonly string[] DropFolderNames = [
            "bin",
            "obj",
            "target",
            "node_modules",
        ];
        /// <summary>
        /// The folders to be dropped during cleanup.
        /// </summary>
        private static readonly string[] DropFolders = DropFolderNames.Select(n => $"{Path.DirectorySeparatorChar}{n}").ToArray();
        /// <summary>
        /// Gets or sets the path where the drop will be performed.
        /// </summary>
        public string CleanupPath { get; private set; } = SolutionPath;
        #endregion properties

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
                    Text = ToLabelText("Path", "Change drop path"),
                    Action = (self) =>
                    {
                        var parentPath = Directory.GetParent(CleanupPath)?.FullName!;

                        CleanupPath = SelectOrChangeToSubPath(parentPath, MaxSubPathDepth);
                    },
                },

                new()
                {
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
            };

            var executionPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var subPaths = TemplatePath.GetSubPaths(CleanupPath, 1)
                                       .Where(p => DropFolderNames.Any(x => ContainsFolder(p, x))
                                                && executionPath!.StartsWith(p) == false)
                                       .ToArray();

            menuItems.AddRange(CreatePageMenuItems(ref mnuIdx, subPaths, (item, menuItem) => 
            {
                menuItem.OptionalKey = "a";
                menuItem.Text = ToLabelText("Cleanup", $"{item.Replace(CleanupPath, ".")}");
                menuItem.Action = (self) => 
                {
                    var path = self.Params["path"]?.ToString() ?? string.Empty;

                    StartProgressBar();
                    TemplatePath.CleanDirectories(path, DropFolders);
                };
                menuItem.Params = new() { { "path", item } };
            }));

            return [.. menuItems.Union(CreateExitMenuItems())];
        }
        /// <summary>
        /// Prints the header for the PlantUML application.
        /// </summary>
        protected override void PrintHeader()
        {
            List<KeyValuePair<string, object>> headerParams =
            [
                new("Drop folders:", string.Join(", ", DropFolderNames)),
                new("Cleanup path:", CleanupPath),
            ];

            base.PrintHeader("Template Cleanup Directories", [.. headerParams]);
        }
        #endregion overrides

        #region app methods
        public static bool ContainsFolder(string directoryPath, string folderName)
        {
            if (!Directory.Exists(directoryPath))
                return false;

            string checkPath = Path.Combine(directoryPath, folderName);

            return Directory.Exists(checkPath);
        }
        /// <summary>
        /// Cleans the specified directories by deleting all files and subdirectories within them.
        /// </summary>
        private void CleanDirectories() => TemplatePath.CleanDirectories(CleanupPath, DropFolders);
        #endregion app methods
    }
}

