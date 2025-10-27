using DynamicWpfFromConfig.Models;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DynamicWpfFromConfig
{
    public partial class MainWindow : Window
    {
        #region Data Import Logic

        /// <summary>
        /// Orchestrates the data loading process based on the 'dataImport' config.
        /// </summary>
        private void LoadDataFromFile()
        {
            var importConfig = _uiConfig?.DataImport;
            if (importConfig?.Parser == null || importConfig.Query == null || importConfig.Mappings == null)
            {
                MessageBox.Show("Data import configuration (parser, query, or mappings) is missing.", "Load Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 1. Get the search value (e.g., the UserID to look for)
                string searchValue = GetSearchValue(importConfig.Query);
                if (string.IsNullOrEmpty(searchValue))
                {
                    MessageBox.Show("No search value provided for the query.", "Load Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // 2. Parse the CSV and find the matching row
                Dictionary<string, string>? dataRow = ParseAndQueryCsv(importConfig, searchValue);

                if (dataRow == null)
                {
                    MessageBox.Show($"No record found where '{importConfig.Query.LookupColumn ?? importConfig.Query.LookupColumnFromControl}' = '{searchValue}'", "Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // 3. Apply the found data to the UI controls
                foreach (var mapping in importConfig.Mappings)
                {
                    if (dataRow.TryGetValue(mapping.ColumnHeader!, out string? value))
                    {
                        var targetControl = FindControlByName(mapping.SourceControlName);
                        if (targetControl != null)
                        {
                            SetValueOnControl(targetControl, value);
                        }
                    }
                }
                MessageBox.Show("Data loaded successfully.", "Load Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load data: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Reads the file and searches for the specific row based on the configuration.
        /// This is the core flexible parser.
        /// </summary>
        private Dictionary<string, string>? ParseAndQueryCsv(DataImportConfig config, string searchValue)
        {
            var parser = config.Parser!;
            var query = config.Query!;

            // 1. Get flexible parameters
            string filePath = GetConfigParameter<string>(config.SourceFile, config.SourceFileFromControl);
            char delimiter = GetConfigParameter<char>(parser.Delimiter, parser.DelimiterFromControl);
            int headerIndex = GetConfigParameter<int>(parser.HeaderRowIndex.ToString(), parser.HeaderRowIndexFromControl);
            string lookupColumn = GetConfigParameter<string>(query.LookupColumn, query.LookupColumnFromControl);

            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(AppContext.BaseDirectory, filePath);
            }
            filePath = Path.GetFullPath(filePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Data file not found.", filePath);
            }

            var lines = File.ReadAllLines(filePath);
            if (lines.Length <= headerIndex)
            {
                throw new InvalidDataException("File does not contain the specified header row index.");
            }

            // 2. Parse headers dynamically
            var headers = lines[headerIndex].Split(delimiter).Select(h => h.Trim()).ToArray();
            int lookupColumnIndex = Array.IndexOf(headers, lookupColumn);

            if (lookupColumnIndex == -1)
            {
                throw new InvalidDataException($"Lookup column '{lookupColumn}' not found in CSV header row.");
            }

            // 3. Loop through data rows to find match
            for (int i = headerIndex + 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(delimiter);
                if (values.Length > lookupColumnIndex && values[lookupColumnIndex].Trim() == searchValue)
                {
                    // Found the row, map it to a dictionary
                    var dataRow = new Dictionary<string, string>();
                    for (int h = 0; h < headers.Length; h++)
                    {
                        if (h < values.Length)
                        {
                            dataRow[headers[h]] = values[h].Trim();
                        }
                    }
                    return dataRow; // Return the first match
                }
            }
            return null; // Not found
        }

        /// <summary>
        /// Helper to get the search value from the query config.
        /// </summary>
        private string GetSearchValue(ImportQueryConfig query)
        {
            return GetConfigParameter<string>(query.LookupValue, query.LookupValueFromControl);
        }

        /// <summary>
        /// Sets the value on a target FrameworkElement based on its type.
        /// </summary>
        private void SetValueOnControl(FrameworkElement control, string value)
        {
            switch (control)
            {
                case TextBox tb:
                    tb.Text = value;
                    break;
                case CheckBox chk:
                    chk.IsChecked = bool.TryParse(value, out var boolValue) && boolValue;
                    break;
                case ComboBox cb:
                    cb.SelectedItem = value;
                    break;
                case Label lb:
                    lb.Content = value;
                    break;
                case TextBlock tbk:
                    tbk.Text = value;
                    break;
            }
        }

        /// <summary>
        /// This is the core helper for flexibility.
        /// It reads a value either from a hardcoded config property
        /// or from a UI control.
        /// </summary>
        private T GetConfigParameter<T>(string? hardcodedValue, string? controlName)
        {
            string valueStr;
            if (!string.IsNullOrEmpty(controlName))
            {
                var control = FindControlByName(controlName);
                if (control == null)
                {
                    throw new Exception($"Configuration error: Control '{controlName}' not found.");
                }
                valueStr = GetValueFromControl(control);
            }
            else if (hardcodedValue != null)
            {
                valueStr = hardcodedValue;
            }
            else
            {
                throw new Exception("Configuration error: Parameter is missing.");
            }

            try
            {
                // Handle 'char' type for delimiter
                if (typeof(T) == typeof(char))
                {
                    if (valueStr.Length > 0)
                        return (T)(object)valueStr[0];
                    else
                        throw new Exception("Configuration error: Delimiter cannot be empty.");
                }
                return (T)Convert.ChangeType(valueStr, typeof(T));
            }
            catch (Exception ex)
            {
                throw new Exception($"Configuration error: Cannot convert value '{valueStr}' to type {typeof(T).Name}.", ex);
            }
        }

        /// <summary>
        /// Handles the "loadfiletoviewer" action.
        /// Loads a full CSV into a DataGrid control (ActionTarget)
        /// AND populates a ListBox with column names (ActionMessage).
        /// </summary>
        private void HandleLoadFileToViewer(string? dataGridName, string? columnChooserName)
        {
            if (string.IsNullOrEmpty(dataGridName) || string.IsNullOrEmpty(columnChooserName))
            {
                MessageBox.Show("Configuration error: 'actionTarget' (DataGrid) and 'actionMessage' (ListBox) must be specified.", "Load Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var grid = FindControlByName(dataGridName) as DataGrid;
            var chooser = FindControlByName(columnChooserName) as ListBox;
            if (grid == null || chooser == null)
            {
                MessageBox.Show($"Configuration error: Controls '{dataGridName}' or '{columnChooserName}' not found.", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Get parameters from the UI controls
                var importConfig = _uiConfig?.DataImport;
                if (importConfig?.Parser == null)
                {
                    MessageBox.Show("Data import configuration or parser is missing.", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; 
                }
                string filePath = GetConfigParameter<string>(importConfig.SourceFile, importConfig.SourceFileFromControl);
                char delimiter = GetConfigParameter<char>(importConfig.Parser.Delimiter, importConfig.Parser.DelimiterFromControl);
                int headerIndex = GetConfigParameter<int>(importConfig.Parser.HeaderRowIndex.ToString(), importConfig.Parser.HeaderRowIndexFromControl);

                if (!Path.IsPathRooted(filePath))
                {
                    filePath = Path.Combine(AppContext.BaseDirectory, filePath);
                }
                filePath = Path.GetFullPath(filePath);

                DataTable dt = ParseFullCsvToDataTable(filePath, delimiter, headerIndex);

                // 1. Bind the result to the DataGrid
                grid.ItemsSource = dt.DefaultView;

                // 2. Populate the column chooser ListBox
                chooser.Items.Clear();
                foreach (DataColumn col in dt.Columns)
                {
                    chooser.Items.Add(col.ColumnName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load CSV content: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                grid.ItemsSource = null;
                chooser.Items.Clear();
            }
        }

        /// <summary>
        /// Parses an entire CSV file into a DataTable.
        /// </summary>
        private DataTable ParseFullCsvToDataTable(string filePath, char delimiter, int headerIndex)
        {
            var dt = new DataTable();
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            var lines = File.ReadAllLines(filePath);
            if (lines.Length <= headerIndex)
            {
                throw new InvalidDataException("File does not contain the specified header row index.");
            }

            // 1. Create Columns from Header Row
            var headers = lines[headerIndex].Split(delimiter);
            foreach (var header in headers)
            {
                dt.Columns.Add(header.Trim());
            }

            // 2. Add Data Rows
            for (int i = headerIndex + 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(delimiter);
                if (values.Length == headers.Length)
                {
                    dt.Rows.Add(values.Select(v => v.Trim()).ToArray());
                }
            }

            return dt;
        }

        #endregion

        #region Data Export Logic

        /// <summary>
        /// Handles the "exportgridselection" action.
        /// Exports only the selected rows (from DataGrid) and selected columns (from ListBox).
        /// </summary>
        private void HandleExportGridSelection(string? gridName, string? columnChooserName)
        {
            var grid = FindControlByName(gridName) as DataGrid;
            var chooser = FindControlByName(columnChooserName) as ListBox;
            if (grid == null || chooser == null)
            {
                MessageBox.Show($"Config error: Controls '{gridName}' or '{columnChooserName}' not found.",
                              "Export Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                return;
            }

            if (grid.SelectedItems == null || grid.SelectedItems.Count == 0)
            {
                MessageBox.Show("No rows are selected in the grid to export.", "Export Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                // 1. Get Export Parameters (re-using the flexible config)
                var exportConfig = _uiConfig?.DataExport;
                if (exportConfig == null) throw new InvalidOperationException("DataExport config is missing.");

                string targetFile = GetConfigParameter<string>(exportConfig.TargetFile, exportConfig.TargetFileFromControl);
                char delimiter = GetConfigParameter<char>(exportConfig.Delimiter, exportConfig.DelimiterFromControl);
                bool includeHeaders = GetConfigParameter<bool>(exportConfig.IncludeHeaders.ToString(), exportConfig.IncludeHeadersFromControl);
                string writeMode = GetConfigParameter<string>(exportConfig.WriteMode, exportConfig.WriteModeFromControl)?.ToLower() ?? "overwrite";

                if (!Path.IsPathRooted(targetFile))
                {
                    targetFile = Path.Combine(AppContext.BaseDirectory, targetFile);
                }
                targetFile = Path.GetFullPath(targetFile);

                string? targetDirectory = Path.GetDirectoryName(targetFile);
                if (!string.IsNullOrEmpty(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                // 2. Get Selected Columns
                var selectedColumns = chooser.SelectedItems.Cast<string>().ToList();
                // If no columns are selected, default to all columns
                if (selectedColumns.Count == 0)
                {
                    selectedColumns = chooser.Items.Cast<string>().ToList();
                }

                // 3. Build CSV
                var sb = new StringBuilder();

                // 4. Handle Headers
                bool fileExists = File.Exists(targetFile);
                bool writeHeader = includeHeaders && (writeMode == "overwrite" || (writeMode == "append" && !fileExists));
                if (writeHeader)
                {
                    sb.AppendLine(string.Join(delimiter, selectedColumns.Select(h => EscapeCsvCell(h, delimiter))));
                }

                // 5. Handle Selected Rows
                foreach (var item in grid.SelectedItems)
                {
                    if (item is not DataRowView rowView) continue;

                    var cells = new List<string>();
                    // Only add cells from the selected columns
                    foreach (string colName in selectedColumns)
                    {
                        string cellValue = rowView[colName]?.ToString() ?? "";
                        cells.Add(EscapeCsvCell(cellValue, delimiter));
                    }
                    sb.AppendLine(string.Join(delimiter, cells));
                }

                // 6. Write to File
                if (writeMode == "append")
                {
                    File.AppendAllText(targetFile, sb.ToString());
                }
                else
                {
                    File.WriteAllText(targetFile, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export selected data: {ex.Message}",
                              "Export Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Helper method to escape a cell for CSV format.
        /// </summary>
        private string EscapeCsvCell(string value, char delimiter)
        {
            bool needsQuotes = value.Contains(delimiter) || value.Contains('\"') || value.Contains('\n');
            if (needsQuotes)
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }

        /// <summary>
        /// Orchestrates the process of collecting data from UI controls and writing it to a CSV file.
        /// </summary>
        private void ExportDataToCsv(bool showMessage, bool convertBoolean)
        {
            var exportConfig = _uiConfig?.DataExport;
            if (exportConfig?.Mappings == null)
            {
                MessageBox.Show("Data export configuration is missing in 'ui-config.json'.", "Export Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 1. Get all export parameters dynamically
                string targetFile = GetConfigParameter<string>(
                    exportConfig.TargetFile,
                    exportConfig.TargetFileFromControl
                );

                if (!Path.IsPathRooted(targetFile))
                {
                    targetFile = Path.Combine(AppContext.BaseDirectory, targetFile);
                }
                targetFile = Path.GetFullPath(targetFile);

                string? targetDirectory = Path.GetDirectoryName(targetFile);
                if (!string.IsNullOrEmpty(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                char delimiter = GetConfigParameter<char>(
                    exportConfig.Delimiter,
                    exportConfig.DelimiterFromControl
                );

                bool includeHeaders = GetConfigParameter<bool>(
                    exportConfig.IncludeHeaders.ToString(),
                    exportConfig.IncludeHeadersFromControl
                );

                string writeMode = GetConfigParameter<string>(
                    exportConfig.WriteMode,
                    exportConfig.WriteModeFromControl
                )?.ToLower() ?? "overwrite";

                if (string.IsNullOrWhiteSpace(targetFile))
                {
                    MessageBox.Show("Export target file is not specified.", "Export Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 2. Collect headers and values from controls
                var headers = new List<string>();
                var values = new List<string>();

                foreach (var mapping in exportConfig.Mappings)
                {
                    headers.Add(mapping.ColumnHeader ?? string.Empty);
                    var sourceControl = FindControlByName(mapping.SourceControlName);
                    string value = sourceControl != null ? GetValueFromControl(sourceControl) : "";

                    if (convertBoolean)
                    {
                        if (string.Equals(value, "True", StringComparison.OrdinalIgnoreCase))
                        {
                            value = "1";
                        }
                        else if (string.Equals(value, "False", StringComparison.OrdinalIgnoreCase))
                        {
                            value = "0";
                        }
                    }

                    // 3. Escape value for CSV (now respects the dynamic delimiter)
                    bool needsQuotes = value.Contains(delimiter) || value.Contains('\"') || value.Contains('\n');
                    if (needsQuotes)
                    {
                        value = $"\"{value.Replace("\"", "\"\"")}\"";
                    }
                    values.Add(value);
                }

                // 4. Build CSV content
                var csvBuilder = new StringBuilder();
                string headerLine = string.Join(delimiter, headers);
                string valueLine = string.Join(delimiter, values);

                // 5. Perform file I/O based on WriteMode
                if (writeMode == "append")
                {
                    bool fileExists = File.Exists(targetFile);
                    bool writeHeader = includeHeaders && !fileExists; // Only write header if file is new

                    if (writeHeader)
                    {
                        csvBuilder.AppendLine(headerLine);
                    }
                    csvBuilder.AppendLine(valueLine);
                    File.AppendAllText(targetFile, csvBuilder.ToString());
                }
                else // Default to "overwrite"
                {
                    if (includeHeaders)
                    {
                        csvBuilder.AppendLine(headerLine);
                    }
                    csvBuilder.AppendLine(valueLine);
                    File.WriteAllText(targetFile, csvBuilder.ToString());
                }

                if (showMessage)
                {
                    MessageBox.Show($"Data successfully saved to '{targetFile}'", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export data: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}