using DynamicWpfFromConfig.Models;
using Microsoft.Win32;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DynamicWpfFromConfig
{
    /// <summary>
    /// The main window of the application, which serves as the rendering engine for the dynamic UI.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        /// <summary>
        /// Holds the entire deserialized UI configuration from the JSON file.
        /// Declared as nullable to satisfy C# nullable reference type analysis.
        /// </summary>
        private UiConfig? _uiConfig;

        // Theme brushes, stored for efficient access when creating controls.
        private Brush? _themeBackgroundBrush;
        private Brush? _themeForegroundBrush;
        private Brush? _themeControlBackgroundBrush;
        private Brush? _themeAccentBrush;
        private Brush? _themeAccentForegroundBrush;
        private Brush? _themeBorderBrush;
        private Brush? _themeErrorBrush;

        /// <summary>
        /// Holds all discovered and loaded plugin instances.
        /// </summary>
        private List<IDynamicPlugin> _loadedPlugins = new List<IDynamicPlugin>();

        /// <summary>
        /// Defines the name of the application settings file.
        /// </summary>
        private const string AppSettingsFile = "app-settings.json";

        /// <summary>
        /// Defines the directory where UI configuration files are stored.
        /// </summary>
        private const string UiConfigDirectory = "ui-configs";

        /// <summary>
        /// Stores the full path to the active UI config file, resolved from app-settings.json.
        /// </summary>
        private string _activeUiConfigPath = string.Empty;

        #endregion

        #region Constructor and Window Events

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// This constructor performs critical pre-initialization tasks, including:
        /// 1. Loading application settings to find the active UI config.
        /// 2. Pre-parsing the UI config to apply window styles (like AllowsTransparency)
        ///    *before* the window is shown and InitializeComponent() is called.
        /// </summary>
        public MainWindow()
        {
            try
            {
                var appSettingsFile = $"{AppContext.BaseDirectory}{AppSettingsFile}";

                // 1. Find and load the main app settings file
                if (!File.Exists(appSettingsFile))
                {
                    throw new FileNotFoundException($"Primary settings file not found: {appSettingsFile}");
                }

                string settingsContent = File.ReadAllText(appSettingsFile);
                var settings = JsonSerializer.Deserialize<AppSettings>(settingsContent);

                if (settings == null || string.IsNullOrEmpty(settings.ActiveUiConfigFile))
                {
                    throw new InvalidDataException($"Failed to read 'ActiveUiConfigFile' from {appSettingsFile}");
                }

                // 2. Resolve the full path to the active UI config
                _activeUiConfigPath = Path.Combine($"{AppContext.BaseDirectory}{UiConfigDirectory}", settings.ActiveUiConfigFile);

                if (!File.Exists(_activeUiConfigPath))
                {
                    throw new FileNotFoundException($"The UI config file specified in {appSettingsFile} was not found: {_activeUiConfigPath}");
                }
            }
            catch (Exception ex)
            {
                // A failure here is fatal, as we don't know which UI to load.
                MessageBox.Show($"Fatal configuration error: {ex.Message}\nApplication will close.", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            try
            {
                // 3. Pre-load: Read the file just to get window style settings
                // This is necessary to apply styles like 'None' and 'AllowsTransparency'
                // before InitializeComponent() is called.
                string jsonContent = File.ReadAllText(_activeUiConfigPath);

                using (JsonDocument document = JsonDocument.Parse(jsonContent))
                {
                    if (document.RootElement.TryGetProperty("window", out JsonElement windowElement))
                    {
                        var earlyWindowConfig = windowElement.Deserialize<WindowConfig>();

                        if (earlyWindowConfig != null)
                        {
                            if (earlyWindowConfig.AllowsTransparency.HasValue)
                            {
                                this.AllowsTransparency = earlyWindowConfig.AllowsTransparency.Value;
                            }
                            if (!string.IsNullOrEmpty(earlyWindowConfig.WindowStyle))
                            {
                                if (Enum.TryParse(earlyWindowConfig.WindowStyle, out WindowStyle style))
                                {
                                    this.WindowStyle = style;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // If pre-loading fails, log it and fall back to default window styles.
                Console.WriteLine($"Error pre-loading window style config: {ex.Message}");
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.AllowsTransparency = false;
            }

            // Finally, initialize the XAML component (this.Window).
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the window.
        /// This is the main entry point for loading plugins and building the dynamic UI.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadPlugins();
                _uiConfig = LoadConfigAndBuildUi(_activeUiConfigPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading UI configuration: {ex.Message}", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region UI Loading and Building

        /// <summary>
        /// Reads the JSON config file, deserializes it, and orchestrates the entire UI build process.
        /// </summary>
        /// <param name="configPath">The path to the ui-config.json file.</param>
        /// <returns>The deserialized UiConfig object.</returns>
        private UiConfig? LoadConfigAndBuildUi(string configPath)
        {
            // 1. Read and deserialize the configuration file.
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException("The configuration file was not found.", configPath);
            }
            string jsonContent = File.ReadAllText(configPath);
            var uiConfig = JsonSerializer.Deserialize<UiConfig>(jsonContent);
            if (uiConfig == null)
            {
                throw new InvalidDataException("Failed to parse the UI configuration file.");
            }

            // 2. Apply window properties.
            if (uiConfig.Window != null)
            {
                this.Title = uiConfig.Window.Title;

                // WindowStyle and AllowsTransparency are set in the constructor (pre-init).
                // We re-set WindowStyle here in case it wasn't set pre-init.
                if (!string.IsNullOrEmpty(uiConfig.Window.WindowStyle))
                {
                    if (Enum.TryParse(uiConfig.Window.WindowStyle, out WindowStyle style))
                    {
                        this.WindowStyle = style;
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Invalid WindowStyle '{uiConfig.Window.WindowStyle}' specified in config.");
                    }
                }

                // Apply sizing properties
                if (Enum.TryParse(uiConfig.Window.SizeToContent, out SizeToContent sizeMode))
                {
                    this.SizeToContent = sizeMode;
                    // If SizeToContent is not Manual, Width/Height may act as initial size
                    if (uiConfig.Window.Width.HasValue) this.Width = uiConfig.Window.Width.Value;
                    if (uiConfig.Window.Height.HasValue) this.Height = uiConfig.Window.Height.Value;
                }
                else
                {
                    // Default to Manual sizing
                    this.SizeToContent = SizeToContent.Manual;
                    if (uiConfig.Window.Width.HasValue) this.Width = uiConfig.Window.Width.Value;
                    if (uiConfig.Window.Height.HasValue) this.Height = uiConfig.Window.Height.Value;
                }

                if (uiConfig.Window.MinHeight.HasValue)
                {
                    this.MinHeight = uiConfig.Window.MinHeight.Value;
                }
                if (uiConfig.Window.MinWidth.HasValue)
                {
                    this.MinWidth = uiConfig.Window.MinWidth.Value;
                }
            }

            // 3. Apply the color theme.
            ApplyTheme(uiConfig.Theme);

            // 4. Build the Grid layout structure.
            BuildGridLayout(uiConfig.LayoutDefinition);

            // 5. Clear any existing controls and generate new ones from the config.
            DynamicControlsContainer.Children.Clear();
            if (uiConfig.Controls != null)
            {
                foreach (var controlModel in uiConfig.Controls)
                {
                    FrameworkElement? control = CreateControl(controlModel);
                    if (control != null)
                    {
                        DynamicControlsContainer.Children.Add(control);
                    }
                }
            }

            return uiConfig;
        }

        /// <summary>
        /// Parses the theme configuration and applies colors to the window and theme brushes.
        /// </summary>
        /// <param name="theme">The theme configuration object, or null to skip theming.</param>
        private void ApplyTheme(ThemeConfig? theme)
        {
            if (theme == null) return;

            // Parse color strings into Brush objects and store them for the factory.
            _themeBackgroundBrush = TryParseColor(theme.Background);
            _themeForegroundBrush = TryParseColor(theme.Foreground);
            _themeControlBackgroundBrush = TryParseColor(theme.ControlBackground);
            _themeAccentBrush = TryParseColor(theme.Accent);
            _themeAccentForegroundBrush = TryParseColor(theme.AccentForeground);
            _themeBorderBrush = TryParseColor(theme.BorderColor);
            _themeErrorBrush = TryParseColor(theme.ErrorForeground);

            // Apply base colors to the window and main container.
            this.Background = _themeBackgroundBrush;
            this.Foreground = _themeForegroundBrush;
            DynamicControlsContainer.Background = _themeBackgroundBrush;
        }

        /// <summary>
        /// Dynamically builds the Grid's rows and columns based on the layout definition.
        /// </summary>
        /// <param name="layout">The layout definition object, or null to skip grid generation.</param>
        private void BuildGridLayout(LayoutDefinition? layout)
        {
            if (layout == null) return;

            DynamicControlsContainer.RowDefinitions.Clear();
            DynamicControlsContainer.ColumnDefinitions.Clear();

            // Create Row Definitions
            if (layout.RowDefinitions != null)
            {
                foreach (var rowDef in layout.RowDefinitions)
                {
                    var gridLengthObj = new GridLengthConverter().ConvertFrom(null, CultureInfo.InvariantCulture, rowDef);
                    if (gridLengthObj is GridLength gridLength)
                    {
                        DynamicControlsContainer.RowDefinitions.Add(new RowDefinition { Height = gridLength });
                    }
                }
            }

            // Create Column Definitions
            if (layout.ColumnDefinitions != null)
            {
                foreach (var colDef in layout.ColumnDefinitions)
                {
                    var gridLengthObj = new GridLengthConverter().ConvertFrom(null, CultureInfo.InvariantCulture, colDef);
                    if (gridLengthObj is GridLength gridLength)
                    {
                        DynamicControlsContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = gridLength });
                    }
                }
            }
        }

        #endregion

        #region Plugin Loading

        /// <summary>
        /// Scans the /plugins directory, loads all valid plugin DLLs,
        /// and stores instances of them in the _loadedPlugins list.
        /// </summary>
        private void LoadPlugins()
        {
            string pluginDir = Path.Combine(AppContext.BaseDirectory, "plugins");
            if (!Directory.Exists(pluginDir))
            {
                Directory.CreateDirectory(pluginDir);
                return; // No plugins to load
            }

            _loadedPlugins.Clear();
            foreach (string dllPath in Directory.GetFiles(pluginDir, "*.dll"))
            {
                try
                {
                    Assembly pluginAssembly = Assembly.LoadFrom(dllPath);
                    foreach (Type type in pluginAssembly.GetTypes())
                    {
                        // Load all public, non-interface types that implement the contract
                        if (typeof(IDynamicPlugin).IsAssignableFrom(type) && !type.IsInterface)
                        {
                            var pluginInstance = Activator.CreateInstance(type) as IDynamicPlugin;
                            if (pluginInstance != null)
                            {
                                _loadedPlugins.Add(pluginInstance);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log and continue if a single plugin fails
                    MessageBox.Show($"Failed to load plugin '{Path.GetFileName(dllPath)}': {ex.Message}", "Plugin Load Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        #endregion
    }
}