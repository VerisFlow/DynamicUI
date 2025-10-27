using System.Text.Json.Serialization;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Represents a single, dynamically-created UI control described in a UI configuration file.
    /// Instances of this model are used by the host to construct WPF controls (for example: "Button", "TextBox", "Grid").
    /// </summary>
    /// <remarks>
    /// Most properties map directly to JSON properties (see <see cref="JsonPropertyNameAttribute"/> on each member).
    /// Many values are optional (nullable) — the host should apply sensible defaults when a property is not provided.
    /// </remarks>
    public class ControlModel
    {
        /// <summary>
        /// The WPF control type to instantiate (e.g., "Button", "TextBox", "Grid", "StackPanel").
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// A unique identifier for the control within the UI configuration.
        /// This name can be used by actions or other controls to reference this control.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Grid row index used when this control is placed inside a Grid.
        /// Zero-based. Host should treat missing or negative values as row 0.
        /// </summary>
        [JsonPropertyName("gridRow")]
        public int GridRow { get; set; }

        /// <summary>
        /// Grid column index used when this control is placed inside a Grid.
        /// Zero-based. Host should treat missing or negative values as column 0.
        /// </summary>
        [JsonPropertyName("gridColumn")]
        public int GridColumn { get; set; }

        /// <summary>
        /// Number of columns to span within a Grid. Null means a span of 1.
        /// </summary>
        [JsonPropertyName("columnSpan")]
        public int? ColumnSpan { get; set; }

        /// <summary>
        /// Number of rows to span within a Grid. Null means a span of 1.
        /// </summary>
        [JsonPropertyName("rowSpan")]
        public int? RowSpan { get; set; }

        /// <summary>
        /// Margin around the control. Typical formats: "5" or "10,5" or "left,top,right,bottom".
        /// Host should parse and apply the margin string to the control.
        /// </summary>
        [JsonPropertyName("margin")]
        public string? Margin { get; set; }

        /// <summary>
        /// Whether the control is visible. Null means visible (true) by default.
        /// </summary>
        [JsonPropertyName("isVisible")]
        public bool? IsVisible { get; set; } = true;

        /// <summary>
        /// Horizontal alignment: "Left", "Center", "Right", or "Stretch".
        /// Null indicates the default alignment for the control type.
        /// </summary>
        [JsonPropertyName("horizontalAlignment")]
        public string? HorizontalAlignment { get; set; }

        /// <summary>
        /// Vertical alignment: "Top", "Center", "Bottom", or "Stretch".
        /// Null indicates the default alignment for the control type.
        /// </summary>
        [JsonPropertyName("verticalAlignment")]
        public string? VerticalAlignment { get; set; }

        /// <summary>
        /// Explicit width in device-independent units. Null means width is not explicitly set.
        /// </summary>
        [JsonPropertyName("width")]
        public double? Width { get; set; }

        /// <summary>
        /// Explicit height in device-independent units. Null means height is not explicitly set.
        /// </summary>
        [JsonPropertyName("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Selection mode for list-style controls (e.g., "Single", "Multiple").
        /// </summary>
        [JsonPropertyName("selectionMode")]
        public string? SelectionMode { get; set; }

        /// <summary>
        /// Text content used by controls such as TextBlock or TextBox.
        /// </summary>
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        /// <summary>
        /// Label text typically displayed alongside the control (for labeled controls).
        /// </summary>
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        /// <summary>
        /// Content for content controls such as Button or Label. Often treated as plain text.
        /// </summary>
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        /// <summary>
        /// Font size in points. Null means use control default.
        /// </summary>
        [JsonPropertyName("fontSize")]
        public int? FontSize { get; set; }

        /// <summary>
        /// When true, text is rendered bold. Null means use control default weight.
        /// </summary>
        [JsonPropertyName("isBold")]
        public bool? IsBold { get; set; }

        /// <summary>
        /// Foreground color (examples: "Red", "#FF0000"). Host should support named colors and hex values.
        /// </summary>
        [JsonPropertyName("foregroundColor")]
        public string? ForegroundColor { get; set; }

        /// <summary>
        /// Background color (examples: "Blue", "#0000FF"). Host should support named colors and hex values.
        /// </summary>
        [JsonPropertyName("backgroundColor")]
        public string? BackgroundColor { get; set; }

        /// <summary>
        /// Padding inside the control. Typical formats: "5" or "10,5" or "left,top,right,bottom".
        /// </summary>
        [JsonPropertyName("padding")]
        public string? Padding { get; set; }

        /// <summary>
        /// Text wrapping behavior for text controls: "NoWrap", "Wrap", etc.
        /// </summary>
        [JsonPropertyName("textWrapping")]
        public string? TextWrapping { get; set; }

        /// <summary>
        /// Name of an action associated with the control (for example, the action performed on click).
        /// </summary>
        [JsonPropertyName("action")]
        public string? Action { get; set; }

        /// <summary>
        /// The target control name for the action (when the action needs to act on another control).
        /// </summary>
        [JsonPropertyName("actionTarget")]
        public string? ActionTarget { get; set; }

        /// <summary>
        /// Optional message or payload passed to the action handler.
        /// </summary>
        [JsonPropertyName("actionMessage")]
        public string? ActionMessage { get; set; }

        /// <summary>
        /// Name of a TextBox control that should receive values related to this control (e.g., a selected file path).
        /// </summary>
        [JsonPropertyName("targetTextBoxName")]
        public string? TargetTextBoxName { get; set; }

        /// <summary>
        /// List of event-to-action mappings. Each entry defines which event to listen for and which action to invoke.
        /// </summary>
        [JsonPropertyName("events")]
        public List<EventActionModel>? Events { get; set; }

        /// <summary>
        /// When true, makes a TextBox read-only. Null means the control's default behavior.
        /// </summary>
        [JsonPropertyName("isReadOnly")]
        public bool? IsReadOnly { get; set; }

        /// <summary>
        /// Items for selector controls such as ComboBox or ListBox.
        /// </summary>
        [JsonPropertyName("items")]
        public List<string>? Items { get; set; }

        /// <summary>
        /// Default selected item for selector controls.
        /// </summary>
        [JsonPropertyName("defaultSelection")]
        public string? DefaultSelection { get; set; }

        /// <summary>
        /// Checked state for Checkbox or RadioButton. Null means use the control default.
        /// </summary>
        [JsonPropertyName("isChecked")]
        public bool? IsChecked { get; set; }

        /// <summary>
        /// Source path or URI for an Image control.
        /// </summary>
        [JsonPropertyName("source")]
        public string? Source { get; set; }

        /// <summary>
        /// Name of the plugin that is related to this control (used when actions are handled by plugins).
        /// </summary>
        [JsonPropertyName("pluginName")]
        public string? PluginName { get; set; }

        /// <summary>
        /// Rows for controls that represent a grid of items (for example, a TitrationPlate).
        /// </summary>
        [JsonPropertyName("rows")]
        public int? Rows { get; set; }

        /// <summary>
        /// Columns for controls that represent a grid of items (for example, a TitrationPlate).
        /// </summary>
        [JsonPropertyName("columns")]
        public int? Columns { get; set; }

        /// <summary>
        /// Row definitions for a Grid control. Example: ["auto", "*", "50"].
        /// </summary>
        [JsonPropertyName("rowDefinitions")]
        public List<string>? RowDefinitions { get; set; }

        /// <summary>
        /// Column definitions for a Grid control. Example: ["auto", "*", "2*"].
        /// </summary>
        [JsonPropertyName("columnDefinitions")]
        public List<string>? ColumnDefinitions { get; set; }

        /// <summary>
        /// Size (diameter) of wells for a TitrationPlate control, in device-independent units.
        /// </summary>
        [JsonPropertyName("wellSize")]
        public double? WellSize { get; set; }

        /// <summary>
        /// Per-well data for a TitrationPlate control.
        /// </summary>
        [JsonPropertyName("wellData")]
        public List<WellModel>? WellData { get; set; }

        /// <summary>
        /// Default visual properties applied to wells when individual well values are not specified.
        /// </summary>
        [JsonPropertyName("wellDefaults")]
        public WellDefaults? WellDefaults { get; set; }

        /// <summary>
        /// Spacing between wells in a TitrationPlate control.
        /// </summary>
        [JsonPropertyName("wellSpacing")]
        public double? WellSpacing { get; set; }

        /// <summary>
        /// Optional text headers for each row in a TitrationPlate.
        /// </summary>
        [JsonPropertyName("rowHeaders")]
        public List<string>? RowHeaders { get; set; }

        /// <summary>
        /// Optional text headers for each column in a TitrationPlate.
        /// </summary>
        [JsonPropertyName("columnHeaders")]
        public List<string>? ColumnHeaders { get; set; }

        /// <summary>
        /// Font size for headers in TitrationPlate (null = default).
        /// </summary>
        [JsonPropertyName("headerFontSize")]
        public double? HeaderFontSize { get; set; }

        /// <summary>
        /// Font weight for headers (e.g., "Bold", "Normal"). Host should map to actual WPF FontWeight.
        /// </summary>
        [JsonPropertyName("headerFontWeight")]
        public string? HeaderFontWeight { get; set; }

        /// <summary>
        /// Border color for the control (e.g., "Black", "#000000").
        /// </summary>
        [JsonPropertyName("borderColor")]
        public string? BorderColor { get; set; }

        /// <summary>
        /// Border thickness. Typical formats: "1" or "1,2,1,2".
        /// </summary>
        [JsonPropertyName("borderThickness")]
        public string? BorderThickness { get; set; }

        /// <summary>
        /// Child controls for container controls (e.g., Grid, StackPanel, Border).
        /// When this control is a container, the host should render these children inside it.
        /// </summary>
        [JsonPropertyName("contentControls")]
        public List<ControlModel>? ContentControls { get; set; }

        /// <summary>
        /// Tab items when this control is a TabControl. Each TabItemModel contains header and content controls.
        /// </summary>
        [JsonPropertyName("tabItems")]
        public List<TabItemModel>? TabItems { get; set; }

        /// <summary>
        /// Name of the plugin that owns or should handle actions for this control when the action is not a built-in host action.
        /// </summary>
        [JsonPropertyName("pluginOwner")]
        public string? PluginOwner { get; set; }

        /// <summary>
        /// Color mapping rules used by log-style display controls to colorize log lines by prefix.
        /// </summary>
        [JsonPropertyName("logColorMap")]
        public List<LogColorMapping>? LogColorMap { get; set; }

        /// <summary>
        /// Arbitrary tag string for storing custom metadata. Host does not interpret this value.
        /// </summary>
        [JsonPropertyName("tag")]
        public string? Tag { get; set; }
    }
}