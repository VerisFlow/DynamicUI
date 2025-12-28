using DynamicWpfFromConfig.Models;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DynamicWpfFromConfig
{
    public partial class MainWindow : Window
    {
        #region Event Handlers

        /// <summary>
        /// Handles the Click event for dynamically generated Buttons.
        /// Routes the action to either the Host or a specified Plugin.
        /// </summary>
        private void DynamicButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not ControlModel model) return;

            string? actionName = model.Action?.ToLowerInvariant();
            if (string.IsNullOrEmpty(actionName)) return;

            // --- ACTION ROUTING LOGIC ---

            // 1. Define all known "Host" actions
            bool isHostAction = new[] {
                "showmessage", "togglevisibility", "browseopenfile",
                "browsesavefile", "exportdata", "exportdata_and_close", "loaddata",
                "loadfiletoviewer", "exportgridselection",
                "minimizeWindow", "maximizeRestoreWindow", "closeWindow", "showAbout"
            }.Contains(actionName);

            if (isHostAction)
            {
                // 2. It's a Host action, handle it locally
                HandleHostAction(model, actionName);
            }
            else
            {
                // 3. Check if the control specifies a plugin owner
                string? ownerPluginName = model.PluginOwner;

                // If no specific owner is defined, fall back to checking ActionTarget (legacy/PluginSurface use)
                if (string.IsNullOrEmpty(ownerPluginName))
                {
                    ownerPluginName = model.ActionTarget;
                }

                if (!string.IsNullOrEmpty(ownerPluginName))
                {
                    // Find and delegate the action to the specified plugin
                    var ownerPlugin = _loadedPlugins.FirstOrDefault(p => p.Name == ownerPluginName);
                    if (ownerPlugin != null)
                    {
                        IPluginContext context = new PluginContext(this);
                        ownerPlugin.HandleAction(actionName, context);
                    }
                    else
                    {
                        // Owner specified, but plugin not found/loaded
                        MessageBox.Show($"Action '{actionName}' targets plugin '{ownerPluginName}', but the plugin was not found.", "Plugin Action Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    // Not a host action, and no plugin owner specified
                    MessageBox.Show($"Unknown action '{actionName}' and no plugin owner was specified for this control.", "Action Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        /// <summary>
        /// A generic event handler for controls like CheckBox, ComboBox, etc.
        /// It reads the control's model and executes the action defined in the 'events' list.
        /// </summary>
        private void DynamicEventHandler(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement element)
            {
                return;
            }

            // The ControlFactory (ApplyCommonProperties) MUST ensure element.Tag = model for this to work.
            if (element.Tag is not ControlModel model)
            {
                // If the Tag isn't the model, we cannot reliably determine the action.
                Console.WriteLine($"Error: Tag for control '{element.Name}' is not a ControlModel. Action cannot be processed.");
                return;
            }

            // Code from here onwards is now guaranteed to have non-null 'element' and 'model'
            if (model.Events == null || !model.Events.Any()) return;

            // Determine which WPF event was raised
            string eventName = string.Empty;
            if (e.RoutedEvent == CheckBox.CheckedEvent) eventName = "Checked";
            else if (e.RoutedEvent == CheckBox.UncheckedEvent) eventName = "Unchecked";

            if (string.IsNullOrEmpty(eventName)) return;

            // Find the specific action defined for this event
            var eventAction = model.Events.FirstOrDefault(evt =>
                string.Equals(evt.On, eventName, StringComparison.OrdinalIgnoreCase));

            // If an action is defined for this event
            if (eventAction != null && !string.IsNullOrEmpty(eventAction.Action))
            {
                string actionName = eventAction.Action.ToLowerInvariant();

                // Check if the action is one handled by the Host
                bool isHostAction = new[] {
                    "showmessage", "togglevisibility", "browseopenfile",
                    "browsesavefile", "exportdata", "exportdata_and_close", "loaddata",
                    "loadfiletoviewer", "exportgridselection",
                    "minimizeWindow", "maximizeRestoreWindow", "closeWindow", "showAbout",
                    "togglegridcolumnvisibility"
                }.Contains(actionName);

                if (isHostAction)
                {
                    // Execute the Host action, passing EventActionModel and the sender element
                    HandleHostAction(eventAction, actionName, element);
                }
                else
                {
                    // Route the action to the appropriate plugin
                    string? ownerPluginName = model.PluginOwner ?? eventAction.ActionTarget;
                    if (!string.IsNullOrEmpty(ownerPluginName))
                    {
                        var ownerPlugin = _loadedPlugins.FirstOrDefault(p => p.Name == ownerPluginName);
                        if (ownerPlugin != null)
                        {
                            IPluginContext context = new PluginContext(this);
                            ownerPlugin.HandleAction(actionName, context);
                        }
                        else
                        {
                            MessageBox.Show($"Action '{actionName}' targets plugin '{ownerPluginName}', but the plugin was not found.", "Plugin Action Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Unknown action '{actionName}' and no plugin owner was specified.", "Action Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        /// <summary>
        /// Handles all built-in "Host" actions (e.g., 'showmessage', 'togglevisibility')
        /// routed from other event handlers.
        /// </summary>
        /// <param name="actionDetails">The model containing action details (either a ControlModel or EventActionModel).</param>
        /// <param name="actionName">The normalized (lowercase) name of the action to perform.</param>
        /// <param name="sender">The control that triggered the event (used for event-specific actions).</param>
        private void HandleHostAction(object actionDetails, string actionName, object? sender = null)
        {
            // Determine source model and action target based on actionDetails type
            ControlModel? model = actionDetails as ControlModel;
            EventActionModel? eventActionModel = actionDetails as EventActionModel;

            string? actionTarget = model?.ActionTarget ?? eventActionModel?.ActionTarget;
            string? actionMessage = model?.ActionMessage;
            string? targetTextBoxName = model?.TargetTextBoxName;

            switch (actionName)
            {
                case "showmessage":
                    if (model != null)
                    {
                        MessageBox.Show(model.ActionMessage ?? "No message specified.");
                    }
                    break;
                case "togglevisibility":
                    if (model != null)
                    {
                        if (FindControlByName(model.ActionTarget) is UIElement target)
                        {
                            target.Visibility = target.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                        }
                    }
                    break;
                case "browseopenfile":
                    var openFileDialog = new OpenFileDialog();
                    if (openFileDialog.ShowDialog() == true)
                    {
                        if (model != null)
                        {
                            if (FindControlByName(model.TargetTextBoxName ?? model.ActionTarget) is TextBox targetTextBox)
                            {
                                targetTextBox.Text = openFileDialog.FileName;
                            }
                        }
                    }
                    break;
                case "browsesavefile":
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "CSV File (*.csv)|*.csv|All Files (*.*)|*.*",
                        DefaultExt = ".csv"
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        if (model != null)
                        {
                            if (FindControlByName(model.TargetTextBoxName ?? model.ActionTarget) is TextBox targetTextBox)
                            {
                                targetTextBox.Text = saveFileDialog.FileName;
                            }
                        }
                    }
                    break;
                case "exportdata":
                    ExportDataToCsv(true, false);
                    break;
                case "exportdata_and_close":
                    ExportDataToCsv(false, true);
                    this.Close();
                    break;
                case "loaddata":
                    LoadDataFromFile();
                    break;
                case "loadfiletoviewer":
                    if (model != null)
                    {
                        HandleLoadFileToViewer(model.ActionTarget, model.ActionMessage);
                    }
                    break;
                case "exportgridselection":
                    if (model != null)
                    {
                        HandleExportGridSelection(model.ActionTarget, model.ActionMessage);
                    }
                    break;
                case "minimizeWindow":
                    this.WindowState = WindowState.Minimized;
                    break;
                case "maximizeRestoreWindow":
                    this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                    break;
                case "closeWindow":
                    this.Close();
                    break;
                case "togglegridcolumnvisibility":
                    if (sender is CheckBox chk && chk.Tag is ControlModel chkModel && chkModel.Tag is string columnName)
                    {
                        var grid = FindControlByName(actionTarget) as DataGrid;
                        if (grid != null)
                        {
                            // Find the column in the DataGrid that matches the CheckBox's Tag
                            var column = grid.Columns.FirstOrDefault(col =>
                            string.Equals(GetColumnBindingPath(col), columnName, StringComparison.OrdinalIgnoreCase));

                            if (column != null)
                            {
                                // Set the column's visibility based on the CheckBox state
                                column.Visibility = chk.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// This event handler is called AFTER the DataGrid auto-generates columns.
        /// It immediately applies the initial CheckBox states to the new columns.
        /// </summary>
        private void DataGrid_AutoGeneratedColumns(object? sender, EventArgs e)
        {
            if (sender is not DataGrid grid) return;

            // 1. Find the panel containing the checkboxes
            var panel = FindControlByName("ColumnSelectorPanel") as Panel;
            if (panel == null) return;

            // 2. Get the current state of ALL checkboxes in that panel
            var states = new Dictionary<string, bool>();
            foreach (var child in panel.Children)
            {
                if (child is CheckBox chk && chk.Tag is ControlModel chkModel && chkModel.Tag is string columnName)
                {
                    states[columnName] = chk.IsChecked ?? false;
                }
            }

            // 3. Loop through the grid's newly generated columns
            foreach (var column in grid.Columns)
            {
                string columnBindingPath = GetColumnBindingPath(column);
                if (string.IsNullOrEmpty(columnBindingPath)) continue;

                // 4. Find the matching checkbox state (using case-insensitive comparison)
                var matchingState = states.FirstOrDefault(kvp =>
                    string.Equals(kvp.Key, columnBindingPath, StringComparison.OrdinalIgnoreCase));

                if (!matchingState.Equals(default(KeyValuePair<string, bool>)))
                {
                    // A checkbox exists for this column. Apply its state.
                    bool isChecked = matchingState.Value;
                    column.Visibility = isChecked ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    // No checkbox found for this column. Default to visible.
                    column.Visibility = Visibility.Visible;
                }
            }
        }

        #endregion
    }
}