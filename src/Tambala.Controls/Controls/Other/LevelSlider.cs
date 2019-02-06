using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.App.Tambala.Controls
{
    /// <summary>
    /// Extends Slider to provide additional labeling properties
    /// </summary>
    public class LevelSlider : Slider
    {
        #region Private
        private const double DefaultBarSize = 140.0;
        private const double DefaultLabelValueSize = 80.0;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelSlider"/> class.
        /// </summary>
        internal LevelSlider()
        {
        }

        static LevelSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LevelSlider), new FrameworkPropertyMetadata(typeof(LevelSlider)));
        }
        #endregion

        /************************************************************************/

        #region BarWidth
        /// <summary>
        /// When horizontal, gets or sets the fixed size of the slider bar.
        /// </summary>
        public double BarWidth
        {
            get => (double)GetValue(BarWidthProperty);
            set => SetValue(BarWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BarWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BarWidthProperty = DependencyProperty.Register
            (
                nameof(BarWidth), typeof(double), typeof(LevelSlider), new PropertyMetadata(DefaultBarSize, OnBarWidthChanged, OnBarWidthCoerce)
            );

        private static void OnBarWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LevelSlider c)
            {
            }
        }

        private static object OnBarWidthCoerce(DependencyObject d, object baseValue)
        {
            double proposed = (double)baseValue;
            return Math.Min(1200.0, Math.Max(60.0, proposed));
        }
        #endregion

        /************************************************************************/

        #region LabelSize
        /// <summary>
        /// Gets or sets the space for the label. Default is 60.0
        /// </summary>
        public double LabelSize
        {
            get => (double)GetValue(LabelSizeProperty);
            set => SetValue(LabelSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LabelSize"/> dependency property
        /// </summary>
        public static readonly DependencyProperty LabelSizeProperty = DependencyProperty.Register
            (
                nameof(LabelSize), typeof(double), typeof(LevelSlider), new PropertyMetadata(DefaultLabelValueSize, OnLabelSizeChanged, OnLabelOrValueSizeCoerce)
            );


        private static void OnLabelSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LevelSlider c)
            {
                c.LabelLength = new GridLength(c.LabelSize);
            }
        }

        private static object OnLabelOrValueSizeCoerce(DependencyObject d, object baseValue)
        {
            double proposed = (double)baseValue;
            return Math.Min(150.0, Math.Max(0.0, proposed));
        }
        #endregion

        /************************************************************************/

        #region ValueSize
        /// <summary>
        /// Gets or sets the space for the vale. Default is 60.0
        /// </summary>
        public double ValueSize
        {
            get => (double)GetValue(ValueSizeProperty);
            set => SetValue(ValueSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ValueSize"/> dependency property
        /// </summary>
        public static readonly DependencyProperty ValueSizeProperty = DependencyProperty.Register
            (
                nameof(ValueSize), typeof(double), typeof(LevelSlider), new PropertyMetadata(DefaultLabelValueSize, OnValueSizeChanged, OnLabelOrValueSizeCoerce)
            );


        private static void OnValueSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LevelSlider c)
            {
                c.ValueLength = new GridLength(c.ValueSize);
            }
        }
        #endregion

        /************************************************************************/

        #region LabelGridLengths (read only)
        /// <summary>
        /// Gets the grid length for the label.
        /// </summary>
        public GridLength LabelLength
        {
            get => (GridLength)GetValue(LabelLengthProperty);
            private set => SetValue(LabelLengthPropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey LabelLengthPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(LabelLength), typeof(GridLength), typeof(LevelSlider), new PropertyMetadata(new GridLength(DefaultLabelValueSize))
            );

        /// <summary>
        /// Identifies the <see cref="LabelLength"/> dependency property
        /// </summary>
        public static readonly DependencyProperty LabelLengthProperty = LabelLengthPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the grid length for the value.
        /// </summary>
        public GridLength ValueLength
        {
            get => (GridLength)GetValue(ValueLengthProperty);
            private set => SetValue(ValueLengthPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ValueLengthPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ValueLength), typeof(GridLength), typeof(LevelSlider), new PropertyMetadata(new GridLength(DefaultLabelValueSize))
            );

        /// <summary>
        /// Identifies the <see cref="ValueLength"/> dependency property
        /// </summary>
        public static readonly DependencyProperty ValueLengthProperty = ValueLengthPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region LabelText
        /// <summary>
        /// Gets or sets the label text
        /// </summary>
        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LabelText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register
            (
                nameof(LabelText), typeof(string), typeof(LevelSlider), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the foreground brush for <see cref="LabelText"/>.
        /// </summary>
        public Brush LabelTextBrush
        {
            get => (Brush)GetValue(LabelTextBrushProperty);
            set => SetValue(LabelTextBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LabelTextBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextBrushProperty = DependencyProperty.Register
            (
                nameof(LabelTextBrush), typeof(Brush), typeof(LevelSlider), new PropertyMetadata(new SolidColorBrush(Colors.Black))
            );
        #endregion

        /************************************************************************/

        #region ValueText
        /// <summary>
        /// Gets or sets the value text
        /// </summary>
        public string ValueText
        {
            get => (string)GetValue(ValueTextProperty);
            set => SetValue(ValueTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ValueText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueTextProperty = DependencyProperty.Register
            (
                nameof(ValueText), typeof(string), typeof(LevelSlider), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the foreground brush for <see cref="ValueText"/>.
        /// </summary>
        public Brush ValueTextBrush
        {
            get => (Brush)GetValue(ValueTextBrushProperty);
            set => SetValue(ValueTextBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ValueTextBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueTextBrushProperty = DependencyProperty.Register
            (
                nameof(ValueTextBrush), typeof(Brush), typeof(LevelSlider), new PropertyMetadata(new SolidColorBrush(Colors.Black))
            );
        #endregion

        /************************************************************************/

        #region LabelHorizontalAlignment
        /// <summary>
        /// Gets or set the horizontal alignment for <see cref="LabelText"/> when the slider orientation is horizontal.
        /// </summary>
        public HorizontalAlignment LabelHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty);
            set => SetValue(LabelHorizontalAlignmentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LabelHorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty = DependencyProperty.Register
            (
                nameof(LabelHorizontalAlignment), typeof(HorizontalAlignment), typeof(LevelSlider), new PropertyMetadata(HorizontalAlignment.Right)
            );
        #endregion

        /************************************************************************/

        #region ValueHorizontalAlignment
        /// <summary>
        /// Gets or set the horizontal alignment for <see cref="ValueText"/> when the slider orientation is horizontal.
        /// </summary>
        public HorizontalAlignment ValueHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(ValueHorizontalAlignmentProperty);
            set => SetValue(ValueHorizontalAlignmentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ValueHorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueHorizontalAlignmentProperty = DependencyProperty.Register
            (
                nameof(ValueHorizontalAlignment), typeof(HorizontalAlignment), typeof(LevelSlider), new PropertyMetadata(HorizontalAlignment.Left)
            );
        #endregion
    }
}
