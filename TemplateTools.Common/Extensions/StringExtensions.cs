//@BaseCode

using System.Security.Cryptography;
using System.Text;

namespace TemplateTools.Common.Extensions
{
    /// <summary>
    /// Provides extension methods for strings.
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Gets or sets the separator used for string splitting.
        /// </summary>
        /// <value>
        /// The separator string. The default value is ";".
        /// </value>
        public static string Separator { get; set; } = ";";
        /// <summary>
        /// Gets or sets the string used for indenting.
        /// </summary>
        /// <value>
        /// The string used for indenting.
        /// </value>
        public static string IndentSpace { get; set; } = "    ";
        /// <summary>
        /// Gets or sets the text that represents a null value.
        /// </summary>
        /// <value>
        /// The text that represents a null value.
        /// </value>
        public static string NullText { get; set; } = "<NULL>";

        /// <summary>
        /// Checks if the input string is null or empty.
        /// </summary>
        /// <param name="source">The input string to be checked.</param>
        /// <param name="argName">The name of the argument being checked.</param>
        /// <exception cref="ArgumentException">Thrown when the input string is null or empty.</exception>
        public static void CheckNotNullOrEmpty(this string? source, string argName)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentException("String is null or empty!", argName);
        }

        /// <summary>
        /// Cuts out a subtring from the given <paramref name="source"/> starting from the specified <paramref name="startIdx"/>.
        /// </summary>
        /// <param name="source">The source string from which to extract a substring.</param>
        /// <param name="startIdx">The starting index of the substring to be cut out.</param>
        /// <returns>The cut out substring.</returns>
        public static string CuttingOut(this string source, int startIdx)
        {
            return source.CuttingOut(startIdx, source.Length - 1);
        }
        /// <summary>
        /// Removes a specified range of characters from the input string.
        /// </summary>
        /// <param name="source">The original string.</param>
        /// <param name="startIdx">The starting index (inclusive) of the characters to remove.</param>
        /// <param name="endIdx">The ending index (inclusive) of the characters to remove.</param>
        /// <returns>A new string with the specified range of characters removed.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when either the startIdx or endIdx is less than zero or greater than or equal to the length of the input string.
        /// </exception>
        public static string CuttingOut(this string source, int startIdx, int endIdx)
        {
            var result = new StringBuilder();

            for (int i = 0; i < source.Length; i++)
            {
                if (i < startIdx || i > endIdx)
                {
                    result.Append(source[i]);
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// Retrieves a substring from this instance. The substring starts at a specified character position and ends before the first occurrence of a specified substring.
        /// </summary>
        /// <param name="source">The source <see cref="string"/>.</param>
        /// <param name="index">The zero-based starting character position of the substring.</param>
        /// <param name="text">A <see cref="string"/> used as the delimiter for the end of the substring.</param>
        /// <returns>A <see cref="string"/> that is equivalent to the substring that begins at <paramref name="index"/> and ends before the first occurrence of <paramref name="text"/>, or <see cref="string.Empty"/> if <paramref name="text"/> is not found.</returns>
        public static string Substring(this string source, int index, string text)
        {
            var result = string.Empty;
            var ofIdx = source.IndexOf(text);

            if (ofIdx >= 0 && ofIdx - index >= 0)
            {
                result = source.Substring(index, ofIdx);
            }
            return result;
        }
        /// <summary>
        /// Extracts the string between specified start and end texts from the source string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="startText">The start text to search for.</param>
        /// <param name="endText">The end text to search for.</param>
        /// <returns>The extracted string between the start and end texts, or an empty string if not found.</returns>
        public static string ExtractBetween(this string source, string startText, string endText)
        {
            var result = string.Empty;
            var sIdx = source.IndexOf(startText) + startText.Length;
            var eIdx = source.IndexOf(endText) - 1;

            if (sIdx > -1 && eIdx > -1 && sIdx <= eIdx)
            {
                result = source.Partialstring(sIdx, eIdx);
            }
            return result;
        }
        /// <summary>
        /// Extracts the substring between the specified opening and closing brackets, starting at index 0.
        /// </summary>
        /// <param name="source">The source string from which the substring should be extracted.</param>
        /// <param name="openingBracket">The character representing the opening bracket.</param>
        /// <param name="closingBracket">The character representing the closing bracket.</param>
        /// <returns>
        /// The substring between the opening and closing brackets starting at index 0, or an empty string if the brackets are not found.
        /// </returns>
        public static string ExtractBetween(this string source, char openingBracket, char closingBracket)
        {
            return source.ExtractBetween(openingBracket, closingBracket, 0);
        }
        /// Extracts a substring between the specified opening and closing brackets from the provided source string, starting from the specified position.
        /// @param source The source string from which to extract the substring.
        /// @param openingBracket The character representing the opening bracket.
        /// @param closingBracket The character representing the closing bracket.
        /// @param startPosition The starting position to begin scanning the source string.
        /// @return The extracted substring, or an empty string if no substring is found.
        public static string ExtractBetween(this string source, char openingBracket, char closingBracket, int startPosition)
        {
            var toScan = true;
            var open = false;
            var openCount = 0;
            var result = new StringBuilder();

            for (int i = startPosition; i > -1 && i < source.Length && toScan; i++)
            {
                if (source[i] == openingBracket && open == false)
                {
                    open = true;
                    openCount = 1;
                }
                else if (source[i] == openingBracket && open)
                {
                    openCount++;
                    result.Append(source[i]);
                }
                else if (source[i] == closingBracket && openCount > 1)
                {
                    openCount--;
                    result.Append(source[i]);
                }
                else if (source[i] == closingBracket && openCount == 1)
                {
                    open = false;
                    openCount = 0;
                    toScan = false;
                }
                else if (openCount > 0)
                {
                    result.Append(source[i]);
                }
            }
            return toScan == false ? result.ToString() : string.Empty;
        }
        /// <summary>
        /// Replaces the text between two specified strings in the source string with the specified replace text.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="startText">The starting string to identify the section to be replaced.</param>
        /// <param name="endText">The ending string to identify the section to be replaced.</param>
        /// <param name="replaceText">The text to replace the section identified by startText and endText.</param>
        /// <returns>A string with the section between startText and endText replaced with replaceText.</returns>
        public static string ReplaceBetween(this string source, string startText, string endText, string replaceText)
        {
            string result;
            var sIdx = source.IndexOf(startText);
            var eIdx = source.IndexOf(endText);

            if (sIdx > -1 && eIdx > -1 && sIdx <= eIdx)
            {
                result = source[..(sIdx + startText.Length)];
                result += replaceText;
                result += source[eIdx..];
            }
            else
            {
                result = source;
            }
            return result;
        }

        /// <summary>
        /// Tries to parse a string value into the specified type and retrieves the parsed value if successful.
        /// </summary>
        /// <param name="value">The string value to be parsed.</param>
        /// <param name="type">The type to parse the value into.</param>
        /// <param name="typeValue">When this method returns, contains the parsed value if the parsing was successful; otherwise, null.</param>
        /// <returns>true if the value was successfully parsed; otherwise, false.</returns>
        public static bool TryParse(this string value, Type type, out object? typeValue)
        {
            bool result = false;

            if (value == null)
            {
                result = true;
                typeValue = null;
            }
            else if (type.IsEnum)
            {
                result = Enum.TryParse(type, value, out typeValue);
            }
            else if (type == typeof(TimeSpan))
            {
                typeValue = TimeSpan.Parse(value);
                result = true;
            }
            else if (type == typeof(DateTime))
            {
                typeValue = DateTime.Parse(value);
                result = true;
            }
            else if (type == typeof(string))
            {
                typeValue = value;
            }
            else
            {
                typeValue = Convert.ChangeType(value, type);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Determines whether the specified string is contained within this instance, ignoring case.
        /// </summary>
        /// <param name="source">The string to search within.</param>
        /// <param name="toCheck">The string to check for.</param>
        /// <returns><c>true</c> if the specified string is found within this instance, ignoring case; otherwise, <c>false</c>.</returns>
        public static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            return source.Contains(toCheck, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// Determines whether the specified string contains any of the specified values, using the default current culture and case-insensitive comparison.
        /// </summary>
        /// <param name="source">The source string to check.</param>
        /// <param name="toChecks">The array of strings to check for.</param>
        /// <returns>true if the source string contains any of the specified values; otherwise, false.</returns>
        public static bool Contains(this string source, params string[] toChecks)
        {
            return source.Contains(StringComparison.CurrentCultureIgnoreCase, toChecks);
        }
        /// <summary>
        /// Determines whether the source string contains any of the specified substrings, using the specified string comparison type.
        /// </summary>
        /// <param name="source">The source string to check.</param>
        /// <param name="comparison">One of the enumeration values that specifies the rules for the comparison.</param>
        /// <param name="toChecks">An array of strings to compare against the source string.</param>
        /// <returns><c>true</c> if the source string contains any of the specified substrings; otherwise, <c>false</c>.</returns>
        public static bool Contains(this string source, StringComparison comparison, params string[] toChecks)
        {
            var result = toChecks.Length > 0;

            foreach (var item in toChecks)
            {
                result = result && source.Contains(item, comparison);
            }
            return result;
        }
        /// <summary>
        /// Determines whether a specified character is contained within a string, excluding characters within specified quotation marks.
        /// </summary>
        /// <param name="source">The string to search within.</param>
        /// <param name="toCheck">The character to search for.</param>
        /// <param name="quotationStart">The opening quotation mark character.</param>
        /// <param name="quotationEnd">The closing quotation mark character.</param>
        /// <returns>
        /// <c>true</c> if the specified character is found within the string, excluding characters within specified quotation marks; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this string source, char toCheck, char quotationStart, char quotationEnd)
        {
            return source.Contains(toCheck, [quotationStart], [quotationEnd]);
        }
        /// <summary>
        /// Checks if a string contains a specified character, taking into account nested quotations.
        /// </summary>
        /// <param name="source">The string to search in.</param>
        /// <param name="toCheck">The character to check for.</param>
        /// <param name="quotationStarts">An array of characters that indicate the start of a quotation.</param>
        /// <param name="quotationEnds">An array of characters that indicate the end of a quotation.</param>
        /// <returns>
        /// True, if the string contains the specified character outside of any quotations;
        /// otherwise, false.
        /// </returns>
        public static bool Contains(this string source, char toCheck, char[] quotationStarts, char[] quotationEnds)
        {
            var result = false;
            var search = quotationStarts.Length == quotationEnds.Length;
            var quotationCounter = new int[quotationStarts.Length];

            for (int i = 0; i < source.Length && search; i++)
            {
                var c = source[i];

                if (quotationStarts.FindIndex(q => q == c) > -1)
                {
                    quotationCounter[quotationStarts.FindIndex(q => q == c)]++;
                }
                else if (quotationEnds.FindIndex(q => q == c) > -1)
                {
                    quotationCounter[quotationEnds.FindIndex(q => q == c)]--;
                }
                else if (quotationCounter.Sum(q => q % 2) == 0)
                {
                    result = c == toCheck;
                    search = result == false;
                }
            }
            return result;
        }

        /// <summary>
        /// Splits each string element in the source collection using the specified separator.
        /// </summary>
        /// <param name="source">The collection of strings to be split.</param>
        /// <param name="separator">The separator used for splitting.</param>
        /// <returns>An enumerable collection of string arrays, where each array contains the split elements.</returns>
        public static IEnumerable<string[]> Split(this IEnumerable<string> source, string separator)
        {
            return source.Select(l => string.IsNullOrEmpty(l) ? [] : l.Split(separator));
        }
        /// <summary>
        /// Splits a collection of strings and applies a mapper function to each resulting string array.
        /// </summary>
        /// <typeparam name="T">The type of the resulting mapped elements.</typeparam>
        /// <param name="source">The collection of strings to be split.</param>
        /// <param name="separator">The separator used to split the strings.</param>
        /// <param name="mapper">The function to be applied to each resulting string array.</param>
        /// <returns>Returns an enumerable collection of mapped elements.</returns>
        public static IEnumerable<T> SplitAndMap<T>(this IEnumerable<string> source, string separator, Func<string[], T> mapper)
        {
            return source.Split(separator).Select(d => mapper(d));
        }
        /// <summary>
        /// Splits a collection of strings and maps the resulting arrays to a specified type.
        /// </summary>
        /// <typeparam name="T">The type to map the arrays to.</typeparam>
        /// <param name="source">The collection of strings to split and map.</param>
        /// <param name="separator">The separator to split the strings.</param>
        /// <param name="mapper">The function that maps the splitted arrays to the specified type.</param>
        /// <returns>An enumerable of the mapped elements.</returns>
        /// <remarks>
        /// The method splits the collection of strings into arrays using the specified separator.
        /// It then obtains the header from the splitted source as the first element of the resulting arrays.
        /// The mapper function is applied to each splitted array with the header, or an empty array if the header is null.
        /// The mapped elements are returned as an enumerable.
        /// </remarks>
        public static IEnumerable<T> SplitAndMap<T>(this IEnumerable<string> source, string separator, Func<string[], string[], T> mapper)
        {
            var splitSource = source.Split(separator);
            var header = splitSource.FirstOrDefault();

            return splitSource.Skip(1).Select(d => mapper(d, header ?? []));
        }

        /// <summary>
        /// Retrieves the value of a string if not null or empty; otherwise, returns a default value.
        /// </summary>
        /// <param name="source">The string to check for null or empty value.</param>
        /// <param name="defaultValue">The default value to return if the string is null or empty.</param>
        /// <returns>The original string value if not null or empty; otherwise, the default value.</returns>
        public static string GetValue(this string source, string defaultValue)
        {
            return string.IsNullOrEmpty(source) ? defaultValue : source;
        }


        /// <summary>
        /// Indicates whether the index is within the string.
        /// </summary>
        /// <param name="source">The string in which the index is tested.</param>
        /// <param name="index">The index to test.</param>
        /// <returns></returns>
        public static bool InRange(this string source, int index)
        {
            return source != null && index > -1 && index < source.Length;
        }

        /// <summary>
        /// Indicates whether the specified string is null or an Empty string.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string? text)
        {
            return string.IsNullOrEmpty(text);
        }
        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if the value parameter is null or String.Empty, or if value consists exclusively of white-space characters.</returns>
        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }
        /// <summary>
        /// Indicates whether the specified string has a content.
        /// </summary>
        /// <param name="source">The string to test.</param>
        /// <returns>true if the value parameter is not null and not empty; otherwise, false.</returns>
        public static bool HasContent(this string? source)
        {
            return !string.IsNullOrEmpty(source);
        }

        /// <summary>
        /// Determines if two string objects have unequal values.
        /// </summary>
        /// <param name="source">The string to compare to the value.</param>
        /// <param name="value">The string to compare to the source.</param>
        /// <returns>True if the values are not equals, false else.</returns>
        public static bool NotEquals(this string source, string value)
        {
            return source.AreEquals(value) == false;
        }
        /// <summary>
        /// Determines if two string objects have equal values.
        /// </summary>
        /// <param name="source">The string to compare to the value.</param>
        /// <param name="value">The string to compare to the source.</param>
        /// <returns>True if the values are equals, false else.</returns>
        public static bool AreEquals(this string source, string value)
        {
            return source == null && value == null || value != null && source != null && source.Equals(value);
        }
        /// <summary>
        /// Determines if two string objects have equal values.
        /// </summary>
        /// <param name="source">The string to compare to the value.</param>
        /// <param name="value">The string to compare to the source.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <returns>True if the values are equals, false else.</returns>
        public static bool AreEquals(this string source, string value, StringComparison stringComparison)
        {
            return source == null && value == null || value != null && source != null && source.Equals(value, stringComparison);
        }

        /// <summary>
        /// Gets the value of the current object or the default value of the parameter.
        /// </summary>
        /// <param name="text">The current objet.</param>
        /// <returns>The value of the current object, or an empty string.</returns>
        public static string GetValueOrDefault(this string text)
        {
            return text.GetValueOrDefault(string.Empty);
        }
        /// <summary>
        /// Gets the value of the current object or the default value of the parameter.
        /// </summary>
        /// <param name="text">The current objet.</param>
        /// <param name="defaultValue">The default value if the object is null or empty.</param>
        /// <returns>The value of the current object, or the parameter default value.</returns>
        public static string GetValueOrDefault(this string text, string defaultValue)
        {
            return string.IsNullOrEmpty(text) ? defaultValue : text;
        }

        /// <summary>
        /// Sets the indentation of the given string with the default indentation space and level 1.
        /// </summary>
        /// <param name="text">The string to set the indentation for.</param>
        /// <returns>The indented string.</returns>
        public static string SetIndent(this string text)
        {
            return text.SetIndent(IndentSpace, 1);
        }
        /// <summary>
        /// Sets the indentation for the given text.
        /// </summary>
        /// <param name="text">The text to apply the indentation to.</param>
        /// <param name="count">The number of indentation spaces.</param>
        /// <returns>The indented text.</returns>
        public static string SetIndent(this string text, int count)
        {
            return text.SetIndent(IndentSpace, count);
        }
        /// <summary>
        /// Sets the indent of the specified string with the given indent space and count.
        /// </summary>
        /// <param name="text">The string to set the indent for.</param>
        /// <param name="indentSpace">The space used for one indent.</param>
        /// <param name="count">The number of indents to set.</param>
        /// <returns>The string with the specified indent set.</returns>
        public static string SetIndent(this string text, string indentSpace, int count)
        {
            StringBuilder sb = new();

            if (text != null)
            {
                for (int i = 0; i < count; i++)
                    sb.Append(indentSpace);        // spaces for one indent.
            }
            sb.Append(text);
            return sb.ToString();
        }
        /// <summary>
        /// Sets indentation for each line in the string array.
        /// </summary>
        /// <param name="lines">The string array to set indentation for.</param>
        /// <param name="count">The number of spaces to use for indentation.</param>
        /// <returns>A new string array with indentation set for each line.</returns>
        /// <remarks>
        /// If <paramref name="lines"/> is null, an empty string array is returned.
        /// </remarks>
        public static string[] SetIndent(this string[] lines, int count)
        {
            if (lines != null)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = lines[i].SetIndent(count);
                }
            }
            return lines ?? [];
        }
        /// <summary>
        /// Sets the indent of each line in the specified collection of strings.
        /// </summary>
        /// <param name="lines">The collection of strings to set the indent for.</param>
        /// <param name="count">The indent count for each line.</param>
        /// <returns>A new collection of strings with the specified indent applied.</returns>
        public static IEnumerable<string> SetIndent(this IEnumerable<string> lines, int count)
        {
            return lines.ToArray().SetIndent(count);
        }

        /// <summary>
        /// This method converts a string array to a coherent text.
        /// </summary>
        /// <param name="lines">The String-Array.</param>
        /// <returns>The compound text.</returns>
        public static string ToText(this IEnumerable<string> lines)
        {
            StringBuilder sb = new();

            foreach (var line in lines)
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                }
                sb.Append(line);
            }
            return sb.ToString();
        }

        /// <summary>
        /// This method converts a string array to a coherent text.
        /// </summary>
        /// <param name="lines">The String-Array.</param>
        /// <param name="lineConvert">Lines converter.</param>
        /// <returns>The compound text.</returns>
        public static string ToText(this IEnumerable<string> lines, Func<string, string> lineConvert)
        {
            StringBuilder sb = new();

            foreach (var line in lines)
            {
                sb.AppendLine(lineConvert == null ? line : lineConvert(line));
            }
            return sb.ToString();
        }

        /// <summary>
        /// This method converts text with line breaks into a string array.
        /// </summary>
        /// <param name="text">The text to be converted.</param>
        /// <returns>The string-array from the text.</returns>
        public static IEnumerable<string> ToLines(this string text)
        {
            List<string> result = [.. text.Split([Environment.NewLine], StringSplitOptions.None)];
            return result;
        }

        /// <summary>
        /// Trims the leading and trailing whitespace of each string in the specified collection and removes consecutive empty strings.
        /// </summary>
        /// <param name="source">The collection of strings to trim.</param>
        /// <returns>A new collection of trimmed strings with consecutive empty strings removed.</returns>
        public static IEnumerable<string?> Trim(this IEnumerable<string> source)
        {
            var result = new List<string?>();
            var prvEmpty = true;

            foreach (var item in source)
            {
                if (string.IsNullOrEmpty(item) && prvEmpty == false)
                {
                    result.Add(string.Empty);
                    prvEmpty = true;
                }
                else if (string.IsNullOrEmpty(item) == false)
                {
                    result.Add(item);
                    prvEmpty = false;
                }
            }
            var lastElem = result.LastOrDefault();

            if (string.IsNullOrEmpty(lastElem))
            {
                _ = result.Remove(lastElem);
            }
            return result;
        }

        /// <summary>
        /// Extracts a substring from a string (excludes from to).
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="from">Starttext</param>
        /// <param name="to">Endtext</param>
        /// <returns>The substring.</returns>
        public static string Partialstring(this string text, string from, string to)
        {
            var result = default(string);

            if (text.HasContent())
            {
                int f = text.IndexOf(from);
                int t = text.IndexOf(to, f + 1) + to.Length - 1;

                result = text.Partialstring(f, t);
            }
            return result ?? String.Empty;
        }

        /// <summary>
        /// Extracts a substring from a string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="from">Startposition</param>
        /// <param name="to">Endposition</param>
        /// <returns>The substring.</returns>
        public static string Partialstring(this string text, int from, int to)
        {
            StringBuilder sb = new();

            if (string.IsNullOrEmpty(text) == false)
            {
                for (int i = from; i >= 0 && i <= to && i < text.Length; i++)
                {
                    sb.Append(text[i]);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Extracts a substring from a string (includes from to).
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="from">Starttext</param>
        /// <param name="to">Endtext</param>
        /// <returns>The substring.</returns>
        public static string Betweenstring(this string text, string from, string to)
        {
            var result = default(string);

            if (text.HasContent())
            {
                int f = text.IndexOf(from) + from.Length;
                int t = text.IndexOf(to, f + 1) - 1;

                result = text.Partialstring(f, t);
            }
            return result ?? String.Empty;
        }

        /// <summary>
        /// Removes the text from the start tag to the end tag.
        /// </summary>
        /// <param name="source">The text from which the partial text should be removed.</param>
        /// <param name="startTag">Startposition</param>
        /// <param name="endTag">Endposition</param>
        /// <returns>The text with the missing partial text.</returns>
        public static string Remove(this string source, string startTag, string endTag)
        {
            StringBuilder result = new();
            int parseIndex = 0;
            int startTagIndex;
            int endTagIndex;

            do
            {
                startTagIndex = source.IndexOf(startTag, parseIndex, StringComparison.CurrentCultureIgnoreCase);
                var startTagEndIndex = startTagIndex > -1 ? startTagIndex + startTag.Length : parseIndex;
                endTagIndex = startTagEndIndex >= 0 ? source.IndexOf(endTag, startTagEndIndex, StringComparison.CurrentCultureIgnoreCase) : -1;
                var endTagEndIndex = endTagIndex > -1 ? endTagIndex + endTag.Length : parseIndex;

                if (startTagIndex > -1 && endTagIndex > startTagIndex)
                {
                    result.Append(source[parseIndex..startTagIndex]);
                    parseIndex = endTagEndIndex;
                }
            } while (startTagIndex > -1 && endTagIndex > -1);

            if (parseIndex < source.Length)
            {
                result.Append(source[parseIndex..]);
            }
            return result.ToString();
        }

        /// <summary>
        /// Removes the specified characters from the left and right sides of a string.
        /// </summary>
        /// <param name="source">The string to remove characters from.</param>
        /// <param name="ignores">The characters to ignore and not remove.</param>
        /// <returns>A new string with the specified characters removed from the left and right sides.</returns>
        public static string RemoveLeftAndRight(this string source, params char[] ignores)
        {
            var result = new StringBuilder();
            var start = 0;
            var end = source.Length - 1;

            while (start < end && ignores.Contains(source[start]))
            {
                start++;
            }

            while (start < end && ignores.Contains(source[end]))
            {
                end--;
            }

            while (start <= end)
            {
                result.Append(source[start]);
                start++;
            }
            return result.ToString();
        }

        /// <summary>
        /// Splits a string into an array of substrings using a specified separator character, while ignoring specified characters.
        /// </summary>
        /// <param name="source">The string to split.</param>
        /// <param name="separator">The character used as the separator.</param>
        /// <param name="ignores">The characters to ignore during the splitting process.</param>
        /// <returns>An array of substrings.</returns>
        public static string[] SplitWithIgnores(this string source, char separator, params char[] ignores)
        {
            var result = new List<string>();
            var stringBuilder = new StringBuilder();
            var ignoreCounts = new int[ignores.Length];

            foreach (var item in source ?? string.Empty)
            {
                var index = Array.IndexOf(ignores, item);

                if (index >= 0)
                {
                    ignoreCounts[index]++;
                }
                if (item == separator && ignoreCounts.Sum() % 2 == 0)
                {
                    if (stringBuilder.Length > 0)
                    {
                        result.Add(stringBuilder.ToString());
                    }
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.Append(item);
                }
            }
            if (stringBuilder.Length > 0)
            {
                result.Add(stringBuilder.ToString());
            }
            return [.. result];
        }
        /// <summary>
        /// Splits a string into a list of substrings based on a specified character and trims each substring.
        /// </summary>
        /// <param name="source">The string to split.</param>
        /// <param name="c">The character used to split the string.</param>
        /// <returns>A list of substrings.</returns>
        public static List<string> SplitByChar(this string source, char c)
        {
            if (source == null)
            {
                return [];
            }

            return [.. source.Split(c).Select(x => x.Trim())];
        }

        /// <summary>
        /// Replaces specific characters in a string with their corresponding ASCII representations.
        /// </summary>
        /// <param name="text">The string to be processed.</param>
        /// <returns>A new string with replaced characters.</returns>
        public static string ReplaceUmlauts(this string text)
        {
            StringBuilder sb = new();

            if (text != null)
            {
                foreach (var item in text)
                {
                    if (item == 223) // sz is replaced by ss
                    {
                        sb.Append("ss");
                    }
                    else if (item == 196)
                    {
                        sb.Append("Ae");
                    }
                    else if (item == 228)
                    {
                        sb.Append("ae");
                    }
                    else if (item == 214)
                    {
                        sb.Append("Oe");
                    }
                    else if (item == 246)
                    {
                        sb.Append("oe");
                    }
                    else if (item == 220)
                    {
                        sb.Append("Ue");
                    }
                    else if (item == 252)
                    {
                        sb.Append("ue");
                    }
                    else
                    {
                        sb.Append(item);
                    }
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Replaces all occurrences of a text enclosed between start and end tags in a given string with a specified replace text.
        /// </summary>
        /// <param name="text">The string in which the replacement will be performed.</param>
        /// <param name="startTag">The starting tag that marks the beginning of the text to be replaced.</param>
        /// <param name="endTag">The ending tag that marks the end of the text to be replaced.</param>
        /// <param name="replaceText">The text that will replace the matched section.</param>
        /// <returns>A new string with all occurrences of the specified text replaced with the given replace text.</returns>
        public static string ReplaceAll(this string text, string startTag, string endTag, string replaceText)
        {
            return text.ReplaceAll(startTag, endTag, s => replaceText);
        }
        /// <summary>
        /// Replaces all occurrences of text enclosed by specified start and end tags in the given string.
        /// </summary>
        /// <param name="text">The input string.</param>
        /// <param name="startTag">The start tag.</param>
        /// <param name="endTag">The end tag.</param>
        /// <param name="replace">A function that specifies the replacement for each occurrence of the enclosed text.
        /// The function takes the enclosed text as input and returns the replacement string.</param>
        /// <returns>A new string with all occurrences of the enclosed text replaced.</returns>
        public static string ReplaceAll(this string text, string startTag, string endTag, Func<string, string> replace)
        {
            int parseIndex = 0;
            int startTagIndex;
            int endTagIndex;
            StringBuilder result = new();

            do
            {
                startTagIndex = text.IndexOf(startTag, parseIndex, StringComparison.CurrentCultureIgnoreCase);
                int startTagEndIndex = startTagIndex > -1 ? startTagIndex + startTag.Length : parseIndex;
                endTagIndex = startTagEndIndex >= 0 ? text.IndexOf(endTag, startTagEndIndex, StringComparison.CurrentCultureIgnoreCase) : -1;
                int endTagEndIndex = endTagIndex > -1 ? endTagIndex + endTag.Length : parseIndex;

                if (startTagIndex > -1 && endTagIndex > startTagIndex)
                {
                    string substr = text.Substring(startTagIndex, endTagIndex - startTagIndex + endTag.Length);

                    result.Append(text[parseIndex..startTagIndex]);
                    if (replace != null)
                    {
                        result.Append(replace(substr));
                    }
                    parseIndex = endTagEndIndex;
                }
            } while (startTagIndex > -1 && endTagIndex > -1);

            if (parseIndex < text.Length)
            {
                result.Append(text[parseIndex..]);
            }
            return result.ToString();
        }
        /// <summary>
        /// Removes all spaces from a string.
        /// </summary>
        /// <param name="source">String from which the spaces are removed.</param>
        /// <returns>String with no spaces.</returns>
        public static string RemoveAll(this string source)
        {
            return source.RemoveAll(" ");
        }
        /// <summary>
        /// Removes all occurrences of specified removeItems from the source string.
        /// </summary>
        /// <param name="source">The original string.</param>
        /// <param name="removeItems">One or more strings to be removed from the source string.</param>
        /// <returns>The modified string with removeItems removed.</returns>
        public static string RemoveAll(this string source, params string[] removeItems)
        {
            var result = source;

            foreach (var item in removeItems)
            {
                result = result.Replace(item, string.Empty);
            }
            return result;
        }

        ///<summary>
        /// Converts a string to an integer.
        ///</summary>
        ///<param name="text">The string to be converted.</param>
        ///<returns>An integer representation of the input string.</returns>
        public static int ToInt(this string text)
        {
            int result = 0;

            foreach (var item in text)
            {
                if (char.IsDigit(item))
                {
                    result *= 10;
                    result = result + item - '0';
                }
            }
            return result;
        }
        /// <summary>
        /// Converts a string to a long integer.
        /// </summary>
        /// <param name="text">The string to be converted.</param>
        /// <returns>The long integer representation of the string. Returns 0 if the string is null or empty.</returns>
        public static long ToLong(this string text)
        {
            long result = 0;

            foreach (var item in text)
            {
                if (char.IsDigit(item))
                {
                    result *= 10;
                    result = result + item - '0';
                }
            }
            return result;
        }
        /// <summary>
        /// Replaces any characters in the input string that are not valid in a filename with underscores (_) and returns the resulting string.
        /// </summary>
        /// <param name="text">The input string to be converted into a valid filename.</param>
        /// <returns>A string where any invalid filename characters are replaced with underscores. If the input string is null, then an empty string is returned.</returns>
        public static string ToFileName(this string text)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                text = text.Replace(c, '_');
            }
            return text ?? String.Empty;
        }

        /// <summary>
        /// Converts a string into an enumerable of nullable type T.
        /// </summary>
        /// <typeparam name="T">The type of the values contained in the enumerable.</typeparam>
        /// <param name="source">The string to be converted.</param>
        /// <param name="separator">The separator used to split the string into individual items.</param>
        /// <returns>An enumerable of nullable type T.</returns>
        /// <remarks>
        /// If the source string is null or empty, an empty enumerable will be returned.
        /// If an item in the source string is equal to null text, a null value will be added to the enumerable.
        /// Otherwise, the item will be converted to the specified type T using <see cref="Convert.ChangeType(object, Type)"/> method.
        /// If the conversion fails, the default value for type T will be added to the enumerable.
        /// </remarks>
        public static IEnumerable<T?> ToEnumerable<T>(this string source, string separator)
        {
            List<T?> result = [];

            if (string.IsNullOrEmpty(source) == false)
            {
                string[] items = source.Split(separator);

                foreach (var item in items)
                {
                    if (item.Equals(NullText))
                    {
                        result.Add(default);
                    }
                    else
                    {
                        try
                        {
                            result.Add((T)Convert.ChangeType(item, typeof(T)));
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            result.Add(default);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Converts the specified string into an array of bytes using the ASCII encoding.
        /// </summary>
        /// <param name="source">The string to convert.</param>
        /// <returns>An array of bytes representing the specified string.</returns>
        public static byte[] ToByteArray(this string source)
        {
            byte[]? result = null;

            if (source != null)
            {
                result = new ASCIIEncoding().GetBytes(source);
            }
            return result ?? [];
        }

        /// <summary>
        /// Encrypts the provided text using the specified key.
        /// </summary>
        /// <param name="text">The text to encrypt.</param>
        /// <param name="keyString">The key used for encryption.</param>
        /// <returns>The encrypted text as a base64 encoded string.</returns>
        public static string Encrypt(this string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using var aesAlg = Aes.Create();
            using var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }

            var iv = aesAlg.IV;

            var decryptedContent = msEncrypt.ToArray();

            var result = new byte[iv.Length + decryptedContent.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

            return Convert.ToBase64String(result);
        }
        /// Decrypts the given cipher text using the specified key.
        /// @param cipherText The cipher text to decrypt.
        /// @param keyString The key used for decryption.
        /// @return The decrypted plain text.
        public static string Decrypt(this string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            string result;
            using var aesAlg = Aes.Create();
            using var decryptor = aesAlg.CreateDecryptor(key, iv);
            using (MemoryStream msDecrypt = new(cipher))
            {
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                result = srDecrypt.ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified string is a word.
        /// </summary>
        /// <param name="text">The string to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified string is a word; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsWord(this string text)
        {
            var result = text.IsNullOrEmpty() == false;

            for (int i = 0; result && i < text.Length; i++)
            {
                result = char.IsLetter(text[i]);
            }
            return result;
        }
        /// <summary>
        /// Creates the plural form of a word in singular.
        /// </summary>
        /// <param name="wordInSingular">The word in singular form.</param>
        /// <returns>The plural form of the input word.</returns>
        public static string CreatePluralWord(this string wordInSingular)
        {
            string result = wordInSingular;

            if (wordInSingular.IsWord())
            {
                if (wordInSingular.EndsWith("y", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = $"{wordInSingular[0..^1]}ies";
                }
                else if (wordInSingular.EndsWith("h", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = $"{wordInSingular}es";
                }
                else if (wordInSingular.EndsWith("x", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = $"{wordInSingular}es";
                }
                else if (wordInSingular.EndsWith("f", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = $"{wordInSingular[0..^1]}ves";
                }
                else if (wordInSingular.EndsWith("ss", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = $"{wordInSingular}es";
                }
                else if (wordInSingular.EndsWith("s", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = $"{wordInSingular}";
                }
                else
                {
                    result = $"{wordInSingular}s";
                }
            }
            return result;
        }
    }
}

