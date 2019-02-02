using System.Windows;
using System.Windows.Media;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Extends Border to provide a divider for a <see cref="SongPresenter"/>
    /// </summary>
    internal class VisualDivider : VisualBorder
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualDivider"/> class.
        /// </summary>
        /// <param name="position">The position</param>
        internal VisualDivider(int position) : base(position)
        {
            Width = 3.0;
            Margin = new Thickness(1);
            Background = new SolidColorBrush(Colors.DarkGray);
            VerticalAlignment = VerticalAlignment.Stretch;
        }
        #endregion
    }
}
