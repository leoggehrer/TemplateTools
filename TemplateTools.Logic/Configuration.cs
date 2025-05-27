//@BaseCode

namespace TemplateTools.Logic
{
    using System.Reflection;
    using TemplateTools.Logic.Common;
    using TemplateTools.Logic.Contracts;
    using TemplateTools.Logic.Models;
    /// <summary>
    /// Represents a configuration for code generation.
    /// </summary>
    public partial class Configuration
    {
        /// <summary>
        /// Initializes the <see cref="Configuration"/> class.
        /// </summary>
        static Configuration()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called before the construction of the class.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        
        #region fields
        private GenerationSetting[]? generationSettings = null;
        #endregion fields
        
        #region properties
        /// <summary>
        /// Gets the array of generation settings.
        /// </summary>
        /// <value>
        /// The generation settings.
        /// </value>
        public GenerationSetting[] GenerationSettings
        {
            get
            {
                if (generationSettings == null)
                {
                    var filePath = Path.Combine(SolutionProperties.SolutionPath, "CodeGeneration.csv");
                    
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            generationSettings = File.ReadAllLines(filePath, System.Text.Encoding.Default)
                                                     .Skip(1)
                                                     .Where(l => l.HasContent() && l.StartsWith('#') == false)
                                                     .Select(l => l.Split(';'))
                                                     .Select(d =>
                            {
                                var gs = new GenerationSetting();
                                
                                try
                                {
                                    gs.UnitType = d[0];
                                    gs.ItemType = d[1];
                                    gs.ItemName = d[2];
                                    gs.Name = d[3];
                                    gs.Value = d[4];
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
                                }
                                return gs;
                            })
                            .ToArray();
                        }
                        catch (Exception ex)
                        {
                            generationSettings = [];
                            System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
                        }
                    }
                    else
                    {
                        generationSettings = [];
                    }
                }
                return generationSettings;
            }
        }
        /// <summary>
        /// Gets or sets the solution properties.
        /// </summary>
        public ISolutionProperties SolutionProperties { get; init; }
        #endregion properties
        
        #region construction
        /// <summary>
        /// Initializes a new instance of the Configuration class with the specified solution properties.
        /// </summary>
        /// <param name="solutionProperties">The solution properties object that represents the configuration of the solution.</param>
        public Configuration(ISolutionProperties solutionProperties)
        {
            Constructing();
            SolutionProperties = solutionProperties;
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of the object.
        /// </summary>
        /// <remarks>
        /// Implement this method in a partial class to add custom logic during the construction process.
        /// </remarks>
        partial void Constructing();
        /// <summary>
        /// This method is called when the object is constructed.
        /// </summary>
        /// <remarks>
        /// This method is automatically called by the framework when the object is constructed. It can be used to perform any initialization tasks required.
        /// </remarks>
        partial void Constructed();
        #endregion construction
        
        #region methods
        /// <summary>
        /// Queries the value of a setting.
        /// </summary>
        /// <typeparam name="T">The type of the desired value.</typeparam>
        /// <param name="unitType">The unit type of the setting.</param>
        /// <param name="itemType">The item type of the setting.</param>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="valueName">The name of the value.</param>
        /// <param name="defaultValue">The default value if the setting is not found or an error occurs.</param>
        /// <returns>The value of the setting with the specified type.</returns>
        /// <remarks>
        /// This method queries the value of a setting with the specified unit type, item type, item name, and value name.
        /// If the setting is not found or an error occurs during the query, the default value specified will be returned.
        /// </remarks>
        public T QuerySettingValue<T>(string unitType, string itemType, string itemName, string valueName, string defaultValue)
        {
            T result;
            
            try
            {
                result = (T)Convert.ChangeType(QuerySettingValue(unitType, itemType, itemName, valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }
        /// <summary>
        /// Queries the value of a setting based on the provided parameters.
        /// </summary>
        /// <param name="unitType">The unit type of the setting.</param>
        /// <param name="itemType">The item type of the setting.</param>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="valueName">The name of the value.</param>
        /// <param name="defaultValue">The default value to return if the setting is not found.</param>
        /// <returns>The value of the setting if found, otherwise the default value.</returns>
        public string QuerySettingValue(UnitType unitType, ItemType itemType, string itemName, string valueName, string defaultValue)
        {
            return QuerySettingValue(unitType.ToString(), itemType.ToString(), itemName, valueName, defaultValue);
        }
        /// <summary>
        /// Retrieves a value for a specific setting from the generation settings list.
        /// </summary>
        /// <param name="unitType">The type of unit.</param>
        /// <param name="itemType">The type of item.</param>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="valueName">The name of the value to retrieve.</param>
        /// <param name="defaultValue">The default value to return if no match is found.</param>
        /// <returns>The retrieved value or the default value if no match is found.</returns>
        public string QuerySettingValue(string unitType, string itemType, string itemName, string valueName, string defaultValue)
        {
            var result = defaultValue;
            var generationSetting = GenerationSettings.FirstOrDefault(e => e.UnitType.Equals(unitType, StringComparison.CurrentCultureIgnoreCase) && e.ItemType == itemType
            && itemName.EndsWith(e.ItemName)
            && e.Name.Equals(valueName, StringComparison.CurrentCultureIgnoreCase));
            
            if (generationSetting != null)
            {
                result = generationSetting.Value;
            }
            return result;
        }
        #endregion methods
    }
}

