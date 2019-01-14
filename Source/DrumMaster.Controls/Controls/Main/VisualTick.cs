using Restless.App.DrumMaster.Controls.Core;
using System.Windows;
using System.Windows.Media;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Extends Border to provide a tick mark for a <see cref="DrumPatternQuarter"/>
    /// </summary>
    internal class VisualTick : VisualBorder, ISelectorUnit
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualTick"/> class.
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="selectorUnit">The selector unit.</param>
        internal VisualTick(int position, PointSelectorUnit selectorUnit) : base(position)
        {
            // TotalTick = totalTick;
            SelectorUnit = selectorUnit;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Bottom;
            Visibility = Visibility.Collapsed;

            switch (SelectorUnit)
            {
                case PointSelectorUnit.QuarterNote:
                    Visibility = Visibility.Visible;
                    Background = Brushes.DarkBlue;
                    Height = 14.0;
                    Width = 2.0;
                    break;
                case PointSelectorUnit.EighthNote:
                    Background = Brushes.Black;
                    Height = 12.0;
                    Width = 1.0;
                    break;
                default:
                    Background = Brushes.DarkGray;
                    Height = 10.0;
                    Width = 1.0;
                    break;
            }
        }
        #endregion

        /************************************************************************/

        #region ISelectorUnit
        /// <summary>
        /// Gets or sets the selector unit.
        /// </summary>
        public PointSelectorUnit SelectorUnit
        {
            get;
            set;
        }
        #endregion


        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(VisualTick)} Unit:{SelectorUnit} Position:{Position}";
        }
        #endregion
    }
}
