namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Provides values for the <see cref="DrumPatternQuarter.QuarterType"/> property.
    /// </summary>
    public enum DrumPatternQuarterType
    {
        /// <summary>
        /// No type has been assigned.
        /// </summary>
        None,
        /// <summary>
        /// The quarter is used in the drum pattern header.
        /// </summary>
        Header,
        /// <summary>
        /// The quarter is used in the drum pattern selector body.
        /// </summary>
        PatternSelector,
        /// <summary>
        /// The quarter is used in the drum pattern velocity selector section.
        /// </summary>
        VelocitySelector,
    }
}
