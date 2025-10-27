using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Configuration that controls how an input data file is parsed (for example CSV).
    /// </summary>
    /// <remarks>
    /// Typical usage: deserialize parser settings from the UI configuration and use them
    /// when reading an input file. Properties with the suffix <c>FromControl</c> indicate
    /// that the effective value can be obtained at runtime from the named control; when provided,
    /// the control-sourced value overrides the corresponding static property.
    /// </remarks>
    public class ParserConfig
    {
        /// <summary>
        /// The parser type (for example, <c>"csv"</c>).
        /// Hosts may support additional parser types (for example, <c>"tsv"</c>); default is <c>"csv"</c>.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; } = "csv";

        /// <summary>
        /// The delimiter used to split fields when parsing text-based formats (default is <c>","</c>).
        /// If <see cref="DelimiterFromControl"/> is set, the runtime value from that control takes precedence.
        /// </summary>
        [JsonPropertyName("delimiter")]
        public string? Delimiter { get; set; } = ",";

        /// <summary>
        /// The name of a control whose current value should be used as the delimiter at runtime.
        /// The control value is interpreted as the delimiter string and overrides <see cref="Delimiter"/>.
        /// </summary>
        [JsonPropertyName("delimiterFromControl")]
        public string? DelimiterFromControl { get; set; }

        /// <summary>
        /// Zero-based index of the header row in the source file. Default is <c>0</c> (first row).
        /// Null indicates no explicit header row; hosts should decide the appropriate behavior.
        /// </summary>
        [JsonPropertyName("headerRowIndex")]
        public int? HeaderRowIndex { get; set; } = 0;

        /// <summary>
        /// The name of a control whose current value supplies the header row index at runtime.
        /// The control value is expected to be parseable as an integer and overrides <see cref="HeaderRowIndex"/>.
        /// </summary>
        [JsonPropertyName("headerRowIndexFromControl")]
        public string? HeaderRowIndexFromControl { get; set; }
    }
}