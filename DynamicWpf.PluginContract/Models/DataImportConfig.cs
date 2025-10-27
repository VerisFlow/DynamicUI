using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Configuration for importing data from a file into UI controls.
    /// </summary>
    /// <remarks>
    /// Properties with the suffix <c>FromControl</c> indicate that the effective
    /// value can be obtained from another control at runtime (the string is interpreted
    /// as the control name whose current value supplies the configuration).
    /// </remarks>
    public class DataImportConfig
    {
        /// <summary>
        /// Static source file path to read data from (for example, a CSV file).
        /// If <see cref="SourceFileFromControl"/> is provided, the runtime value from that control takes precedence.
        /// </summary>
        [JsonPropertyName("sourceFile")]
        public string? SourceFile { get; set; }

        /// <summary>
        /// Name of a control whose current value should be used as the source file path at runtime.
        /// When present, this overrides <see cref="SourceFile"/>.
        /// </summary>
        [JsonPropertyName("sourceFileFromControl")]
        public string? SourceFileFromControl { get; set; }

        /// <summary>
        /// Parser configuration that defines how to parse the input file (for example, delimiter, parser type, header row index).
        /// See <see cref="ParserConfig"/> for available parser options.
        /// </summary>
        [JsonPropertyName("parser")]
        public ParserConfig? Parser { get; set; }

        /// <summary>
        /// Optional query configuration that can be used to locate a specific row to import (for example, by matching a lookup column/value).
        /// See <see cref="ImportQueryConfig"/> for lookup options and <c>FromControl</c> variants.
        /// </summary>
        [JsonPropertyName("query")]
        public ImportQueryConfig? Query { get; set; }

        /// <summary>
        /// Mappings that map file columns to UI control names.
        /// Each <see cref="DataMapping"/> normally specifies the source column header and the destination control name.
        /// </summary>
        [JsonPropertyName("mappings")]
        public List<DataMapping>? Mappings { get; set; }

        /// <summary>
        /// Optional header mappings to rename headers from the source file before applying <see cref="Mappings"/>.
        /// Each <see cref="HeaderMapping"/> specifies an original header (<c>from</c>) and the desired header name (<c>to</c>).
        /// </summary>
        [JsonPropertyName("headerMappings")]
        public List<HeaderMapping>? HeaderMappings { get; set; }
    }
}