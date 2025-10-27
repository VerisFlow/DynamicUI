/**
 * ===================================================================
 * AI KNOWLEDGE BASE (GENERATIVE)
 * -------------------------------------------------------------------
 * This file is the single source of truth for an AI tasked with
 * *generating* a valid `ui-config.json` file.
 *
 * It contains:
 * 1. KNOWLEDGE INTERFACES: The schema for this knowledge base itself.
 * 2. ENRICHED DATA SCHEMA: The complete, annotated TypeScript interfaces
 * for `ui-config.json`.
 * 3. CAPABILITIES & RULES: An explicit list of all available "types",
 * "actions", and "plugins" and the rules/dependencies for using them.
 * ===================================================================
 */

// ===================================================================
// SECTION 1: KNOWLEDGE INTERFACES (The Schema for this file)
// ===================================================================

/**
 * Describes a Host-level action (built-in to MainWindow).
 */
interface HostActionInfo {
    /** The string to use for the 'action' property in ControlModel. */
    actionName: string;
    /** A description of what this action does. */
    summary: string;
    /**
     * The properties from ControlModel that this action uses.
     * e.g., 'showmessage' uses 'actionMessage'.
     */
    usedProperties?: readonly (keyof ControlModel)[];
}

/**
 * Describes a creatable control type.
 */
interface ControlTypeInfo {
    /** The string to use for the 'type' property in ControlModel. */
    typeName: string;
    /** A description of what this control is. */
    summary: string;
}

/**
 * Describes the capabilities and dependencies of a single plugin.
 */
interface PluginCapability {
    /** The string to use for `pluginName` in a 'PluginSurface' control. */
    pluginName: string;
    /** A description of what this plugin does. */
    summary: string;
    /** A list of all actions this plugin provides. */
    actions: readonly {
        actionName: string;
        summary: string;
        usedProperties?: readonly (keyof ControlModel)[];
    }[];
    /**
     * CRITICAL: A list of control 'name's that the Host UI *must*
     * provide for this plugin to function correctly.
     */
    hostDependencies: readonly {
        /** The exact 'name' the Host control must have. */
        name: string;
        /** The reason this control is needed (which plugin feature depends on it). */
        reason: string;
    }[];
}

/**
 * The root interface for the "Cookbook" of capabilities.
 */
interface CapabilitiesKnowledge {
    controlTypes: readonly ControlTypeInfo[];
    hostActions: readonly HostActionInfo[];
    availablePlugins: readonly PluginCapability[];
}

// ===================================================================
// SECTION 2: ENRICHED DATA SCHEMA (The "Vocabulary")
// ===================================================================
// This section defines the complete structure of the "ui-config.json" file.
// Generic `string` types have been replaced with specific string literals
// where possible to guide the AI.
// ===================================================================

/**
 * Represents the "app-settings.json" file.
 * NOTE: This is NOT part of the "ui-config.json" file itself.
 */
interface AppSettings {
    "ActiveUiConfigFile"?: string;
}

/**
 * Represents the entire UI configuration deserialized from the ui-config.json file.
 * This is the root object for building the dynamic UI.
 */
interface UiConfig {
    "window"?: WindowConfig;
    "theme"?: ThemeConfig;
    "layoutDefinition"?: LayoutDefinition;
    "controls"?: ControlModel[];
    "dataExport"?: DataExportConfig;
    "dataImport"?: DataImportConfig;
}

/**
 * Represents a single UI control to be created dynamically.
 */
interface ControlModel {
    /**
     * The type of control to create.
     * See `hostCapabilities.controlTypes` for all valid built-in types.
     */
    "type"?:
    | "Label"
    | "TextBlock"
    | "TextBox"
    | "Image"
    | "Border"
    | "Grid"
    | "TabControl"
    | "WrapPanel"
    | "CheckBox"
    | "DataGrid"
    | "ComboBox"
    | "ListBox"
    | "Button"
    | "TitrationPlate"
    | "PluginSurface";

    /** A unique name to identify the control for actions and data mapping. */
    "name"?: string;

    "gridRow": number;
    "gridColumn": number;
    "columnSpan"?: number;
    "rowSpan"?: number;
    "margin"?: string;
    "isVisible"?: boolean;

