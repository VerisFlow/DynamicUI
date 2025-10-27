using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Defines a mapping between a UI event (for example, "Checked" or "Click")
    /// and an action the host or a plugin should perform when that event occurs.
    /// </summary>
    /// <remarks>
    /// Instances of this model are typically deserialized from UI configuration JSON
    /// and used by the runtime to wire events to actions that manipulate other controls
    /// or trigger application behavior.
    /// </remarks>
    public class EventActionModel
    {
        /// <summary>
        /// The event name to listen for on the control (for example, "Checked", "Click", "SelectionChanged").
        /// </summary>
        [JsonPropertyName("on")]
        public string? On { get; set; }

        /// <summary>
        /// The name of the action to perform when the event fires (for example, "enablecontrol", "settext", "export").
        /// Hosts may implement a set of built-in actions and allow plugins to provide additional actions.
        /// </summary>
        [JsonPropertyName("action")]
        public string? Action { get; set; }

        /// <summary>
        /// The target control name (the <c>Name</c> value from a <see cref="ControlModel"/>) that the action should affect.
        /// If the action does not require a target this value may be null or empty.
        /// </summary>
        [JsonPropertyName("actionTarget")]
        public string? ActionTarget { get; set; }
    }
}