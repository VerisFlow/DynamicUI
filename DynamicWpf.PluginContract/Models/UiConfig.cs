using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Root model that represents the entire UI configuration.
    /// </summary>
    /// <remarks>
    /// Typical usage: deserialize a JSON file (for example, <c>ui-config.json</c>) into this type
    /// and pass the instance to the host renderer which will construct the window, apply theme,
    /// build the layout and materialize controls and data import/export behaviors.
    /// </remarks>
    public class UiConfig
    {
        /// <summary>
        /// Configuration values for the main application window (title, size, style, etc.).
        /// </summary>
        [JsonPropertyName("window")]
        public WindowConfig? Window { get; set; }

        /// <summary>
        /// Theme and color settings applied application-wide (background, accent, error color, etc.).
        /// </summary>
        [JsonPropertyName("theme")]
        public ThemeConfig? Theme { get; set; }

        /// <summary>
        /// Defines the root grid's row and column definitions used to structure the main layout.
        /// </summary>
        [JsonPropertyName("layoutDefinition")]
        public LayoutDefinition? LayoutDefinition { get; set; }

        /// <summary>
        /// Collection of control definitions that the host will instantiate.
        /// Each <see cref="ControlModel"/> describes a single UI element or a container with children.
        /// </summary>
        [JsonPropertyName("controls")]
        public List<ControlModel>? Controls { get; set; }

        /// <summary>
        /// Optional configuration that describes how to export UI data to a file (for example CSV).
        /// </summary>
        [JsonPropertyName("dataExport")]
        public DataExportConfig? DataExport { get; set; }

        /// <summary>
        /// Optional configuration that describes how to import data from a file into UI controls.
        /// </summary>
        [JsonPropertyName("dataImport")]
        public DataImportConfig? DataImport { get; set; }
    }
}