    /** Valid values: "Left", "Center", "Right", "Stretch" */
    "horizontalAlignment"?: "Left" | "Center" | "Right" | "Stretch";
    /** Valid values: "Top", "Center", "Bottom", "Stretch" */
    "verticalAlignment"?: "Top" | "Center" | "Bottom" | "Stretch";

    "width"?: number;
    "height"?: number;

    /** For DataGrid/ListBox. Valid values: "Single", "Multiple", "Extended" */
    "selectionMode"?: "Single" | "Multiple" | "Extended";

    "text"?: string;
    "label"?: string;
    "content"?: string;
    "fontSize"?: number;
    "isBold"?: boolean;
    "foregroundColor"?: string;
    "backgroundColor"?: string;
    "padding"?: string;

    /** Valid values: "NoWrap", "Wrap" */
    "textWrapping"?: "NoWrap" | "Wrap";

    /**
     * The action to perform.
     * See `hostCapabilities.actions` and `pluginCapabilities` for all valid actions.
     */
    "action"?: string; // See capabilities.hostActions and capabilities.availablePlugins[*].actions
    "actionTarget"?: string;
    "actionMessage"?: string;
    "targetTextBoxName"?: string;
    "events"?: EventActionModel[];
    "isReadOnly"?: boolean;
    "items"?: string[];
    "defaultSelection"?: string;
    "isChecked"?: boolean;
    "source"?: string;

    /** For "PluginSurface" type. See `pluginCapabilities` for valid names. */
    "pluginName"?: string; // See capabilities.availablePlugins[*].pluginName

    "rows"?: number;
    "columns"?: number;
    "rowDefinitions"?: string[];
    "columnDefinitions"?: string[];
    "wellSize"?: number;
    "wellData"?: WellModel[];
    "wellDefaults"?: WellDefaults;
    "wellSpacing"?: number;
    "rowHeaders"?: string[];
    "columnHeaders"?: string[];
    "headerFontSize"?: number;
    "headerFontWeight"?: string;
    "borderColor"?: string;
    "borderThickness"?: string;
    "contentControls"?: ControlModel[];
    "tabItems"?: TabItemModel[];
    "pluginOwner"?: string;
    "logColorMap"?: LogColorMapping[];
    "tag"?: string;
}

/**
 * Defines a complex event-action mapping.
 * (Used by `ControlModel.events`)
 */
interface EventActionModel {
    /** Event name (e.g., "Checked", "Unchecked", "TextChanged") */
    "on"?: string;
    /** Action name (e.g., "togglegridcolumnvisibility") */
    "action"?: string;
    /** The 'Name' of the control to affect */
    "actionTarget"?: string;
}

/**
 * Represents a single TabItem within a TabControl.
 * (Used by `ControlModel.tabItems`)
 */
interface TabItemModel {
    "header"?: string;
    "contentControls"?: ControlModel[];
}

/**
 * Represents a color mapping for specific log levels (e.g., [WARN]).
 * (Used by `ControlModel.logColorMap`)
 */
interface LogColorMapping {
    "prefix"?: string;
    "color"?: string;
}

/**
 * Represents the "window" object in the JSON configuration.
 */
interface WindowConfig {
    "title"?: string;
    "width"?: number;
    "height"?: number;
    /** Valid values: "Width", "Height", "WidthAndHeight", "Manual" */
    "sizeToContent"?: "Width" | "Height" | "WidthAndHeight" | "Manual";
    "minHeight"?: number;
    "minWidth"?: number;
    /** Valid values: "None", "SingleBorderWindow", "ThreeDBorderWindow", "ToolWindow" */
    "windowStyle"?: "None" | "SingleBorderWindow" | "ThreeDBorderWindow" | "ToolWindow";
    "allowsTransparency"?: boolean;
}

/**
 * Represents the "theme" object in the JSON, defining application colors.
 */
interface ThemeConfig {
    "background"?: string;
    "foreground"?: string;
    "controlBackground"?: string;
    "accent"?: string;
    "accentForeground"?: string;
    "borderColor"?: string;
    "errorForeground"?: string;
}

/**
 * Represents the "layoutDefinition" object for the root Grid panel.
 */
interface LayoutDefinition {
    "columnDefinitions"?: string[];
    "rowDefinitions"?: string[];
}

/**
 * Represents the "dataExport" object for saving data to a file.
 */
