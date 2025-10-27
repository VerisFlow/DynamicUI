using DynamicWpfFromConfig.Models;
using System.Collections.Generic;

namespace DynamicWpfFromConfig.Models
{
    /// <summary>
    /// Contract that all dynamic plugins must implement.
    /// The Host discovers implementations (typically by scanning the /plugins folder)
    /// and instantiates them to obtain UI controls and handle actions.
    /// </summary>
    /// <remarks>
    /// Implementation guidance:
    /// - Implementations should be public and have a public parameterless constructor so the Host can instantiate them.
    /// - The <see cref="Name"/> should be unique across plugins and is used by the Host for display (for example, a Tab header).
    /// - Avoid long-running work on the UI thread inside <see cref="GetControls"/> and <see cref="HandleAction"/>; if needed, offload work to background threads and marshal UI updates via <see cref="IPluginContext"/>.
    /// - Keep plugin state minimal and thread-safe. The Host may call these methods from the UI thread.
    /// - Prefer returning stable, idempotent data from <see cref="GetControls"/>; the Host may call it more than once.
    /// </remarks>
    public interface IDynamicPlugin
    {
        /// <summary>
        /// The unique display name of the plugin.
        /// Used by the Host to identify and present the plugin (for example, in a Tab header).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Called by the Host to obtain the set of UI controls the plugin provides.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ControlModel"/> instances describing the plugin's UI.
        /// The Host will construct visual elements from these models.
        /// </returns>
        /// <remarks>
        /// Implementations should return a ready-to-render description and avoid side effects.
        /// If control content depends on external data, plugins should load that data asynchronously
        /// and update controls via <see cref="IPluginContext"/> when ready.
        /// </remarks>
        List<ControlModel> GetControls();

        /// <summary>
        /// Called by the Host when the user triggers an action originating from one of the plugin's controls.
        /// </summary>
        /// <param name="actionName">The action identifier defined in the control model (for example, "export" or "openFile").</param>
        /// <param name="context">
        /// A sandboxed interface to interact with the Host UI and services. Use <see cref="IPluginContext"/>
        /// to read/write control values, show dialogs, bind data, and perform other Host-provided operations.
        /// </param>
        /// <remarks>
        /// - Keep action handling responsive; perform expensive operations off the UI thread.
        /// - Validate inputs and use the provided <see cref="IPluginContext"/> for UI interactions and error reporting.
        /// - Exceptions should be handled by the plugin; unexpected exceptions may be surfaced by the Host.
        /// </remarks>
        void HandleAction(string actionName, IPluginContext context);
    }
}