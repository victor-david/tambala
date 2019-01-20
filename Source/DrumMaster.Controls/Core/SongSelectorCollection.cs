﻿using System;

namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Represents a collection of <see cref="PointSelector"/> objects that comprise a song.
    /// </summary>
    public class SongSelectorCollection
    {
        #region Private
        private readonly PointSelector[,] points;
        private readonly int rowLen;
        private readonly int colLen;
        private int[] selectedRows; // = new int[Constants.DrumPattern.MaxCount] { 1, 2, 3, 5 };
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SongSelectorCollection"/> class.
        /// </summary>
        internal SongSelectorCollection()
        {
            points = new PointSelector[Constants.DrumPattern.MaxCount, Constants.Selector.Count.Max];
            rowLen = points.GetLength(0);
            colLen = points.GetLength(1);
            selectedRows = new int[Constants.DrumPattern.MaxCount] { -1, -1, -1, -1 };
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Clears all objects from the collection
        /// </summary>
        internal void Clear()
        {
            for (int row = 0; row < rowLen; row++)
            {
                for (int col = 0; col < colLen; col++)
                {
                    points[row, col] = null;
                }
            }
        }

        /// <summary>
        /// Adds a new item for the specified row and position.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="position">The position.</param>
        /// <param name="selector">The selector.</param>
        internal void Add(int row, int position, PointSelector selector)
        {
            if (row < 0 || row > rowLen - 1) throw new ArgumentOutOfRangeException(nameof(row));
            if (position < 0 || position > colLen - 1) throw new ArgumentOutOfRangeException(nameof(position));
            points[row, position] = selector ?? throw new ArgumentNullException(nameof(selector));
        }

        /// <summary>
        /// Gets the count of <see cref="PointSelector"/> objects
        /// that are currently selected.
        /// </summary>
        /// <returns></returns>
        internal int GetSelectedCount()
        {
            int count = 0;
            DoForAllSelected((p) => count++);
            return count;
        }

        /// <summary>
        /// Gets the highest numbered position of all <see cref="PointSelector"/> objects.
        /// </summary>
        /// <returns>The highest numbered position, zero if nothing selected.</returns>
        internal int GetMaxSelectedPosition()
        {
            int max = 0;
            DoForAllSelected((p) => max = Math.Max(max, p.ThreadSafePosition));
            return max;
        }

        internal int GetRowAtPosition(int position)
        {
            if (position < 0 || position > colLen - 1) throw new ArgumentOutOfRangeException(nameof(position));
            for (int row=0; row < rowLen; row++)
            {
                if (points[row, position] != null && points[row, position].ThreadSafeIsSelected)
                {
                    return row;
                }
            }
            return -1;
        }

        internal int[] GetRowsAtPosition(int position)
        {
            for (int row = 0; row < rowLen; row++)
            {
                selectedRows[row] = -1;
                if (points[row, position] != null && points[row, position].ThreadSafeIsSelected)
                {
                    selectedRows[row] = row;
                }
            }
            return selectedRows;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void DoForAllSelected(Action<PointSelector> action)
        {
            for (int row = 0; row < rowLen; row++)
            {
                for (int col = 0; col < colLen; col++)
                {
                    if (points[row, col] != null && points[row, col].ThreadSafeIsSelected)
                    {
                        action(points[row, col]);
                    }
                }
            }
        }
        #endregion
    }
}