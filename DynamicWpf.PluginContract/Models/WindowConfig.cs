using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Configuration for the application's main window.
    /// </summary>
    /// <remarks>
    /// Instances of this class are typically deserialized from a UI configuration file
    /// and used by the host to initialize the main window's appearance and sizing.
    /// Sizes are expressed in WPF device-independent units (1/96 inch per unit).
    /// </remarks>
    public class WindowConfig
    {
        /// <summary>
        /// The window title text displayed in the title bar.
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// The desired window width in device-independent units.
        /// Null means the host should use its default or let the layout determine width.
        /// </summary>
        [JsonPropertyName("width")]
        public int? Width { get; set; }

        /// <summary>
        /// The desired window height in device-independent units.
        /// Null means the host should use its default or let the layout determine height.
        /// </summary>
        [JsonPropertyName("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Controls automatic sizing behavior.
        /// Expected values (examples): <c>"Width"</c>, <c>"Height"</c>, <c>"WidthAndHeight"</c>, <c>"Manual"</c>.
        /// When set to a size-to-content mode the host should size the window to its content accordingly.
        /// </summary>
        [JsonPropertyName("sizeToContent")]
        public string? SizeToContent { get; set; }

        /// <summary>
        /// Minimum allowed window height in device-independent units.
        /// Null means no specific minimum is enforced by configuration.
        /// </summary>
        [JsonPropertyName("minHeight")]
        public double? MinHeight { get; set; }

        /// <summary>
        /// Minimum allowed window width in device-independent units.
        /// Null means no specific minimum is enforced by configuration.
        /// </summary>
        [JsonPropertyName("minWidth")]
        public double? MinWidth { get; set; }

        /// <summary>
        /// The window style that affects borders and the title bar.
        /// Common values include <c>"None"</c>, <c>"SingleBorderWindow"</c>, <c>"ThreeDBorderWindow"</c>, and <c>"ToolWindow"</c>.
        /// Hosts should map this string to the appropriate WPF <c>WindowStyle</c> value.
        /// </summary>
        [JsonPropertyName("windowStyle")]
        public string? WindowStyle { get; set; }

        /// <summary>
        /// Indicates whether the window allows transparency.
        /// Note: in WPF transparency is typically effective only when <see cref="WindowStyle"/> is set to <c>"None"</c>;
        /// hosts should also ensure proper background settings and be aware of platform limitations.
        /// </summary>
        [JsonPropertyName("allowsTransparency")]
        public bool? AllowsTransparency { get; set; }
    }
}   