interface DataExportConfig {
    "targetFile"?: string;
    "targetFileFromControl"?: string;
    "mappings"?: DataMapping[];
    "delimiter"?: string;
    "delimiterFromControl"?: string;
    "includeHeaders"?: boolean;
    "includeHeadersFromControl"?: string;
    /** Valid values: "overwrite", "append" */
    "writeMode"?: "overwrite" | "append";
    "writeModeFromControl"?: string;
}

/**
 * Represents a single mapping between a UI control and a data file column.
 * (Used by `DataExportConfig` and `DataImportConfig`)
 */
interface DataMapping {
    "columnHeader"?: string;
    "sourceControlName"?: string;
}

/**
 * Represents the "dataImport" configuration for loading data into controls.
 */
interface DataImportConfig {
    "sourceFile"?: string;
    "sourceFileFromControl"?: string;
    "parser"?: ParserConfig;
    "query"?: ImportQueryConfig;
    "mappings"?: DataMapping[];
    "headerMappings"?: HeaderMapping[];
}

/**
 * Defines the properties for parsing a file.
 * (Used by `DataImportConfig.parser`)
 */
interface ParserConfig {
    "type"?: string;
    "delimiter"?: string;
    "delimiterFromControl"?: string;
    "headerRowIndex"?: number;
    "headerRowIndexFromControl"?: string;
}

/**
 * Defines the query to find a specific row in the file.
 * (Used by `DataImportConfig.query`)
 */
interface ImportQueryConfig {
    "lookupColumn"?: string;
    "lookupColumnFromControl"?: string;
    "lookupValue"?: string;
    "lookupValueFromControl"?: string;
}

/**
 * Represents a single mapping for renaming a CSV header.
 * (Used by `DataImportConfig.headerMappings`)
 */
interface HeaderMapping {
    "from"?: string;
    "to"?: string;
}

/**
 * Represents default visual properties for wells in a TitrationPlate.
 * (Used by `ControlModel.wellDefaults`)
 */
interface WellDefaults {
    "borderThickness"?: number;
    "fontSize"?: number;
    "fontWeight"?: string;
    "fontColor"?: string;
}

/**
 * Represents a single well in a TitrationPlate.
 * (Used by `ControlModel.wellData`)
 */
interface WellModel {
    "row": number;
    "column": number;
    "label"?: string;
    "color"?: string;
    "tooltip"?: string;
    "fontSize"?: number;
    "fontWeight"?: string;
    "fontColor"?: string;
    "mixtureRatio"?: string;
    "volume"?: string;
}

// ===================================================================
// SECTION 3: CAPABILITIES & RULES (The "Cookbook")
// ===================================================================
// This section provides the explicit rules and available options
// for the AI to use when generating a config.
// ===================================================================

/**
 * Defines all built-in capabilities of the Host application (MainWindow).
 */
