using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Maps a log line prefix to a display color used by log-style controls.
    /// </summary>
    /// <remarks>
    /// Instances are typically provided in a UI configuration file to instruct the host
    /// how to colorize log entries. The <see cref="Prefix"/> is matched against the start
    /// of each log line; when a match is found the host should render that line using the
    /// specified <see cref="Color"/>. Hosts may treat the prefix match as case-sensitive
    /// or case-insensitive — document and implement consistent behavior.
    ///
    /// Color values commonly accept named colors (for example, "Red") or hex color codes
    /// (for example, "#FFFF00" or "#AARRGGBB"). If <see cref="Color"/> is null or invalid
    /// the host should fall back to a default foreground color.
    /// </remarks>
    public class LogColorMapping
    {
        /// <summary>
        /// Log entry prefix to match (for example, "[WARN]", "ERROR:", or "DEBUG").
        /// </summary>
        /// <remarks>
        /// The host should compare this value against the beginning of each log line.
        /// Leading/trailing whitespace in the configured prefix should be considered
        /// significant unless the host explicitly trims values.
        /// </remarks>
        [JsonPropertyName("prefix")]
        public string? Prefix { get; set; } // e.g., "[WARN]"


        /// <summary>
        /// The color to apply when <see cref="Prefix"/> matches a log line.
        /// </summary>
        /// <remarks>
        /// Acceptable formats include named colors ("Red") or hex values ("#RRGGBB" or "#AARRGGBB").
        /// Hosts should validate and gracefully handle invalid color strings.
        /// </remarks>
        [JsonPropertyName("color")]
        public string? Color { get; set; } // e.g., "#FFFF00"
    }
}