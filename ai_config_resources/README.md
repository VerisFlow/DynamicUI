# AI Configuration Generation Assets

This directory contains the knowledge base, templates, and examples required for an AI assistant to generate configuration files and integration code.

The primary goal is to use a generative AI to create the necessary assets to run a standalone, configuration-driven UI application (`DynamicWpfFromConfig`) as a dialog within a Hamilton Venus Method.

## File Descriptions

### 1. `ui.knowledgebase.ts` (UI Knowledge Base)

* **Purpose:** This file is the **single source of truth** for the `DynamicWpfFromConfig` application. It defines the complete, valid schema for any `ui-config.json` file.
* **Content:** It contains TypeScript interfaces detailing all available control types (e.g., `Button`, `Grid`), host actions (e.g., `exportdata_and_close`), plugin capabilities, and schema rules.
* **Usage:** An AI should use this file as its primary reference or context when tasked with **generating any new `ui-config.json` file**. This is the general "dictionary" and "rulebook" for the UI.

### 2. `ShellDialogPrompt.template.md` (HSL Library Prompt Template)

* **Purpose:** This is a "few-shot" prompt template designed to generate the Hamilton-specific *integration code*. This code bridges the gap between the Venus method and the external UI application.
* **Content:** It instructs the AI to generate two Hamilton Standard Language (HSL) files (`.hs_` and `.hsi`). These files create a library that, when called from Venus, does the following:
    1.  Executes the `DynamicWpfFromConfig` application (which loads a `ui-config.json`).
    2.  Waits for the UI application to close.
    3.  Reads the CSV file (e.g., `run-config.csv`) that the UI application *writes to*.
    4.  Parses the CSV data and passes it to the Venus method's output variables.
* **Usage:** This is the core template for integrating the UI into Venus. It **requires** a specific set of inputs, such as the `CSV_Header` names and output parameter names.

### 3. `UiConfigPrompt.example.md` (HSL-Specific UI Config Prompt)

* **Purpose:** This file is **not a general example**. It is a specific example prompt that shows how to generate a `ui-config.json` file that is **perfectly matched for use with `ShellDialogPrompt.template.md`**.
* **Content:** It demonstrates how to request a UI config where the control `name` properties and `dataExport` mappings (like `columnHeader`) are explicitly defined.
* **Usage:** Use this as a reference *only when* generating a config for the Hamilton Venus integration workflow. The `CSV_Header`s defined in this prompt (and in the resulting `ui-config.json`) are the **exact** values you must provide as inputs to the `ShellDialogPrompt.template.md`.

## Overall Workflow (Venus Integration)

This is the specific workflow for creating a new dialog for Hamilton Venus:

1.  A developer decides they need a new, complex user dialog in their Venus method.
2.  They write a new prompt (based on `UiConfigPrompt.example.md`) to define their UI. This prompt **must** clearly state the `CSV_Header` names that will be used for data export (e.g., "Enable_LibPrep", "ParaFloat").
3.  The AI uses this specific prompt **and** the general `ui.knowledgebase.ts` (for validation) to generate the `ui-config.json`.
4.  The developer takes the `ShellDialogPrompt.template.md` and provides its required inputs. Crucially, the `FunctionParameters` (like `CSV_Header`) **must exactly match** the `columnHeader` names from the `ui-config.json` generated in step 3.
5.  The AI uses this filled-in template to generate the `.hs_` and `.hsi` library files.
6.  The developer places the `ui-config.json`, the `DynamicUI` executable, and the new HSL library files into their Venus method project.

**Note:** If you only want to generate a `ui-config.json` for a different purpose (not Venus integration), you should **not** use `UiConfigPrompt.example.md`. Instead, write your own prompt and instruct the AI to use `ui.knowledgebase.ts` as its guide.