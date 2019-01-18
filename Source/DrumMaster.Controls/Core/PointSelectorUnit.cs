using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Provides enumerated values for selector units.
    /// </summary>
    public enum PointSelectorUnit
    {
        /// <summary>
        /// No unit is associated with the selector.
        /// </summary>
        None = 0,
        /// <summary>
        /// The unit is a quarter note.
        /// </summary>
        QuarterNote,
        /// <summary>
        /// The unit is an eighth note.
        /// </summary>
        EighthNote,
        /// <summary>
        /// The unit is an eighth note triplet.
        /// </summary>
        EighthNoteTriplet,
        /// <summary>
        /// The unit is a sixteenth note.
        /// </summary>
        SixteenthNote,
        /// <summary>
        /// The unit is a thirty second note.
        /// </summary>
        ThirtySecondNote,
    }
}
