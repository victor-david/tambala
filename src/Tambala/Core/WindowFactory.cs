/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Controls;
using Restless.Tambala.View;
using Restless.Tambala.ViewModel;
using System.Windows;

namespace Restless.Tambala.Core
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
                MainWindow window = new()
                {
                    DataContext = new MainWindowViewModel()
                };

                TextOptions.SetTextFormattingMode(window);
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
            /// <param name="projectContainer">The project container associated with the window.</param>
            /// <returns>The window</returns>
            public static AudioRenderWindow Create(ProjectContainer projectContainer)
            {
                AudioRenderWindow window = new()
                {
                    Owner = Application.Current.MainWindow,
                };

                window.DataContext = new AudioRenderWindowViewModel(window, projectContainer);
                TextOptions.SetTextFormattingMode(window);
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
                AboutWindow window = new()
                {
                    Owner = Application.Current.MainWindow,
                    DataContext = new AboutWindowViewModel()
                };
                TextOptions.SetTextFormattingMode(window);
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