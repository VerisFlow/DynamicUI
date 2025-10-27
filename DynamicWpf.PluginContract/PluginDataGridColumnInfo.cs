namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// A lightweight DTO that carries DataGrid column metadata between the Host and plugins.
    /// </summary>
    /// <remarks>
    /// The Contract project avoids referencing WPF assemblies by using this simple model.
    /// Hosts populate instances of this class to describe visible columns (header text and the
    /// bound property name) so plugins can inspect or act upon the grid schema without direct UI references.
    /// </remarks>
    public class PluginDataGridColumnInfo
    {
        /// <summary>
        /// The text displayed in the column header (for example, "Full Name").
        /// </summary>
        public string Header { get; set; } = string.Empty;

        /// <summary>
        /// The name of the property the column is bound to (for example, "UserName").
        /// Plugins can use this value to reference the corresponding data field.
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;
    }
}