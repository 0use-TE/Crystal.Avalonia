using System.Threading.Tasks;

namespace Crystal.Avalonia
{
    /// <summary>
    /// Defines lifecycle hooks for ViewModel initialization and cleanup
    /// when used with ViewModelLocator or ViewLocator.
    /// </summary>
    /// <remarks>
    /// Both <see cref="ViewModelLocator"/> and ViewLocator subscribe to the View's
    /// Loaded and Unloaded events when the DataContext implements this interface.
    /// <see cref="OnLoadedAsync"/> is invoked on every Loaded. The <c>isFirstLoad</c> argument
    /// is <c>true</c> only the first time that ViewModel instance is loaded.
    /// Transient ViewModels therefore always see <c>true</c>; a singleton sees <c>false</c> on later visits.
    /// <see cref="OnUnloaded"/> is invoked on every Unload. Exceptions are caught and written to
    /// <see cref="System.Diagnostics.Trace"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class MyViewModel : ObservableObject, ILifecycleAware
    /// {
    ///     public async Task OnLoadedAsync(bool isFirstLoad)
    ///     {
    ///         if (!isFirstLoad) return;
    ///         await LoadDataAsync();
    ///     }
    ///
    ///     public Task OnUnloaded() => Task.CompletedTask;
    /// }
    /// </code>
    /// </example>
    public interface ILifecycleAware
    {
        /// <summary>
        /// Called when the View is loaded and ready for interaction.
        /// </summary>
        /// <param name="isFirstLoad">
        /// <c>true</c> the first time this ViewModel instance is loaded;
        /// <c>false</c> on subsequent loads of the same instance (for example a singleton after navigation).
        /// </param>
        Task OnLoadedAsync(bool isFirstLoad);

        /// <summary>
        /// Called when the View is unloaded and no longer part of the visual tree.
        /// Invoked on every unload.
        /// </summary>
        Task OnUnloaded();
    }
}
