using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Windows;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents the base class for a sizeable pattern selector. This class must be inherited.
    /// </summary>
    public abstract class SizeablePatternSelector : ControlObjectBase
    {
        #region Private
        #endregion
        
        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SizeablePatternSelector"/> class.
        /// </summary>
        internal SizeablePatternSelector()
        {
        }

        static SizeablePatternSelector()
        {
        }
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
                nameof(SelectorSize), typeof(double), typeof(SizeablePatternSelector), new PropertyMetadata(Constants.SongSelector.Size.Default, OnSelectorSizeChanged, OnSelectorSizeCoerce)
            );

        private static object OnSelectorSizeCoerce(DependencyObject d, object baseValue)
        {
            double proposed = (double)baseValue;
            return Math.Min(Constants.SongSelector.Size.Max, Math.Max(Constants.SongSelector.Size.Min, proposed));
        }

        private static void OnSelectorSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SizeablePatternSelector c)
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
                nameof(DivisionCount), typeof(int), typeof(SizeablePatternSelector), new PropertyMetadata(Constants.SongSelector.Division.Default, OnDivisionChanged, OnDivisionCountCoerce)
            );

        private static object OnDivisionCountCoerce(DependencyObject d, object baseValue)
        {
            int proposed = (int)baseValue;
            return Math.Min(Constants.SongSelector.Division.Max, Math.Max(Constants.SongSelector.Division.Min, proposed));
        }

        private static void OnDivisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SizeablePatternSelector c)
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
        #endregion
    }
}
