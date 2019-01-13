using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Controls.Obsolete
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
        /// A step will play on pass 1, 3, 5, 7, etc.
        /// </summary>
        OddPass,
        /// <summary>
        /// A step will play on pass 2, 4, 6, 8 etc.
        /// </summary>
        SecondPass,
        /// <summary>
        /// A step will play on pass 3, 6, 9, etc.
        /// </summary>
        ThirdPass3,
        /// <summary>
        /// A step will play on pass 3, 7, 11, 15, etc.
        /// </summary>
        ThirdPass4,
        /// <summary>
        /// A step will play on pass 4, 8, 12, 16, etc.
        /// </summary>
        FourthPass,

    }
}
