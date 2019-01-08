using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Windows;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents the base class for a pattern selector. This class must be inherited.
    /// </summary>
    public abstract class PatternSelector : SizeablePatternSelector
    {
        #region Private
        #endregion
        
        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumPatternSelector"/> class.
        /// </summary>
        /// <param name="type">The type of selector</param>
        internal PatternSelector(SongDrumPatternSelectorType type)
        {
            SelectorType = type;
        }

        static PatternSelector()
        {
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
                nameof(Position), typeof(int), typeof(PatternSelector), new PropertyMetadata(0, OnPositionChanged)
            );

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PatternSelector c)
            {
                c.OnPositionChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region SelectorType
        /// <summary>
        /// Gets or sets the selector type
        /// </summary>
        public SongDrumPatternSelectorType SelectorType
        {
            get => (SongDrumPatternSelectorType)GetValue(SelectorTypeProperty);
            private set => SetValue(SelectorTypePropertyKey, value);
        }

        private static readonly DependencyPropertyKey SelectorTypePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(SelectorType), typeof(SongDrumPatternSelectorType), typeof(PatternSelector), new PropertyMetadata(SongDrumPatternSelectorType.Standard)
            );

        /// <summary>
        /// Identifies the <see cref="SelectorType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectorTypeProperty = SelectorTypePropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when <see cref="Position"/> changes. A derived class can override
        /// this method to perform updates. The base implementation does nothing.
        /// </summary>
        protected virtual void OnPositionChanged()
        {
        }
        #endregion
    }
}
