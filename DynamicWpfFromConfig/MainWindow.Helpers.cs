using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DynamicWpfFromConfig
{
    public partial class MainWindow : Window
    {
        #region Helper Methods

        /// <summary>
        /// A helper method that extracts the string value from different types of WPF controls.
        /// (This method is unchanged)
        /// </summary>
        private string GetValueFromControl(FrameworkElement control)
        {
            return control switch
            {
                TextBox tb => tb.Text,
                CheckBox chk => chk.IsChecked?.ToString() ?? string.Empty,
                ComboBox cb => cb.SelectedItem?.ToString() ?? string.Empty,
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Sets a specific property on a given FrameworkElement.
        /// This is used by the PluginContext to update UI elements.
        /// </summary>
        private void SetControlProperty(FrameworkElement control, string propertyName, string value)
        {
            switch (propertyName.ToLowerInvariant())
            {
                case "text":
                case "content": 
                    SetValueOnControl(control, value);
                    break;
                case "ischecked":
                    if (control is CheckBox chk)
                        chk.IsChecked = bool.TryParse(value, out var b) && b;
                    break;
                case "visibility":
                    if (Enum.TryParse(value, out Visibility visibility))
                    {
                        control.Visibility = visibility;
                    }
                    break;
            }
        }

        /// <summary>
        /// Helper to get the binding path from different DataGridColumn types.
        /// </summary>
        private string GetColumnBindingPath(DataGridColumn column)
        {
            if (column is DataGridBoundColumn boundColumn && boundColumn.Binding is Binding binding)
            {
                return binding.Path.Path;
            }
            return string.Empty;
        }

        /// <summary>
        /// Finds a dynamically created control by its assigned name.
        /// </summary>
        /// <param name="name">The name of the control to find.</param>
        /// <returns>The FrameworkElement if found; otherwise, null.</returns>
        private FrameworkElement? FindControlByName(string? name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            return (FrameworkElement?)this.FindName(name);
        }

        /// <summary>
        /// Safely parses a color string (e.g., "Red", "#FF0000") into a Brush object.
        /// </summary>
        /// <param name="colorName">The string representing the color.</param>
        /// <returns>A SolidColorBrush object, or null if parsing fails.</returns>
        private Brush? TryParseColor(string? colorName)
        {
            if (string.IsNullOrEmpty(colorName)) return null;
            try
            {
                var brushObj = new BrushConverter().ConvertFrom(null, CultureInfo.InvariantCulture, colorName);
                return brushObj as Brush;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}