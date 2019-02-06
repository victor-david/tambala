using Restless.App.Tambala.Controls;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Restless.App.Tambala.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ApplicationWindow
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
        public MainWindow()
        {
            InitializeComponent();
            // Standard icon. Blue.
            StandardIcon = BitmapFrame.Create(new Uri($"pack://application:,,,/Tambala;component/Resources/Images/Icon.Drum.Blue.128.ico"));
            // Minimized icon. Red.
            MinimizedIcon = BitmapFrame.Create(new Uri($"pack://application:,,,/Tambala;component/Resources/Images/Icon.Drum.Red.128.ico"));
            Icon = StandardIcon;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the window state changes.
        /// </summary>
        /// <param name="e">The args.</param>
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState == WindowState.Minimized)
                Icon = MinimizedIcon;
            else
                Icon = StandardIcon;
        }
        #endregion
    }
}
