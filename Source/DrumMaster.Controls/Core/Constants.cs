using System;
using System.Windows;

namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Provides static constant values that are used throughout the control library.
    /// </summary>
    public static class Constants
    {
        #region Selector
        /// <summary>
        /// Provides static values that define characteristics of the song presenter
        /// </summary>
        public static class Selector
        {
            /// <summary>
            /// Provides static values that define selector size characteristics.
            /// </summary>
            public static class Size
            {
                /// <summary>
                /// The minimum selector size.
                /// </summary>
                public const double Min = 20.0;

                /// <summary>
                /// The maximum selector size.
                /// </summary>
                public const double Max = 30.0;

                /// <summary>
                /// The default selector size.
                /// </summary>
                public const double Default = 24.0;
            }

            /// <summary>
            /// Provides static values that define selector division characteristics.
            /// </summary>
            public static class Division
            {
                /// <summary>
                /// The minimum number of selector divisions.
                /// </summary>
                public const int Min = 3;

                /// <summary>
                /// The maximum number of selector divisions.
                /// </summary>
                public const int Max = 6;

                /// <summary>
                /// The default number of selector divisions.
                /// </summary>
                public const int Default = 4;
            }

            /// <summary>
            /// Provides static values that define selector count characteristics.
            /// </summary>
            public static class Count
            {
                /// <summary>
                /// The minimum selector count for the song pattern selector.
                /// </summary>
                public const int Min = 16;

                /// <summary>
                /// The maximum selector count for the song pattern selector.
                /// </summary>
                public const int Max = 96;

                /// <summary>
                /// The default selector count for the song pattern selector.
                /// </summary>
                public const int Default = 46;

            }
            /// <summary>
            /// Gets the fixed width of the first column.
            /// </summary>
            public static int FirstColumnWidth = 64;


        }
        #endregion

        /************************************************************************/

        #region DrumPattern
        /// <summary>
        /// Provides static values that define drum pattern characteristics.
        /// </summary>
        public static class DrumPattern
        {
            /// <summary>
            /// Provides static values that define selector size characteristics.
            /// </summary>
            public static class QuarterNote
            {
                /// <summary>
                /// The minimum number of quarter notes for a drum pattern.
                /// </summary>
                public const int Min = 2;

                /// <summary>
                /// The maximum number of quarter notes for a drum pattern.
                /// </summary>
                public const int Max = 8;

                /// <summary>
                /// The default number of quarter notes for a drum pattern.
                /// </summary>
                public const int Default = 4;
            }

            /// <summary>
            /// Provides static values that define quarter note tick divisions for a drum pattern
            /// </summary>
            public static class TotalTick
            {
                /// <summary>
                /// Total number of ticks (including the quarter note) when divided into quarter notes.
                /// </summary>
                public const int Quarter = 1;

                /// <summary>
                /// Total number of ticks (including the quarter note) when divided into eighth notes.
                /// </summary>
                public const int Eighth = 2;

                /// <summary>
                /// Total number of ticks (including the quarter note) when divided into eighth note triplet.
                /// </summary>
                public const int EighthTriplet = 3;

                /// <summary>
                /// Total number of ticks (including the quarter note) when divided into sixteenth notes.
                /// </summary>
                public const int Sixteenth = 4;

                /// <summary>
                /// Total number of ticks (including the quarter note) when divided into 32nd notes.
                /// </summary>
                public const int ThirtySecond = 8;

                /// <summary>
                /// The minimum value of total ticks per quarter note
                /// </summary>
                public const int Min = Eighth;

                /// <summary>
                /// The maximum value of total ticks per quarter note.
                /// </summary>
                public const int Max = ThirtySecond;

                /// <summary>
                /// The default value of total ticks per quarter note.
                /// </summary>
                public const int Default = Sixteenth;
            }

            /// <summary>
            /// Provides static values that scale values for a drum pattern.
            /// </summary>
            public static class Scale
            {
                /// <summary>
                /// The minimum scale allowed.
                /// </summary>
                public const double Min = 300.0;

                /// <summary>
                /// The maxium scale allowed.
                /// </summary>
                public const double Max = 400.0; // was 180

                /// <summary>
                /// The default scale allowed.
                /// </summary>
                public const double Default = 300.0;
            }

            /// <summary>
            /// The number of allowable drum patterns
            /// </summary>
            public const int MaxCount = 4;

            /// <summary>
            /// The lowest common denominator
            /// </summary>
            public const int LowestCommon = 24;

            /// <summary>
            /// Gets the fixed width of the first column.
            /// </summary>
            public const int FirstColumnWidth = 252;

        }
        #endregion
    }
}
