# TraceLogic.Adapter

**Source Code:** `https://github.com/VerisFlow/TraceLogicLocal`

## Overview

This project is a plugin for the `DynamicWpf` application. It serves as an adapter, implementing the `IDynamicPlugin` interface to expose the functionality of an external `TraceLogic.Core` library to the main host application.

This is a .NET 8 project that acts as the "glue" between the generic UI host and the specialized trace-parsing business logic.

## Dependencies

This project relies on two key dependencies:

1.  **`DynamicWpf.PluginContract.csproj`**: This is the shared contract library. This project references it to implement the `IDynamicPlugin` interface and to use the `IPluginContext` for interacting with the host UI.
2.  **`TraceLogic.Core.dll`**: This is a pre-compiled external library (added as a `Reference`) that contains the actual business logic for parsing and exporting trace files.

## Core Functionality

The plugin is implemented in the `TraceLogicAdapter` class and identifies itself to the host as the **"TraceLogic Analyzer"**.

### Internal State

The adapter maintains an internal state to manage data between actions:

* `_analysisResult` (`TraceAnalysisResult?`): This private field holds the complete parsed analysis result after a user loads a file. This allows subsequent actions, like 'Export', to operate on the same data without re-parsing.

### Plugin Interface Implementation

The adapter implements the `IDynamicPlugin` interface through two primary methods:

* **`public List<ControlModel> GetControls()`**
    This method is called by the host application at startup. It returns a list of `ControlModel` definitions that describe the UI elements this plugin requires the host to render.
* **`public void HandleAction(string actionName, IPluginContext context)`**
    This is the central action router. When a user interacts with one of the plugin's UI controls, the host calls this method with the corresponding `actionName`. The method then routes the request to the appropriate private helper method.

### Key Actions & Internal Methods

The `HandleAction` method delegates work to two main private methods:

1.  **`ProcessFile(IPluginContext context)`**
    * **Purpose:** Prompts the user to select a trace file, parses it, and updates the host UI.
    * **Workflow:**
        1.  Calls `context.ShowOpenFileDialog()` to get a file path from the user.
        2.  Uses the external `TraceFileParser.Parse()` method (from `TraceLogic.Core`) to analyze the file.
        3.  Stores the parsed data in the `_analysisResult` state variable.
        4.  Calls `context.BindDataToGrid()` to send the parsed data to a `DataGrid` in the host UI.
        5.  Calls `context.SetValue()` to update other host UI controls (e.g., showing/hiding panels).
        6.  Uses `context.ShowMessage()` to report errors if parsing fails.

2.  **`ExportData(IPluginContext context)`**
    * **Purpose:** Exports the currently loaded analysis data to a CSV file, based on user-selected columns.
    * **Workflow:**
        1.  Checks if `_analysisResult` contains data. If not, it shows an error message (`context.ShowMessage()`).
        2.  Calls `context.GetCheckBoxStates()` to get a list of columns the user has selected from a container of CheckBoxes in the host UI.
        3.  Calls `context.ShowSaveFileDialog()` to get a destination file path.
        4.  Uses the external `DataExporter.ExportToCsv()` (from `TraceLogic.Core`) to save the data.

## Build Output

The project is configured to build its output DLL directly into the `..\Output\plugins` directory. This allows the host application to dynamically discover and load the plugin at runtime.