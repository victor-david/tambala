using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Provides a enumeration that describes how often an activated step will play.
    /// </summary>
    public enum StepPlayFrequency
    {
        /// <summary>
        /// None. The step will not play.
        /// </summary>
        None,
        /// <summary>
        /// A step will play on every pass. 
        /// </summary>
        EveryPass,
        /// <summary>
        /// A step will play on pass 1,3,5,7, etc.
        /// </summary>
        EveryOddPass,
        /// <summary>
        /// A step will play on pass 2,4,6 etc.
        /// </summary>
        EverySecondPass,
        /// <summary>
        /// A step will play on pass 3, 6, 9, etc.
        /// </summary>
        EveryThirdPass,
        /// <summary>
        /// A step will play every fourth pass.
        /// </summary>
        EveryFourthPass,
        /// <summary>
        /// The default.
        /// </summary>
        Default = None
    }
}
