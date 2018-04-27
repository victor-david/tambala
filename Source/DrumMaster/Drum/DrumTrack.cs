using Restless.App.DrumMaster.Core;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Drum
{
    /// <summary>
    /// Represents a drum track
    /// </summary>
    public class DrumTrack : ObjectBase
    {
        #region Private
        private XAudio2 device;

        #endregion

        /************************************************************************/

        #region Public constants
        //public const int MinSteps = 2;
        //public const int MaxSteps = 48;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the drum piece associated with this track.
        /// </summary>
        public DrumPiece Piece
        {
            get;
        }
        
        /// <summary>
        /// Gets the number of steps for this track, i.e. total number of subdivsions.
        /// </summary>
        public int Steps
        {
            get;
        }

        public DrumMatrixVoice[] Voices
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumTrack"/> class.
        /// </summary>
        /// <param name="piece">The drum piece for the track</param>
        /// <param name="steps">The total number of steps, i.e. all subdivisions.</param>
        public DrumTrack(XAudio2 device, DrumPiece piece, int steps)
        {
            ValidateNull(device, nameof(device));
            ValidateNull(piece, nameof(piece));
            ValidateOperation(steps < DrumPattern.MinTotalSteps || steps > DrumPattern.MaxTotalSteps, $"Steps must be between {DrumPattern.MinTotalSteps} and {DrumPattern.MaxTotalSteps} inclusive");
            this.device = device;
            Piece = piece;
            Steps = steps;
            Voices = new DrumMatrixVoice[Steps];

            for (int step = 0; step < Steps; step++)
            {
                Voices[step] = new DrumMatrixVoice(device, Piece);
            }


        }
        #endregion

        public void Play(int step, int operationSet)
        {
            Voices[step].Play(operationSet);
        }
    }
}
