//@BaseCode

using System.Diagnostics;
using TemplateTools.Logic;
using TemplateTools.Logic.Git;

namespace TemplateTools.ConApp.Apps
{
    /// <summary>
    /// Represents the CodeGeneratorApp class.
    /// </summary>
    internal partial class CodeGeneratorApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the CodeGeneratorApp class.
        /// </summary>
        static CodeGeneratorApp()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called when the class is being constructed.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        /// <remarks>
        /// This method is called internally and is intended for internal use only.
        /// </remarks>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region Instance-Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGeneratorApp"/> class.
        /// </summary>
        public CodeGeneratorApp()
        {
            Constructing();
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
        /// Gets or sets the value indicating whether the file should be grouped.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the file should be grouped; otherwise, <c>false</c>.
        /// </value>
        private bool WriteToGroupFile { get; set; } = false;
        /// <summary>
        /// Gets or sets the value indicating whether the file should be added info header.
        /// </summary>
        private bool WriteInfoHeader { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether the empty folders in the source path will be deleted.
        /// </summary>
        private bool IncludeCleanDirectory { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether generated files should be excluded from GIT.
        /// </summary>
        /// <remarks>
        /// By default, generated files are included in the GIT repository.
        /// However, setting this property to <c>true</c> will exclude generated files from being tracked by  GIT.
        /// </remarks>
        /// <value>
        /// <c>true</c> to exclude generated files from GIT; otherwise, <c>false</c> to include generated files in GIT.
        /// </value>
        private bool ExcludeGeneratedFilesFromGIT { get; set; } = true;
        /// <summary>
        /// Gets or sets the path of the solution.
        /// </summary>
        private string CodeSolutionPath { get; set; } = SolutionPath;
        #endregion properties

        #region overrides
        /// <summary>
        /// Creates an array of menu items for the application menu.
        /// </summary>
        /// <returns>An array of MenuItem objects representing the menu items.</returns>
        protected override MenuItem[] CreateMenuItems()
        {
            var mnuIdx = 0;
            var menuItems = new MenuItem[]
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
                    Text = ToLabelText("Generation file", "Change generation file option"),
                    Action = (self) => WriteToGroupFile = !WriteToGroupFile
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Add info header", "Change add info header option"),
                    Action = (self) => WriteInfoHeader = !WriteInfoHeader
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Delete folders", "Change delete empty folders option"),
                    Action = (self) => IncludeCleanDirectory = !IncludeCleanDirectory
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Exclude files", "Change the exclusion of generated files from GIT"),
                    Action = (self) => ExcludeGeneratedFilesFromGIT = !ExcludeGeneratedFilesFromGIT
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Source path", "Change the source solution path"),
                    Action = (self) => 
                    {
                        var result = ChangeTemplateSolutionPath(CodeSolutionPath, MaxSubPathDepth, ReposPath);

                        if (result.HasContent())
                        {
                            CodeSolutionPath = result;
                        }
                    }
                },
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
                    Text = ToLabelText("Compile", "Compile logic project"),
                    Action = (self) => CompileProject(),
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text =  ToLabelText("Delete files", "Delete generated files"),
                    Action = (self) => DeleteGeneratedFiles(),
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text =  ToLabelText("Delete folders", "Delete empty folders in the path"),
                    Action = (self) => DeleteEmptyFolders(),
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text =  ToLabelText("Start", "Start code generation"),
                    Action = (self) => StartCodeGeneration(),
                },
            };
            return [.. menuItems.Union(CreateExitMenuItems())];
        }
        /// <summary>
        /// Prints the header for the PlantUML application.
        /// </summary>
        protected override void PrintHeader()
        {
            List<KeyValuePair<string, object>> headerParams =
            [
                new("Solution path:", CodeSolutionPath),
                new(new string('-', 33), ""),
                new("Write generated source into:", WriteToGroupFile ? "Group files" : "Single files"),
                new("Write info header into source:", WriteInfoHeader),
                new("Delete empty folders in the path:", IncludeCleanDirectory),
                new("Exclude generated files from GIT:", ExcludeGeneratedFilesFromGIT),
            ];

            base.PrintHeader("Template Code Generator", [.. headerParams]);
        }
        #endregion overrides

        #region app methods
        /// <summary>
        ///     Compiles the project.
        /// </summary>
        /// <remarks>
        ///     This method executes the build process for the project specified in the <see cref="CodeSolutionPath"/> and displays a progress bar while the build is in progress.
        ///     After the build is completed, the user is prompted to press any key to continue.
        /// </remarks>
        private void CompileProject()
        {
            var solutionProperties = SolutionProperties.Create(CodeSolutionPath);

            PrintHeader();
            StartProgressBar();
            PrintLine("Compile project...");
            ExecuteBuildProject(solutionProperties);
            StopProgressBar();
            Print("Press enter...");
            ReadLine();
        }

