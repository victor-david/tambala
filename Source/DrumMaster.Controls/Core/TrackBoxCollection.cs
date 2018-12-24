using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Represents a collection of <see cref="TrackBox"/> objects.
    /// </summary>
    public class TrackBoxCollection : List<TrackBox>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackBoxCollection"/> class.
        /// </summary>
        internal TrackBoxCollection()
        {

        }

        /************************************************************************/

        /// <summary>
        /// Shifts the notes and/or volume in this collection to the left.
        /// </summary>
        /// <param name="shiftFrequency">true to shift the notes (i.e. frequency)</param>
        /// <param name="shiftVolume">true to shift the volume bias.</param>
        public void ShiftLeft(bool shiftFrequency, bool shiftVolume)
        {
            if (Count > 1)
            {
                StepPlayFrequency firstBoxFreq = this[0].PlayFrequency;
                float firstBoxBias = this[0].VolumeBias;
                for (int k = 0; k < Count - 1; k++)
                {
                    if (shiftFrequency)
                    {
                        this[k].PlayFrequency = this[k + 1].PlayFrequency;
                    }
                    if (shiftVolume)
                    {
                        this[k].VolumeBias = this[k + 1].VolumeBias;
                    }
                }
                if (shiftFrequency)
                {
                    this[Count - 1].PlayFrequency = firstBoxFreq;
                }
                if (shiftVolume)
                {
                    this[Count - 1].VolumeBias = firstBoxBias;
                }
            }
        }

        /// <summary>
        /// Shifts the notes and/or volume in this collection to the right.
        /// </summary>
        /// <param name="shiftFrequency">true to shift the notes (i.e. frequency)</param>
        /// <param name="shiftVolume">true to shift the volume bias.</param>
        public void ShiftRight(bool shiftFrequency, bool shiftVolume)
        {
            if (Count > 1)
            {
                StepPlayFrequency lastBoxFreq = this[Count - 1].PlayFrequency;
                float lastBoxBias = this[Count - 1].VolumeBias;

                for (int k = Count - 1; k > 0; k--)
                {
                    if (shiftFrequency)
                    {
                        this[k].PlayFrequency = this[k - 1].PlayFrequency;
                    }
                    if (shiftVolume)
                    {
                        this[k].VolumeBias = this[k - 1].VolumeBias;
                    }
                }
                if (shiftFrequency)
                {
                    this[0].PlayFrequency = lastBoxFreq;
                }
                if (shiftVolume)
                {
                    this[0].VolumeBias = lastBoxBias;
                }
            }
        }

    }
}
