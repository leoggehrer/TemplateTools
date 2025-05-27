//@BaseCode

using Microsoft.CodeAnalysis;
using CodeGenPreprocessor = TemplateTools.Logic.Preprocessor;

namespace TemplateTools.ConApp.Apps
{
    /// <summary>
    /// Represents an application for preprocessing templates.
    /// </summary>
    public partial class PreprocessorApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Represents the PreprocessorApp class.
        /// </summary>
        /// <summary>
        /// Initializes a new instance of the <see cref="PreprocessorApp"/> class.
        /// </summary>
        static PreprocessorApp()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called before the constructor of the class is executed.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region properties
        /// <summary>
        /// Gets or sets the array of defines.
        /// </summary>
        private string[] Defines { get; set; } = [];
        /// <summary>
        /// Gets or sets a value indicating whether the defines have changed.
        /// </summary>
        private bool ChangedDefines { get; set; } = false;
        /// <summary>
        /// Gets or sets the path of the solution.
        /// </summary>
        private string PreprocessorSolutionPath { get; set; } = SolutionPath;
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
                    Text = ToLabelText("Path", "Change preprocessor solution path"),
                    Action = (self) => PreprocessorSolutionPath = ChangeTemplateSolutionPath(PreprocessorSolutionPath, MaxSubPathDepth),
                },

                new()
                {
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
            };

            Defines = CodeGenPreprocessor.ProjectFile.ReadDefinesInProjectFiles(PreprocessorSolutionPath);

            for (int idx = 0; idx < Defines.Length; idx++)
            {
                var define = Defines[idx];
                var text = $"Set definition {define}";
                var description = string.Empty;
                var foreColor = ConsoleColor.Green;

                if (define.EndsWith("_ON"))
                {
                    description = $" ==> {define.Replace("_ON", "_OFF")}";
                    foreColor = ConsoleColor.Green;
                }
                else
                {
                    description = $" ==> {define.Replace("_OFF", "_ON")}";
                    foreColor = ConsoleColor.Yellow;
                }

                if (define.StartsWith("IDINT_") || define.StartsWith("ROWVERSION_") || define.StartsWith("POSTGRES_") || define.StartsWith("DOCKER_"))
                {
                    menuItems.Add(new()
                    {
                        Key = "---",
                        Text = new string('-', 65),
                        Action = (self) => { },
                        ForegroundColor = ConsoleColor.DarkGray,
                    });
                }

                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "a",
                    Text = ToLabelText(text, description, 40, ' '),
                    Action = (self) => 
                    {
                        var i = Convert.ToInt32(self.Params["idx"]);

                        if (SwitchDefine(Defines, i))
                        {
                            ChangedDefines = true;
                        }
                    },
                    ForegroundColor = foreColor,
                    Params = new() { { "idx", idx } },
                });
            }

            menuItems.Add(new()
            {
                Key = "---",
                Text = new string('-', 65),
                Action = (self) => { },
                ForegroundColor = ConsoleColor.DarkGreen,
            });
            menuItems.Add(new()
            {
                Key = $"{++mnuIdx}",
                Text = ToLabelText("Start", "Start assignment process"),
                Action = (self) => 
                {
                    ChangedDefines = false;
                    SettingDefines();
                },
            });
            return [.. menuItems.Union(CreateExitMenuItems())];
        }

        /// <summary>
        /// Performs additional actions after the execution of the method.
        /// If the <see cref="ChangedDefines"/> flag is set, writes the defines in project files and sets preprocessor define comments in files.
        /// </summary>
        protected override void AfterExecution()
        {
            if (ChangedDefines)
            {
                ChangedDefines = false;
                SettingDefines();
            }
            base.AfterExecution();
        }
        /// <summary>
        /// Prints the header for the PlantUML application.
        /// </summary>
        /// <param name="sourcePath">The path of the solution.</param>
        protected override void PrintHeader()
        {
            List<KeyValuePair<string, object>> headerParams = [new("Solution path:", PreprocessorSolutionPath)];

            base.PrintHeader("Template Setting Defines", [.. headerParams]);
        }
        #endregion overrides

        #region app methods
        /// <summary>
        /// Sets the defines in project files and sets define comments in files.
        /// </summary>
        private void SettingDefines()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Setting defines in project files...");
            CodeGenPreprocessor.ProjectFile.WriteDefinesInProjectFiles(PreprocessorSolutionPath, Defines);
            PrintLine("Setting defines comments in files...");
            CodeGenPreprocessor.PreprocessorCommentHelper.SetPreprocessorDefineCommentsInFiles(PreprocessorSolutionPath, Defines);
            StopProgressBar();
        }
        /// <summary>
        /// Switches the value of a define in the given array based on the specified index.
        /// </summary>
        /// <param name="defines">The array of defines.</param>
        /// <param name="idx">The index of the define to switch.</param>
        /// <returns>True if the define was switched successfully, false otherwise.</returns>
        private static bool SwitchDefine(string[] defines, int idx)
        {
            bool result = false;

            if (idx >= 0 && idx < defines.Length)
            {
                if (defines[idx].EndsWith("_ON"))
                {
                    if (defines[idx].StartsWith("IDINT_") == false
                        && defines[idx].StartsWith("IDLONG_") == false
                        && defines[idx].StartsWith("IDGUID_") == false
                        && defines[idx].StartsWith("POSTGRES_") == false
                        && defines[idx].StartsWith("SQLSERVER_") == false
                        && defines[idx].StartsWith("SQLITE_") == false)
                    {
                        defines[idx] = defines[idx].Replace("_ON", "_OFF");
                    }
                }
                else
                {
                    if (defines[idx].StartsWith("IDINT_") == true)
                    {
                        SwitchDefine(defines, "IDINT_", "ON");
                        SwitchDefine(defines, "IDLONG_", "OFF");
                        SwitchDefine(defines, "IDGUID_", "OFF");
                    }
                    else if (defines[idx].StartsWith("IDLONG_") == true)
                    {
                        SwitchDefine(defines, "IDINT_", "OFF");
                        SwitchDefine(defines, "IDLONG_", "ON");
                        SwitchDefine(defines, "IDGUID_", "OFF");
                    }
                    else if (defines[idx].StartsWith("IDGUID_") == true)
                    {
                        SwitchDefine(defines, "IDINT_", "OFF");
                        SwitchDefine(defines, "IDLONG_", "OFF");
                        SwitchDefine(defines, "IDGUID_", "ON");
                    }
                    else if (defines[idx].StartsWith("POSTGRES_") == true)
                    {
                        SwitchDefine(defines, "POSTGRES_", "ON");
                        SwitchDefine(defines, "SQLSERVER_", "OFF");
                        SwitchDefine(defines, "SQLITE_", "OFF");
                    }
                    else if (defines[idx].StartsWith("SQLSERVER_") == true)
                    {
                        SwitchDefine(defines, "POSTGRES_", "OFF");
                        SwitchDefine(defines, "SQLSERVER_", "ON");
                        SwitchDefine(defines, "SQLITE_", "OFF");
                    }
                    else if (defines[idx].StartsWith("SQLITE_") == true)
                    {
                        SwitchDefine(defines, "POSTGRES_", "OFF");
                        SwitchDefine(defines, "SQLSERVER_", "OFF");
                        SwitchDefine(defines, "SQLITE_", "ON");
                    }
                    else
                    {
                        defines[idx] = defines[idx].Replace("_OFF", "_ON");
                    }
                }
                result = true;
            }
            return result;
        }
        /// <summary>
        /// Switches the defines in the project file using the specified prefix and postfix.
        /// </summary>
        /// <param name="defines">An array of strings that represents the defines to be switched.</param>
        /// <param name="definePrefix">A string that represents the prefix to be added to the defines.</param>
        /// <param name="definePostfix">A string that represents the postfix to be added to the defines.</param>
        private static void SwitchDefine(string[] defines, string definePrefix, string definePostfix)
        {
            CodeGenPreprocessor.ProjectFile.SwitchDefine(defines, definePrefix, definePostfix);
        }
        #endregion app methods
    }
}

