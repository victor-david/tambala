using Restless.App.DrumMaster.Controls;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Restless.App.DrumMaster.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AboutWindow : ApplicationWindow
    {
        #region Private
        private readonly ImageSource StandardIcon;
        private readonly ImageSource MinimizedIcon;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
            // Standard icon. Blue.
            StandardIcon = BitmapFrame.Create(new Uri($"pack://application:,,,/DrumMaster;component/Resources/Images/Icon.Drum.Blue.128.ico"));
            // Minimized icon. Red.
            MinimizedIcon = BitmapFrame.Create(new Uri($"pack://application:,,,/DrumMaster;component/Resources/Images/Icon.Drum.Red.128.ico"));
            Icon = StandardIcon;
            ResizeMode = ResizeMode.NoResize;
            Height = 340;
            Width = 520;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion
    }
}
