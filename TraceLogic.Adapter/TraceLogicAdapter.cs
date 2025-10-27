using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DynamicWpfFromConfig.Models;
using TraceLogic.Core.Exporting;
using TraceLogic.Core.Models;
using TraceLogic.Core.Parsing;

namespace TraceLogic.Adapter
{
    /// <summary>
    /// Adapter that exposes TraceLogic.Core functionality to the host application as a dynamic plugin.
    /// </summary>
    /// <remarks>
    /// - Implements <see cref="IDynamicPlugin"/> so the Host can discover and load the plugin.
    /// - Delegates parsing and exporting work to TraceLogic.Core classes, while using <see cref="IPluginContext"/>
    ///   to interact with the Host UI (dialogs, control updates, data binding).
    /// </remarks>
    public class TraceLogicAdapter : IDynamicPlugin
    {
        /// <summary>
        /// Holds the parsed analysis result after a successful parse operation.
        /// The adapter keeps this in memory so subsequent actions (for example export) can operate on the same dataset.
        /// </summary>
        private TraceAnalysisResult? _analysisResult;

        /// <summary>
        /// Plugin display name used by the Host (for example, a Tab header).
        /// </summary>
        public string Name => "TraceLogic Analyzer";

        /// <summary>
        /// Returns the control descriptions the Host should render for this plugin.
        /// The Host will materialize controls from these models and wire actions back to <see cref="HandleAction"/>.
        /// </summary>
        public List<ControlModel> GetControls()
        {
            // Return a small set of controls: load button, status text, results grid, and export button.
            return new List<ControlModel>
            {
                new ControlModel
                {
                    Type = "Button",
                    Content = "Select Trace File (.trc)",
                    Action = "tracelogic_load", // plugin-specific action name that Host will forward to HandleAction
                    Margin = "5"
                },
                new ControlModel
                {
                    Type = "TextBlock",
                    Name = "StatusTextBlock", // used for status updates via IPluginContext.SetValue
                    Text = "Please load a .trc file to begin.",
                    Margin = "5"
                },
                new ControlModel
                {
                    Type = "DataGrid",
                    Name = "LiquidTransferGrid", // target grid where parsed results will be bound
                    Margin = "5",
                    Height = 400, // keep a reasonable fixed height for the example UI
                    SelectionMode = "Extended" // allow the user to select multiple rows
                },
                new ControlModel
                {
                    Type = "Button",
                    Content = "Export Visible Data to CSV",
                    Action = "tracelogic_export", // plugin-specific action name for export
                    Margin = "5"
                }
            };
        }

        /// <summary>
        /// Called by the Host when a control invokes an action.
        /// Routes action names to the appropriate internal handlers.
        /// </summary>
        /// <param name="actionName">Action identifier from the ControlModel (for example "tracelogic_load").</param>
        /// <param name="context">A Host-provided context used to interact with UI and dialogs.</param>
        public void HandleAction(string actionName, IPluginContext context)
        {
            switch (actionName)
            {
                case "tracelogic_load":
                    // Load and parse a trace file, then bind results to the grid.
                    ProcessFile(context);
                    break;

                case "tracelogic_export":
                    // Export currently parsed/visible data to CSV.
                    ExportData(context);
                    break;

                default:
                    // Unknown action: show a message for diagnostic purposes.
                    context.ShowMessage($"Unhandled action: {actionName}");
                    break;
            }
        }

