//@BaseCode

using System.Text;

namespace TemplateTools.Logic.Preprocessor
{
    /// <summary>
    /// Provides utility methods for working with project files and defines.
    /// </summary>
    public static partial class ProjectFile
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ProjectFile"/> class.
        /// </summary>
        static ProjectFile()
        {
            ClassConstructing();
            Defines =
            [
                "ACCOUNT_OFF",
                "IDINT_OFF",
                "IDLONG_OFF",
                "IDGUID_OFF",
                "ROWVERSION_OFF",
                "EXTERNALGUID_OFF",
                "POSTGRES_OFF",
                "SQLSERVER_OFF",
                "SQLITE_OFF",
                "DOCKER_OFF",
                "DEVELOP_OFF",
                "DBOPERATION_OFF",
                $"{GeneratedCodePrefix}_OFF",
            ];
            ClassConstructed();
        }
        /// <summary>
        /// Represents a partial method that is called when the class is being constructed.
        /// </summary>
        /// <remarks>
        /// This method is called before the constructor of the class is executed.
        /// It can be implemented in a partial class to provide additional logic during class construction.
        /// </remarks>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called after the constructor of the class is executed.
        /// It is used to perform any additional initialization tasks.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors
        
        /// <summary>
        /// Gets the postfix value for On operations.
        /// </summary>
        /// <value>
        /// The "_ON" postfix value.
        /// </value>
        public static string OnPostfix => "_ON";
        /// <summary>
        /// Gets the postfix value used when the property is turned off.
        /// </summary>
        /// <value>
        /// The postfix value that is appended to the property value when it is turned off.
        /// </value>
        public static string OffPostfix => "_OFF";
        /// <summary>
        /// Gets the prefix used for generated code.
        /// </summary>
        /// <value>
        /// The prefix used for generated code.
        /// </value>
        public static string GeneratedCodePrefix => "GENERATEDCODE";
        /// <summary>
        /// Gets or sets an array of strings representing the defines.
        /// </summary>
        /// <value>
        /// An array of strings representing the defines.
        /// </value>
        public static string[] Defines { get; set; }
        
        /// <summary>
        /// Switches the defined value at the specified index within the given array of defines.
        /// </summary>
        /// <param name="defines">The array of defines.</param>
        /// <param name="idx">The index of the defined value to switch.</param>
        /// <remarks>
        /// The method only performs the switch if the index is within the bounds of the array.
        /// If the defined value at the specified index ends with the <see cref="OnPostfix"/> string,
        /// it will be replaced with the <see cref="OffPostfix"/> string.
        /// If the defined value at the specified index ends with the <see cref="OffPostfix"/> string,
        /// it will be replaced with the <see cref="OnPostfix"/> string.
        /// </remarks>
        public static void SwitchDefine(string[] defines, int idx)
        {
            if (idx >= 0 && idx < defines.Length)
            {
                if (defines[idx].EndsWith(OnPostfix))
                {
                    defines[idx] = defines[idx].Replace(OnPostfix, OffPostfix);
                }
                else if (defines[idx].EndsWith(OffPostfix))
                {
                    defines[idx] = defines[idx].Replace(OffPostfix, OnPostfix);
                }
            }
        }
        /// <summary>
        /// Switches the define strings in the given array with the specified prefix and postfix.
        /// </summary>
        /// <param name="defines">The array of define strings.</param>
        /// <param name="definePrefix">The prefix used for identifying the define strings to be switched.</param>
        /// <param name="definePostfix">The postfix to be added to the switched define strings.</param>
        public static void SwitchDefine(string[] defines, string definePrefix, string definePostfix)
        {
            bool hasSet = false;
            
            for (int i = 0; i < defines.Length && hasSet == false; i++)
            {
                if (defines[i].StartsWith(definePrefix))
                {
                    hasSet = true;
                    defines[i] = $"{definePrefix}{definePostfix}";
                }
            }
        }
        
        /// <summary>
        /// Reads the defined constants in a project file.
        /// </summary>
        /// <param name="filePath">The path of the project file.</param>
        /// <param name="startDefines">The starting defines to include in the result.</param>
        /// <returns>An array containing the defined constants.</returns>
        private static string[] ReadDefinesInProjectFile(string filePath, params string[] startDefines)
        {
            var result = new List<string>();
            
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath, Encoding.Default);
                
