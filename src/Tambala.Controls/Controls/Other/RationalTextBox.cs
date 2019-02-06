using System.Windows;
using System.Windows.Controls;

namespace Restless.App.Tambala.Controls
{
    /// <summary>
    /// Extends TextBox to provide extra functionality
    /// </summary>
    public class RationalTextBox : TextBox
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RationalTextBox"/> class.
        /// </summary>
        internal RationalTextBox()
        {

        }
        #endregion

        /************************************************************************/

        #region AllowZeroLength
        /// <summary>
        /// Gets or sets a value that determines if zero length text, or all while space, is allowed.
        /// The default is false.
        /// </summary>
        public bool AllowZeroLength
        {
            get => (bool)GetValue(AllowZeroLengthProperty);
            set => SetValue(AllowZeroLengthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AllowZeroLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowZeroLengthProperty = DependencyProperty.Register
            (
                nameof(AllowZeroLength), typeof(bool), typeof(RationalTextBox), new PropertyMetadata(false)
            );
        #endregion

        /************************************************************************/

        #region ZeroLengthText
        /// <summary>
        /// Gets or sets the text that will be used in place of empty text.
        /// Only applies if <see cref="AllowZeroLength"/> is false.
        /// </summary>
        public string ZeroLengthText
        {
            get => (string)GetValue(ZeroLengthTextProperty);
            set => SetValue(ZeroLengthTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ZeroLengthText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZeroLengthTextProperty = DependencyProperty.Register
            (
                nameof(ZeroLengthText), typeof(string), typeof(RationalTextBox), new PropertyMetadata("----", null, OnZeroLengthTextCoerce)
            );

        private static object OnZeroLengthTextCoerce(DependencyObject d, object baseValue)
        {
            string proposed = (string)baseValue;
            if (string.IsNullOrWhiteSpace(proposed))
            {
                return "----";
            }
            return proposed;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the control loses focus to enforce <see cref="AllowZeroLength"/>.
        /// </summary>
        /// <param name="e">The parms</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (string.IsNullOrWhiteSpace(Text) && !AllowZeroLength)
            {
                Text = ZeroLengthText;
            }
        }
        #endregion
    }
}
