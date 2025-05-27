//@BaseCode

using TemplateTools.Logic.Common;
using TemplateTools.Logic.Contracts;
using TemplateTools.Logic.Models;

namespace TemplateTools.Logic.Generation
{
    /// <summary>
    /// Represents a diagram generator class that generates entity diagrams.
    /// </summary>
    /// <param name="solutionProperties">The solution properties.</param>
    internal sealed partial class DiagramGenerator(ISolutionProperties solutionProperties) : GeneratorObject(solutionProperties)
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
        internal ItemProperties ItemProperties => _itemProperties ??= new ItemProperties(SolutionProperties.SolutionName, StaticLiterals.LogicExtension);
        #endregion properties

        /// <summary>
        /// Generates entity diagrams for all entity types in the entity project.
        /// </summary>
        /// <returns>
        /// An enumerable collection of <see cref="GeneratedItem"/> representing the generated entity diagrams.
        /// </returns>
        public IEnumerable<GeneratedItem> GenerateEntityDiagrams()
        {
            var result = new List<GeneratedItem>();
            var entityProject = EntityProject.Create(SolutionProperties);

            // Create entity type diagrams
            foreach (var type in entityProject.AllEntityTypes)
            {
                result.Add(GenerateClassDiagram(type, UnitType.Logic, ItemType.EntityClassDiagram));
            }
            return result;
        }
        /// <summary>
        /// Generates a class diagram for a given type.
        /// </summary>
        /// <param name="type">The type for which to generate the class diagram.</param>
        /// <param name="unitType">The unit type of the generated item.</param>
        /// <param name="itemType">The item type of the generated item.</param>
        /// <returns>The generated class diagram as a <see cref="GeneratedItem"/>.</returns>
        public static GeneratedItem GenerateClassDiagram(Type type, UnitType unitType, ItemType itemType)
        {
            var fileName = $"{type.Name}{StaticLiterals.PlantUmlFileExtension}";
            var diagramSubFilePath = Path.Combine(ItemProperties.CreateSubPathFromType(type), fileName);
            var result = new Models.GeneratedItem(unitType, itemType)
            {
                FullName = type.FullName!,
                FileExtension = StaticLiterals.PlantUmlFileExtension,
                SubFilePath = diagramSubFilePath,
            };
            result.Source.AddRange(CommonModules.PlantUML.UMLCreator.CreateClassDiagram(CommonModules.PlantUML.DiagramCreationFlags.All, [type]));
            return result;
        }
    }
}
