using Restless.App.DrumMaster.Controls;
using Restless.App.DrumMaster.View;
using Restless.App.DrumMaster.ViewModel;
using System.Windows;

namespace Restless.App.DrumMaster.Core
{
    /// <summary>
    /// Provides static methods for creating application windows.
    /// </summary>
    public static class WindowFactory
    {
        #region Main
        /// <summary>
        /// Provides static methods for creating the main application window.
        /// </summary>
        public static class Main
        {
            /// <summary>
            /// Creates an instance of MainWindow and its corresponding view model.
            /// </summary>
            /// <returns>The window</returns>
            public static MainWindow Create()
            {
                var window = new MainWindow();
                TextOptions.SetTextFormattingMode(window);
                var viewModel = new MainWindowViewModel(window);
                window.DataContext = viewModel;
                return window;
            }
        }

        /// <summary>
        /// Provides static methods for creating the audio render window.
        /// </summary>
        public static class AudioRender
        {
            /// <summary>
            /// Creates an instance of AudioRenderWindow and its corresponding view model.
            /// </summary>
            /// <returns>The window</returns>
            public static AudioRenderWindow Create(ProjectContainer projectContainer)
            {
                var window = new AudioRenderWindow
                {
                    Owner = Application.Current.MainWindow
                };
                TextOptions.SetTextFormattingMode(window);
                var viewModel = new AudioRenderWindowViewModel(window, projectContainer);
                window.DataContext = viewModel;
                return window;
            }
        }

        /// <summary>
        /// Provides static methods for creating the about window.
        /// </summary>
        public static class About
        {
            /// <summary>
            /// Creates an instance of AboutWindow and its corresponding view model.
            /// </summary>
            /// <returns>The window</returns>
            public static AboutWindow Create()
            {
                var window = new AboutWindow
                {
                    Owner = Application.Current.MainWindow
                };
                TextOptions.SetTextFormattingMode(window);
                var viewModel = new AboutWindowViewModel(window);
                window.DataContext = viewModel;
                return window;
            }

        }
        #endregion

        /************************************************************************/

        #region TextOptions (private)
        private static class TextOptions
        {
            public static void SetTextFormattingMode(DependencyObject element)
            {
                System.Windows.Media.TextOptions.SetTextFormattingMode(element, System.Windows.Media.TextFormattingMode.Display);
            }
        }
        #endregion
    }
}
