using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Defines a query used to locate a specific row in an imported data file.
    /// </summary>
    /// <remarks>
    /// The query typically identifies a lookup column and a lookup value to match
    /// a single row (for example, find the row where the "Id" column equals "42").
    /// Properties with the suffix <c>FromControl</c> indicate the effective value
    /// can be obtained at runtime from the named control; when present, the
    /// <c>FromControl</c> value takes precedence over the corresponding static value.
    /// </remarks>
    public class ImportQueryConfig
    {
        /// <summary>
        /// The name of the source file column to use for the lookup (for example, a header name).
        /// </summary>
        [JsonPropertyName("lookupColumn")]
        public string? LookupColumn { get; set; }

        /// <summary>
        /// The name of a control whose current value supplies the lookup column name at runtime.
        /// When provided, this control-supplied value overrides <see cref="LookupColumn"/>.
        /// </summary>
        [JsonPropertyName("lookupColumnFromControl")]
        public string? LookupColumnFromControl { get; set; }

        /// <summary>
        /// The value to match in the lookup column when locating the target row.
        /// </summary>
        [JsonPropertyName("lookupValue")]
        public string? LookupValue { get; set; }

        /// <summary>
        /// The name of a control whose current value supplies the lookup value at runtime.
        /// When provided, this control-supplied value overrides <see cref="LookupValue"/>.
        /// </summary>
        [JsonPropertyName("lookupValueFromControl")]
        public string? LookupValueFromControl { get; set; }
    }
}