//@BaseCode
namespace TemplateTools.Logic.Generation
{
    using System.Text;
    /// <summary>
    /// A utility class for handling file-related operations.
    /// </summary>
    internal class FileHandler
    {
        /// <summary>
        /// Dettermine if the file is a TypeScript or C# file.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        /// <returns>True if the file is a TypeScript or C# file, false otherwise.</returns>
        public static bool IsCustomizeableFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            return extension.Equals(".ts") || extension.Equals(".cs");
        }
        /// <summary>
        /// Creates a custom file path based on the given file path.
        /// </summary>
        /// <param name="filePath">The original file path.</param>
        /// <returns>The custom file path.</returns>
        public static string CreateCustomFilePath(string filePath)
        {
            var path = Path.GetDirectoryName(filePath);
            var customFileName = $"{Path.GetFileNameWithoutExtension(filePath)}{StaticLiterals.CustomFileExtension}";
            
            return Path.Combine(path!, customFileName);
        }
        
        /// <summary>
        /// Reads angular custom parts from a specified file.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        /// <returns>An enumerable collection of string containing the angular custom parts.</returns>
        public static IEnumerable<string> ReadCustomParts(string filePath)
        {
            var result = new List<string>();
            var imports = ReadCustomImports(filePath).Where(l => string.IsNullOrEmpty(l.Trim()) == false);
            var code = ReadCustomCode(filePath).Where(l => string.IsNullOrEmpty(l.Trim()) == false);
            
            if (imports.Any())
            {
                result.Add(StaticLiterals.CustomImportBeginLabel);
                result.AddRange(imports);
                result.Add(StaticLiterals.CustomImportEndLabel);
            }
            
            if (code.Any())
            {
                result.Add(StaticLiterals.CustomCodeBeginLabel);
                result.AddRange(code);
                result.Add(StaticLiterals.CustomCodeEndLabel);
            }
            
            return result;
        }
        /// <summary>
        /// Reads the custom imports from an Angular file.
        /// </summary>
        /// <param name="filePath">The file path to read the custom imports from.</param>
        /// <returns>An enumerable collection of custom imports as strings.</returns>
        public static IEnumerable<string> ReadCustomImports(string filePath)
        {
            var result = new List<string>();
            
            result.AddRange(ReadCustomPart(filePath, StaticLiterals.CustomImportBeginLabel, StaticLiterals.CustomImportEndLabel));
            
            return result;
        }
        /// <summary>
        /// Reads the custom code from the specified file path.
        /// </summary>
        /// <param name="filePath">The file path of the file to read.</param>
        /// <returns>An enumerable collection of strings representing the custom code read from the file.</returns>
        public static IEnumerable<string> ReadCustomCode(string filePath)
        {
            var result = new List<string>();
            
            result.AddRange(ReadCustomPart(filePath, StaticLiterals.CustomCodeBeginLabel, StaticLiterals.CustomCodeEndLabel));
            
            return result;
        }
        /// <summary>
        /// Reads a custom part from a file.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        /// <param name="beginLabel">The starting label indicating the beginning of the custom part.</param>
        /// <param name="endLabel">The ending label indicating the end of the custom part.</param>
        /// <returns>An enumerable collection of strings representing the lines of text within the custom part of the file.</returns>
        public static IEnumerable<string> ReadCustomPart(string filePath, string beginLabel, string endLabel)
        {
            var result = new List<string>();
            
            if (File.Exists(filePath))
            {
                var source = File.ReadAllText(filePath, Encoding.Default);
                
                foreach (var item in source.GetAllTags(new string[] { $"{beginLabel}{Environment.NewLine}", $"{endLabel}" })
                                           .OrderBy(e => e.StartTagIndex))
                {
                    if (item.InnerText.HasContent())
                    {
                        result.AddRange(item.InnerText.ToLines().Where(l => l.HasContent()));
                    }
                }
            }
            return result;
        }
        
        /// <summary>
        /// Saves the Angular custom parts to a specified file path.
        /// </summary>
        /// <param name="filePath">The file path to save the custom parts at.</param>
        /// <returns>The file path where the custom parts were saved.</returns>
        public static string SaveCustomParts(string filePath)
        {
            var result = CreateCustomFilePath(filePath);
            var lines = ReadCustomParts(filePath);
            
            if (File.Exists(result))
            {
                File.Delete(result);
            }
            if (lines.Any())
            {
                File.WriteAllLines(result, lines, Encoding.UTF8);
            }
            return result;
        }
        /// <summary>
        /// Reads the contents of a file specified by the given file path, adds each line of the file to a list, deletes the file, and returns the list of lines.
        /// </summary>
        /// <param name="filePath">The file path of the file to be read and deleted.</param>
        /// <returns>A list of strings containing each line from the file.</returns>
        public static IEnumerable<string> ReadAndDelete(string filePath)
        {
            var result = new List<string>();
            
            if (File.Exists(filePath))
            {
                result.AddRange(File.ReadAllLines(filePath, Encoding.UTF8));
                File.Delete(filePath);
            }
            return result;
        }
    }
}

