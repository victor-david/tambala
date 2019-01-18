namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Defines properties for classes that implement quarter note partitioning
    /// </summary>
    internal interface IQuarterNote
    {
        /// <summary>
        /// Gets or sets the number of quarter notes.
        /// </summary>
        int QuarterNoteCount { get; set; }

        /// <summary>
        /// Gets or sets the number of ticks per quarter note.
        /// </summary>
        int TicksPerQuarterNote { get; set; }
        
        /// <summary>
        /// Gets or sets the scale to use when presenting the quarter notes.
        /// </summary>
        double Scale { get; set; }
    }
}
