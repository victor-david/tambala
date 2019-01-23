namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Defines properties for selectors.
    /// </summary>
    internal interface ISelector : ISelectable
    {
        /// <summary>
        /// Gets or sets the selector size.
        /// </summary>
        double SelectorSize { get; set; }
        /// <summary>
        /// Gets or sets the division count
        /// </summary>
        int DivisionCount { get; set; }
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        int Position { get; set; }
    }
}
