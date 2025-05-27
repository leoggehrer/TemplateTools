//@BaseCode

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace TemplateTools.Logic.Extensions
{
    /// <summary>
    /// Utility class containing extension methods for formatting C# code.
    /// </summary>
    public static class FormatterExtensions
    {
        /// <summary>
        /// Formats a given C# code by replacing line breaks with the platform-specific line break and then formatting the code using a predefined set of rules.
        /// </summary>
        /// <param name="source">The C# code to be formatted.</param>
        /// <returns>The formatted C# code.</returns>
        public static string FormatCSharpSyntax(this string source)
        {
            var tree = CSharpSyntaxTree.ParseText(source);
            var root = tree.GetRoot();
            using var workspace = new AdhocWorkspace();
            var formattedRoot = Formatter.Format(root, workspace);
            
            return formattedRoot.ToFullString();
        }
        /// <summary>
        /// Formats the given C# code by applying appropriate indentation.
        /// </summary>
        /// <param name="lines">An enumerable collection of C# code lines to be formatted.</param>
        /// <returns>An enumerable collection of formatted C# code lines.</returns>
        public static IEnumerable<string> FormatCSharpSyntax(this IEnumerable<string> source)
        {
            return source.ToText().FormatCSharpSyntax().ToLines();
        }
    }
}
