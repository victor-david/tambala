/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Controls;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Restless.Tambala.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AboutWindow : AppWindow
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
            StandardIcon = BitmapFrame.Create(new Uri($"pack://application:,,,/Tambala;component/Resources/Images/Icon.Drum.Blue.128.ico"));
            // Minimized icon. Red.
            MinimizedIcon = BitmapFrame.Create(new Uri($"pack://application:,,,/Tambala;component/Resources/Images/Icon.Drum.Red.128.ico"));
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