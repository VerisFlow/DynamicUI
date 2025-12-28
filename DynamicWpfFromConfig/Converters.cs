using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace DynamicWpfFromConfig
{
    /// <summary>
    /// Helper class to safely convert strings from the JSON model into WPF objects.
    /// </summary>
    public static class Converters
    {
        /// <summary>
        /// Attempts to convert a string (like "#FF0000" or "Red") into a WPF Brush.
        /// </summary>
        public static bool TryGetBrush(string colorString, out Brush? result)
        {
            result = null;
            if (string.IsNullOrEmpty(colorString))
            {
                return false;
            }

            try
            {
                // BrushConverter is a built-in WPF tool for this.
                object? convertedValue = new BrushConverter().ConvertFrom(null, CultureInfo.InvariantCulture, colorString);
                if (convertedValue is Brush brush)
                {
                    result = brush;
                    return true;
                }
                return false;
            }
            catch (Exception) // Catches invalid formats
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to convert a string (like "1" or "1,2,3,4") into a WPF Thickness.
        /// </summary>
        public static bool TryGetThickness(string thicknessString, out Thickness result)
        {
            result = new Thickness(0); // Default value
            if (string.IsNullOrEmpty(thicknessString))
            {
                return false;
            }

            try
            {
                // ThicknessConverter is a built-in WPF tool for this.
                object? convertedValue = new ThicknessConverter().ConvertFrom(null, CultureInfo.InvariantCulture, thicknessString);
                if (convertedValue is Thickness thickness)
                {
                    result = thickness;
                    return true;
                }
                return false;
            }
            catch (Exception) // Catches invalid formats
            {
                return false;
            }
        }
    }
}