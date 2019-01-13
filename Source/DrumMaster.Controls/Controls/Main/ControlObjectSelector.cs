using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Windows;
using System.Windows.Media;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents the base class for a sizeable pattern selector. This class must be inherited.
    /// </summary>
    public abstract class ControlObjectSelector : ControlObject, ISelector
    {
        #region Private
        #endregion
        
        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlObjectSelector"/> class.
        /// </summary>
        internal ControlObjectSelector(PointSelectorType type)
        {
            SelectorType = type;
        }

        static ControlObjectSelector()
        {
        }
        #endregion

        /************************************************************************/

        #region SelectorType
        /// <summary>
        /// Gets the selector type
        /// </summary>
        public PointSelectorType SelectorType
        {
            get => (PointSelectorType)GetValue(SelectorTypeProperty);
            private set => SetValue(SelectorTypePropertyKey, value);
        }

        private static readonly DependencyPropertyKey SelectorTypePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(SelectorType), typeof(PointSelectorType), typeof(ControlObjectSelector), new PropertyMetadata(PointSelectorType.None)
            );

        /// <summary>
        /// Identifies the <see cref="SelectorType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectorTypeProperty = SelectorTypePropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region SelectorSize
        /// <summary>
        /// Gets or sets the size of the selector
        /// </summary>
        public double SelectorSize
        {
            get => (double)GetValue(SelectorSizeProperty);
            set => SetValue(SelectorSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectorSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectorSizeProperty = DependencyProperty.Register
            (
                nameof(SelectorSize), typeof(double), typeof(ControlObjectSelector), new PropertyMetadata(Constants.Selector.Size.Default, OnSelectorSizeChanged, OnSelectorSizeCoerce)
            );

        private static object OnSelectorSizeCoerce(DependencyObject d, object baseValue)
        {
            double proposed = (double)baseValue;
            return Math.Min(Constants.Selector.Size.Max, Math.Max(Constants.Selector.Size.Min, proposed));
        }

        private static void OnSelectorSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ControlObjectSelector c)
            {
                c.OnSelectorSizeChanged();
            }
        }

        /// <summary>
        /// Gets the minimum allowed value for <see cref="SelectorSize"/>.
        /// Used to bind to the control template.
        /// </summary>
        public double MinSelectorSize
        {
            get => Constants.Selector.Size.Min;
        }
        
        /// <summary>
        /// Gets the maximum allowed value for <see cref="SelectorSize"/>.
        /// Used to bind to the control template.
        /// </summary>
        public double MaxSelectorSize
        {
            get => Constants.Selector.Size.Max;
        }
        #endregion

        /************************************************************************/

        #region DivisionCount
        /// <summary>
        /// Gets or sets the division count, i.e. how many
        /// selector points in each division.
        /// </summary>
        public int DivisionCount
        {
            get => (int)GetValue(DivisionCountProperty);
            set => SetValue(DivisionCountProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DivisionCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DivisionCountProperty = DependencyProperty.Register
            (
                nameof(DivisionCount), typeof(int), typeof(ControlObjectSelector), new PropertyMetadata(Constants.Selector.Division.Default, OnDivisionChanged, OnDivisionCountCoerce)
            );

        private static object OnDivisionCountCoerce(DependencyObject d, object baseValue)
        {
            int proposed = (int)baseValue;
            return Math.Min(Constants.Selector.Division.Max, Math.Max(Constants.Selector.Division.Min, proposed));
        }

        private static void OnDivisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ControlObjectSelector c)
            {
                c.OnDivisionCountChanged();
            }
        }

        /// <summary>
        /// Gets the minimum allowed value for <see cref="DivisionCount"/>.
        /// Used to bind to the control template.
        /// </summary>
        public int MinDivisionCount
        {
            get => Constants.Selector.Division.Min;
        }

        /// <summary>
        /// Gets the maximum allowed value for <see cref="DivisionCount"/>.
        /// Used to bind to the control template.
        /// </summary>
        public int MaxDivisionCount
        {
            get => Constants.Selector.Division.Max;
        }
        #endregion

        /************************************************************************/

        #region Position
        /// <summary>
        /// Gets or sets the position, i.e. the sequence value
        /// </summary>
        public int Position
        {
            get => (int)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register
            (
                nameof(Position), typeof(int), typeof(ControlObjectSelector), new PropertyMetadata(0, OnPositionChanged)
            );

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ControlObjectSelector c)
            {
                c.OnPositionChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region IsSelected
        /// <summary>
        /// Gets or sets a boolean value that indicates if the control is selected.
        /// A derived class decides how or if to use this property.
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSelected"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register
            (
                nameof(IsSelected), typeof(bool), typeof(ControlObjectSelector), new FrameworkPropertyMetadata(false, OnIsSelectedChanged)
            );

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ControlObjectSelector c)
            {
                c.OnIsSelectedChanged();
                c.OnIsSelectedBrushChanged();
            }
        }

        /// <summary>
        /// Gets or sets a brush to use when <see cref="IsSelected"/> is true.
        /// </summary>
        public Brush IsSelectedBrush
        {
            get => (Brush)GetValue(IsSelectedBrushProperty);
            set => SetValue(IsSelectedBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSelectedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedBrushProperty = DependencyProperty.Register
            (
                nameof(IsSelectedBrush), typeof(Brush), typeof(ControlObjectSelector), new PropertyMetadata(null, OnIsSelectedBrushChanged)
            );

        /// <summary>
        /// Gets or sets a brush to use when <see cref="IsSelected"/> is false.
        /// </summary>
        public Brush IsDeselectedBrush
        {
            get => (Brush)GetValue(IsDeselectedBrushProperty);
            set => SetValue(IsDeselectedBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsDeselectedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDeselectedBrushProperty = DependencyProperty.Register
            (
                nameof(IsDeselectedBrush), typeof(Brush), typeof(ControlObjectSelector), new PropertyMetadata(null, OnIsSelectedBrushChanged)
            );

        private static void OnIsSelectedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ControlObjectSelector c)
            {
                c.OnIsSelectedBrushChanged();
            }
        }

        /// <summary>
        /// Gets the active brush according to the value of <see cref="IsSelected"/>.
        /// A derived class decides how or if to use this property.
        /// </summary>
        public Brush ActiveIsSelectedBrush
        {
            get => (Brush)GetValue(ActiveIsSelectedBrushProperty);
            private set => SetValue(ActiveIsSelectedBrushPropertyKey, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsDeselectedBrush"/> dependency property.
        /// </summary>
        private static readonly DependencyPropertyKey ActiveIsSelectedBrushPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ActiveIsSelectedBrush), typeof(Brush), typeof(ControlObjectSelector), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ActiveIsSelectedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveIsSelectedBrushProperty = ActiveIsSelectedBrushPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the <see cref="SelectorSize"/> property changes.
        /// A derived class can override this method to provide further processing.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void OnSelectorSizeChanged()
        {
        }

        /// <summary>
        /// Called when the <see cref="DivisionCount"/> property changes.
        /// A derived class can override this method to provide further processing.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void OnDivisionCountChanged()
        {
        }

        /// <summary>
        /// Called when <see cref="Position"/> changes. A derived class can override
        /// this method to perform updates. The base implementation does nothing.
        /// </summary>
        protected virtual void OnPositionChanged()
        {
        }

        /// <summary>
        /// Called when <see cref="IsSelected"/> is changed. A derived class can override this method to perform updates as needed.
        /// The base implementaion does nothing.
        /// </summary>
        protected virtual void OnIsSelectedChanged()
        {
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void OnIsSelectedBrushChanged()
        {
            ActiveIsSelectedBrush = IsSelected ? IsSelectedBrush : IsDeselectedBrush;
        }
        #endregion
    }
}
