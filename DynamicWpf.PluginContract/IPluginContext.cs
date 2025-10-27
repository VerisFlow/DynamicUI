using System.Collections.Generic;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Provides a sandboxed API the Host exposes to plugins for interacting with the UI and host services.
    /// </summary>
    /// <remarks>
    /// The Host implements this interface and passes an instance to plugins when invoking
    /// <see cref="IDynamicPlugin.HandleAction(string,IPluginContext)"/>.
    /// Implementations should validate inputs, perform UI updates on the UI thread, and keep the surface
    /// small and safe for untrusted plugin code. Methods should favor returning simple values and
    /// avoid exposing host internals.
    /// </remarks>
    public interface IPluginContext
    {
        /// <summary>
        /// Gets the current string value of a named UI control.
        /// </summary>
        /// <param name="controlName">The <c>Name</c> of the control to read (as defined in the control model).</param>
        /// <returns>
        /// The current control value as a string. If the named control does not exist the Host may return
        /// <c>null</c> or an empty string according to its policy — plugins should handle either case.
        /// </returns>
        /// <remarks>
        /// Typical callers use this to obtain user-entered text or a control's selected value. The Host
        /// may need to marshal to the UI thread to read the value; plugin authors should avoid assuming this call is free.
        /// </remarks>
        string GetValue(string controlName);

        /// <summary>
        /// Sets a property value on a named UI control.
        /// </summary>
        /// <param name="controlName">The <c>Name</c> of the control to modify.</param>
        /// <param name="propertyName">The control property to set (for example, <c>"Text"</c>, <c>"IsEnabled"</c>, <c>"Content"</c>).</param>
        /// <param name="value">The new value to apply, expressed as a string. The Host is responsible for converting/validating the value for the target property.</param>
        /// <remarks>
        /// The Host should perform the update on the UI thread. If the control or property does not exist the Host
        /// should either ignore the request or report an error via the plugin's configured error mechanism.
        /// </remarks>
        void SetValue(string controlName, string propertyName, string value);

        /// <summary>
        /// Requests the Host show a simple message to the user.
        /// </summary>
        /// <param name="message">The message text to display.</param>
        /// <remarks>
        /// The Host may display a message box, toast, or other UI affordance. Avoid using this for frequent updates.
        /// </remarks>
        void ShowMessage(string message);

        /// <summary>
        /// Requests the Host to display an Open File dialog and return the selected file path.
        /// </summary>
        /// <param name="filter">A file filter string (for example: <c>"Text files (*.txt)|*.txt|All files (*.*)|*.*"</c>).</param>
        /// <returns>The selected file path, or <c>null</c> if the dialog was cancelled.</returns>
        /// <remarks>
        /// The Host controls the initial directory and dialog options. Plugins should treat <c>null</c> as "cancelled".
        /// </remarks>
        string? ShowOpenFileDialog(string filter);

        /// <summary>
        /// Requests the Host to display a Save File dialog and return the selected file path.
        /// </summary>
        /// <param name="defaultFileName">Suggested default file name presented by the dialog.</param>
        /// <param name="filter">A file filter string (for example: <c>"CSV File (*.csv)|*.csv"</c>).</param>
        /// <returns>The selected file path, or <c>null</c> if the dialog was cancelled.</returns>
        string? ShowSaveFileDialog(string defaultFileName, string filter);

        /// <summary>
        /// Asks the Host to bind a data source to a named DataGrid control.
        /// </summary>
        /// <param name="gridName">The <c>Name</c> of the DataGrid control to bind.</param>
        /// <param name="itemsSource">The data to bind (for example a <see cref="System.Collections.IEnumerable"/> such as <c>List&lt;T&gt;</c> or a DataTable).</param>
        /// <remarks>
        /// The Host is responsible for applying the binding on the UI thread. The plugin is encouraged to
        /// provide simple, serializable collections and avoid passing complex UI objects.
        /// </remarks>
        void BindDataToGrid(string gridName, object itemsSource);

        /// <summary>
        /// Gets information about the visible columns of a named DataGrid.
        /// </summary>
        /// <param name="gridName">The <c>Name</c> of the DataGrid control.</param>
        /// <returns>A list of <see cref="PluginDataGridColumnInfo"/> describing each visible column; the list may be empty if the grid has no columns or the name is not found.</returns>
        /// <remarks>
        /// This method allows plugins to inspect column headers and bound property names without referencing host UI types.
        /// </remarks>
        List<PluginDataGridColumnInfo> GetGridColumnInfo(string gridName);

        /// <summary>
        /// Returns the checked state of CheckBox controls contained in a named container.
        /// </summary>
        /// <param name="containerName">The <c>Name</c> of a container control (for example, a <c>WrapPanel</c> or <c>StackPanel</c>).</param>
        /// <returns>
        /// A dictionary mapping each CheckBox's <c>Tag</c> (typically a column name or identifier) to its <c>IsChecked</c> state.
        /// If no checkboxes are found, an empty dictionary is returned.
        /// </returns>
        /// <remarks>
        /// The Host should determine which children to inspect and how to populate the Tag->state mapping.
        /// Plugin code should not assume ordering of entries in the returned dictionary.
        /// </remarks>
        Dictionary<string, bool> GetCheckBoxStates(string containerName);
    }
}