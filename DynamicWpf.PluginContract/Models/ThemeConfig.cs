using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Defines application-wide color and theme settings used by the host when rendering UI.
    /// </summary>
    /// <remarks>
    /// Color properties typically accept named colors (for example, "Red") or hex color codes
    /// (for example, "#FF0000" or "#AARRGGBB"). Null values indicate that the host should
    /// fall back to its default theme or system colors.
    /// </remarks>
    public class ThemeConfig
    {
        /// <summary>
        /// Application background color (window background).
        /// Example values: "White", "#FFFFFF", "#FF1E1E1E".
        /// If null, host should use its default background.
        /// </summary>
        [JsonPropertyName("background")]
        public string? Background { get; set; }

        /// <summary>
        /// Default foreground color (text color) used across controls.
        /// Example values: "Black", "#000000".
        /// If null, host should use its default foreground color.
        /// </summary>
        [JsonPropertyName("foreground")]
        public string? Foreground { get; set; }

        /// <summary>
        /// Background color for controls (for example, panels, textboxes).
        /// This helps ensure controls are visually distinct from the window background.
        /// </summary>
        [JsonPropertyName("controlBackground")]
        public string? ControlBackground { get; set; }

        /// <summary>
        /// Accent color used for primary interactive elements (for example, buttons, highlights).
        /// </summary>
        [JsonPropertyName("accent")]
        public string? Accent { get; set; }

        /// <summary>
        /// Foreground color to use on top of the <see cref="Accent"/> color (for example, accent button text).
        /// Should provide sufficient contrast with <see cref="Accent"/>.
        /// </summary>
        [JsonPropertyName("accentForeground")]
        public string? AccentForeground { get; set; }

        /// <summary>
        /// Default border color used by controls and panels.
        /// Example values: "Gray", "#FFCCCCCC".
        /// </summary>
        [JsonPropertyName("borderColor")]
        public string? BorderColor { get; set; }

        /// <summary>
        /// Foreground color used to render error state text (for example, validation messages).
        /// Example values: "Red", "#FFFF0000".
        /// </summary>
        [JsonPropertyName("errorForeground")]
        public string? ErrorForeground { get; set; }
    }
}