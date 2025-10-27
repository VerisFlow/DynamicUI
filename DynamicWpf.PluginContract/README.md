# DynamicWpf.PluginContract

This project is the central shared library (contract) for the DynamicWpfFromConfig application. It defines the common data models and interfaces necessary for the main host application and any external plugins to communicate.

Both the host application and all plugin projects **must** reference this library.

## Project Structure

This is a .NET 8 library project. It contains two primary categories of definitions: Plugin Interfaces and Configuration Models.

### Plugin Interfaces

These are the core interfaces that define the plugin system.

* `IDynamicPlugin.cs`: The primary interface that all external plugins must implement. This serves as the entry point for the plugin.
* `IPluginContext.cs`: Defines the contract for the "context" object provided by the host application to a plugin. This allows a plugin to safely interact with the host, such as getting/setting values or triggering actions.
* `IPluginDataGridColumnInfo.cs`: A specialized interface used for plugins that need to provide custom column definitions for a `DataGrid` control.

### Configuration Models (`Models/`)

This folder contains all the C# classes that represent the deserialized `ui-config.json` file. These models define the complete structure of the UI.

* `UiConfig.cs`: The root object representing the entire `ui-config.json` file.
* `ControlModel.cs`: The most important model, defining a single UI element (like a Button, TextBox, or Grid) and all its possible properties (layout, appearance, actions, etc.).
* `WindowConfig.cs`: Defines properties for the main application window (title, size, etc.).
* `ThemeConfig.cs`: Defines application-wide theme colors.
* `LayoutDefinition.cs`: Defines the root grid's row and column definitions.
* `DataExportConfig.cs` / `DataImportConfig.cs`: Models that define the configuration for data import and export operations.
* **Supporting Models:** Various other models are included to support complex controls and configurations, such as:
    * `TabItemModel.cs`
    * `WellModel.cs` / `WellDefaults.cs`
    * `DataMapping.cs`
    * `EventActionModel.cs`
    * `ParserConfig.cs`
    * And others...