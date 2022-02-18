/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Provides a panel to hold <see cref="BoxRadioButton"/> objects
    /// and enable one central binding for values assigned to each.
    /// </summary>
    public class BoxRadioButtonPanel : Grid
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxRadioButtonPanel"/> class.
        /// </summary>
        public BoxRadioButtonPanel()
        {
            AddHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(CheckedEventHandler));
        }
        #endregion

        /************************************************************************/

        #region SelectedValue
        /// <summary>
        /// Gets or sets the selected value
        /// </summary>
        public int SelectedValue
        {
            get => (int)GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }
        
        /// <summary>
        /// Identifies the <see cref="SelectedValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =  DependencyProperty.Register
            (
                nameof(SelectedValue), typeof(int), typeof(BoxRadioButtonPanel), new PropertyMetadata(0, OnSelectedValueChanged)
            );

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BoxRadioButtonPanel c)
            {
                c.UpdateChildren();
            }
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Called when the initialization process is complete
        /// </summary>
        public override void EndInit()
        {
            UpdateChildren();
            base.EndInit();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void CheckedEventHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is BoxRadioButton button)
            {
                SelectedValue = button.Value;
                e.Handled = true;
            }
        }

        private void UpdateChildren()
        {
            foreach (var child in Children.OfType<BoxRadioButton>())
            {
                child.IsChecked = child.Value == SelectedValue;
            }
        }
        #endregion
    }
}