using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Restless.App.Tambala.Controls
{
    /// <summary>
    /// Extends ToggleButton to provide additional functionality.
    /// </summary>
    public class OnOff : ToggleButton
    {
        #region Private
        private const double DefaultSelectorSize = 20.0;
        private const double BarHeightPercentage = 0.65;
        private const double BarWidthPercentage = 2.15;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OnOff"/> class.
        /// </summary>
        internal OnOff()
        {
        }

        static OnOff()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OnOff), new FrameworkPropertyMetadata(typeof(OnOff)));
        }
        #endregion

        /************************************************************************/

        #region Id
        /// <summary>
        /// Gets or sets an id for this instance.
        /// </summary>
        public string Id
        {
            get => (string)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Id"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register
            (
                nameof(Id), typeof(string), typeof(OnOff), new PropertyMetadata(null)
            );
        #endregion

        /************************************************************************/

        #region IsCheckBox
        /// <summary>
        /// Gets or sets a value that indicates if this is a check box.
        /// When true, uses another control template. The default is false.
        /// </summary>
        public bool IsCheckBox
        {
            get => (bool)GetValue(IsCheckBoxProperty);
            set => SetValue(IsCheckBoxProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsCheckBox"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsCheckBoxProperty = DependencyProperty.Register
            (
                nameof(IsCheckBox), typeof(bool), typeof(OnOff), new PropertyMetadata(false)
            );
        #endregion

        /************************************************************************/

        #region SelectorSize
        /// <summary>
        /// Gets or sets the selector size. The default is 20.0.
        /// </summary>
        public double SelectorSize
        {
            get =>(double)GetValue(SelectorSizeProperty);
            set => SetValue(SelectorSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectorSize"/> dependency property
        /// </summary>
        public static readonly DependencyProperty SelectorSizeProperty = DependencyProperty.Register
            (
                nameof(SelectorSize), typeof(double), typeof(OnOff), new PropertyMetadata(DefaultSelectorSize, OnSelectorSizeChanged, OnSelectorSizeCoerce)
            );

        private static void OnSelectorSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OnOff c)
            {
                c.CornerRadius = c.SelectorSize / 2;
                c.BarHeight = c.SelectorSize * BarHeightPercentage;
                c.BarWidth = c.SelectorSize * BarWidthPercentage;
            }
        }

        private static object OnSelectorSizeCoerce(DependencyObject d, object baseValue)
        {
            double proposed = Math.Abs(Math.Round((double)baseValue));
            proposed = Math.Min(48, Math.Max(16, proposed));
            if (proposed % 2 == 1)
            {
                proposed++;
            }
            return proposed;
        }

        /// <summary>
        /// Gets the corner radius associated with <see cref="SelectorSize"/>.
        /// </summary>
        public double CornerRadius
        {
            get => (double)GetValue(CornerRadiusProperty);
            private set => SetValue(CornerRadiusPropertyKey, value);
        }

        private static readonly DependencyPropertyKey CornerRadiusPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(CornerRadius), typeof(double), typeof(OnOff), new PropertyMetadata(DefaultSelectorSize / 2)
            );

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = CornerRadiusPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region BarHeight / BarWidth
        /// <summary>
        /// Gets the bar height associated with <see cref="SelectorSize"/>.
        /// </summary>
        public double BarHeight
        {
            get => (double)GetValue(BarHeightProperty);
            private set => SetValue(BarHeightPropertyKey, value);
        }

        private static readonly DependencyPropertyKey BarHeightPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(BarHeight), typeof(double), typeof(OnOff), new PropertyMetadata(DefaultSelectorSize * BarHeightPercentage)
            );

        /// <summary>
        /// Identifies the <see cref="BarHeight"/> dependency property
        /// </summary>
        public static readonly DependencyProperty BarHeightProperty = BarHeightPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the bar width associated with <see cref="SelectorSize"/>.
        /// </summary>
        public double BarWidth
        {
            get => (double)GetValue(BarWidthProperty);
            private set => SetValue(BarWidthPropertyKey, value);
        }

        private static readonly DependencyPropertyKey BarWidthPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(BarWidth), typeof(double), typeof(OnOff), new PropertyMetadata(DefaultSelectorSize * BarWidthPercentage)
            );

        /// <summary>
        /// Identifies the <see cref="BarWidth"/> dependency property
        /// </summary>
        public static readonly DependencyProperty BarWidthProperty = BarWidthPropertyKey.DependencyProperty;


        #endregion

        /************************************************************************/

        #region OnValue / Text
        /// <summary>
        /// Gets or sets the value to use for <see cref="ActiveValue"/> when IsChecked is true.
        /// The default is boolean true.
        /// </summary>
        public object OnValue
        {
            get => GetValue(OnValueProperty);
            set => SetValue(OnValueProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="OnValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OnValueProperty = DependencyProperty.Register
            (
                nameof(OnValue), typeof(object), typeof(OnOff), new PropertyMetadata(true)
            );

        /// <summary>
        /// Gets or sets the text to use for the on side of the control
        /// </summary>
        public string OnText
        {
            get => (string)GetValue(OnTextProperty);
            set => SetValue(OnTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="OnText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OnTextProperty = DependencyProperty.Register
            (
                nameof(OnText), typeof(string), typeof(OnOff), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the foreground brush for <see cref="OnText"/>.
        /// </summary>
        public Brush OnTextBrush
        {
            get => (Brush)GetValue(OnTextBrushProperty);
            set => SetValue(OnTextBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="OnTextBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OnTextBrushProperty = DependencyProperty.Register
            (
                nameof(OnTextBrush), typeof(Brush), typeof(OnOff), new PropertyMetadata(new SolidColorBrush(Colors.Black))
            );
        #endregion

        /************************************************************************/

        #region OffValue / Text
        /// <summary>
        /// Gets or sets the value to use for <see cref="ActiveValue"/> when IsChecked is false.
        /// The default is boolean false.
        /// </summary>
        public object OffValue
        {
            get => GetValue(OffValueProperty);
            set => SetValue(OffValueProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="OffValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffValueProperty = DependencyProperty.Register
            (
                nameof(OffValue), typeof(object), typeof(OnOff), new PropertyMetadata(false)
            );

        /// <summary>
        /// Gets or sets the text to use for the off side of the control
        /// </summary>
        public string OffText
        {
            get => (string)GetValue(OffTextProperty);
            set => SetValue(OffTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="OffText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffTextProperty = DependencyProperty.Register
            (
                nameof(OffText), typeof(string), typeof(OnOff), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the foreground brush for <see cref="OffText"/>.
        /// </summary>
        public Brush OffTextBrush
        {
            get => (Brush)GetValue(OffTextBrushProperty);
            set => SetValue(OffTextBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="OnTextBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffTextBrushProperty = DependencyProperty.Register
            (
                nameof(OffTextBrush), typeof(Brush), typeof(OnOff), new PropertyMetadata(new SolidColorBrush(Colors.Black))
            );
        #endregion

        /************************************************************************/

        #region IndeterminateValue
        /// <summary>
        /// Gets or sets the value to use for <see cref="ActiveValue"/> when IsChecked is Indeterminate
        /// The default is null.
        /// </summary>
        public object IndeterminateValue
        {
            get => GetValue(IndeterminateValueProperty);
            set => SetValue(IndeterminateValueProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IndeterminateValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IndeterminateValueProperty = DependencyProperty.Register
            (
                nameof(IndeterminateValue), typeof(object), typeof(OnOff), new PropertyMetadata(null)
            );
        #endregion

        /************************************************************************/

        #region ActiveValue
        /// <summary>
        /// Gets the active value.
        /// </summary>
        public object ActiveValue
        {
            get => GetValue(ActiveValueProperty);
            private set => SetValue(ActiveValuePropertyKey, value);
        }

        private static readonly DependencyPropertyKey ActiveValuePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ActiveValue), typeof(object), typeof(OnOff), new PropertyMetadata(false, OnActiveValueChanged)
            );

        /// <summary>
        /// Identifies the <see cref="OffValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveValueProperty = ActiveValuePropertyKey.DependencyProperty;

        private static void OnActiveValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OnOff c)
            {
                c.RaiseEvent(new RoutedEventArgs(ActiveValueChangedEvent));
            }
        }
        #endregion

        /************************************************************************/

        #region ActiveValueChanged
        /// <summary>
        /// Represents a routed event that is raised when the <see cref="ActiveValue"/> changes.
        /// </summary>
        public event RoutedEventHandler ActiveValueChanged
        {
            add => AddHandler(ActiveValueChangedEvent, value);
            remove => RemoveHandler(ActiveValueChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="ActiveValueChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ActiveValueChangedEvent = EventManager.RegisterRoutedEvent
            (
                nameof(ActiveValueChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(OnOff)
            );
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when a ToggleButton raises a ToggleButton.Checked event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);
            ActiveValue = OnValue;
            e.Handled = true;
        }

        /// <summary>
        /// Called when a ToggleButton raises a ToggleButton.Unchecked event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnUnchecked(RoutedEventArgs e)
        {
            base.OnUnchecked(e);
            ActiveValue = OffValue;
            e.Handled = true;
        }

        /// <summary>
        /// Called when a ToggleButton raises a ToggleButton.Indeterminate event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnIndeterminate(RoutedEventArgs e)
        {
            base.OnIndeterminate(e);
            ActiveValue = IndeterminateValue;
            e.Handled = true;
        }
        #endregion
    }
}
