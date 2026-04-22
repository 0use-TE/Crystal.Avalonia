using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Crystal.Avalonia
{
    /// <summary>
    /// Defines lifecycle hooks for ViewModel initialization and cleanup
    /// when used with ViewModelLocator or ViewLocator.
    /// </summary>
    /// <remarks>
    /// Both <see cref="ViewModelLocator"/> and <see cref="ViewLocator"/> automatically
    /// subscribe to the View's Loaded and Unloaded events when the DataContext implements
    /// this interface. They call <see cref="OnLoadedAsync"/> once on first load and
    /// <see cref="OnUnloaded"/> once on unload, then unsubscribe immediately to prevent
    /// memory leaks. Exceptions thrown by either method are caught and written to
    /// <see cref="System.Diagnostics.Trace"/>.
    /// </remarks>
    /// <example>
    /// A ViewModel that initializes data on load and cleans up on unload:
    /// <code>
    /// public class MyViewModel : ObservableObject, ILifecycleAware
    /// {
    ///     public async Task OnLoadedAsync()
    ///     {
    ///         await LoadDataAsync();
    ///     }
    ///
    ///     public async Task OnUnloaded()
    ///     {
    ///         await SaveStateAsync();
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface ILifecycleAware
    {
        /// <summary>
        /// Called once when the View is loaded and ready for interaction.
        /// Use for async initialization like loading data, starting animations,
        /// or subscribing to events.
        /// </summary>
        Task OnLoadedAsync();

        /// <summary>
        /// Called once when the View is unloaded and no longer part of the visual tree.
        /// Use for cleanup like releasing resources, saving state, or canceling operations.
        /// </summary>
        Task OnUnloaded();
    }
}