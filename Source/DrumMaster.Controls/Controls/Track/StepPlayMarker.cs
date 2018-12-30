using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.App.DrumMaster.Controls
{
    public class StepPlayMarker : Control
    {
        #region Properties
        /// <summary>
        /// Gets or set the inner brush.
        /// </summary>
        public Brush InnerBrush
        {
            get => (Brush)GetValue(InnerBrushProperty);
            set => SetValue(InnerBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="InnerBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerBrushProperty = DependencyProperty.Register
            (
                nameof(InnerBrush), typeof(Brush), typeof(StepPlayMarker), new PropertyMetadata(new SolidColorBrush(Colors.DarkGray))
            );

        /// <summary>
        /// Gets or sets the corner radius for the marker.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register
            (
                nameof(CornerRadius), typeof(CornerRadius), typeof(StepPlayMarker), new PropertyMetadata(new CornerRadius(0))
            );
        #endregion

        /************************************************************************/

        #region Constructors (Public / Static)
        /// <summary>
        /// Creates a new instance of <see cref="StepPlayMarker"/>
        /// </summary>
        public StepPlayMarker()
        {
        }

        static StepPlayMarker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StepPlayMarker), new FrameworkPropertyMetadata(typeof(StepPlayMarker)));
        }
        #endregion
    }
}
