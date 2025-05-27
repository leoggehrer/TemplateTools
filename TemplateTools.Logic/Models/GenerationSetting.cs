//@BaseCode

namespace TemplateTools.Logic.Models
{
    /// <summary>
    /// Represents a generation setting.
    /// </summary>
    public class GenerationSetting
    {
        /// <summary>
        /// Gets or sets the unit type.
        /// </summary>
        /// <value>
        /// The unit type.
        /// </value>
        public string UnitType { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the type of the item.
        /// </summary>
        /// <value>
        /// The type of the item.
        /// </value>
        public string ItemType { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        /// <value>
        /// The name of the item.
        /// </value>
        public string ItemName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        /// <value>The value of the property.</value>
        public string Value { get; set; } = string.Empty;
        
        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>
        /// A string containing the concatenated values of UnitType, ItemType, ItemName, Name, and Value, separated by hyphens.
        /// </returns>
        public override string ToString()
        {
            return $"{UnitType}-{ItemType}-{ItemName}-{Name}-{Value}";
        }
    }
}

