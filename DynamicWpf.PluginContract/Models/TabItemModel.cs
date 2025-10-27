using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Represents a single <c>TabItem</c> used inside a <c>TabControl</c>.
    /// </summary>
    /// <remarks>
    /// Instances are typically deserialized from a UI configuration file and used by the host
    /// to create the tab header and render the contained controls when the tab is selected.
    /// </remarks>
    public class TabItemModel
    {
        /// <summary>
        /// The text displayed in the tab header.
        /// </summary>
        /// <remarks>
        /// If null or empty the host may choose to render an icon, an empty header, or use a default label.
        /// </remarks>
        [JsonPropertyName("header")]
        public string? Header { get; set; }

        /// <summary>
        /// The list of controls to be rendered inside this tab item.
        /// </summary>
        /// <remarks>
        /// Each entry is a <see cref="ControlModel"/> describing a UI element or a container.
        /// When this collection is null or empty the tab will be rendered with no content.
        /// </remarks>
        [JsonPropertyName("contentControls")]
        public List<ControlModel>? ContentControls { get; set; }
    }
}