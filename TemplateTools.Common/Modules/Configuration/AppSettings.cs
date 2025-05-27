//@BaseCode
using Microsoft.Extensions.Configuration;

namespace TemplateTools.Common.Modules.Configuration
{
    /// <summary>
    /// Singleton class to manage application settings.
    /// </summary>
    public sealed partial class AppSettings : Contracts.ISettings
    {
        #region fields
        private static readonly AppSettings _instance = new();
        private static readonly IConfigurationRoot _configurationRoot;
        #endregion fields

        /// <summary>
        /// Static constructor to initialize the configuration.
        /// </summary>
        static AppSettings()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environmentName ?? "Development"}.json", optional: true)
                    .AddEnvironmentVariables();

            _configurationRoot = builder.Build();
        }

        #region properties
        /// <summary>
        /// Gets the singleton instance of the AppSettings class.
        /// </summary>
        public static AppSettings Instance => _instance;
        #endregion properties

        /// <summary>
        /// Private constructor to prevent instantiation.
        /// </summary>
        private AppSettings()
        {
        }

        /// <summary>
        /// Indexer to get the configuration value by key.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value.</returns>
        public string? this[string key] => _configurationRoot[key];
    }
}
