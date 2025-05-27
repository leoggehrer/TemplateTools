//@BaseCode

namespace TemplateTools.Common.Extensions
{
    /// <summary>
    /// Represents a header for a tag.
    /// </summary>
    /// <remarks>
    /// This class is used to store and manipulate data related to a tag header.
    /// </remarks>
    public partial class TagInfo
    {
        /// <summary>
        /// Represents a TagHeader object, which contains information about the source of a tag header.
        /// </summary>
        /// <remarks>
        /// Initializes a new instance of the <see cref="TagHeader"/> class with a specified source.
        /// </remarks>
        /// <param name="source">The source of the tag header.</param>
        internal partial class TagHeader(string source)
        {
            /// <summary>
            /// Gets the source of the object as a string.
            /// </summary>
            /// <returns>The source of the object.</returns>
            public string Source { get; } = source;
        }
        ///<summary>
        /// Gets or sets the header of the tag.
        ///</summary>
        ///<returns>
        /// The header of the tag. Returns null if no header is set.
        ///</returns>
        internal TagHeader? Header { get; set; }
        
        /// <summary>
        /// Gets the source value from the header, if not null; otherwise, returns an empty string.
        /// </summary>
        /// <returns>
        /// The source value.
        /// </returns>
        public string Source => Header?.Source ?? string.Empty;
        /// <summary>
        /// Gets or sets the start tag for the string.
        /// </summary>
        /// <value>
        /// The start tag for the string.
        /// </value>
        public string StartTag { get; internal set; } = string.Empty;
        /// <summary>
        /// Gets or sets the start tag index.
        /// </summary>
        /// <value>The start tag index.</value>
        public int StartTagIndex { get; internal set; }
        /// <summary>
        /// Gets or sets the end tag for the string.
        /// </summary>
        /// <value>
        /// The end tag for the string. The default value is an empty string.
        /// </value>
        public string EndTag { get; internal set; } = string.Empty;
        /// Gets or sets the index of the end tag.
        /// This property represents the index of the end tag in a string or document.
        /// It is updated internally and cannot be modified externally.
        /// @value - The new value for the index of the end tag.
        public int EndTagIndex { get; internal set; }
        /// <summary>
        /// Gets the index of the end tag plus the length of the end tag.
        /// </summary>
        /// <returns>The index of the end tag plus the length of the end tag.</returns>
        public int EndIndex => EndTagIndex + EndTag.Length;
        
        /// <summary>
        /// Retrieves the full text from the source string based on the specified start and end tag indices.
        /// </summary>
        /// <returns>The full text from the source string.</returns>
        /// <remarks>
        /// The start and end tag indices must be within the valid range of the source string.
        /// The full text includes the characters from the start tag until the end of the end tag.
        /// </remarks>
        public string FullText => Source.Partialstring(StartTagIndex, EndTagIndex + EndTag.Length - 1);
        /// <summary>
        /// Gets the inner text of the specified source.
        /// </summary>
        /// <returns>The inner text.</returns>
        /// <remarks>
        /// This property retrieves the inner text of the source by using the indices of the start and end tags.
        /// </remarks>
        public string InnerText => Source.Partialstring(StartTagIndex + StartTag.Length, EndTagIndex - 1);
        
        /// <summary>
        /// Retrieves the text from the inner text and returns it.
        /// </summary>
        /// <returns>The retrieved text.</returns>
        public string GetText()
        {
            return GetText(InnerText);
        }
        /// <summary>
        /// Retrieves the text between the start and end tags in the source string.
        /// </summary>
        /// <param name="innerText">The text to be inserted between the start and end tags.</param>
        /// <returns>The modified string with the inserted text.</returns>
        public string GetText(string innerText)
        {
            return Source?.Partialstring(0, StartTagIndex + StartTag.Length - 1) + innerText + Source?.Partialstring(EndTagIndex, Source.Length - 1);
        }
        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>The full text representation of the object.</returns>
        public override string ToString()
        {
            return FullText;
        }
        
        public static TagInfo operator -(TagInfo left, TagInfo right)
        {
            return new TagInfo
            {
                Header = left.Header,
                StartTag = left.EndTag,
                StartTagIndex = left.EndTagIndex,
                EndTag = right.StartTag,
                EndTagIndex = right.StartTagIndex
            };
        }
    }
}

