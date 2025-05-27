//@BaseCode

using TemplateTools.Logic.Common;

namespace TemplateTools.Logic.Models
{
    /// <summary>
    /// Represents a generated item.
    /// </summary>
    internal partial class GeneratedItem : Contracts.IGeneratedItem
    {
        /// <summary>
        /// Initializes a new instance of the GeneratedItem class.
        /// </summary>
        public GeneratedItem()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedItem"/> class.
        /// </summary>
        /// <param name="unitType">The type of unit for the generated item.</param>
        /// <param name="itemType">The type of item for the generated item.</param>
        public GeneratedItem(UnitType unitType, ItemType itemType)
        {
            UnitType = unitType;
            ItemType = itemType;
        }
        /// <summary>
        /// Gets the unit type of the property.
        /// </summary>
        /// <value>The unit type.</value>
        public UnitType UnitType { get; }
        /// <summary>
        /// Gets the type of the item.
        /// </summary>
        /// <value>
        /// The type of the item.
        /// </value>
        public ItemType ItemType { get; }
        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>
        /// A string representing the full name.
        /// </value>
        public string FullName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the sub file path.
        /// </summary>
        /// <value>
        /// The sub file path.
        /// </value>
        public string SubFilePath { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the file extension associated with the file.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> representing the file extension.
        /// </value>
        public string FileExtension { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the source code as an enumerable collection of strings.
        /// </summary>
        /// <value>
        /// The source code as an enumerable collection of strings.
        /// </value>
        public IEnumerable<string> SourceCode => Source;
        
        /// <summary>
        /// Gets the source list of strings.
        /// </summary>
        /// <value>
        /// The source list of strings.
        /// </value>
        public List<string> Source { get; } = [];
        
        /// <summary>
        /// Adds an item to the source.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <remarks>
        /// This method adds the specified item to the source.
        /// </remarks>
        public void Add(string item)
        {
            Source.Add(item);
        }
        /// <summary>
        /// Adds the elements of the specified collection to the end of the source collection.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the source collection.</param>
        /// <remarks>
        /// Any duplicate elements in the collection are appended to the source collection.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">Thrown when the collection parameter is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the collection contains a null element.</exception>
        public void AddRange(IEnumerable<string> collection)
        {
            Source.AddRange(collection);
        }
        /// <summary>
        /// Envelopes the source code with a namespace and optional using statements.
        /// </summary>
        /// <param name="nameSpace">The namespace to be added.</param>
        /// <param name="usings">Optional using statements.</param>
        public void EnvelopeWithANamespace(string nameSpace, params string[] usings)
        {
            var codeLines = new List<string>();
            
            if (nameSpace.HasContent())
            {
                codeLines.Add($"namespace {nameSpace}");
                codeLines.Add("{");
                codeLines.AddRange(usings);
            }
            codeLines.AddRange(Source.Eject());
            if (nameSpace.HasContent())
            {
                codeLines.Add("}");
            }
            Source.AddRange(codeLines);
        }
        /// <summary>
        /// Formats the C# code present in the collection, and adds the formatted code back to the collection.
        /// </summary>
        /// <remarks>
        /// This method uses the "FormatCSharpCode" extension method to format the C# code in the collection.
        /// It extracts the code from the collection, formats it, and then adds the formatted code back to the collection.
        /// </remarks>
        public void FormatCSharpCode()
        {
            Source.AddRange(Extensions.FormatterExtensions.FormatCSharpSyntax(Source.Eject()));
        }
        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object. The string includes the unit type, item type, and full name.
        /// </returns>
        public override string ToString()
        {
            return $"{UnitType,-15}{ItemType,-20}{FullName,-30}";
        }
    }
}

