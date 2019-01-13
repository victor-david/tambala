using Restless.App.DrumMaster.Controls.Core;
using System.Windows;
using System.Windows.Controls;

namespace Restless.App.DrumMaster.Controls
{
    public class VisualQuarterNote : Control
    {
        #region Constructors
        internal VisualQuarterNote(int quarterNote)
        {
            QuarterNote = quarterNote;
        }

        static VisualQuarterNote()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VisualQuarterNote), new FrameworkPropertyMetadata(typeof(VisualQuarterNote)));
        }
        #endregion

        /************************************************************************/

        #region QuarterNote
        /// <summary>
        /// Gets the quarter note value, 1,2,3,4,5, etc.
        /// </summary>
        public int QuarterNote
        {
            get => (int)GetValue(QuarterNoteProperty);
            private set => SetValue(QuarterNotePropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey QuarterNotePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(QuarterNote), typeof(int), typeof(VisualQuarterNote), new PropertyMetadata(0)
            );

        /// <summary>
        /// Identifies the <see cref="QuarterNote"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty QuarterNoteProperty = QuarterNotePropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region TotalTicks
        /// <summary>
        /// Gets or sets the total number of ticks for this quarter note, including the quarter note itself.
        /// </summary>
        public int TotalTicks
        {
            get { return (int)GetValue(TotalTicksProperty); }
            set { SetValue(TotalTicksProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="TotalTicks"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TotalTicksProperty = DependencyProperty.Register
            (
                nameof(TotalTicks), typeof(int), typeof(VisualQuarterNote), new PropertyMetadata(Constants.DrumPattern.TotalTick.Default)
            );
        #endregion

    }
}
