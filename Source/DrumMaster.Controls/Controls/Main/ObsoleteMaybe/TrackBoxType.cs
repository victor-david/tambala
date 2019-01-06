namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Identifies the type of <see cref="TrackBox"/> objects
    /// used in a <see cref="TrackBoxContainerBase"/> container.
    /// </summary>
    public enum TrackBoxType
    {
        /// <summary>
        /// No type has been assigned.
        /// </summary>
        None,
        /// <summary>
        /// The container contains header boxes.
        /// </summary>
        Header,
        /// <summary>
        /// The container contains track step boxes.
        /// </summary>
        TrackStep,
    }
}
