using DynamicWpfFromConfig.Models;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DynamicWpfFromConfig
{
    public partial class MainWindow : Window
    {
        #region Control Factory

        /// <summary>
        /// Factory method to create a WPF FrameworkElement based on a ControlModel from the config.
        /// This is where controls are instantiated and themed.
        /// </summary>
        /// <param name="model">The model representing the control to create.</param>
        /// <returns>A fully configured and themed FrameworkElement, or null if the type is unrecognized.</returns>
        private FrameworkElement? CreateControl(ControlModel model)
        {
            FrameworkElement? element = null;

            switch (model.Type?.ToLowerInvariant())
            {
                case "label":
                    // Simple text label.
                    element = new Label
                    {
                        Content = model.Content,
                        Foreground = _themeForegroundBrush
                    };
                    break;
                case "textblock":
                    // Versatile text block with styling.
                    var foregroundBrush = TryParseColor(model.ForegroundColor) ?? _themeForegroundBrush;
                    var backgroundBrush = TryParseColor(model.BackgroundColor);

                    var padding = new Thickness();
                    if (!string.IsNullOrEmpty(model.Padding))
                    {
                        var paddingObj = new ThicknessConverter().ConvertFrom(null, CultureInfo.InvariantCulture, model.Padding);
                        if (paddingObj is Thickness p)
                        {
                            padding = p;
                        }
                    }

                    element = new TextBlock
                    {
                        Text = model.Text,
                        FontSize = model.FontSize ?? 12,
                        FontWeight = model.IsBold == true ? FontWeights.Bold : FontWeights.Normal,
                        TextWrapping = model.TextWrapping == "Wrap" ? TextWrapping.Wrap : TextWrapping.NoWrap,
                        Foreground = foregroundBrush,
                        Background = backgroundBrush,
                        Padding = padding
                    };
                    break;
                case "textbox":
                    // Standard text input box.
                    var textBox = new TextBox
                    {
                        Text = model.Text,
                        IsReadOnly = model.IsReadOnly ?? false,
                        Background = _themeControlBackgroundBrush,
                        Foreground = _themeForegroundBrush
                    };

                    if (!string.IsNullOrEmpty(model.BorderColor) && Converters.TryGetBrush(model.BorderColor, out var customBorderBrush))
                    {
                        textBox.BorderBrush = customBorderBrush;
                    }
                    else
                    {
                        textBox.BorderBrush = _themeBorderBrush;
                    }

                    if (!string.IsNullOrEmpty(model.BorderThickness) && Converters.TryGetThickness(model.BorderThickness, out var customThickness))
                    {
                        textBox.BorderThickness = customThickness;
                    }

                    element = textBox;
                    break;
                case "image":
                    // Image element loaded from source (see helper method).
                    element = CreateImageElement(model);
                    break;
                case "border":
                    // Border container, can have nested controls.
                    var border = new Border();
                    border.Background = TryParseColor(model.BackgroundColor);
                    border.BorderBrush = TryParseColor(model.BorderColor) ?? _themeBorderBrush;

                    if (!string.IsNullOrEmpty(model.BorderThickness))
                    {
                        var thicknessObj = new ThicknessConverter().ConvertFrom(null, CultureInfo.InvariantCulture, model.BorderThickness);
                        if (thicknessObj is Thickness thickness)
                        {
                            border.BorderThickness = thickness;
                        }
                    }
                    else
                    {
                        border.BorderThickness = new Thickness(1);
                    }

                    if (!string.IsNullOrEmpty(model.Padding))
                    {
                        var paddingObj = new ThicknessConverter().ConvertFrom(null, CultureInfo.InvariantCulture, model.Padding);
                        if (paddingObj is Thickness p)
                        {
                            border.Padding = p;
                        }
                    }

                    // --- Handle Nested Controls ---
                    if (model.ContentControls != null && model.ContentControls.Any())
                    {
                        // 1. Create a container (Grid)
                        var nestedGrid = CreateNestedGridContainer(model.ContentControls); // Use helper
                        border.Child = nestedGrid;

                        // 2. Create and add children
                        foreach (var childModel in model.ContentControls)
                        {
                            FrameworkElement? childElement = CreateControl(childModel);
                            if (childElement != null)
                            {
                                // Manually set Grid Row/Column relative to the NESTED grid
                                Grid.SetRow(childElement, childModel.GridRow);
                                Grid.SetColumn(childElement, childModel.GridColumn);
                                if (childModel.RowSpan.HasValue) Grid.SetRowSpan(childElement, childModel.RowSpan.Value);
                                if (childModel.ColumnSpan.HasValue) Grid.SetColumnSpan(childElement, childModel.ColumnSpan.Value);

                                // Apply other properties EXCEPT Grid position
                                ApplyCommonProperties(childElement, childModel, applyGridPosition: false);

                                nestedGrid.Children.Add(childElement);
                            }
                        }
                    }
                    element = border;
                    break;
                case "grid":
                    // Grid container, supports row/col definitions and nested controls.
                    // Create the Grid itself using a helper
                    var gridPanel = CreateNestedGridContainer(model.ContentControls, model);

                    // --- Handle Nested Controls ---
                    if (model.ContentControls != null && model.ContentControls.Any())
                    {
                        // Create and add children (same logic as Border)
                        foreach (var childModel in model.ContentControls)
                        {
                            FrameworkElement? childElement = CreateControl(childModel);
                            if (childElement != null)
                            {
                                Grid.SetRow(childElement, childModel.GridRow);
                                Grid.SetColumn(childElement, childModel.GridColumn);
                                if (childModel.RowSpan.HasValue) Grid.SetRowSpan(childElement, childModel.RowSpan.Value);
                                if (childModel.ColumnSpan.HasValue) Grid.SetColumnSpan(childElement, childModel.ColumnSpan.Value);
                                ApplyCommonProperties(childElement, childModel, applyGridPosition: false);
                                gridPanel.Children.Add(childElement);
                            }
                        }
                    }
                    element = gridPanel;
                    break;
                case "tabcontrol":
                    // Tabbed interface container.
                    var tabControl = new TabControl();
                    ApplyThemeToTabControl(tabControl);

                    if (model.TabItems != null)
                    {
                        foreach (var tabItemModel in model.TabItems)
                        {
                            var tabItem = new TabItem
                            {
                                Header = tabItemModel.Header
                            };

                            // Create content for the TabItem
                            if (tabItemModel.ContentControls != null && tabItemModel.ContentControls.Any())
                            {
                                // Assume TabItem content uses a Grid
                                var tabGrid = CreateNestedGridContainer(tabItemModel.ContentControls);
                                tabItem.Content = tabGrid;

                                // Create and add controls within the tab's grid
                                foreach (var contentModel in tabItemModel.ContentControls)
                                {
                                    FrameworkElement? contentElement = CreateControl(contentModel);
                                    if (contentElement != null)
                                    {
                                        Grid.SetRow(contentElement, contentModel.GridRow);
                                        Grid.SetColumn(contentElement, contentModel.GridColumn);
                                        if (contentModel.RowSpan.HasValue) Grid.SetRowSpan(contentElement, contentModel.RowSpan.Value);
                                        if (contentModel.ColumnSpan.HasValue) Grid.SetColumnSpan(contentElement, contentModel.ColumnSpan.Value);
                                        ApplyCommonProperties(contentElement, contentModel, applyGridPosition: false);
                                        tabGrid.Children.Add(contentElement);
                                    }
                                }
                            }
                            tabControl.Items.Add(tabItem);
                        }
                    }
                    element = tabControl;
                    break;
                case "wrappanel":
                    // Panel that wraps content to the next line.
                    var wrapPanel = new WrapPanel
                    {
                        Orientation = Orientation.Horizontal
                    };

                    // --- Handle Nested Controls ('contentControls') ---
                    // WrapPanel uses Children, similar logic to Grid/Border
                    if (model.ContentControls != null && model.ContentControls.Any())
                    {
                        foreach (var childModel in model.ContentControls)
                        {
                            FrameworkElement? childElement = CreateControl(childModel);
                            if (childElement != null)
                            {
                                // Apply properties like Margin, Alignment, etc.
                                // WrapPanel doesn't use Grid.Row/Column, so no need to set them here.
                                ApplyCommonProperties(childElement, childModel, applyGridPosition: false);
                                wrapPanel.Children.Add(childElement);
                            }
                        }
                    }
                    element = wrapPanel;
                    break;
                case "checkbox":
                    // Standard check box.
                    element = new CheckBox
                    {
                        Content = model.Content,
                        IsChecked = model.IsChecked ?? false,
                        Foreground = _themeForegroundBrush,
                        Tag = model,
                    };
                    // Hook up the generic event handler for Checked/Unchecked events.
                    ((CheckBox)element).Checked += DynamicEventHandler;
                    ((CheckBox)element).Unchecked += DynamicEventHandler;
                    break;
                case "datagrid":
                    // Grid for displaying tabular data.
                    var grid = new DataGrid
                    {
                        IsReadOnly = true,
                        AutoGenerateColumns = true,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                    };

                    // Allow "Extended" (Ctrl+Click) or "Multiple" row selection
                    if (Enum.TryParse(model.SelectionMode, out DataGridSelectionMode selectionMode))
                    {
                        grid.SelectionMode = selectionMode;
                    }

                    // Ensure we select entire rows
                    grid.SelectionUnit = DataGridSelectionUnit.FullRow;

                    element = grid;
                    break;
                case "combobox":
                    // Dropdown selection box.
                    var comboBox = new ComboBox
                    {
                        Background = Brushes.White,
                        BorderBrush = _themeBorderBrush,
                        Foreground = Brushes.Black
                    };
                    if (model.Items != null)
                    {
                        foreach (var item in model.Items)
                        {
                            comboBox.Items.Add(item);
                        }
                    }
                    comboBox.SelectedItem = model.DefaultSelection;
                    element = comboBox;
                    break;
                case "listbox":
                    // List box, specially configured to support LogColorMap.
                    var listBox = new ListBox
                    {
                        Foreground = _themeForegroundBrush,
                        Background = TryParseColor(model.BackgroundColor) ?? _themeControlBackgroundBrush
                    };

                    // Allow "Multiple" or "Extended" (Ctrl+Click) column selection
                    if (model.SelectionMode?.ToLowerInvariant() == "multiple" || model.SelectionMode?.ToLowerInvariant() == "extended")
                    {
                        listBox.SelectionMode = SelectionMode.Extended;
                    }

                    if (model.Items != null)
                    {
                        // Get the default brush (using theme foreground or defaulting to white)
                        Brush defaultForeground = _themeForegroundBrush ?? Brushes.White;

                        foreach (var item in model.Items)
                        {
                            // 1. Find the color that should be used for this log item
                            Brush itemBrush = GetLogForegroundBrush(item, model.LogColorMap, defaultForeground);

                            // 2. Wrap the string in a TextBlock with the correct color applied
                            var textBlock = new TextBlock
                            {
                                Text = item,
                                Foreground = itemBrush,
                                // Prevent horizontal scrolling issues on long lines
                                TextWrapping = TextWrapping.NoWrap
                            };

                            // 3. Add the TextBlock (the content of the ListBoxItem)
                            listBox.Items.Add(textBlock);
                        }
                    }

                    element = listBox;
                    break;
                case "button":
                    // Standard clickable button.
                    element = new Button
                    {
                        Content = model.Content,
                        Tag = model, // Associate model for event handler
                        Background = _themeAccentBrush,
                        Foreground = _themeAccentForegroundBrush,
                        BorderBrush = _themeAccentBrush,
                        FontSize = model.FontSize ?? 12
                    };
                    // Hook up the specific click handler for button actions.
                    ((Button)element).Click += DynamicButton_Click;
                    break;
                case "titrationplate":
                    // Custom titration plate control (see helper method).
                    element = CreateTitrationPlateElement(model);
                    break;
                case "pluginsurface":
                    // A container that hosts UI controls defined by a plugin.
                    // 1. Get the plugin name from the config
                    string? pluginName = model.PluginName;
                    if (string.IsNullOrEmpty(pluginName))
                    {
                        element = new TextBlock { Text = "PluginSurface error: 'pluginName' is not specified." };
                        break;
                    }

                    // 2. Find the plugin that was loaded earlier
                    var plugin = _loadedPlugins.FirstOrDefault(p => p.Name == pluginName);

                    if (plugin != null)
                    {
                        // 3. Create a container for the plugin's UI
                        var container = new Border
                        {
                            BorderBrush = _themeBorderBrush,
                            BorderThickness = new Thickness(1),
                            Background = _themeControlBackgroundBrush
                        };
                        var content = new StackPanel { Margin = new Thickness(10) };
                        container.Child = new ScrollViewer
                        {
                            Content = content,
                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                        };

                        // 4. Get the plugin's controls and render them
                        foreach (var controlModel in plugin.GetControls())
                        {
                            // IMPORTANT: Tag the control with the plugin's name for action routing
                            controlModel.ActionTarget = plugin.Name;

                            var uiElement = CreateControl(controlModel);
                            if (uiElement != null)
                            {
                                content.Children.Add(uiElement);
                            }
                        }
                        element = container;
                    }
                    else
                    {
                        element = new TextBlock { Text = $"Error: Plugin '{pluginName}' not found." };
                    }
                    break;
            }

            // Apply common layout properties (Grid position, alignment, etc.) to the created element.
            if (element != null)
            {
                ApplyCommonProperties(element, model);
            }

            return element;
        }

        /// <summary>
        /// Helper method to create and configure an Image element, with robust error handling.
        /// </summary>
        /// <param name="model">The model for the image control.</param>
        /// <returns>An Image element or a TextBlock with an error message.</returns>
        private FrameworkElement CreateImageElement(ControlModel model)
        {
            if (string.IsNullOrEmpty(model.Source))
                return new TextBlock { Text = "Image source is missing.", Foreground = _themeErrorBrush };

            try
            {
                Uri imageUri;
                // Handle both absolute (http, C:\) and relative paths.
                if (Uri.IsWellFormedUriString(model.Source, UriKind.Absolute))
                {
                    imageUri = new Uri(model.Source, UriKind.Absolute);
                }
                else
                {
                    // Assume relative path from the application's base directory.
                    string fullPath = Path.Combine(AppContext.BaseDirectory, model.Source);
                    if (!File.Exists(fullPath))
                    {
                        return new TextBlock { Text = $"Image not found: {model.Source}", Foreground = _themeErrorBrush };
                    }
                    imageUri = new Uri(fullPath);
                }
                return new Image
                {
                    Source = new BitmapImage(imageUri),
                    Height = model.Height ?? double.NaN,
                    Stretch = Stretch.Uniform
                };
            }
            catch (Exception ex)
            {
                // Catch errors from invalid URIs, unsupported formats, or permission issues.
                return new TextBlock { Text = $"Error loading image: {ex.Message}", Foreground = _themeErrorBrush, TextWrapping = TextWrapping.Wrap };
            }
        }

        /// <summary>
        /// Creates a Titration Plate control, now with optional row and column headers.
        /// The entire assembly (headers + plate) is wrapped in a master grid.
        /// The plate itself still supports two modes:
        /// 1. Fixed-Size Mode: If 'wellSize' is defined, it creates a grid with fixed-size cells
        ///    wrapped in a ScrollViewer.
        /// 2. Responsive Mode: If 'wellSize' is not defined, it creates a responsive grid inside a Viewbox.
        /// </summary>
        /// <param name="model">The control model defining the plate's properties.</param>
        /// <returns>A FrameworkElement containing the rendered titration plate and its headers.</returns>
        private FrameworkElement? CreateTitrationPlateElement(ControlModel model)
        {
            if (!model.Rows.HasValue || !model.Columns.HasValue || model.Rows.Value <= 0 || model.Columns.Value <= 0)
            {
                return new TextBlock { Text = "TitrationPlate 'rows' and 'columns' must be positive integers.", Foreground = _themeErrorBrush };
            }

            // --- Master Grid for Layout ---
            // This grid will hold the headers and the actual plate.
            var masterGrid = new Grid();
            bool hasRowHeaders = model.RowHeaders?.Any() ?? false;
            bool hasColumnHeaders = model.ColumnHeaders?.Any() ?? false;

            // --- Define Layout for Master Grid ---
            if (hasRowHeaders)
            {
                masterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Column 0 for Row Headers
            }
            masterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Main column for Plate

            if (hasColumnHeaders)
            {
                masterGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Row 0 for Column Headers
            }
            masterGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Main row for Plate

            // Determine the grid cell where the plate will be placed.
            int plateRow = hasColumnHeaders ? 1 : 0;
            int plateColumn = hasRowHeaders ? 1 : 0;

            // --- Header Creation Logic ---
            var headerFontWeight = model.HeaderFontWeight != null
                ? (new FontWeightConverter().ConvertFrom(null, CultureInfo.InvariantCulture, model.HeaderFontWeight) as FontWeight?) ?? FontWeights.Normal
                : FontWeights.Normal;

            // Create and Populate Row Headers (if they exist)
            if (hasRowHeaders)
            {
                var rowHeaderGrid = new Grid();
                Grid.SetRow(rowHeaderGrid, plateRow);
                Grid.SetColumn(rowHeaderGrid, 0);

                // Define rows in the header grid to match the plate's row definitions
                for (int i = 0; i < model.Rows.Value; i++)
                {
                    rowHeaderGrid.RowDefinitions.Add(new RowDefinition { Height = model.WellSize.HasValue ? new GridLength(model.WellSize.Value) : new GridLength(1, GridUnitType.Star) });
                }

                for (int i = 0; i < model.RowHeaders!.Count; i++)
                {
                    if (i >= model.Rows.Value) break;

                    var headerBlock = new TextBlock
                    {
                        Text = model.RowHeaders[i],
                        FontSize = model.HeaderFontSize ?? 12,
                        FontWeight = headerFontWeight,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(0, 0, 10, 0)
                    };

                    Grid.SetRow(headerBlock, i);
                    rowHeaderGrid.Children.Add(headerBlock);
                }
                masterGrid.Children.Add(rowHeaderGrid);
            }

            // Create and Populate Column Headers (if they exist)
            if (hasColumnHeaders)
            {
                var colHeaderGrid = new Grid();
                Grid.SetRow(colHeaderGrid, 0);
                Grid.SetColumn(colHeaderGrid, plateColumn);

                // Define columns in the header grid to match the plate's column definitions
                for (int i = 0; i < model.Columns.Value; i++)
                {
                    colHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = model.WellSize.HasValue ? new GridLength(model.WellSize.Value) : new GridLength(1, GridUnitType.Star) });
                }

                for (int i = 0; i < model.ColumnHeaders!.Count; i++)
                {
                    if (i >= model.Columns.Value) break;

                    var headerBlock = new TextBlock
                    {
                        Text = model.ColumnHeaders[i],
                        FontSize = model.HeaderFontSize ?? 12,
                        FontWeight = headerFontWeight,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 5)
                    };

                    Grid.SetColumn(headerBlock, i);
                    colHeaderGrid.Children.Add(headerBlock);
                }
                masterGrid.Children.Add(colHeaderGrid);
            }

            // --- Plate and Well Creation Logic ---
            var plateGrid = new Grid();
            int numRows = model.Rows.Value;
            int numColumns = model.Columns.Value;

            bool isFixedSize = model.WellSize.HasValue && model.WellSize.Value > 0;

            // Define the plate grid's rows and columns
            if (isFixedSize)
            {
                double size = model.WellSize!.Value;
                for (int i = 0; i < numRows; i++) { plateGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(size) }); }
                for (int i = 0; i < numColumns; i++) { plateGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(size) }); }
            }
            else
            {
                for (int i = 0; i < numRows; i++) { plateGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); }
                for (int i = 0; i < numColumns; i++) { plateGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); }
            }

            Style circularWellStyle = CreateWellStyle();
            // Create a lookup for fast access to well-specific data
            var wellDataMap = model.WellData?.ToDictionary(w => (w.Row, w.Column)) ?? new Dictionary<(int, int), WellModel>();
            var wellMargin = new Thickness(model.WellSpacing ?? 1.0);

            // Get default well styles
            double defaultBorderThickness = model.WellDefaults?.BorderThickness ?? 1.0;
            double defaultFontSize = model.WellDefaults?.FontSize ?? 8.0;
            FontWeight defaultFontWeight = (new FontWeightConverter().ConvertFrom(null, CultureInfo.InvariantCulture, model.WellDefaults?.FontWeight ?? "Normal") as FontWeight?) ?? FontWeights.Normal;
            Brush defaultFontColor = TryParseColor(model.WellDefaults?.FontColor) ?? Brushes.White;

            // Create and style each well
            for (int r = 0; r < numRows; r++)
            {
                for (int c = 0; c < numColumns; c++)
                {
                    var wellButton = new Button
                    {
                        Style = circularWellStyle,
                        BorderBrush = Brushes.Gray,
                        Margin = wellMargin
                    };

                    // Apply specific data if it exists, otherwise use defaults
                    if (wellDataMap.TryGetValue((r, c), out var wellModel))
                    {
                        wellButton.Content = wellModel.Label;
                        wellButton.ToolTip = wellModel.Tooltip;
                        wellButton.Background = TryParseColor(wellModel.Color) ?? Brushes.WhiteSmoke;
                        wellButton.FontSize = wellModel.FontSize ?? defaultFontSize;
                        wellButton.FontWeight = wellModel.FontWeight != null
                            ? (new FontWeightConverter().ConvertFrom(null, CultureInfo.InvariantCulture, wellModel.FontWeight) as FontWeight?) ?? FontWeights.Normal
                            : defaultFontWeight;
                        wellButton.Foreground = TryParseColor(wellModel.FontColor) ?? defaultFontColor;
                        wellButton.BorderThickness = new Thickness(defaultBorderThickness);
                    }
                    else
                    {
                        // Apply defaults for an empty well
                        wellButton.Background = Brushes.WhiteSmoke;
                        wellButton.BorderThickness = new Thickness(defaultBorderThickness);
                    }

                    Grid.SetRow(wellButton, r);
                    Grid.SetColumn(wellButton, c);
                    plateGrid.Children.Add(wellButton);
                }
            }

            // --- Wrapper Selection and Placement ---
            FrameworkElement plateContainer;
            if (isFixedSize)
            {
                // Fixed-size plates get a ScrollViewer in case they overflow
                plateContainer = new ScrollViewer
                {
                    Content = plateGrid,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                };
            }
            else
            {
                // Responsive plates get a Viewbox to scale uniformly
                var border = new Border
                {
                    BorderBrush = Brushes.Transparent,
                    BorderThickness = new Thickness(1),
                    Child = plateGrid
                };
                plateContainer = new Viewbox
                {
                    Child = border,
                    Stretch = Stretch.Uniform
                };
            }

            // --- Add the Plate Container to the Master Grid ---
            Grid.SetRow(plateContainer, plateRow);
            Grid.SetColumn(plateContainer, plateColumn);
            masterGrid.Children.Add(plateContainer);

            return masterGrid;
        }

        #endregion

        #region Factory Helpers

        /// <summary>
        /// Applies common layout properties (Grid Position, Alignment, Margin, etc.) to any FrameworkElement.
        /// Also registers the control's name with the window for later lookup.
        /// </summary>
        /// <param name="element">The element to apply properties to.</param>
        /// <param name="model">The model containing the layout properties.</param>
        /// <param name="applyGridPosition">If true, applies Grid.Row/Column properties. Set to false for nested controls where position is set manually.</param>
        private void ApplyCommonProperties(FrameworkElement element, ControlModel model, bool applyGridPosition = true)
        {
            // Register the name so the control can be found later by other actions (e.g., FindControlByName).
            if (!string.IsNullOrEmpty(model.Name))
            {
                element.Name = model.Name;
                try { this.UnregisterName(model.Name); } catch { /* Ignore if name was not registered */ }
                this.RegisterName(model.Name, element);
            }

            if (applyGridPosition)
            {
                // This logic now ONLY runs for top-level controls added directly to the main window's grid
                Grid.SetRow(element, model.GridRow);
                Grid.SetColumn(element, model.GridColumn);
                if (model.ColumnSpan.HasValue) Grid.SetColumnSpan(element, model.ColumnSpan.Value);
                if (model.RowSpan.HasValue) Grid.SetRowSpan(element, model.RowSpan.Value);
            }

            // Set other common layout properties.
            if (!string.IsNullOrEmpty(model.Margin))
            {
                var marginObj = new ThicknessConverter().ConvertFrom(null, CultureInfo.InvariantCulture, model.Margin);
                if (marginObj is Thickness margin)
                {
                    element.Margin = margin;
                }
            }
            if (model.Width.HasValue)
            {
                element.Width = model.Width.Value;
            }
            if (model.Height.HasValue)
            {
                element.Height = model.Height.Value;
            }
            if (Enum.TryParse(model.HorizontalAlignment, out HorizontalAlignment horizontalAlignment))
            {
                element.HorizontalAlignment = horizontalAlignment;
            }
            if (Enum.TryParse(model.VerticalAlignment, out VerticalAlignment verticalAlignment))
            {
                element.VerticalAlignment = verticalAlignment;
            }

            // Apply visibility (default is Visible).
            element.Visibility = model.IsVisible == false ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Applies basic theme colors to a TabControl.
        /// </summary>
        /// <param name="tabControl">The TabControl instance to theme.</param>
        private void ApplyThemeToTabControl(TabControl tabControl)
        {
            // Apply basic background/foreground, border if needed
            tabControl.Background = _themeControlBackgroundBrush;
            tabControl.Foreground = _themeForegroundBrush;
            tabControl.BorderBrush = _themeBorderBrush;
            tabControl.BorderThickness = new Thickness(1);
        }

        /// <summary>
        /// Creates a Grid container, optionally applying Row/Column definitions from a model.
        /// This is used for nested layouts within Borders, Grids, or TabItems.
        /// </summary>
        /// <param name="children">The list of child models that will be added to this grid. (Available for future sizing logic).</param>
        /// <param name="parentModel">The model for the parent container (e.g., a "Grid") which may contain Row/Column definitions.</param>
        /// <returns>A new Grid, configured with definitions if provided.</returns>
        private Grid CreateNestedGridContainer(List<ControlModel>? children, ControlModel? parentModel = null)
        {
            var grid = new Grid();

            // Apply row/column definitions if provided by the parent Grid model
            if (parentModel?.Type?.ToLowerInvariant() == "grid")
            {
                if (parentModel.RowDefinitions != null)
                {
                    foreach (var rowDef in parentModel.RowDefinitions)
                    {
                        var gridLengthObj = new GridLengthConverter().ConvertFrom(null, CultureInfo.InvariantCulture, rowDef);
                        if (gridLengthObj is GridLength gl) grid.RowDefinitions.Add(new RowDefinition { Height = gl });
                    }
                }
                if (parentModel.ColumnDefinitions != null)
                {
                    foreach (var colDef in parentModel.ColumnDefinitions)
                    {
                        var gridLengthObj = new GridLengthConverter().ConvertFrom(null, CultureInfo.InvariantCulture, colDef);
                        if (gridLengthObj is GridLength gl) grid.ColumnDefinitions.Add(new ColumnDefinition { Width = gl });
                    }
                }
            }
            // If definitions are not provided, or for Border/TabItem, Grid defaults to a single cell ("*").
            // A more robust implementation could auto-calculate rows/cols based on children's GridRow/GridColumn properties.

            return grid;
        }

        /// <summary>
        /// Finds the configured Brush for a log entry based on defined prefixes in the ControlModel.
        /// </summary>
        /// <param name="logText">The full text of the log entry.</param>
        /// <param name="colorMap">The list of color mappings defined in the ControlModel's JSON.</param>
        /// <param name="defaultBrush">The default Brush to use if no prefix matches (usually theme foreground).</param>
        /// <returns>The determined Brush for the log entry.</returns>
        private Brush GetLogForegroundBrush(string logText, List<LogColorMapping>? colorMap, Brush? defaultBrush)
        {
            if (colorMap != null)
            {
                foreach (var mapping in colorMap)
                {
                    // Check if the log text starts with the configured prefix (case-insensitive)
                    if (!string.IsNullOrEmpty(mapping.Prefix) &&
                        logText.Trim().StartsWith(mapping.Prefix.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        // Found a match, parse and return the custom color
                        return TryParseColor(mapping.Color) ?? defaultBrush ?? Brushes.White;
                    }
                }
            }
            // If no match is found or map is null, return the default color
            return defaultBrush ?? Brushes.White;
        }

        /// <summary>
        /// Creates a reusable Style that renders a Button as a circle.
        /// This final version removes the inner Viewbox from the template, allowing the
        /// FontSize property to be correctly applied to the button's content.
        /// </summary>
        /// <returns>A Style object to be applied to a Button.</returns>
        private Style CreateWellStyle()
        {
            var style = new Style(typeof(Button));

            var template = new ControlTemplate(typeof(Button));

            // 1. Outer Border (acts as the border color)
            var outerBorderFactory = new FrameworkElementFactory(typeof(Border));
            outerBorderFactory.SetBinding(Border.BackgroundProperty, new Binding("BorderBrush") { RelativeSource = RelativeSource.TemplatedParent });
            outerBorderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(100));

            // 2. Inner Border (acts as the main background)
            var innerBorderFactory = new FrameworkElementFactory(typeof(Border));
            innerBorderFactory.SetBinding(Border.BackgroundProperty, new Binding("Background") { RelativeSource = RelativeSource.TemplatedParent });
            innerBorderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(100));
            // Bind the margin to the BorderThickness so the outer border is visible
            innerBorderFactory.SetBinding(FrameworkElement.MarginProperty, new Binding("BorderThickness") { RelativeSource = RelativeSource.TemplatedParent });

            // 3. Content Presenter (displays the button's Content, e.g., text)
            var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            // Build the template tree
            innerBorderFactory.AppendChild(contentPresenterFactory);
            outerBorderFactory.AppendChild(innerBorderFactory);

            template.VisualTree = outerBorderFactory;
            style.Setters.Add(new Setter(Control.TemplateProperty, template));

            return style;
        }

        #endregion
    }
}