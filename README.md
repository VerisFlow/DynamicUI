# Dynamic WPF UI from JSON

## Overview

This is a WPF application solution whose core feature is the dynamic generation of its user interface entirely from JSON configuration files. It supports a plugin architecture for extensibility and includes built-in, flexible CSV data import/export capabilities.

This design makes it possible to completely change the application's appearance, layout, and functionality without recompiling the code.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Download & Installation

You can download the latest pre-compiled demo package from the **[Releases]** page on GitHub.

1.  Download the `DynamicWpfDemo-v[version]-win-x64.zip` file (e.g., `v1.0.0`).
2.  Unzip the package to a convenient location. **It is critical to keep the internal folder structure intact.**
3.  **Prerequisite: Install .NET 8.0**
    * Before running the application, you **must** install the **[.NET 8.0 Desktop Runtime (x64)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)**, as this is a framework-dependent deployment.
4.  **Package Contents**
    The unzipped folder contains the complete demo. The Hamilton method is configured to use relative paths to find all necessary components.
    * `/UiApplication/`: Contains the main executable (`DynamicWpfFromConfig.exe`), `app-settings.json`, sample `ui-configs/`, and `plugins/`.
    * `/Deck/`: Contains the example Hamilton VENUS deck layout file (`.lay`).
    * `/Library/`: Contains the HSL library files (`.hs_`, `.hsi`) needed for the integration.
    * `/Method/`: Contains the example Hamilton VENUS method file (`.med`).
5.  **How to Use**
    * **To run the standalone UI:** Navigate to the `UiApplication` folder and run `DynamicWpfFromConfig.exe`.
    * **To use the Hamilton Integration:** Open your Hamilton VENUS software and directly open the `.med` file located inside the unzipped `/Method/` folder. The method will work as long as the `UiApplication`, `Deck`, and `Library` folders remain in the same relative locations.

## Core Features

* **JSON-Driven UI:** Window styles, layout (Grid rows/columns), controls, properties, and themes are all defined by JSON files.
* **Plugin System:** Supports loading external `.dll` plugins from the `/plugins` directory to extend functionality.
* **Dynamic Data Operations:**
    * Includes a flexible CSV data importer that supports dynamically getting parsing parameters from UI controls.
    * Supports querying and loading data into form controls.
    * Includes a flexible CSV data exporter, supporting the export of form data or selected `DataGrid` rows.
* **Composite Controls:** Includes custom-rendered controls like `TitrationPlate` and `PluginSurface`.
* **Events and Actions:** Supports binding "Actions" to controls within the JSON, which can be handled by the main window or routed to a plugin.

## Execution Requirements

To run the compiled `DynamicWpfFromConfig.exe` application, the following file structure is required in the output directory:
```
(Application Root) 
	|-- DynamicWpfFromConfig.exe 
	|-- app-settings.json 
	|-- /ui-configs/ 
	| 		|-- ui-config.json (or as specified in app-settings) 
	|
	|-- /plugins/ 
	| 		|-- TraceLogic.Adapter.dll 
	| 		|-- TraceLogic.Core.dll (dependency for the adapter)
```

* **`app-settings.json`**: This file is mandatory. It must contain the `ActiveUiConfigFile` key pointing to the UI definition file to load.
* **`/ui-configs/`**: This directory must contain the JSON configuration file specified in `app-settings.json`.
* **`/plugins/`**: This directory must exist and holds all plugin DLLs (like `TraceLogic.Adapter.dll`) and their dependencies (like `TraceLogic.Core.dll`).

## Current Capabilities & Future Development

The application provides a foundational set of features that can be expanded.

* **Built-in Host Actions:** The main application handles a limited set of core actions, including:
    * `showmessage`
    * `togglevisibility`
    * `loadfiletoviewer` (load CSV to DataGrid)
    * `exportgridselection` (export DataGrid selection)
    * `exportdata` (export form data)
    * `loaddata` (load form data)
    * (and other file browsing/window actions)

