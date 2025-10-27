using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Represents application-level settings loaded from the project's settings file
    /// (for example, <c>app-settings.json</c>).
    /// </summary>
    /// <remarks>
    /// This class holds only a small set of settings used by the host application
    /// to determine which UI configuration file to load at startup.
    /// </remarks>
    public class AppSettings
    {
        /// <summary>
        /// The file name (or relative path) of the active UI configuration file.
        /// This maps to the JSON property <c>"ActiveUiConfigFile"</c> in the settings file.
        /// </summary>
        /// <remarks>
        /// - Nullable: when null or empty the application should fall back to a default UI config.
        /// - Format: typically a file name under the application's <c>ui-configs</c> folder (e.g. "default-ui.json").
        /// </remarks>
        [JsonPropertyName("ActiveUiConfigFile")]
        public string? ActiveUiConfigFile { get; set; }
    }
}