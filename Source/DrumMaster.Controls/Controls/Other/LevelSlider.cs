﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Extends Slider to provide additional labeling properties
    /// </summary>
    public class LevelSlider : Slider
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelSlider"/> class.
        /// </summary>
        internal LevelSlider()
        {
        }

        static LevelSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LevelSlider), new FrameworkPropertyMetadata(typeof(LevelSlider)));
        }
        #endregion

        /************************************************************************/

        #region BarSize
        /// <summary>
        /// Gets or sets the fixed size of the slider bar.
        /// When vertical, this is the height. When horizontal,the width.
        /// </summary>
        public double BarSize
        {
            get => (double)GetValue(BarSizeProperty);
            set => SetValue(BarSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BarSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BarSizeProperty = DependencyProperty.Register
            (
                nameof(BarSize), typeof(double), typeof(LevelSlider), new PropertyMetadata(double.NaN, null, OnBarSizeCoerce)
            );

        private static object OnBarSizeCoerce(DependencyObject d, object baseValue)
        {
            double proposed = (double)baseValue;
            return Math.Min(1200.0, Math.Max(60.0, proposed));
        }
        #endregion

        /************************************************************************/

        #region LabelText
        /// <summary>
        /// Gets or sets the label text
        /// </summary>
        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LabelText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register
            (
                nameof(LabelText), typeof(string), typeof(LevelSlider), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the foreground brush for <see cref="LabelText"/>.
        /// </summary>
        public Brush LabelTextBrush
        {
            get => (Brush)GetValue(LabelTextBrushProperty);
            set => SetValue(LabelTextBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LabelTextBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTextBrushProperty = DependencyProperty.Register
            (
                nameof(LabelTextBrush), typeof(Brush), typeof(LevelSlider), new PropertyMetadata(new SolidColorBrush(Colors.Black))
            );
        #endregion

        /************************************************************************/

        #region ValueText
        /// <summary>
        /// Gets or sets the value text
        /// </summary>
        public string ValueText
        {
            get => (string)GetValue(ValueTextProperty);
            set => SetValue(ValueTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ValueText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueTextProperty = DependencyProperty.Register
            (
                nameof(ValueText), typeof(string), typeof(LevelSlider), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the foreground brush for <see cref="ValueText"/>.
        /// </summary>
        public Brush ValueTextBrush
        {
            get => (Brush)GetValue(ValueTextBrushProperty);
            set => SetValue(ValueTextBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ValueTextBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueTextBrushProperty = DependencyProperty.Register
            (
                nameof(ValueTextBrush), typeof(Brush), typeof(LevelSlider), new PropertyMetadata(new SolidColorBrush(Colors.Black))
            );
        #endregion
    }
}
