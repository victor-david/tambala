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
        /// Provides static values that define selector characteristics.
        /// </summary>
        public static class SongSelector
        {
            /// <summary>
            /// Provides static values that define selector size characteristics.
            /// </summary
            public static class Size
            {
                /// <summary>
                /// The minimum selector size for the song pattern selector
                /// </summary>
                public const double Min = 20.0;

                /// <summary>
                /// The maximum selector size for the song pattern selector
                /// </summary>
                public const double Max = 30.0;

                /// <summary>
                /// The default selector size for the song pattern selector
                /// </summary>
                public const double Default = 23.0;
            }

            /// <summary>
            /// Provides static values that define selector count characteristics.
            /// </summary
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
                public const int Default = 48;

            }

            /// <summary>
            /// Provides static values that define selector division characteristics.
            /// </summary
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
        }
        #endregion

        /************************************************************************/
    }
}
