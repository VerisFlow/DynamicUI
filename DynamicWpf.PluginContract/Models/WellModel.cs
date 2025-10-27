using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Represents a single well cell in a TitrationPlate-like control.
    /// </summary>
    /// <remarks>
    /// Instances are typically deserialized from UI configuration JSON and used by the host
    /// to render each well's appearance and metadata (label, color, tooltip, etc.).
    /// Row and Column are zero-based indices that locate the well within the plate.
    /// </remarks>
    public class WellModel
    {
        /// <summary>
        /// Zero-based row index of the well within the plate.
        /// </summary>
        [JsonPropertyName("row")]
        public int Row { get; set; }

        /// <summary>
        /// Zero-based column index of the well within the plate.
        /// </summary>
        [JsonPropertyName("column")]
        public int Column { get; set; }

        /// <summary>
        /// Optional display label rendered inside or beside the well.
        /// </summary>
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        /// <summary>
        /// Background color for the well (named color or hex code, e.g. "Red" or "#FF0000").
        /// </summary>
        [JsonPropertyName("color")]
        public string? Color { get; set; }

        /// <summary>
        /// Optional tooltip text shown on mouse hover.
        /// </summary>
        [JsonPropertyName("tooltip")]
        public string? Tooltip { get; set; }

        /// <summary>
        /// Font size used for the well label (device-independent units). Null uses the host default.
        /// </summary>
        [JsonPropertyName("fontSize")]
        public double? FontSize { get; set; }

        /// <summary>
        /// Font weight for the label (for example, "Normal", "Bold"). Host should map to an actual FontWeight.
        /// </summary>
        [JsonPropertyName("fontWeight")]
        public string? FontWeight { get; set; }

        /// <summary>
        /// Font color for the label (named color or hex code).
        /// </summary>
        [JsonPropertyName("fontColor")]
        public string? FontColor { get; set; }

        /// <summary>
        /// Optional mixture ratio metadata (format is application-specific, e.g. "1:2" or "50%/50%").
        /// </summary>
        [JsonPropertyName("mixtureRatio")]
        public string? MixtureRatio { get; set; }

        /// <summary>
        /// Optional volume metadata for the well (string to allow units, e.g. "10µL").
        /// </summary>
        [JsonPropertyName("volume")]
        public string? Volume { get; set; }
    }
}