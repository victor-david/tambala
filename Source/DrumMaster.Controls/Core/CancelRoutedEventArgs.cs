using System.Windows;

namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Represents the arguments used for a routed event that can ve canceled.
    /// </summary>
    public class CancelRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets or sets a value that indicates if the event should be canceled.
        /// </summary>
        public bool Cancel
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelRoutedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event</param>
        internal CancelRoutedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {

        }
    }

    /// <summary>
    /// Delegate for <see cref="CancelRoutedEventArgs"/>
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="args">The args</param>
    public delegate void CancelRoutedEventHandler(object sender, CancelRoutedEventArgs e);
}
