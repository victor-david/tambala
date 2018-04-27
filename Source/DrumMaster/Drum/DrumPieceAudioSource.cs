using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Drum
{
    /// <summary>
    /// Provides enumerated value that describe the source of the drum piece.
    /// </summary>
    public enum DrumPieceAudioSource
    {
        /// <summary>
        /// The drum piece audio is from a file.
        /// </summary>
        FileSystem,
        /// <summary>
        /// The drum piece audio is from an application resource.
        /// </summary>
        AppResource
    }
}
