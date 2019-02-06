using System.Windows;
using System.Windows.Media;

namespace Restless.App.Tambala.Controls
{
    /// <summary>
    /// Extends Border to provide a selector <see cref="SongPresenter"/>
    /// </summary>
    internal class VisualSelector : VisualBorder
    {
        #region Private
        private bool isSelected;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualSelector"/> class.
        /// </summary>
        /// <param name="position">The position</param>
        internal VisualSelector(int position) : base(position)
        {
            Padding = new Thickness(3, 0, 3, 0);
            Margin = new Thickness(2);
            CornerRadius = new CornerRadius(1);
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets a boolean vlaue that indicates whether this item is selected.
        /// When setting to true, <see cref="IsSelectedBrush"/> is applied.
        /// </summary>
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                Background = isSelected ? IsSelectedBrush : null;
            }
        }

        /// <summary>
        /// Gets or sets the brush to be applied when <see cref="IsSelected"/> is true;
        /// </summary>
        public Brush IsSelectedBrush
        {
            get; set;
        }
        #endregion
    }
}