* **Included Plugins:** The solution currently includes one example plugin:
    * `TraceLogic Analyzer`: (from `TraceLogic.Adapter.csproj`) Demonstrates file parsing and data binding to the host UI.

* **Future Development:** This project is a proof-of-concept. Further development is needed to support a wider range of built-in UI controls, more complex host actions, and a more robust plugin API.

## Solution Projects

This solution consists of three main projects that work together to create the application.

### 1. `DynamicWpfFromConfig` (The Host Application)

This is the main WPF application and the solution's entry point.

* **Responsibility:** Its primary job is to read the JSON configuration, build the UI, and load plugins.
* **Internal Structure:** The main `MainWindow` logic is split into multiple `partial class` files for better organization (e.g., `MainWindow.Factory.cs` for creating controls, `MainWindow.Data.cs` for CSV logic, `MainWindow.Plugins.cs` for plugin management).

### 2. `DynamicWpf.PluginContract` (The Shared Contract)

This is the central shared library that defines the "contract" for communication.

* **Requirement:** Both the host application (`DynamicWpfFromConfig`) and all plugin projects (like `TraceLogic.Adapter`) **must** reference this library.
* **Plugin Interfaces:** It defines the core interfaces the plugin system relies on:
    * `IDynamicPlugin.cs`
    * `IPluginContext.cs`
* **Configuration Models:** This project also contains all the C# classes (e.g., `UiConfig.cs`, `ControlModel.cs`, `DataExportConfig.cs`) that represent the deserialized `ui-config.json` file.

### 3. `TraceLogic.Adapter` (Example Plugin)

This is an example plugin that demonstrates how to extend the application's functionality.

* **Purpose:** It acts as an "adapter" to expose the functionality of an external `TraceLogic.Core.dll` library to the host application.
* **Implementation:** It implements the `IDynamicPlugin` interface.
* **Build:** The project is configured to output its DLL directly into the `..\Output\plugins` directory, allowing the host application to find and load it at runtime.

## AI Development Assets (ai_config_resources/)

This repository includes a folder named `ai_config_resources` which contains assets for using Generative AI to accelerate development.

Its primary goal is to provide the necessary "knowledge" and "templates" for an AI to generate:

1.  New `ui-config.json` files for this application.
2.  The HSL (Hamilton Standard Language) integration code (`.hs_` / `.hsi` files) required to use this application as a dialog within a Hamilton Venus method.

Key files in this folder include:

* **`ui.knowledgebase.ts`**: The "single source of truth" for the UI config schema. An AI should use this as context to ensure any generated JSON is valid.
* **`ShellDialogPrompt.template.md`**: A "few-shot" prompt template for generating the HSL integration code.
* **`UiConfigPrompt.example.md`**: A specific example prompt showing how to generate a `ui-config.json` that is correctly configured for the Hamilton HSL integration workflow.

Please refer to the `README.md` file *inside* that folder for a detailed explanation of the workflow.

## Application Workflow

1.  The `DynamicWpfFromConfig` application starts.
2.  It reads `app-settings.json` to find the value of `ActiveUiConfigFile`.
3.  It loads the specified `.json` file from the `/ui-configs/` directory.
4.  It deserializes this JSON into the C# models defined in `DynamicWpf.PluginContract`.
5.  The application loads all plugin DLLs found in the `/plugins` folder.
6.  `MainWindow.Factory.cs` iterates through the `ControlModel` list and builds the UI.
7.  If a control is a `PluginSurface`, the host asks the specified plugin (`IDynamicPlugin`) to provide its controls.
8.  When a user triggers an action, `MainWindow.Events.cs` either handles it as a built-in "Host Action" or routes it to the correct plugin's `HandleAction` method.
9.  The plugin (e.g., `TraceLogic.Adapter`) performs its logic and uses the `IPluginContext` to communicate results back to the host UI.