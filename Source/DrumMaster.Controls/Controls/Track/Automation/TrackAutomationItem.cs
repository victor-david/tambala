using System;

namespace Restless.App.DrumMaster.Controls.Automation
{
    /// <summary>
    /// Represents a track automation item
    /// </summary>
    public class TrackAutomationItem
    {
        #region Private
        #endregion
        
        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the automation type
        /// </summary>
        public TrackAutomationType AutomationType
        {
            get;
        }

        /// <summary>
        /// Gets the pass number when this automation item begins.
        /// </summary>
        public int FirstPass
        {
            get;
        }

        /// <summary>
        /// Gets the number of passes that this automation item lasts.
        /// </summary>
        public int Duration
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackAutomationItem"/> class.
        /// </summary>
        /// <param name="firstPass">The first pass.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="automationType">the type of automation.</param>
        internal TrackAutomationItem(int firstPass, int duration, TrackAutomationType automationType)
        {
            if (firstPass < 1) throw new ArgumentException($"{nameof(FirstPass)} must be one or greater");
            if (duration < 1) throw new ArgumentException($"{nameof(Duration)} must be one or greater");
            FirstPass = firstPass;
            Duration = duration;
            AutomationType = automationType;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets a string representation of this item.
        /// </summary>
        /// <returns>A string that describes the item</returns>
        public override string ToString()
        {
            return $"TrackAutomationItem. Type: {AutomationType} Start: {FirstPass} Duration: {Duration}";
        }
        #endregion
    }
}
