/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Restless.App.Tambala.Controls
{
    /// <summary>
    /// Represents a button that contains an image
    /// </summary>
    public class ImageButton : ButtonBase
    {
        #region Public properties
        /// <summary>
        /// Gets or sets the image source for this button.
        /// </summary>
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register
            (
                nameof(ImageSource), typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the image size.
        /// </summary>
        public int ImageSize
        {
            get => (int)GetValue(ImageSizeProperty);
            set => SetValue(ImageSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ImageSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageSizeProperty = DependencyProperty.Register
            (
                nameof(ImageSize), typeof(int), typeof(ImageButton), new PropertyMetadata(32)
            );

        /// <summary>
        /// Gets or sets the text associated with the button.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register
            (
                nameof(Text), typeof(string), typeof(ImageButton), new PropertyMetadata(null)
            );
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageButton"/> class.
        /// </summary>
        public ImageButton()
        {
        }

        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }
        #endregion

    }
}