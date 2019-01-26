using System;
using System.Windows;

namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Provides static constant values that are used throughout the control library.
    /// </summary>
    public static class Constants
    {
        #region InitialVoicePool
        /// <summary>
        /// Provides static values for a track's initial voice pool size
        /// </summary>
        public static class InitialVoicePool
        {
            /// <summary>
            /// Normal voice pool initial size.
            /// </summary>
            public const int Normal = 16;
            /// <summary>
            /// Medium voice pool initial size.
            /// </summary>
            public const int Medium = 32;
            /// <summary>
            /// High voice pool initial size.
            /// </summary>
            public const int High = 48;
        }
        #endregion

        /************************************************************************/

        #region SongSelector
        /// <summary>
        /// Provides static values that define characteristics of the song presenter
        /// </summary>
        public static class SongSelector
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
            /// Gets the minimum width of the first column.
            /// </summary>
            public const int MinFirstColumnWidth = 64;

            /// <summary>
            /// Gets the maximum width of the first column.
            /// </summary>
            public const int MaxFirstColumnWidth = 138;


        }
        #endregion

        /************************************************************************/

        #region Tempo
        /// <summary>
        /// Provides static values that define tempo characteristics.
        /// </summary>
        public static class Tempo
        {
            /// <summary>
            /// The minimum tempo allowed.
            /// </summary>
            public const double Min = 45;

            /// <summary>
            /// The maximum tempo allowed.
            /// </summary>
            public const double Max = 240;

            /// <summary>
            /// The default tempo.
            /// </summary>
            public const double Default = 120;

            /// <summary>
            /// The default tempo text.
            /// </summary>
            public const string DefaultText = "Tempo";
        }
        #endregion

        /************************************************************************/

        #region Volume
        /// <summary>
        /// Provides static values that define volume characteristics.
        /// </summary>
        public static class Volume
        {
            /// <summary>
            /// Provides static volume values for the master and drum pattern
            /// submix voices. Values are expressed in decibels.
            /// </summary>
            public static class Main
            {
                /// <summary>
                /// The minimum volume allowed in decibels.
                /// </summary>
                public const float Min = -48.0f; // was -64

                /// <summary>
                /// The maximum volume allowed in decibels.
                /// </summary>
                public const float Max = 18.0f;
            }

            /// <summary>
            /// Provides static volume values for individual point selectors.
            /// Values are expressed in decibels.
            /// </summary>
            /// <remarks>
            /// Selector volume is used on individual beat selectors to provide accents
            /// and ghost notes. It has a smaller range than main volume.
            /// </remarks>
            public static class Selector
            {
                /// <summary>
                /// The minimum volume allowed in decibels.
                /// </summary>
                public const float Min = -9.5f;

                /// <summary>
                /// The maximum volume allowed in decibels.
                /// </summary>
                public const float Max = 9.5f;

            }

            /// <summary>
            /// The default volume
            /// </summary>
            public const float Default = 0.0f;

            /// <summary>
            /// The default volume text.
            /// </summary>
            public const string DefaultText = "Volume";

            /// <summary>
            /// The default short volume text.
            /// </summary>
            public const string DefaultShortText = "Vol";
        }
        #endregion

        /************************************************************************/

        #region Pitch
        /// <summary>
        /// Provides static values that define pitch characteristics.
        /// </summary>
        /// <remarks>
        /// Pitches are expressed as input rate/output rate ratios between 1/1,024 and 1,024/1, inclusive.
        /// A ratio of 1/1,024 lowers pitch by 10 octaves, while a ratio of 1,024/1 raises it by 10 octaves. 
        /// This class provides values expressed in semitones
        /// </remarks>
        public static class Pitch
        {
            /// <summary>
            /// The minimum value for pitch change in semitones. Corresponds to 3 octaves down.
            /// </summary>
            public const float Min = -36.0f;

            /// <summary>
            /// The maximum value for pitch change in semitones. Corresponds to 3 octave up.
            /// </summary>
            public const float Max = 36.0f;

            /// <summary>
            /// The default pitch. Corresponds to no pitch change.
            /// </summary>
            public const float Default = 0.0f;
        }
        #endregion

        /************************************************************************/

        #region Panning
        /// <summary>
        /// Provides static values that define panning characteristics.
        /// </summary>
        /// <remarks>
        /// Panning is described as a value between zero and 1, zero being full left,
        /// 0.5 being center and and 1.0 being full right.
        /// </remarks>
        public static class Panning
        {
            /// <summary>
            /// The mimimum panning value. represents all the way left.
            /// </summary>
            public const float Min = 0.0f;

            /// <summary>
            /// The maximum panning value. represents all the way right.
            /// </summary>
            public const float Max = 1.0f;

            /// <summary>
            /// The default panning value. represents all the way right.
            /// </summary>
            public const float Default = 0.5f;
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
            public static class QuarterNoteCount
            {
                /// <summary>
                /// The minimum number of quarter notes for a drum pattern.
                /// </summary>
                public const int Min = 2;

                /// <summary>
                /// The maximum number of quarter notes for a drum pattern.
                /// </summary>
                public const int Max = 6;

                /// <summary>
                /// The default number of quarter notes for a drum pattern.
                /// </summary>
                public const int Default = 4;
            }

            /// <summary>
            /// Provides static values that define quarter note tick divisions for a drum pattern
            /// </summary>
            public static class TicksPerQuarterNote
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
                public const double Max = 400.0;

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
            /// Gets the fixed width of the first column.
            /// </summary>
            public const int FirstColumnWidth = 252;
        }
        #endregion
    }
}