        /// <summary>
        /// Deletes all generated files and directories from the solution path.
        /// </summary>
        private void DeleteGeneratedFiles()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Delete all generated files...");
            Generator.DeleteGeneratedFiles(CodeSolutionPath);
            if (IncludeCleanDirectory)
            {
                PrintLine("Delete all empty folders...");
                Generator.CleanDirectories(CodeSolutionPath);
            }
            PrintLine("Delete all generated files ignored from git...");
            GitIgnoreManager.DeleteIgnoreEntries(CodeSolutionPath);
            StopProgressBar();
        }

        /// <summary>
        /// Deletes all empty folders within the specified solution path.
        /// </summary>
        /// <remarks>
        /// This method invokes the <see cref="ProgressBar.Start"/> method to display a progress bar
        /// and writes a message to the console indicating that all empty folders are being deleted.
        /// It then calls the <see cref="Generator.CleanDirectories(string)"/> method to perform the deletion.
        /// </remarks>
        private void DeleteEmptyFolders()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Delete all empty folders...");
            Generator.CleanDirectories(CodeSolutionPath);
            StopProgressBar();
        }

        /// <summary>
        /// Starts the code generation process.
        /// </summary>
        /// <remarks>
        /// This method executes the necessary commands to generate code based on the specified solution properties.
        /// </remarks>
        /// <seealso cref="SolutionProperties"/>
        /// <seealso cref="ExecuteBuildProject(SolutionProperties)"/>
        /// <seealso cref="ExecuteRunProject(SolutionProperties, string)"/>
        private void StartCodeGeneration()
        {
            var command = string.Empty;
            var solutionProperties = SolutionProperties.Create(CodeSolutionPath);

            PrintHeader();
            PrintLine("Start code generation...");
            ExecuteBuildProject(solutionProperties);

            if (WriteToGroupFile)
            {
                command = "1 " + command;
            }
            if (!WriteInfoHeader)
            {
                command = "2 " + command;
            }

            if (IncludeCleanDirectory == false)
            {
                command = "3 " + command;
            }

            if (ExcludeGeneratedFilesFromGIT == false)
            {
                command = "4 " + command;
            }
            PrintHeader();
            PrintLine("Start code generation...");
            ExecuteRunProject(SolutionProperties.Create(CodeSolutionPath), command + "7"); // Start code generation 7
        }
        #endregion app methods
        /// <summary>
        /// Executes the build process for the specified solution.
        /// </summary>
        /// <param name="solutionProperties">The SolutionProperties object containing the necessary information for the build process.</param>
        /// <returns>The path where the solution was compiled.</returns>
        private static string ExecuteBuildProject(SolutionProperties solutionProperties)
        {
            var counter = 0;
            var maxWaiting = 10 * 60 * 1000;    // 10 minutes
            var startCompilePath = Path.Combine(Path.GetTempPath(), solutionProperties.SolutionName);
            var compilePath = startCompilePath;
            bool deleteError;

            do
            {
                deleteError = false;
                if (Directory.Exists(compilePath))
                {
                    try
                    {
                        Directory.Delete(compilePath, true);
                    }
                    catch
                    {
                        deleteError = true;
                        compilePath = $"{startCompilePath}{++counter}";
                    }
                }
            } while (deleteError != false);

            var arguments = $"build \"{solutionProperties.LogicCSProjectFilePath}\" -c Release -o \"{compilePath}\"";

            PrintLine($"dotnet {arguments}");

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var csprojStartInfo = new ProcessStartInfo("dotnet")
                {
                    Arguments = arguments,
                    UseShellExecute = false
                };
                Process.Start(csprojStartInfo)?.WaitForExit(maxWaiting);
                solutionProperties.CompilePath = compilePath;
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                var csprojStartInfo = new ProcessStartInfo("dotnet")
                {
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                };
                Process.Start(csprojStartInfo)?.WaitForExit(maxWaiting);
                solutionProperties.CompilePath = compilePath;
            }
            return compilePath;
        }
        /// <summary>
        /// Executes the run project for the given solution properties and execute arguments.
        /// </summary>
        /// <param name="solutionProperties">The solution properties.</param>
        /// <param name="executeArgs">The execute arguments.</param>
        /// <returns>The project path.</returns>
        private string ExecuteRunProject(SolutionProperties solutionProperties, string executeArgs)
        {
            var maxWaiting = 10 * 60 * 1000;    // 10 minutes
            var projectPath = $"{solutionProperties.SolutionName}.CodeGenApp{Path.DirectorySeparatorChar}{solutionProperties.SolutionName}.CodeGenApp.csproj";
            var arguments = $"run --project \"{solutionProperties.SolutionPath}{Path.DirectorySeparatorChar}{projectPath}\" {executeArgs}";

            PrintHeader();
            PrintLine($"dotnet {arguments}");

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var csprojStartInfo = new ProcessStartInfo("dotnet")
                {
                    Arguments = arguments,
                    UseShellExecute = false
                };
                Process.Start(csprojStartInfo)?.WaitForExit(maxWaiting);
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                var csprojStartInfo = new ProcessStartInfo("dotnet")
                {
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                };
                Process.Start(csprojStartInfo)?.WaitForExit(maxWaiting);
            }
            return projectPath;
        }
    }
}

