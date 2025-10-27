using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Default visual properties applied to wells when individual well values are not specified.
    /// </summary>
    /// <remarks>
    /// Instances of this class are typically provided as part of a control configuration
    /// (for example, a TitrationPlate) so the host can apply consistent defaults for appearance.
    /// Null property values indicate the host should fall back to its own defaults or styles.
    /// </remarks>
    public class WellDefaults
    {
        /// <summary>
        /// Default border thickness for wells, in device-independent units (1/96 inch per unit).
        /// Null means the host should use its default border thickness.
        /// </summary>
        [JsonPropertyName("borderThickness")]
        public double? BorderThickness { get; set; }

        /// <summary>
        /// Default font size for well labels, in device-independent units.
        /// Null means the host should use its default font size.
        /// </summary>
        [JsonPropertyName("fontSize")]
        public double? FontSize { get; set; }

        /// <summary>
        /// Default font weight for well labels (for example, "Normal" or "Bold").
        /// The host should map this string to an actual FontWeight value.
        /// Null means the host should use its default font weight.
        /// </summary>
        [JsonPropertyName("fontWeight")]
        public string? FontWeight { get; set; }

        /// <summary>
        /// Default font color for well labels. Accepts named colors (for example, "Black")
        /// or hex color codes (for example, "#FF0000" or "#AARRGGBB").
        /// Null means the host should use its default font color.
        /// </summary>
        [JsonPropertyName("fontColor")]
        public string? FontColor { get; set; }
    }
}