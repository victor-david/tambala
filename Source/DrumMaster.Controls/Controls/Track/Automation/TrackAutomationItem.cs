using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Controls.Automation
{
    /// <summary>
    /// Represents a track automation item
    /// </summary>
    public class TrackAutomationItem
    {
        #region Properties
        /// <summary>
        /// Gets the pass number when this automation item begins.
        /// </summary>
        public int StartPass
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
        internal TrackAutomationItem()
        {

        }
        #endregion
    }
}
