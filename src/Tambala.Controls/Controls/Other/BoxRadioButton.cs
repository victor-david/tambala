/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Windows;
using System.Windows.Controls;

namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Represents a custom radio button. See <see cref="BoxRadioButtonPanel"/> also.
    /// </summary>
    public class BoxRadioButton : RadioButton
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxRadioButton"/> class.
        /// </summary>
        public BoxRadioButton()
        {
        }

        static BoxRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BoxRadioButton), new FrameworkPropertyMetadata(typeof(BoxRadioButton)));
        }
        #endregion

        /************************************************************************/

        #region Value
        /// <summary>
        /// Gets or sets an abitrary integer value to associate with the <see cref="BoxRadioButton"/>
        /// </summary>
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register
            (
                nameof(Value), typeof(int), typeof(BoxRadioButton), new PropertyMetadata(0)
            );

        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>A descriptive string.</returns>
        public override string ToString()
        {
            return $"{nameof(BoxRadioButton)} Value: {Value} IsChecked: {IsChecked}";
        }
        #endregion
    }
}