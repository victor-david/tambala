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
        QuarterNote = 1,
        /// <summary>
        /// The unit is an eighth note.
        /// </summary>
        EighthNote = 2,
        /// <summary>
        /// The unit is an eighth note triplet.
        /// </summary>
        EighthNoteTriplet = 3,
        /// <summary>
        /// The unit is a sixteenth note.
        /// </summary>
        SixteenthNote = 4,
        /// <summary>
        /// The unit is a thirty second note.
        /// </summary>
        ThirtySecondNote = 8,
    }
}
