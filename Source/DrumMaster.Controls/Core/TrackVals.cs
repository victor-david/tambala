using System;
using System.Windows;

namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Provides static values that apply to a track layout and its tracks.
    /// </summary>
    public static class TrackVals
    {
        #region Track
        /// <summary>
        /// Provides static values that define track characteristics.
        /// </summary>
        public static class Track
        {
            /// <summary>
            /// The minimum number of tracks allowed on a <see cref="TrackContainer"/>.
            /// </summary>
            public const int Min = 0;

            /// <summary>
            /// The maximum number of tracks allowed on a <see cref="TrackContainer"/>.
            /// </summary>
            public const int Max = 12;

            /// <summary>
            /// The default number of tracks placed automatically on a new <see cref="TrackContainer"/>.
            /// </summary>
            public const int Default = 3;
        }
        #endregion

        /************************************************************************/

        #region Box size
        /// <summary>
        /// Provides static values that define box size characteristics.
        /// </summary>
        public static class BoxSize
        {
            /// <summary>
            /// The minimum box size for the track header and the track's beats / steps.
            /// </summary>
            public const double Min = 36;

            /// <summary>
            /// The maximum box size for the track header and the track's beats / steps.
            /// </summary>
            public const double Max = 72;

            /// <summary>
            /// The default box size for the track header and the track's beats / steps.
            /// </summary>
            public const double Default = 48;

            /// <summary>
            /// The default text that describes the box size
            /// </summary>
            public const string DefaultText = "Size";
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

        #region Measures
        /// <summary>
        /// Provides static values that define characteristics of measures.
        /// </summary>
        public static class Measures
        {
            /// <summary>
            /// The minimum number of measures allowed.
            /// </summary>
            public const int Min = 1;

            /// <summary>
            /// The maximum number of measures allowed.
            /// </summary>
            public const int Max = 4;

            /// <summary>
            /// The default number of measures
            /// </summary>
            public const int Default = 1;

            /// <summary>
            /// The default measures text.
            /// </summary>
            public const string DefaultText = "Measures";
        }
        #endregion

        /************************************************************************/

        #region Beats
        /// <summary>
        /// Provides static values that define beats characteristics.
        /// </summary>
        public static class Beats
        {
            /// <summary>
            /// The minimum number of beats allowed.
            /// </summary>
            public const int Min = 3;

            /// <summary>
            /// The maximum number of beats allowed.
            /// </summary>
            public const int Max = 8;

            /// <summary>
            /// The default number of beats
            /// </summary>
            public const int Default = 4;

            /// <summary>
            /// The default beats text.
            /// </summary>
            public const string DefaultText = "Beats";
        }
        #endregion

        /************************************************************************/

        #region Steps per beat
        /// <summary>
        /// Provides static values that define steps per beats characteristics.
        /// </summary>
        public static class StepsPerBeat
        {
            /// <summary>
            /// The minimum number of steps per beat.
            /// </summary>
            public const int Min = 1;

            /// <summary>
            /// The maximum number of steps per beat.
            /// </summary>
            public const int Max = 6;

            /// <summary>
            /// The default number of steps per beat.
            /// </summary>
            public const int Default = 4;

            /// <summary>
            /// The default steps per beat text.
            /// </summary>
            public const string DefaultText = "Steps per Beat";

        }
        #endregion

        /************************************************************************/

        #region TotalSteps
        /// <summary>
        /// Provides static values that define total steps characteristics.
        /// </summary>
        public static class TotalSteps
        {
            /// <summary>
            /// The minimum number of total steps.
            /// </summary>
            public const int Min = Beats.Min * StepsPerBeat.Min;

            /// <summary>
            /// The maximum number of total steps.
            /// </summary>
            public const int Max = Beats.Max * StepsPerBeat.Max;

            /// <summary>
            /// The default number of total steps.
            /// </summary>
            public const int Default = Beats.Default * StepsPerBeat.Default;

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
            /// The minimum volume allowed in decibels.
            /// </summary>
            public const float Min = -64.0f; 

            /// <summary>
            /// The maximum volume allowed in decibels.
            /// </summary>
            public const float Max = 18.0f;

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
            public const string DefaultShortText = "Vol:";
        }
        #endregion

        /************************************************************************/

        #region Volume Bias
        /// <summary>
        /// Provides static values that define volume bias.
        /// See remarks for how these values are used.
        /// </summary>
        /// <remarks>
        /// Volume bias is used on individual beats to provide accents
        /// and ghost notes. The user can set the volume bias of each beat
        /// and the volume will be adjusted accordingly during playback.
        /// Volume bias is expressed as dB.
        /// </remarks>
        public static class VolumeBias
        {
            /// <summary>
            /// The minimum volume bias allowed in decibels.
            /// </summary>
            public const float Min = -9.5f;

            /// <summary>
            /// The maximum volume bias allowed in decibels.
            /// </summary>
            public const float Max = 9.5f;

            /// <summary>
            /// The default volume bias, i.e. none.
            /// </summary>
            public const float Default = 0.0f;
        }
        #endregion

        /************************************************************************/

        #region Human Volume Bias
        /// <summary>
        /// Provides static values that define human volume bias. 
        /// See remarks for how these values are used.
        /// </summary>
        /// <remarks>
        /// If enabled (i.e. not 0.0, off), human volume bias is applied randomly
        /// to individual beats during playback. The human bias calculation
        /// gets a value between -humanBias (softer hit) and +humanBias (harder hit).
        /// Human bias is expressed as dB.
        /// </remarks>
        public static class HumanVolumeBias
        {
            /// <summary>
            /// The minimum human volume bias allowed in decibels.
            /// </summary>
            public const float Min = 0.0f;

            /// <summary>
            /// The maximum human volume bias allowed in decibels.
            /// </summary>
            public const float Max = 7.5f;

            /// <summary>
            /// The default human volume bias, i.e. off.
            /// </summary>
            public const float Default = 0.0f;
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
    }
}
