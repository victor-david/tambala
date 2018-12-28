using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a custom check box control.
    /// </summary>
    public class BoxCheckBox : CheckBox
    {
        #region Public properties
        /// <summary>
        /// Gets or sets the box size
        /// </summary>
        public double BoxSize
        {
            get => (double)GetValue(BoxSizeProperty);
            set => SetValue(BoxSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BoxSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BoxSizeProperty = DependencyProperty.Register
            (
                nameof(BoxSize), typeof(double), typeof(BoxCheckBox), new PropertyMetadata(24.0, null, OnBoxSizeCoerce)
            );

        private static object OnBoxSizeCoerce(DependencyObject d, object baseValue)
        {
            double proposed = (double)baseValue;
            return Math.Min(32, Math.Max(16, proposed));
        }

        /// <summary>
        /// Gets or sets the brush used when the control is checked.
        /// </summary>
        public Brush CheckedBrush
        {
            get => (Brush)GetValue(CheckedBrushProperty);
            set => SetValue(CheckedBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CheckedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CheckedBrushProperty = DependencyProperty.Register
            (
                nameof(CheckedBrush), typeof(Brush), typeof(BoxCheckBox), new PropertyMetadata(new SolidColorBrush(Colors.DarkGray))
            );
        #endregion

        /************************************************************************/

        #region Constructors (Internal / Static)
        /// <summary>
        /// Creates a new instance of the <see cref="BoxCheckBox"/> class
        /// </summary>
        public BoxCheckBox()
        {
            BorderThickness = new Thickness(2.0);
            BorderBrush = new SolidColorBrush(Colors.DarkBlue);
        }

        static BoxCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BoxCheckBox), new FrameworkPropertyMetadata(typeof(BoxCheckBox)));
        }
        #endregion

        /************************************************************************/





    }
}
