using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Defines the root grid's row and column definitions for the main window layout.
    /// </summary>
    /// <remarks>
    /// Each entry in <see cref="ColumnDefinitions"/> and <see cref="RowDefinitions"/> is a string
    /// that represents a WPF <c>GridLength</c>-style value. Common formats:
    /// <list type="bullet">
    /// <item><c>"auto"</c> — size to content</item>
    /// <item><c>"*</c> or <c>"2*"</c> — star sizing (proportional)</item>
    /// <item><c>"50"</c> — pixel value</item>
    /// </list>
    /// Null or an empty list indicates the host should use its default single-row/column layout.
    /// Hosts should parse these strings into concrete <c>RowDefinition</c>/<c>ColumnDefinition</c>
    /// values when constructing the visual tree.
    /// </remarks>
    public class LayoutDefinition
    {
        /// <summary>
        /// Column definitions for the root grid. Each string maps to a Grid column length
        /// (for example: <c>["auto", "*", "2*"]</c> or <c>["100", "*, 50"]</c>).
        /// </summary>
        [JsonPropertyName("columnDefinitions")]
        public List<string>? ColumnDefinitions { get; set; }

        /// <summary>
        /// Row definitions for the root grid. Each string maps to a Grid row length
        /// (for example: <c>["auto", "*", "50"]</c>).
        /// </summary>
        [JsonPropertyName("rowDefinitions")]
        public List<string>? RowDefinitions { get; set; }
    }
}