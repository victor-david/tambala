using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Restless.App.DrumMaster.Controls.Core
{
    public class CancelRoutedEventArgs : RoutedEventArgs
    {
        public bool Cancel
        {
            get;
            set;
        }

        internal CancelRoutedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {

        }
    }

    public delegate void CancelRoutedEventHandler(object sender, CancelRoutedEventArgs args);
}
