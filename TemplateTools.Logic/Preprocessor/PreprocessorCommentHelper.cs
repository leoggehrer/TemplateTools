//@BaseCode
using System.Text;

namespace TemplateTools.Logic.Preprocessor
{
    /// <summary>
    /// Helper class for modifying files to set preprocessor defines and comments.
    /// </summary>
    public class PreprocessorCommentHelper
    {
        /// <summary>
        /// Sets preprocessor define comments in files.
        /// </summary>
        /// <param name="path">The path specifying the directory where the files are located.</param>
        /// <param name="defineItems">An array of strings representing the preprocessor define items.</param>
        /// <remarks>
        /// For each preprocessor define item, sets preprocessor define comments in files with the specified path.
        /// The method applies to files with the extensions "*.cshtml" and "*.razor" by calling SetPreprocessorDefineCommentsInFiles.
        /// It also applies to files with the extension "*.json" by calling SetPreprocessorDefineBlockCommentsInFiles.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">Thrown when path or defineItems is null.</exception>
        public static void SetPreprocessorDefineCommentsInFiles(string path, params string[] defineItems)
        {
            foreach (var define in defineItems.Select(d => d.ToUpper()))
            {
                SetPreprocessorDefineCommentsInFiles(path, define, "*.cshtml", "*.razor");
                SetPreprocessorDefineBlockCommentsInFiles(path, define, "*.json");
            }
        }
        ///<summary>
        /// Sets preprocessor define comments in files.
        ///</summary>
        ///<param name="path">The path where the files are located.</param>
        ///<param name="define">The preprocessor define to search for.</param>
        ///<param name="searchPatterns">An array of search patterns used to filter the files.</param>
        public static void SetPreprocessorDefineCommentsInFiles(string path, string define, params string[] searchPatterns)
        {
            var files = searchPatterns.SelectMany(searchPattern => Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
                                      .Where(f => CommonStaticLiterals.IgnoreSubFolders.Any(i => f.Contains(i, StringComparison.CurrentCultureIgnoreCase)) == false)
                                      .ToArray();
            var searchIfStart_1 = $"@*#if {define}*@@*{Environment.NewLine}";
            var searchIfEnd_1 = "*@@*#endif*@";
            var replaceIfStart_1 = $"@*#if {define}*@{Environment.NewLine}";
            var replaceIfEnd_1 = "@*#endif*@";
            
            var searchIfStart_2 = define.EndsWith("_ON") ? $"@*#if {define.Replace("_ON", "_OFF")}*@{Environment.NewLine}"
                                                         : $"@*#if {define.Replace("_OFF", "_ON")}*@{Environment.NewLine}";
            var searchIfEnd_2 = "@*#endif*@";
            var replaceIfStart_2 = define.EndsWith("_ON") ? $"@*#if {define.Replace("_ON", "_OFF")}*@@*{Environment.NewLine}"
                                                          : $"@*#if {define.Replace("_OFF", "_ON")}*@@*{Environment.NewLine}";
            var replaceIfEnd_2 = "*@@*#endif*@";
            var searchTags = new[] { searchIfStart_1, searchIfEnd_1, searchIfStart_2, searchIfEnd_2 };
            var replaceTags = new[] { replaceIfStart_1, replaceIfEnd_1, replaceIfStart_2, replaceIfEnd_2 };
            
            foreach (var file in files)
            {
                var hasChanged = false;
                var text = File.ReadAllText(file, Encoding.Default);
                
                for (var i = 0; i < searchTags.Length; i += 2)
                {
                    var startIndex = 0;
                    var replaceText = string.Empty;
                    
                    foreach (var tag in text.GetAllTags(searchTags[i], searchTags[i + 1]))
                    {
                        hasChanged = true;
                        
                        if (tag.StartTagIndex >= startIndex)
                        {
                            replaceText += text.Partialstring(startIndex, tag.StartTagIndex - 1);
                            replaceText += replaceTags[i];
                            replaceText += tag.InnerText;
                            replaceText += replaceTags[i + 1];
                            
                            startIndex += tag.EndTagIndex + tag.EndTag.Length;
                        }
                    }
                    if (hasChanged && startIndex < text.Length)
                    {
                        replaceText += text.Partialstring(startIndex, text.Length);
                    }
                    text = replaceText.HasContent() ? replaceText : text;
                }
                if (hasChanged)
                {
                    File.WriteAllText(file, text, Encoding.Default);
                }
            }
        }
        /// <summary>
        /// Modifies files to set preprocessor defines with block comments, based on specified search patterns.
        /// </summary>
        /// <param name="path">The directory path to search for files.</param>
        /// <param name="define">The preprocessor define to search and replace within files.</param>
        /// <param name="searchPatterns">An array of search patterns to match files in the specified directory.</param>
        public static void SetPreprocessorDefineBlockCommentsInFiles(string path, string define, params string[] searchPatterns)
        {
            var files = searchPatterns.SelectMany(searchPattern => Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
                                      .Where(f => CommonStaticLiterals.IgnoreSubFolders.Any(i => f.Contains(i, StringComparison.CurrentCultureIgnoreCase)) == false)
                                      .ToArray();
            
            var searchIfStart_1 = $"/*#if {define}*//*{Environment.NewLine}";
            var searchIfEnd_1 = "*//*#endif*/";
            var replaceIfStart_1 = $"/*#if {define}*/{Environment.NewLine}";
            var replaceIfEnd_1 = "/*#endif*/";
            
            var searchIfStart_2 = define.EndsWith("_ON") ? $"/*#if {define.Replace("_ON", "_OFF")}*/{Environment.NewLine}"
                                                         : $"/*#if {define.Replace("_OFF", "_ON")}*/{Environment.NewLine}";
            var searchIfEnd_2 = "/*#endif*/";
            var replaceIfStart_2 = define.EndsWith("_ON") ? $"/*#if {define.Replace("_ON", "_OFF")}*//*{Environment.NewLine}"
                                                          : $"/*#if {define.Replace("_OFF", "_ON")}*//*{Environment.NewLine}";
            var replaceIfEnd_2 = "*//*#endif*/";
            var searchTags = new[] { searchIfStart_1, searchIfEnd_1, searchIfStart_2, searchIfEnd_2 };
            var replaceTags = new[] { replaceIfStart_1, replaceIfEnd_1, replaceIfStart_2, replaceIfEnd_2 };
            
            foreach (var file in files)
            {
                var hasChanged = false;
                var text = File.ReadAllText(file, Encoding.Default);
                
                for (var i = 0; i < searchTags.Length; i += 2)
                {
                    var startIndex = 0;
                    var replaceText = string.Empty;
                    
                    foreach (var tag in text.GetAllTags(searchTags[i], searchTags[i + 1]))
                    {
                        hasChanged = true;
                        
                        if (tag.StartTagIndex >= startIndex)
                        {
                            replaceText += text.Partialstring(startIndex, tag.StartTagIndex - 1);
                            replaceText += replaceTags[i];
                            replaceText += tag.InnerText;
                            replaceText += replaceTags[i + 1];
                            
                            startIndex += tag.EndTagIndex + tag.EndTag.Length;
                        }
                    }
                    if (hasChanged && startIndex < text.Length)
                    {
                        replaceText += text.Partialstring(startIndex, text.Length);
                    }
                    text = replaceText.HasContent() ? replaceText : text;
                }
                if (hasChanged)
                {
                    File.WriteAllText(file, text, Encoding.Default);
                }
            }
        }
    }
}

