# UI Configuration Models Reference

This directory (`/Models`) contains the C# data models used to deserialize the application's `ui-config.json` configuration file.

Following C# best practices, each public class is in its own file (e.g., `UiConfig.cs`). This README serves as a high-level guide and "map" to understand how these models fit together to form the complete configuration.

## 1. High-Level Structure (The "Big Picture")

The entire configuration is loaded into the `UiConfig` class. This is the root object.

| Class File | JSON Property | Description |
| :--- | :--- | :--- |
| `UiConfig.cs` | (Root Object) | The main class that holds all other configuration sections. |
| `WindowConfig.cs` | `window` | Defines the main window's properties (title, size, style). |
| `ThemeConfig.cs` | `theme` | Defines application-wide colors (background, accent, etc.). |
| `LayoutDefinition.cs` | `layoutDefinition` | Defines the root `Grid` panel's row and column structure. |
| `ControlModel.cs` | `controls` | **(Core)** A list of all UI elements to be rendered in the window. |
| `DataExportConfig.cs` | `dataExport` | Configuration for saving UI data to an external file. |
| `DataImportConfig.cs` | `dataImport` | Configuration for loading external data into UI controls. |

---

## 2. Core UI Definition (`ControlModel` and its Children)

This is the most important set of models, defining the "what" and "where" of your UI.

### `ControlModel.cs`
This is the most critical class. An instance of `ControlModel` represents a single UI element (e.g., a `Button`, `TextBox`, `Grid`, `ComboBox`, or a custom plugin). It contains properties for:

* **Identity:** `type`, `name`
* **Layout:** `gridRow`, `gridColumn`, `columnSpan`, `margin`, `horizontalAlignment`, etc.
* **Content:** `text`, `label`, `content`, `fontSize`, `isBold`, `foregroundColor`.
* **Input State:** `isReadOnly`, `items` (for lists), `isChecked` (for checkboxes).
* **Actions:** `action`, `actionTarget`, `pluginOwner` (defines what happens on click).
* **Containers:** `contentControls` (a list of *nested* `ControlModel`s if this is a `Grid` or `StackPanel`).

### Sub-Models used by `ControlModel`

These classes are used as properties *within* `ControlModel` to define more complex behaviors.

| Class File | Used in `ControlModel` Property | Purpose |
| :--- | :--- | :--- |
| `EventActionModel.cs` | `events` (List) | Defines a complex event-to-action link. (e.g., "On `Checked`, perform `enablecontrol` on `myTextBox`"). |
| `TabItemModel.cs` | `tabItems` (List) | Defines a single tab when `ControlModel.type` is "TabControl". Contains `header` and `contentControls`. |
| `LogColorMapping.cs` | `logColorMap` (List) | Used by logging controls to map a log prefix (e.g., `[WARN]`) to a display color. |

---

## 3. Data Import & Export Models

These models define the configuration for the `dataImport` and `dataExport` sections of the `UiConfig`.

### `DataExportConfig.cs`
* **Purpose:** The main configuration for *saving* data. Defines the `targetFile`, `delimiter`, `writeMode` (overwrite/append), and a list of `DataMapping`s.

### `DataImportConfig.cs`
* **Purpose:** The main configuration for *loading* data. Defines the `sourceFile` and nests other configuration objects: `ParserConfig`, `ImportQueryConfig`, and lists of `DataMapping`s / `HeaderMapping`s.

### Helper Models for Data I/O

| Class File | Used By | Purpose |
| :--- | :--- | :--- |
| `DataMapping.cs` | `DataExportConfig`, `DataImportConfig` | The key link between UI and data. Maps a `columnHeader` in the file to a `sourceControlName` in the UI. |
| `ParserConfig.cs` | `DataImportConfig.parser` | Tells the importer *how* to read the file (e.g., `type: "csv"`, `delimiter: ","`, `headerRowIndex: 0`). |
| `ImportQueryConfig.cs`| `DataImportConfig.query` | Defines how to find a *specific row* to import (e.g., "find row where `lookupColumn` 'ID' has `lookupValue` '123'"). |
| `HeaderMapping.cs` | `DataImportConfig.headerMappings` | Allows renaming columns during import (e.g., map `from: "user_name"` in the file `to: "UserName"` for internal use). |

---

## 4. Specialized Control Models (TitrationPlate)

These models are used by the `ControlModel` for a specific, complex custom control (likely a "TitrationPlate").

| Class File | Used in `ControlModel` Property | Purpose |
| :--- | :--- | :--- |
| `WellModel.cs` | `wellData` (List) | Defines the data and appearance for a *single well* in the plate (e.g., `row`, `column`, `label`, `color`, `volume`). |
| `WellDefaults.cs` | `wellDefaults` | Sets the default visual properties (like `fontSize`) for *all* wells in the plate, which can be overridden by an individual `WellModel`. |

---

## 5. Application Settings (Special Case)

### `AppSettings.cs`
* **Purpose:** This class is *not* part of `ui-config.json`. It represents the `app-settings.json` file, which is used by the application to find *which* `ui-config.json` file to load (e.g., `ActiveUiConfigFile`).

---

## 6. Model Relationship Diagram

This tree shows how the models are nested, starting from the `UiConfig` root.
```
[ UiConfig ] 
	| 
	+-- [ WindowConfig ] 
	| 
	+-- [ ThemeConfig ] 
	| 
	+-- [ LayoutDefinition ] 
	| 
	+-- [ DataExportConfig ] 
	|		| 
	|		+-- [ DataMapping ] (List)
	| 
	+-- [ DataImportConfig ] 
	|		| 
	|		+-- [ ParserConfig ] 
	|		| 
	|		+-- [ ImportQueryConfig ] 
	|		| 
	|		+-- [ DataMapping ] (List) 
	|		| 
	|		+-- [ HeaderMapping ] (List)  
	| 
	+-- [ ControlModel ] (List) 
			| 
			+-- [ EventActionModel ] (List) 
			|
			+-- [ LogColorMapping ] (List) 
			| 
			+-- [ WellDefaults ] 
			| 
			+-- [ WellModel ] (List) 
			| 
			+-- [ TabItemModel ] (List) 
			|		| 
			|		+-- [ ControlModel ] (List) <-- Recursive 
			| 
			+-- [ ControlModel ] (List, as "contentControls") <-- Recursive
```