export const hostCapabilities: {
    controlTypes: readonly ControlTypeInfo[];
    hostActions: readonly HostActionInfo[];
} = {
    /**
     * All valid values for the 'type' property in a ControlModel
     * that the Host (MainWindow.Factory.cs) can create.
     */
    controlTypes: [
        { typeName: "Label", summary: "A standard text label." },
        { typeName: "TextBlock", summary: "A block of text with wrapping and styling options." },
        { typeName: "TextBox", summary: "A text input box." },
        { typeName: "Image", summary: "Displays an image from a 'source' path." },
        { typeName: "Border", summary: "A container with a border and background. Can hold 'contentControls'." },
        { typeName: "Grid", summary: "A container for 'contentControls' with 'rowDefinitions' and 'columnDefinitions'." },
        { typeName: "TabControl", summary: "A container for 'tabItems'." },
        { typeName: "WrapPanel", summary: "A container for 'contentControls' that wraps them." },
        { typeName: "CheckBox", summary: "A standard checkbox." },
        { typeName: "DataGrid", summary: "A grid to display data collections." },
        { typeName: "ComboBox", summary: "A dropdown selection box." },
        { typeName: "ListBox", summary: "A list of items. Can be styled with 'logColorMap'." },
        { typeName: "Button", summary: "A clickable button that performs an 'action'." },
        { typeName: "TitrationPlate", summary: "A custom control for visualizing a well plate." },
        { typeName: "PluginSurface", summary: "A container that renders the UI of a plugin specified by 'pluginName'." },
    ] as const,

    /**
     * All valid values for the 'action' property in a ControlModel
     * that the Host (MainWindow.Events.cs) can handle directly.
     */
    hostActions: [
        { actionName: "showmessage", summary: "Shows a simple MessageBox. Uses 'actionMessage'.", usedProperties: ["actionMessage"] },
        { actionName: "togglevisibility", summary: "Toggles the Visibility of a control specified by 'actionTarget'.", usedProperties: ["actionTarget"] },
        { actionName: "browseopenfile", summary: "Shows an OpenFileDialog and puts the result in a TextBox specified by 'targetTextBoxName' or 'actionTarget'.", usedProperties: ["targetTextBoxName", "actionTarget"] },
        { actionName: "browsesavefile", summary: "Shows a SaveFileDialog and puts the result in a TextBox specified by 'targetTextBoxName' or 'actionTarget'.", usedProperties: ["targetTextBoxName", "actionTarget"] },
        { actionName: "exportdata", summary: "Triggers the CSV export defined in the root 'dataExport' object (for form controls).", usedProperties: [] },
        { actionName: "exportdata_and_close", summary: "Triggers the CSV export (like 'exportdata') and then closes the window.", usedProperties: [] },
        { actionName: "loaddata", summary: "Triggers the CSV import defined in the root 'dataImport' object (for form controls).", usedProperties: [] },
        { actionName: "loadfiletoviewer", summary: "Loads a full CSV (using 'dataImport' settings) into a DataGrid ('actionTarget') and a ListBox ('actionMessage').", usedProperties: ["actionTarget", "actionMessage"] },
        { actionName: "exportgridselection", summary: "Exports selected rows from a DataGrid ('actionTarget') using selected columns from a ListBox ('actionMessage').", usedProperties: ["actionTarget", "actionMessage"] },
        { actionName: "minimizeWindow", summary: "Minimizes the main window.", usedProperties: [] },
        { actionName: "maximizeRestoreWindow", summary: "Maximizes or restores the main window.", usedProperties: [] },
        { actionName: "closeWindow", summary: "Closes the application.", usedProperties: [] },
        { actionName: "togglegridcolumnvisibility", summary: "Event action (e.g., on CheckBox 'Checked'). Toggles a DataGrid column's visibility. The DataGrid is 'actionTarget', and the CheckBox's 'tag' must be the column's binding name.", usedProperties: ["actionTarget", "tag"] },
    ] as const,
};

/**
 * Defines the capabilities and dependencies of all available plugins.
 */
export const pluginCapabilities: {
    availablePlugins: readonly PluginCapability[];
} = {
    /**
     * A list of all available plugins that can be loaded.
     */
    availablePlugins: [
        {
            pluginName: "TraceLogic Analyzer",
            summary: "A plugin that parses and analyzes .trc trace files.",
            /** Actions this plugin provides (to be used in `action` property) */
            actions: [
                { actionName: "tracelogic_load", summary: "Shows an OpenFileDialog for a .trc file, parses it, and binds the results to the plugin's 'LiquidTransferGrid'.", usedProperties: [] },
                { actionName: "tracelogic_export", summary: "Exports the currently loaded data from the 'LiquidTransferGrid' to a CSV file.", usedProperties: [] },
            ],
            /**
             * CRITICAL: Host-side dependencies.
             * The main `ui-config.json` *must* define these controls for the plugin to work.
             */
            hostDependencies: [
                {
                    name: "WelcomeMessage",
                    reason: "The plugin calls context.SetValue('WelcomeMessage', 'Visibility', ...) to hide a welcome message when data is loaded.",
                },
                {
                    name: "ExportOptionsPanel",
                    reason: "The plugin calls context.SetValue('ExportOptionsPanel', 'Visibility', ...) to show export UI when data is loaded.",
                },
                {
                    name: "ExportButton",
                    reason: "The plugin calls context.SetValue('ExportButton', 'Visibility', ...) to show an export button when data isT loaded.",
                },
                {
                    name: "ColumnSelectorPanel",
                    reason: "The 'tracelogic_export' action depends on context.GetCheckBoxStates('ColumnSelectorPanel') to know which columns to export.",
                },
            ],
        },
        // Future plugins would be added here
    ] as const,
};