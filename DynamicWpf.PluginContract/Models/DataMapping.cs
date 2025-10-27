using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Represents a single mapping between a data column in a source file (for example a CSV)
    /// and a destination UI control name. Used by import/export configuration to map file
    /// columns to control values and vice-versa.
    /// </summary>
    public class DataMapping
    {
        /// <summary>
        /// The header name of the column in the source file.
        /// When importing, this identifies which column's value should be read.
        /// When exporting, this is used as the column header (if headers are included).
        /// </summary>
        [JsonPropertyName("columnHeader")]
        public string? ColumnHeader { get; set; }

        /// <summary>
        /// The name of the UI control that supplies or receives the value for this mapping.
        /// This should match the <c>name</c> property of a <see cref="ControlModel"/> in the UI configuration.
        /// </summary>
        [JsonPropertyName("sourceControlName")]
        public string? SourceControlName { get; set; }
    }
}