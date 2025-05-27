//@BaseCode

namespace TemplateTools.Common.Extensions
{
    /// <summary>
    /// Represents a header for a divide-tag.
    /// </summary>
    /// <remarks>
    /// This class is used for storing and manipulating data related to splitting a string.
    /// </remarks>
    public partial class DivideInfo : TagInfo
    {
        /// <summary>
        /// Creates a new instance of the DivideInfo class with the specified start and end indexes.
        /// </summary>
        /// <param name="startIdx">The start index for the DivideInfo.</param>
        /// <param name="endIdx">The end index for the DivideInfo.</param>
        public DivideInfo(int startIdx, int endIdx)
        {
            StartTag = string.Empty;
            StartTagIndex = startIdx;
            EndTag = string.Empty;
            EndTagIndex = endIdx;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DivideInfo"/> class.
        /// </summary>
        /// <param name="tagInfo">The <see cref="TagInfo"/> object containing the start and end tag information.</param>
        public DivideInfo(TagInfo tagInfo)
        {
            StartTag = tagInfo.StartTag;
            StartTagIndex = tagInfo.StartTagIndex;
            EndTag = tagInfo.EndTag;
            EndTagIndex = tagInfo.EndTagIndex;
        }
        /// <summary>
        /// Gets or sets the text value.
        /// </summary>
        /// <value>The text value.</value>
        public string Text { get; internal set; } = String.Empty;
    }
}
