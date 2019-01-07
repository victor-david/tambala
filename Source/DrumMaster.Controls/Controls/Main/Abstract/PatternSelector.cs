using System.Windows;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents the base class for a pattern selector. This class must be inherited.
    /// </summary>
    public abstract class PatternSelector : DependencyControlObject
    {
        #region Private
        #endregion
        
        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SongPatternSelector"/> class.
        /// </summary>
        internal PatternSelector()
        {
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
                nameof(Position), typeof(int), typeof(PatternSelector), new PropertyMetadata(0)
            );
        #endregion

        /************************************************************************/

        #region SelectorType
        /// <summary>
        /// Gets or sets the selector type
        /// </summary>
        public SongPatternSelectorType SelectorType
        {
            get => (SongPatternSelectorType)GetValue(SelectorTypeProperty);
            set => SetValue(SelectorTypeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectorType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectorTypeProperty = DependencyProperty.Register
            (
                nameof(SelectorType), typeof(SongPatternSelectorType), typeof(PatternSelector), new PropertyMetadata(SongPatternSelectorType.Standard)
            );
        #endregion
    }
}
