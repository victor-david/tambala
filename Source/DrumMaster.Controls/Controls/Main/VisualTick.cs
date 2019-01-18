using Restless.App.DrumMaster.Controls.Core;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Extends Border to provide a tick mark for a <see cref="DrumPatternQuarter"/>
    /// </summary>
    internal class VisualTick : VisualBorder, ISelectorUnit
    {
        #region Private
        private readonly Brush originalBrush;
        private double originalWidth;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualTick"/> class.
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="selectorUnit">The selector unit.</param>
        internal VisualTick(int position, PointSelectorUnit selectorUnit) : base(position)
        {
            SelectorUnit = selectorUnit;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Bottom;
            Visibility = Visibility.Collapsed;

            switch (SelectorUnit)
            {
                case PointSelectorUnit.QuarterNote:
                    Visibility = Visibility.Visible;
                    Background = originalBrush = Brushes.DarkBlue;
                    Height = 14.0;
                    Width = originalWidth = 2.0;
                    break;
                case PointSelectorUnit.EighthNote:
                    Background = originalBrush = Brushes.Black;
                    Height = 12.0;
                    Width = originalWidth = 1.0;
                    break;
                default:
                    Background = originalBrush = Brushes.DarkGray;
                    Height = 10.0;
                    Width = originalWidth = 1.0;
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
        /// Calls Dispatcher.BeginInvoke to add highlight to the tick.
        /// </summary>
        public void InvokeAddTickHighlight()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback
                ((args) =>
                {
                    Background = Brushes.Red;
                    Width = Width * 3.0;
                    return null;
                }), null);
        }

        /// <summary>
        /// Calls Dispatcher.BeginInvoke to remove highlight from the tick.
        /// </summary>
        public void InvokeRemoveTickHighlight()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback
                ((args) =>
                {
                    Background = originalBrush;
                    Width = originalWidth;
                    return null;
                }), null);
        }
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
