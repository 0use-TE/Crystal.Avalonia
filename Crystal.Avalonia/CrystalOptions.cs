namespace Crystal.Avalonia
{
    /// <summary>
    /// Configuration options for the Crystal.Avalonia framework.
    /// Registered as a singleton in DI and available via <see cref="CrystalApplication.Options"/>.
    /// Configure by overriding <see cref="CrystalApplication.ConfigureOptions"/>.
    /// </summary>
    public class CrystalOptions
    {
        /// <summary>
        /// Gets or sets whether ViewModel-first view location is enabled.
        /// When enabled, a ViewLocator is added to <c>Application.DataTemplates</c>
        /// so a ContentControl can display a registered View for a ViewModel.
        /// Defaults to <c>true</c>.
        /// </summary>
        /// <remarks>
        /// This option does <b>not</b> control <see cref="ViewModelLocator.AutoWireViewModelProperty"/>.
        /// View-first auto-wiring via the attached property always works when a mapping is registered.
        /// </remarks>
        public bool EnableViewLocator { get; set; } = true;
    }
}
