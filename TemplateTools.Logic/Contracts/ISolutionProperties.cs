//@BaseCode
namespace TemplateTools.Logic.Contracts
{
    /// <summary>
    /// Represents a set of solution properties that describe various aspects of a software solution.
    /// </summary>
    public interface ISolutionProperties
    {
        #region project extensions
        /// <summary>
        /// Gets the common extension of the property.
        /// </summary>
        string CommonExtension { get; }
        /// <summary>
        /// Gets the logic extension of the property.
        /// </summary>
        string LogicExtension { get; }
        /// <summary>
        /// Gets the WebApiExtension property.
        /// </summary>
        string WebApiExtension { get; }
        /// <summary>
        /// Gets the Angular extension associated with the property.
        /// </summary>
        string AngularExtension { get; }
        /// <summary>
        /// Gets the MVVMExtension property value.
        /// </summary>
        string MVVMExtension { get; }
        #endregion project extensions
        
        #region properties
        ///<summary>
        /// Gets the path of the solution.
        ///</summary>
        string SolutionPath { get; }
        ///<summary>
        /// Gets the name of the solution.
        ///</summary>
        string SolutionName { get; }
        ///<summary>
        ///Gets the file path of the solution.
        ///</summary>
        string SolutionFilePath { get; }
        /// <summary>
        /// Gets or sets the compile path for the string.
        /// </summary>
        string? CompilePath { get; set; }
        /// <summary>
        /// Gets or sets an array of types representing logic assemblies.
        /// </summary>
        Type[] LogicAssemblyTypes { get; set; }
        /// <summary>
        /// Gets the file path of the compiled logic assembly.
        /// </summary>
        string? CompileLogicAssemblyFilePath { get; }
        
        /// <summary>
        /// Gets the collection of template project names.
        /// </summary>
        IEnumerable<string> TemplateProjectNames { get; }
        /// <summary>
        /// Gets the collection of all template project names.
        /// </summary>
        IEnumerable<string> AllTemplateProjectNames { get; }
        /// <summary>
        /// Gets the collection of paths to template projects.
        /// </summary>
        IEnumerable<string> TemplateProjectPaths { get; }
        
        /// <summary>
        /// Gets the file path of the logic C# project.
        /// </summary>
        string LogicCSProjectFilePath { get; }
        /// <summary>
        /// Gets the file path of the logic assembly.
        /// </summary>
        string LogicAssemblyFilePath { get; }
        /// <summary>
        /// Gets the common project name.
        /// </summary>
        string CommonProjectName { get; }
        /// <summary>
        /// Gets the logic project name.
        /// </summary>
        string LogicProjectName { get; }
        ///<summary>
        /// Gets the logic sub path.
        ///</summary>
        string LogicSubPath { get; }
        /// <summary>
        /// Gets the subpath for logic controllers.
        /// </summary>
        string LogicControllersSubPath { get; }
        /// <summary>
        /// Gets the sub path of the LogicDataContext.
        /// </summary>
        string LogicDataContextSubPath { get; }
        /// <summary>
        /// Gets the sub path for the logic entities.
        /// </summary>
        string LogicEntitiesSubPath { get; }
        
        /// <summary>
        /// Gets the name of the Web Api project.
        /// </summary>
        string WebApiProjectName { get; }
        /// <summary>
        /// Gets the path of the WebApi.
        /// </summary>
        string WebApiSubPath { get; }
        /// <summary>
        /// Gets the subpath of the web API controllers.
        /// </summary>
        string WebApiControllersSubPath { get; }
        
        /// <summary>
        /// Gets the name of the MVVM application project.
        /// </summary>
        string MVVMAppProjectName { get; }
        /// <summary>
        /// Gets the subpath of the MVVM application.
        /// </summary>
        string MVVMAppSubPath { get; }
        
        /// <summary>
        /// Gets the name of the Angular app project.
        /// </summary>
        /// <value>The name of the Angular app project.</value>
        string AngularAppProjectName { get; }
        #endregion properties
        
        #region methods
        /// <summary>
        /// Checks if the given file path corresponds to a template project file.
        /// </summary>
        /// <param name="filePath">The path of the file to be checked.</param>
        /// <returns>True if the file is a template project file; otherwise, false.</returns>
        bool IsTemplateProjectFile(string filePath);
        /// <summary>
        /// Retrieves the name of the project from the given project path.
        /// </summary>
        /// <param name="projectPath">The full path of the project.</param>
        /// <returns>The name of the project.</returns>
        string GetProjectNameFromPath(string projectPath);
        /// <summary>
        /// Retrieves the project name from the given file path.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        /// <returns>The project name extracted from the file path.</returns>
        string GetProjectNameFromFile(string filePath);
        #endregion methods
    }
}

