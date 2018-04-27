using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Drum
{
    /// <summary>
    /// Provides an enumeration that is used to categorize the type of a drum piece.
    /// </summary>
    public enum DrumPieceType
    {
        /// <summary>
        /// The drum piece is a cymbal.
        /// </summary>
        Cymbal,
        /// <summary>
        /// The drum piece is a high hat.
        /// </summary>
        HiHat,
        /// <summary>
        /// The drum piece is a kick drum.
        /// </summary>
        Kick,
        /// <summary>
        /// The drum piece is a snare drum.
        /// </summary>
        Snare,
        /// <summary>
        /// The drum piece is a tom.
        /// </summary>
        Tom,
        /// <summary>
        /// The drum piece type is unknown.
        /// </summary>
        Unknown,
    }
}