                foreach (var line in lines)
                {
                    var defines = line.ExtractBetween("<DefineConstants>", "</DefineConstants>");
                    
                    if (defines.HasContent())
                    {
                        defines.Split(";", StringSplitOptions.RemoveEmptyEntries)
                               .ToList()
                               .ForEach(e =>
                        {
                            var tmp = result.FirstOrDefault(x => x.RemoveAll("OFF", "ON") == e.RemoveAll("OFF", "ON"));
                            
                            if (string.IsNullOrEmpty(tmp))
                            {
                                result.Add(e);
                            }
                        });
                    }
                }
            }
            
            foreach (var startDefine in startDefines)
            {
                var tmp = result.FirstOrDefault(x => x.RemoveAll("OFF", "ON") == startDefine.RemoveAll("OFF", "ON"));
                
                if (string.IsNullOrEmpty(tmp))
                {
                    result.Add(startDefine);
                }
            }
            return [.. result];
        }
        /// <summary>
        /// Reads defines in project files.
        /// </summary>
        /// <param name="path">The path to the project files.</param>
        /// <returns>An array of strings containing the defines.</returns>
        public static string[] ReadDefinesInProjectFiles(string path)
        {
            return ReadDefinesInProjectFiles(path, Defines);
        }
        /// <summary>
        /// Reads the defines in project files and returns an array of strings.
        /// </summary>
        /// <param name="path">The path to the directory containing the project files.</param>
        /// <param name="startDefines">An array of strings representing the starting defines.</param>
        /// <returns>An array of strings containing the unique defines found in the project files.</returns>
        /// <remarks>
        /// The method searches for .csproj files in the specified directory and all its subdirectories, excluding files that contain ".AngularApp" in their name.
        /// It then reads the defines in each project file and adds them to the result list after removing any occurrences of "OFF" or "ON" from the defines.
        /// The method also checks if the startDefines already exist in the result list before adding them.
        /// The final result is an array of unique defines found in the project files, including the startDefines if they are not already present.
        /// </remarks>
        public static string[] ReadDefinesInProjectFiles(string path, string[] startDefines)
        {
            var result = new List<string>();
            var files = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories)
                                 .Where(f => f.Contains(".AngularApp") == false)
                                 .OrderBy(f => f);
            
            foreach (var file in files)
            {
                var defines = ReadDefinesInProjectFile(file);
                
                defines.ToList()
                       .ForEach(e =>
                {
                    var tmp = result.FirstOrDefault(x => x.RemoveAll("OFF", "ON") == e.RemoveAll("OFF", "ON"));
                    
                    if (string.IsNullOrEmpty(tmp))
                    {
                        result.Add(e);
                    }
                });
            }
            
            foreach (var startDefine in startDefines)
            {
                var tmp = result.FirstOrDefault(x => x.RemoveAll("OFF", "ON") == startDefine.RemoveAll("OFF", "ON"));
                
                if (string.IsNullOrEmpty(tmp))
                {
                    result.Add(startDefine);
                }
            }
            return [.. result];
        }
        
        /// <summary>
        /// Writes the provided defines to the project file at the specified file path.
        /// </summary>
        /// <param name="filePath">The path of the project file to write defines into.</param>
        /// <param name="defines">The array of defines to write into the project file.</param>
        /// <remarks>
        /// This method reads the project file at the given path, searches for the <DefineConstants> element,
        /// and replaces its value with the provided defines. If the <DefineConstants> element does not exist,
        /// a new <PropertyGroup> element is created and inserted before the </PropertyGroup> tag in the project file,
        /// containing the provided defines.
        /// </remarks>
        private static void WriteDefinesInProjectFile(string filePath, string[] defines)
        {
            var defineConstants = string.Join(";", defines);
            
            if (File.Exists(filePath))
            {
                var hasChanged = false;
                var result = new List<string>();
                var lines = File.ReadAllLines(filePath, Encoding.Default);
                
                foreach (var line in lines)
                {
                    if (line.Contains("<DefineConstants>", "</DefineConstants>"))
                    {
                        hasChanged = true;
                        result.Add(line.ReplaceBetween("<DefineConstants>", "</DefineConstants>", defineConstants));
                    }
                    else
                    {
                        result.Add(line);
                    }
                }
                if (hasChanged == false && defineConstants.Length > 0)
                {
                    var insertIdx = result.FindIndex(e => e.Contains("</PropertyGroup>"));
                    
                    insertIdx = insertIdx < 0 ? result.Count - 2 : insertIdx;
                    hasChanged = true;
                    
                    result.InsertRange(insertIdx + 1,
                    [
                        string.Empty,
                        "  <PropertyGroup>",
                        $"    <DefineConstants>{defineConstants}</DefineConstants>",
                        "  </PropertyGroup>",
                    ]);
                }
                if (hasChanged)
                {
                    File.WriteAllLines(filePath, [.. result], Encoding.Default);
                }
            }
        }
        /// <summary>
        /// Writes the provided defines in all project files found within the specified path.
        /// </summary>
        /// <param name="path">The path to search for project files.</param>
        /// <param name="defines">The array of defines to be written.</param>
        /// <returns>None</returns>
        public static void WriteDefinesInProjectFiles(string path, string[] defines)
        {
            var files = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                WriteDefinesInProjectFile(file, defines);
            }
        }
    }
}

