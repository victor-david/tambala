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
        /// If activated, a step will play on every pass.
        /// </summary>
        EveryPass,
        /// <summary>
        /// If activated, a step will play on odd numbered passes only.
        /// </summary>
        OddPassOnly,
        /// <summary>
        /// If activated, a step will play on even numbered passes only.
        /// </summary>
        EvenPassOnly,
        /// <summary>
        /// The default.
        /// </summary>
        Default = None


    }
}
