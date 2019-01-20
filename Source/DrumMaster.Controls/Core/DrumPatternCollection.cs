using System;

namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Represents a collection of <see cref="DrumPattern"/> objects.
    /// </summary>
    public class DrumPatternCollection : GenericList<DrumPattern>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumPatternCollection"/> class.
        /// </summary>
        internal DrumPatternCollection()
        {
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Creates a <see cref="DrumPatternCollection"/> from this collection
        /// using the specified indices.
        /// </summary>
        /// <param name="indices">The indices</param>
        /// <returns>A list of drum patterns</returns>
        internal DrumPatternCollection CreateFromIndices(int[] indices)
        {
            DrumPatternCollection result = new DrumPatternCollection();
            foreach (int idx in indices)
            {
                if (idx >= 0)
                {
                    result.Add(this[idx]);
                }
            }
            return result;
        }

        /// <summary>
        /// Get the largest quarter note count from the list of drum patterns.
        /// </summary>
        /// <returns>The largest quarter note count.</returns>
        /// <remarks>
        /// When playing a song that has more than one pattern at the same position,
        /// the number of quarter notes to play is determined by the pattern at that
        /// position with the highest number of quarter notes. The play methods then
        /// cycles through the quarter notes; any patterns that have less than the
        /// maximum only play through their individual quarter note count.
        /// </remarks>
        internal int GetMaxQuarterNoteCount()
        {
            int max = 0;
            foreach (DrumPattern p in this)
            {
                max = Math.Max(max, p.ThreadSafeController.ThreadSafeQuarterNoteCount);
            }
            return max;
        }
        #endregion
    }
}