using Restless.App.Tambala.Controls.Core;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Restless.App.Tambala.Controls
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
        internal ControlObjectSelector()
        {
            SelectCommand = new RelayCommand((p) => IsSelected = !IsSelected);
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
            set => SetValue(SelectorTypeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectorType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectorTypeProperty = DependencyProperty.Register
            (
                nameof(SelectorType), typeof(PointSelectorType), typeof(ControlObjectSelector), new PropertyMetadata(PointSelectorType.None)
            );
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
                nameof(SelectorSize), typeof(double), typeof(ControlObjectSelector), new PropertyMetadata(Constants.SongSelector.Size.Default, OnSelectorSizeChanged, OnSelectorSizeCoerce)
            );

        private static object OnSelectorSizeCoerce(DependencyObject d, object baseValue)
        {
            double proposed = (double)baseValue;
            return Math.Min(Constants.SongSelector.Size.Max, Math.Max(Constants.SongSelector.Size.Min, proposed));
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
            get => Constants.SongSelector.Size.Min;
        }
        
        /// <summary>
        /// Gets the maximum allowed value for <see cref="SelectorSize"/>.
        /// Used to bind to the control template.
        /// </summary>
        public double MaxSelectorSize
        {
            get => Constants.SongSelector.Size.Max;
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
                nameof(DivisionCount), typeof(int), typeof(ControlObjectSelector), new PropertyMetadata(Constants.SongSelector.Division.Default, OnDivisionChanged, OnDivisionCountCoerce)
            );

        private static object OnDivisionCountCoerce(DependencyObject d, object baseValue)
        {
            int proposed = (int)baseValue;
            return Math.Min(Constants.SongSelector.Division.Max, Math.Max(Constants.SongSelector.Division.Min, proposed));
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
            get => Constants.SongSelector.Division.Min;
        }

        /// <summary>
        /// Gets the maximum allowed value for <see cref="DivisionCount"/>.
        /// Used to bind to the control template.
        /// </summary>
        public int MaxDivisionCount
        {
            get => Constants.SongSelector.Division.Max;
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
                c.ThreadSafePosition = c.Position;
                c.OnPositionChanged();
            }
        }
        /// <summary>
        /// Gets the thread safe value of <see cref="Position"/>.
        /// </summary>
        internal int ThreadSafePosition
        {
            get;
            private set;
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
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register
            (
                nameof(IsSelected), typeof(bool), typeof(ControlObjectSelector), new FrameworkPropertyMetadata(false, OnIsSelectedChanged)
            );

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ControlObjectSelector c)
            {
                c.ThreadSafeIsSelected = c.IsSelected;
                c.OnIsSelectedChanged();
                c.OnIsSelectedBrushChanged();
                c.RaiseEvent(new RoutedEventArgs(IsSelectedChangedEvent));
            }
        }

        /// <summary>
        /// Provides notification when the <see cref="IsSelected"/> property is set to true.
        /// </summary>
        public event RoutedEventHandler IsSelectedChanged
        {
            add => AddHandler(IsSelectedChangedEvent, value);
            remove => RemoveHandler(IsSelectedChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSelectedChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent IsSelectedChangedEvent = EventManager.RegisterRoutedEvent
            (
                nameof(IsSelectedChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ControlObjectSelector)
            );

        /// <summary>
        /// Gets the thread safe value of <see cref="IsSelected"/>.
        /// </summary>
        internal bool ThreadSafeIsSelected
        {
            get;
            private set;
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

        #region SelectCommand
        /// <summary>
        /// Gets the select command. This command toggles <see cref="IsSelected"/>.
        /// </summary>
        public ICommand SelectCommand
        {
            get => (ICommand)GetValue(SelectCommandProperty);
            private set => SetValue(SelectCommandPropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey SelectCommandPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(SelectCommand), typeof(ICommand), typeof(ControlObjectSelector), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectCommandProperty = SelectCommandPropertyKey.DependencyProperty;
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
