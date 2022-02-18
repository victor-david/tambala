/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Extends button to provide a drum pattern selector for <see cref="SongPresenter"/>
    /// </summary>
    public class DrumPatternSelector : Button
    {
        #region Private
        private bool isSelected;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the position within <see cref="SongPresenter"/>
        /// </summary>
        public int Position
        {
            get;
        }

        /// <summary>
        /// Gets or sets a boolean vlaue that indicates whether this item is selected.
        /// </summary>
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                Background = isSelected ? IsSelectedBackground : Brushes.Transparent;
                BorderBrush = isSelected ? IsSelectedBorderBrush : Brushes.Transparent;
                Foreground = isSelected ? IsSelectedForeground : IsUnselectedForeground;
            }
        }

        /// <summary>
        /// Gets or sets the background brush when <see cref="IsSelected"/> is true
        /// </summary>
        public Brush IsSelectedBackground
        {
            get => (Brush)GetValue(IsSelectedBackgroundProperty);
            set => SetValue(IsSelectedBackgroundProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSelectedBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedBackgroundProperty = DependencyProperty.Register
            (
                nameof(IsSelectedBackground), typeof(Brush), typeof(DrumPatternSelector), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the border brush when <see cref="IsSelected"/> is true
        /// </summary>
        public Brush IsSelectedBorderBrush
        {
            get => (Brush)GetValue(IsSelectedBorderBrushProperty);
            set => SetValue(IsSelectedBorderBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSelectedBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedBorderBrushProperty = DependencyProperty.Register
            (
                nameof(IsSelectedBorderBrush), typeof(Brush), typeof(DrumPatternSelector), new FrameworkPropertyMetadata(null)
            );


        /// <summary>
        /// Gets or sets the foreground when the selector is selected.
        /// </summary>
        public Brush IsSelectedForeground
        {
            get => (Brush)GetValue(IsSelectedForegroundProperty);
            set => SetValue(IsSelectedForegroundProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSelectedForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedForegroundProperty = DependencyProperty.Register
            (
                nameof(IsSelectedForeground), typeof(Brush), typeof(DrumPatternSelector), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the foreground when the selector is not selected.
        /// </summary>
        public Brush IsUnselectedForeground
        {
            get => (Brush)GetValue(IsUnselectedForegroundProperty);
            set => SetValue(IsUnselectedForegroundProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsUnselectedForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsUnselectedForegroundProperty = DependencyProperty.Register
            (
                nameof(IsUnselectedForeground), typeof(Brush), typeof(DrumPatternSelector), new FrameworkPropertyMetadata(null)
            );
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumPatternSelector"/> class.
        /// </summary>
        /// <param name="position"></param>
        internal DrumPatternSelector(int position)
        {
            Position = position;
        }

        static DrumPatternSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrumPatternSelector), new FrameworkPropertyMetadata(typeof(DrumPatternSelector)));
        }
        #endregion
    }
}