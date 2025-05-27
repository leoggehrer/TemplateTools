//@BaseCode

namespace TemplateTools.Logic.Contracts
{
    /// <summary>
    /// Represents a generated item.
    /// </summary>
    public interface IGeneratedItem
    {
        /// <summary>
        /// Gets the unit type of the common instance.
        /// </summary>
        /// <value>
        /// The unit type.
        /// </value>
        Common.UnitType UnitType { get; }
        /// <summary>
        /// Gets the type of the item.
        /// </summary>
        /// <returns>
        /// The type of the item.
        /// </returns>
        Common.ItemType ItemType { get; }
        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <remarks>
        /// This property represents the full name of an entity.
        /// </remarks>
        /// <returns>The full name of the entity.</returns>
        string FullName { get; }
        /// <summary>
        /// Gets the sub file path.
        /// </summary>
        /// <value>The sub file path.</value>
        string SubFilePath { get; }
        ///<summary>
        /// Gets the file extension.
        ///</summary>
        ///
        ///<value>
        /// The file extension.
        ///</value>
        string FileExtension { get; }
        /// <summary>
        /// Gets the source code.
        /// </summary>
        /// <value>
        /// An enumerable collection of string representing the source code.
        /// </value>
        IEnumerable<string> SourceCode { get; }
    }
}

