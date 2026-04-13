namespace Crystal.Avalonia
{
    /// <summary>
    /// Provides global configuration options for the Crystal.Avalonia framework.
    /// </summary>
    public static class CrystalOptions
    {
        /// <summary>
        /// Gets or sets whether the ViewModelLocator is enabled.
        /// When enabled, the system automatically resolves the corresponding ViewModel
        /// based on the View type and injects it into the DataContext.
        /// Defaults to <c>true</c>.
        /// </summary>
        /// <remarks>
        /// When enabled, the <see cref="ViewModelLocator.AutoWireViewModelProperty"/> attached property takes effect.
        /// You can set <c>ViewModelLocator.AutoWireViewModel="True"</c> in XAML to enable auto-binding.
        /// </remarks>
        public static bool EnableViewModelLocator { get; set; } = true;
    }
}
