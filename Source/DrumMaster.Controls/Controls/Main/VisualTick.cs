using System.Windows;
using System.Windows.Media;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Extends Border to provide a tick mark for a <see cref="DrumPatternPresenter"/>
    /// </summary>
    internal class VisualTick : VisualBorder
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualTick"/> class.
        /// </summary>
        /// <param name="position">The position</param>
        internal VisualTick(int position, bool isQuarter) : base(position)
        {
            Background = isQuarter ? new SolidColorBrush(Colors.DarkBlue) : new SolidColorBrush(Colors.DarkGray);
            Height = isQuarter ? 14 : 10;
            Width = isQuarter ? 2 : 1;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Bottom;
        }
        #endregion
    }
}
