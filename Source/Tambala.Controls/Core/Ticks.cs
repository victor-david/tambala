using System.Collections.Generic;

namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Defines characteristics of ticks and provides position mapping.
    /// </summary>
    internal static class Ticks
    {
        /// <summary>
        /// Gets the lowest common denominator for ticks.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This value represents the lowest common denominator for the various types of ticks.
        /// A quarter note needs 1 tick; 8th note 2 ticks; 16th note 4 ticks; 32nd note 8 ticks;
        /// 8th triplet 3 ticks.
        /// </para>
        /// <para>
        /// This value enables all types of ticks to be evenly distributed within a quarter note.
        /// LowestCommon is the total number of positions within a quarter note.
        /// See <see cref="FullTickPositionMap"/>, <see cref="UniqueTickPositionMap"/>, and <see cref="Playable"/>
        /// for more information.
        /// </para>
        /// </remarks>
        internal const int LowestCommon = 24;

        /// <summary>
        /// Provides a dictionary indexed by tick count that maps the tick count
        /// to its position within the quarter note.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This data structure maps the number of ticks to its position or positions within 
        /// the quarter note. All positions are mapped including those that share a position
        /// with another tick count.
        /// </para>
        /// <para>
        /// This data structure does not map the quarter note itself; that is always at position zero.
        /// </para>
        /// </remarks>
        internal static readonly Dictionary<int, List<int>> FullTickPositionMap = new Dictionary<int, List<int>>
        {
            { 2, new List<int>() { 12 } }, // 8th
            { 4, new List<int>() { 6, 12, 18 } }, // 16th
            { 8, new List<int>() { 3, 6, 9, 12, 15, 18, 21 } }, // 32nd 
            { 3, new List<int>() { 8, 16 } }, // 8th triplet
        };

        /// <summary>
        /// Provides a dictionary indexed by <see cref="PointSelectorUnit"/> that maps the selector
        /// unit to its position within the quarter note.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This data structure maps point selector units to their position or positions within
        /// the quarter note. Unlike <see cref="FullTickPositionMap"/>, it only maps positions
        /// that are specific to that selector unit. For example, a sixteenth note shares a position
        /// with an eighth note, but this data structure only maps the positions of the sixteenth note
        /// that are specific to it. Similarly, a 32nd note share positions with both sixteenth notes
        /// and eighth notes, but only the positions unique to the 32nd note are mapped.
        /// </para>
        /// <para>
        /// This data structure does not map the quarter note itself; that is always at position zero.
        /// </para>
        /// </remarks>
        internal static readonly Dictionary<PointSelectorUnit, List<int>> UniqueTickPositionMap = new Dictionary<PointSelectorUnit, List<int>>
        {
            { PointSelectorUnit.EighthNote, new List<int>() { 12 } }, // 8th
            { PointSelectorUnit.SixteenthNote, new List<int>() { 6, 18 } }, // 16th
            { PointSelectorUnit.ThirtySecondNote, new List<int>() { 3, 9, 15, 21 } }, // 32nd 
            { PointSelectorUnit.EighthNoteTriplet, new List<int>() { 8, 16 } }, // 8th triplet
        };

        /// <summary>
        /// Provides a hash set of playable positions.
        /// </summary>
        /// <remarks>
        /// Of the total positions represented by <see cref="LowestCommon"/>, only some of
        /// the positions are playable. This data structure provides those playable positions.
        /// </remarks>
        internal static readonly HashSet<int> Playable = new HashSet<int>
        {
            0, 3, 6, 8, 9, 12, 15, 16, 18, 21
        };
    }
}
