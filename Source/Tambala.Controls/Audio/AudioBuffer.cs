using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.Tambala.Controls.Audio
{
    /// <summary>
    /// Extends SharpDX.XAudio2.AudioBuffer to encapsulate other properties.
    /// </summary>
    internal class AudioBuffer : SharpDX.XAudio2.AudioBuffer
    {
        /// <summary>
        /// From this assembly, gets or sets the wave format.
        /// </summary>
        internal WaveFormat WaveFormat
        {
            get;
            set;
        }

        /// <summary>
        /// From this assembly, gets or sets the decoded packet info.
        /// </summary>
        internal uint[] DecodedPacketsInfo
        {
            get;
            set;
        }
    }
}