        /// <summary>
        /// Prompts the user for a trace file, parses it using TraceLogic.Core, and updates the Host UI.
        /// </summary>
        /// <param name="context">Host context for dialogs, control updates, and data binding.</param>
        private void ProcessFile(IPluginContext context)
        {
            // 1) Ask the Host to open a file selection dialog.
            string? filePath = context.ShowOpenFileDialog("Trace files (*.trc)|*.trc|All files (*.*)|*.*");

            // If user cancelled the dialog, nothing to do.
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            // 2) Use TraceLogic.Core parser to read and analyze the file.
            //    Parsing may populate Errors and LiquidTransfers on the result.
            var parser = new TraceFileParser();
            _analysisResult = parser.Parse(filePath);

            // 3) If parser reported errors, notify the user and update UI visibility accordingly.
            if (_analysisResult.Errors != null && _analysisResult.Errors.Any())
            {
                context.ShowMessage("Parsing Error: " + string.Join("\n", _analysisResult.Errors));

                // Show/hide UI elements via IPluginContext.SetValue.
                // The exact property names and values depend on how the Host interprets them.
                context.SetValue("WelcomeMessage", "Visibility", "Visible");
                context.SetValue("LiquidTransferGrid", "Visibility", "Collapsed");
                context.SetValue("ExportOptionsPanel", "Visibility", "Collapsed");
                return;
            }

            // 4) Successful parse: update visibility and bind parsed data to the grid.
            context.SetValue("WelcomeMessage", "Visibility", "Collapsed"); // hide any welcome text
            context.SetValue("LiquidTransferGrid", "Visibility", "Visible"); // show the grid
            context.SetValue("ExportOptionsPanel", "Visibility", "Visible"); // show export controls (if present)
            context.SetValue("ExportButton", "Visibility", "Visible");     // show export button

            // 5) Bind the parsed liquid transfer events to the Host's DataGrid control.
            //    BindDataToGrid will marshal and set ItemsSource on the UI thread inside the Host.
            context.BindDataToGrid("LiquidTransferGrid", _analysisResult.LiquidTransfers);

            // 6) Update a status text block with a friendly message including file name and count.
            context.SetValue("StatusTextBlock", "Text",
                $"Successfully parsed {_analysisResult.LiquidTransfers.Count} events from {Path.GetFileName(filePath)}.");
        }

        /// <summary>
        /// Exports the parsed liquid transfer data to a CSV file.
        /// Reads the user's selected columns from a container of CheckBoxes and uses TraceLogic.Core exporter.
        /// </summary>
        /// <param name="context">Host context for dialogs, control queries and messages.</param>
        private void ExportData(IPluginContext context)
        {
            // Ensure we have parsed data to export.
            if (_analysisResult?.LiquidTransfers == null || !_analysisResult.LiquidTransfers.Any())
            {
                context.ShowMessage("There is no data to export.");
                return;
            }

            // 1) Ask the Host for a save location.
            string? savePath = context.ShowSaveFileDialog(
                $"{Path.GetFileNameWithoutExtension(_analysisResult.FileName)}_Export",
                "CSV File (*.csv)|*.csv"
            );

            if (string.IsNullOrEmpty(savePath))
            {
                // User cancelled the Save dialog.
                return;
            }

            try
            {
                // --- DETERMINE COLUMNS TO EXPORT ---
                // 2) Query Host for CheckBox states inside the "ColumnSelectorPanel".
                //    The returned dictionary maps a Tag (assumed to be the property name) -> IsChecked.
                Dictionary<string, bool> columnStates = context.GetCheckBoxStates("ColumnSelectorPanel");

                // 3) Build a list of column names that are checked.
                List<string> selectedColumnHeaders = columnStates
                    .Where(kvp => kvp.Value)
                    .Select(kvp => kvp.Key)
                    .ToList();

                if (!selectedColumnHeaders.Any())
                {
                    context.ShowMessage("No columns selected for export.");
                    return;
                }

                // 4) Convert the selected property names into DataGridColumnInfo instances expected by the exporter.
                //    Here we use the Tag/property name as both Header and PropertyName for simplicity.
                List<DataGridColumnInfo> columnsToExport = selectedColumnHeaders.Select(colName => new DataGridColumnInfo
                {
                    Header = colName,
                    PropertyName = colName
                }).ToList();

                // --- PERFORM EXPORT ---
                // Use TraceLogic.Core exporter to write only the selected columns to CSV.
                DataExporter.ExportToCsv(_analysisResult.LiquidTransfers, columnsToExport, savePath);

                // Notify the user of success.
                context.ShowMessage($"Data successfully exported to:\n{savePath}");
            }
            catch (Exception ex)
            {
                // Catch and report any error that occurred during export.
                context.ShowMessage($"An error occurred during export:\n{ex.Message}");
            }
        }
    }
}