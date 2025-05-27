//@BaseCode

namespace TemplateTools.Logic.Extensions
{
    /// <summary>
    /// Provides extension methods for string manipulation.
    /// </summary>
    public static partial class StringExtensions
    {
        ///<summary>
        ///Converts the first character of a string to lowercase (camel case) and returns the updated string.
        ///</summary>
        ///<param name="value">The string to convert.</param>
        ///<returns>A string with the first character converted to lowercase (camel case).</returns>
        ///<exception cref="ArgumentNullException">Thrown when the input string is null or empty.</exception>
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }
            return string.Concat(value[..1].ToLower(), value.AsSpan(1));
        }
        ///<summary>
        /// Removes empty lines from a string.
        ///</summary>
        ///<param name="source">The string to remove empty lines from.</param>
        ///<returns>A new string with empty lines removed.</returns>
        public static string RemoveEmptyLines(this string source)
        {
            var result = new List<string>();
            
            foreach (var line in source.ToLines())
            {
                if (line.Replace(" ", string.Empty) != string.Empty)
                {
                    result.Add(line);
                }
            }
            return result.ToText();
        }
        
        /// <summary>
        /// Removes C# comments from the given source string.
        /// </summary>
        /// <param name="source">The input string to remove the comments from.</param>
        /// <returns>The source string without any C# comments.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the source string is null or empty.</exception>
        public static string RemoveCSharpComments(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            return source.RemoveCSharpXmlComments()
                         .RemoveCSharpLineComments()
                         .RemoveCSharpBlockComments();
        }
        /// <summary>
        /// Removes C# XML comments from the input string.
        /// </summary>
        /// <param name="source">The source string to remove C# XML comments from.</param>
        /// <returns>A string without C# XML comments.</returns>
        public static string RemoveCSharpXmlComments(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            bool cutting;
            var result = source;
            
            do
            {
                var startIdx = GetLineXmlCommentPosition(result);
                var endIdx = result.IndexOf(Environment.NewLine, startIdx + 1);
                
                cutting = false;
                if (startIdx >= 0 && endIdx >= 0)
                {
                    cutting = true;
                    result = result.CuttingOut(startIdx, endIdx);
                }
                else if (startIdx >= 0 && endIdx == -1)
                {
                    result = result.CuttingOut(startIdx);
                }
            } while (cutting);
            
            return result;
        }
        /// <summary>
        /// Removes C# line comments from the specified source string.
        /// </summary>
        /// <param name="source">The source string to remove C# line comments from.</param>
        /// <returns>The modified string with C# line comments removed.</returns>
        public static string RemoveCSharpLineComments(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            bool cutting;
            var result = source;
            
            do
            {
                var startIdx = GetLineCommentPosition(result);
                var endIdx = result.IndexOf(Environment.NewLine, startIdx + 1);
                
                cutting = false;
                if (startIdx >= 0 && endIdx >= 0)
                {
                    cutting = true;
                    result = result.CuttingOut(startIdx, endIdx);
                }
                else if (startIdx >= 0 && endIdx == -1)
                {
                    result = result.CuttingOut(startIdx);
                }
            } while (cutting);
            
            return result;
        }
        /// <summary>
        /// Removes C# block comments from the specified source string.
        /// </summary>
        /// <param name="source">The source string from which block comments will be removed.</param>
        /// <returns>
        /// The source string without any C# block comments.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the source string is null or empty.</exception>
        public static string RemoveCSharpBlockComments(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            bool cutting;
            var result = source;
            
            do
            {
                var startIdx = result.IndexOf(@"/*");
                var endIdx = result.IndexOf(@"*/", startIdx + 1);
                
                cutting = false;
                if (startIdx >= 0 && endIdx >= 0)
                {
                    cutting = true;
                    result = result.CuttingOut(startIdx, endIdx);
                }
            } while (cutting);
            
            return result;
        }
        
        /// <summary>
        /// Removes C# regions from the given source code.
        /// </summary>
        /// <param name="source">The source code string to remove C# regions from.</param>
        /// <returns>The updated source code string with C# regions removed.</returns>
        public static string RemoveCSharpRegions(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            return source.ToLines().Where(l => l.Contains(@"#region") == false
            && l.Contains(@"#endregion") == false).ToText();
        }
        
        /// <summary>
        /// Gets the position of the first occurrence of a line comment ("//") in the given string.
        /// </summary>
        /// <param name="line">The string in which to search for the line comment position.</param>
        /// <returns>
        /// The position of the first occurrence of the line comment in the string, or -1 if no line comment is found.
        /// </returns>
        private static int GetLineCommentPosition(string line)
        {
            var result = -1;
            var quotationCount = 0;
            
            if (string.IsNullOrEmpty(line) == false)
            {
                for (int i = 0; i < line.Length - 1 && result == -1; i++)
                {
                    var c = line[i];
                    
                    if (c == '"')
                    {
                        quotationCount++;
                    }
                    if (quotationCount % 2 == 0 && c == '/' && line[i + 1] == '/')
                    {
                        result = i;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Gets the position of the first occurrence of a line comment sequence ('///') in the provided string.
        /// </summary>
        /// <param name="line">The string to search for the line comment sequence.</param>
        /// <returns>
        /// The index of the first occurrence of the line comment sequence in the string.
        /// If the line is null, empty, or does not contain the line comment sequence, -1 is returned.
        /// </returns>
        private static int GetLineXmlCommentPosition(string line)
        {
            var result = -1;
            var quotationCount = 0;
            
            if (string.IsNullOrEmpty(line) == false)
            {
                for (int i = 0; i < line.Length - 2 && result == -1; i++)
                {
                    var c = line[i];
                    
                    if (c == '"')
                    {
                        quotationCount++;
                    }
                    if (quotationCount % 2 == 0 && c == '/' && line[i + 1] == '/' && line[i + 2] == '/')
                    {
                        result = i;
                    }
                }
            }
            return result;
        }
    }
}

