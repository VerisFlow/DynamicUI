using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Represents a single header rename mapping used during data import.
    /// </summary>
    /// <remarks>
    /// When importing a file (for example CSV) this mapping allows the importer to
    /// translate an original header name (<c>From</c>) found in the file to an
    /// internal header name (<c>To</c>) before applying data mappings or processing.
    /// </remarks>
    public class HeaderMapping
    {
        /// <summary>
        /// The original header name as it appears in the source file.
        /// </summary>
        [JsonPropertyName("from")]
        public string? From { get; set; }

        /// <summary>
        /// The new header name to use internally after renaming.
        /// This value is used in subsequent mapping steps instead of the original header.
        /// </summary>
        [JsonPropertyName("to")]
        public string? To { get; set; }
    }
}