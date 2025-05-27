//@BaseCode

namespace TemplateTools.Logic.Generation
{
    using System.Reflection;
    using TemplateTools.Logic.Contracts;
    using TemplateTools.Logic.Models;

    /// <summary>
    /// Represents a common generator that is responsible for generating logic-related code.
    /// </summary>
    internal sealed partial class CommonGenerator : ModelGenerator
    {
        #region fields
        private ItemProperties? _itemProperties;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets or sets the ItemProperties for the current instance.
        /// </summary>
        /// <value>
        /// The ItemProperties for the current instance.
        /// </value>
        protected override ItemProperties ItemProperties => _itemProperties ??= new ItemProperties(SolutionProperties.SolutionName, StaticLiterals.CommonExtension);

        /// <summary>
        /// Gets or sets a value indicating whether all entity contracts should be generated.
        /// </summary>
        public bool GenerateEntityContracts { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all view contracts should be generated.
        /// </summary>
        public bool GenerateViewContracts { get; set; }
        #endregion properties

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicGenerator"/> class.
        /// </summary>
        /// <param name="solutionProperties">The solution properties.</param>
        public CommonGenerator(ISolutionProperties solutionProperties) : base(solutionProperties)
        {
            var generateAll = QuerySetting<string>(Common.ItemType.AllItems, StaticLiterals.AllItems, StaticLiterals.Generate, "True");

            GenerateEntityContracts = QuerySetting<bool>(Common.ItemType.EntityContract, StaticLiterals.AllItems, StaticLiterals.Generate, generateAll);
            GenerateViewContracts = QuerySetting<bool>(Common.ItemType.ViewContract, StaticLiterals.AllItems, StaticLiterals.Generate, generateAll);
        }
        #endregion constructors

        #region generations
        /// <summary>
        /// Generates all the required items for the common project.
        /// </summary>
        /// <returns>An enumerable list of generated items.</returns>
        public IEnumerable<IGeneratedItem> GenerateAll()
        {
            var result = new List<IGeneratedItem>();

            result.AddRange(CreateEntityContracts());

            return result;
        }
        /// <summary>
        ///   Determines whether the specified type should generate default values.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is not a generation entity; otherwise, <c>false</c>.
        /// </returns>
        private static bool GetGenerateDefault(Type type)
        {
            return EntityProject.IsCustomEntity(type);
        }

        /// <summary>
        /// Creates entity contracts.
        /// </summary>
        /// <returns>An enumerable collection of generated items.</returns>
        private List<GeneratedItem> CreateEntityContracts()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            foreach (var type in entityProject.AllEntityTypes)
            {
                var defaultValue = (GenerateEntityContracts && GetGenerateDefault(type)).ToString();

                if (CanCreate(type) 
                    && QuerySetting<bool>(Common.ItemType.EntityContract, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(CreateEntityContract(type, Common.UnitType.Common, Common.ItemType.EntityContract));
                }
            }

            foreach (var type in entityProject.AllViewTypes)
            {
                var defaultValue = (GenerateViewContracts && GetGenerateDefault(type)).ToString();

                if (CanCreate(type)
                    && QuerySetting<bool>(Common.ItemType.ViewContract, type, StaticLiterals.Generate, defaultValue))
                {
                    result.Add(CreateEntityContract(type, Common.UnitType.Common, Common.ItemType.ViewContract));
                }
            }
            return result;
        }
        #endregion generations

        #region query settings
        /// <summary>
        /// Queries a setting value and converts it to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to which the setting value will be converted.</typeparam>
        /// <param name="itemType">The common item type.</param>
        /// <param name="type">The type of the setting value.</param>
        /// <param name="valueName">The name of the setting value.</param>
        /// <param name="defaultValue">The default value to use if the setting value cannot be queried or converted.</param>
        /// <returns>
        /// The queried setting value converted to the specified type. If the setting value cannot be queried or converted,
        /// the default value will be returned.
        /// </returns>
        /// <remarks>
        /// If an exception occurs during the query or conversion process, the default value will be returned
        /// and the error message will be written to the debug output.
        /// </remarks>
        private T QuerySetting<T>(Common.ItemType itemType, Type type, string valueName, string defaultValue)
        {
            T result;

            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(Common.UnitType.Logic, itemType, ItemProperties.CreateSubTypeFromEntity(type), valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }
        /// <summary>
        /// Executes a query to retrieve a setting value and returns the result as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to which the setting value should be converted.</typeparam>
        /// <param name="itemType">The type of item to query for the setting value.</param>
        /// <param name="itemName">The name of the item to query for the setting value.</param>
        /// <param name="valueName">The name of the value to query for.</param>
        /// <param name="defaultValue">The default value to return if the query fails or the value cannot be converted.</param>
        /// <returns>The setting value as the specified type.</returns>
        private T QuerySetting<T>(Common.ItemType itemType, string itemName, string valueName, string defaultValue)
        {
            T result;

            try
            {
                result = (T)Convert.ChangeType(QueryGenerationSettingValue(Common.UnitType.Common, itemType, itemName, valueName, defaultValue), typeof(T));
            }
            catch (Exception ex)
            {
                result = (T)Convert.ChangeType(defaultValue, typeof(T));
                System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }
            return result;
        }
        #endregion query settings
    }
}

