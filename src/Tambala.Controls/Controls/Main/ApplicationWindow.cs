/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the MIT License
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Restless.App.Tambala.Controls
{
    /// <summary>
    /// Represents a custom application window.
    /// </summary>
    public class ApplicationWindow : Window
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationWindow"/> class.
        /// </summary>
        public ApplicationWindow()
        {
            Background = Brushes.White;
            IconImageSource = new BitmapImage(new Uri("/Tambala.Controls;component/Resources/Images/Image.Drum.White.32.png", UriKind.Relative));
            MinimizeImageSource = new BitmapImage(new Uri("/Tambala.Controls;component/Resources/Images/Image.Window.Minimize.32.png", UriKind.Relative));
            MaximizeImageSource = new BitmapImage(new Uri("/Tambala.Controls;component/Resources/Images/Image.Window.Maximize.32.png", UriKind.Relative));
            RestoreImageSource = new BitmapImage(new Uri("/Tambala.Controls;component/Resources/Images/Image.Window.Restore.32.png", UriKind.Relative));
            CloseImageSource = new BitmapImage(new Uri("/Tambala.Controls;component/Resources/Images/Image.Window.Close.32.png", UriKind.Relative));

            MinimizeCommand = new RelayCommand((p) => WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand((p) => WindowState = WindowState.Maximized);
            RestoreCommand = new RelayCommand((p) => WindowState = WindowState.Normal);
            CloseCommand = new RelayCommand((p) => Close());
        }

        static ApplicationWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApplicationWindow), new FrameworkPropertyMetadata(typeof(ApplicationWindow)));
        }
        #endregion

        /************************************************************************/

        #region IconImageSource
        /// <summary>
        /// Gets or sets the image to use for the application icon.
        /// </summary>
        public ImageSource IconImageSource
        {
            get => (ImageSource)GetValue(IconImageSourceProperty);
            set => SetValue(IconImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IconImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconImageSourceProperty = DependencyProperty.Register
            (
                nameof(IconImageSource), typeof(ImageSource), typeof(ApplicationWindow), new PropertyMetadata(null)
            );
        #endregion

        /************************************************************************/

        #region Minimize
        /// <summary>
        /// Gets or sets the image to use for the Minimize button
        /// </summary>
        public ImageSource MinimizeImageSource
        {
            get => (ImageSource)GetValue(MinimizeImageSourceProperty);
            set => SetValue(MinimizeImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MinimizeImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimizeImageSourceProperty = DependencyProperty.Register
            (
                nameof(MinimizeImageSource), typeof(ImageSource), typeof(ApplicationWindow), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets the Minimize command.
        /// </summary>
        public ICommand MinimizeCommand
        {
            get => (ICommand)GetValue(MinimizeCommandProperty);
            private set => SetValue(MinimizeCommandPropertyKey, value);
        }

        private static readonly DependencyPropertyKey MinimizeCommandPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(MinimizeCommand), typeof(ICommand), typeof(ApplicationWindow), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="MinimizeCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimizeCommandProperty = MinimizeCommandPropertyKey.DependencyProperty;

        #endregion

        /************************************************************************/

        #region Maximize
        /// <summary>
        /// Gets or sets the image to use for the Maximize button.
        /// </summary>
        public ImageSource MaximizeImageSource
        {
            get => (ImageSource)GetValue(MaximizeImageSourceProperty);
            set => SetValue(MaximizeImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MaximizeImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximizeImageSourceProperty = DependencyProperty.Register
            (
                nameof(MaximizeImageSource), typeof(ImageSource), typeof(ApplicationWindow), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets the command that maximizes the window.
        /// </summary>
        public ICommand MaximizeCommand
        {
            get => (ICommand)GetValue(MaximizeCommandProperty);
            private set => SetValue(MaximizeCommandPropertyKey, value);
        }

        private static readonly DependencyPropertyKey MaximizeCommandPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(MaximizeCommand), typeof(ICommand), typeof(ApplicationWindow), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="MaximizeCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximizeCommandProperty = MaximizeCommandPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Restore
        /// <summary>
        /// Gets or sets the image to use for the Restore window button
        /// </summary>
        public ImageSource RestoreImageSource
        {
            get => (ImageSource)GetValue(RestoreImageSourceProperty);
            set => SetValue(RestoreImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RestoreImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RestoreImageSourceProperty = DependencyProperty.Register
            (
                nameof(RestoreImageSource), typeof(ImageSource), typeof(ApplicationWindow), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets the command that restores the window.
        /// </summary>
        public ICommand RestoreCommand
        {
            get => (ICommand)GetValue(RestoreCommandProperty);
            private set => SetValue(RestoreCommandPropertyKey, value);
        }

        private static readonly DependencyPropertyKey RestoreCommandPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(RestoreCommand), typeof(ICommand), typeof(ApplicationWindow), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="RestoreCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RestoreCommandProperty = RestoreCommandPropertyKey.DependencyProperty;

        #endregion

        /************************************************************************/

        #region Close
        /// <summary>
        /// Gets or sets the image to use for the Close button
        /// </summary>
        public ImageSource CloseImageSource
        {
            get => (ImageSource)GetValue(CloseImageSourceProperty);
            set => SetValue(CloseImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CloseImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CloseImageSourceProperty = DependencyProperty.Register
            (
                nameof(CloseImageSource), typeof(ImageSource), typeof(ApplicationWindow), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets the Close command.
        /// </summary>
        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            private set => SetValue(CloseCommandPropertyKey, value);
        }

        private static readonly DependencyPropertyKey CloseCommandPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(CloseCommand), typeof(ICommand), typeof(ApplicationWindow), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="CloseCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CloseCommandProperty = CloseCommandPropertyKey.DependencyProperty;
        #endregion
    }
}