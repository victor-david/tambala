using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Controls.Audio
{
    /// <summary>
    /// Provides event arguments for the for audio render.
    /// </summary>
    public class AudioRenderEventArgs : EventArgs
    {
        #region Public properties
        /// <summary>
        /// Gets the audio render parameters associated with the event.
        /// </summary>
        public AudioRenderParameters AudioParms
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor (internal)
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRenderParameters"/> class.
        /// </summary>
        /// <param name="audioParms">The audio parameters</param>
        internal AudioRenderEventArgs(AudioRenderParameters audioParms)
        {
            AudioParms = audioParms ?? throw new ArgumentNullException(nameof(audioParms));
        }
        #endregion
    }
}
