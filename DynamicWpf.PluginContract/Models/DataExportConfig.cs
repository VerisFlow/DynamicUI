using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Configuration for exporting data from UI controls to a file (for example CSV).
    /// </summary>
    /// <remarks>
    /// Properties with the suffix <c>FromControl</c> indicate that the effective
    /// value can be read from a control at runtime (the string value is interpreted
    /// as a control name whose current value supplies the configuration).
    /// </remarks>
    public class DataExportConfig
    {
        /// <summary>
        /// Static target file path where exported data will be written.
        /// If <see cref="TargetFileFromControl"/> is set, that value takes precedence at runtime.
        /// </summary>
        [JsonPropertyName("targetFile")]
        public string? TargetFile { get; set; }

        /// <summary>
        /// Name of a control whose current value should be used as the target file path.
        /// When provided, this control-sourced value overrides <see cref="TargetFile"/>.
        /// </summary>
        [JsonPropertyName("targetFileFromControl")]
        public string? TargetFileFromControl { get; set; }

        /// <summary>
        /// Mapping definitions that determine which UI control values map to which output columns.
        /// Each <see cref="DataMapping"/> specifies a <c>columnHeader</c> and the <c>sourceControlName</c>.
        /// </summary>
        [JsonPropertyName("mappings")]
        public List<DataMapping>? Mappings { get; set; }

        /// <summary>
        /// Delimiter used between values in the exported file (default is comma: ",").
        /// If <see cref="DelimiterFromControl"/> is set, the control value overrides this.
        /// </summary>
        [JsonPropertyName("delimiter")]
        public string? Delimiter { get; set; } = ",";

        /// <summary>
        /// Name of a control whose current value supplies the delimiter string at runtime.
        /// Overrides <see cref="Delimiter"/> when provided.
        /// </summary>
        [JsonPropertyName("delimiterFromControl")]
        public string? DelimiterFromControl { get; set; }

        /// <summary>
        /// Whether to include column headers in the exported output (default is true).
        /// If <see cref="IncludeHeadersFromControl"/> is provided, that control's value is used instead.
        /// </summary>
        [JsonPropertyName("includeHeaders")]
        public bool? IncludeHeaders { get; set; } = true;

        /// <summary>
        /// Name of a control whose value determines whether to include headers in the export.
        /// Expected to be a value that can be interpreted as a boolean.
        /// </summary>
        [JsonPropertyName("includeHeadersFromControl")]
        public string? IncludeHeadersFromControl { get; set; }

        /// <summary>
        /// File write behavior: commonly <c>"overwrite"</c> or <c>"append"</c>. Default is <c>"overwrite"</c>.
        /// If <see cref="WriteModeFromControl"/> is set, the control's value overrides this setting.
        /// </summary>
        [JsonPropertyName("writeMode")]
        public string? WriteMode { get; set; } = "overwrite"; // e.g., "overwrite" or "append"

        /// <summary>
        /// Name of a control whose value supplies the write mode at runtime.
        /// Expected values mirror <see cref="WriteMode"/> (for example, "append" or "overwrite").
        /// </summary>
        [JsonPropertyName("writeModeFromControl")]
        public string? WriteModeFromControl { get; set; }
    }
}