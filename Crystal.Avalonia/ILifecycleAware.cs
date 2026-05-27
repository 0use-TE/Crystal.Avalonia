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
    /// <code>
    /// public class MyViewModel : ObservableObject, ILifecycleAware
    /// {
    ///     public async Task OnLoadedAsync() => await LoadDataAsync();
    ///     public async Task OnUnloaded() => await SaveStateAsync();
    /// }
    /// </code>
    /// </example>
    public interface ILifecycleAware
    {
        /// <summary>
        /// Called once when the View is loaded and ready for interaction.
        /// </summary>
        Task OnLoadedAsync();

        /// <summary>
        /// Called once when the View is unloaded and no longer part of the visual tree.
        /// </summary>
        Task OnUnloaded();
    }
}
