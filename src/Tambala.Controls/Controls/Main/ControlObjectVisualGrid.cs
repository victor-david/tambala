/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Windows;
using System.Windows.Controls;

namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Extends <see cref="ControlObjectVisual"/> for classes that require a visual Grid
    /// that is defined as a property of the class. This class must be inherited.
    /// </summary>
    public abstract class ControlObjectVisualGrid : ControlObjectVisual
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of <see cref="ControlObjectVisualGrid"/>.
        /// </summary>
        internal ControlObjectVisualGrid()
        {
            VisualElement = new Grid();
        }
        #endregion

        /************************************************************************/

        #region Grid
        /// <summary>
        /// Gets the underlying <see cref="ControlObjectVisual.VisualElement"/> as a Grid.
        /// </summary>
        public Grid Grid
        {
            get => VisualElement as Grid;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Adds a row definition of the specified height and returns its index.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <returns>The index of the newly added row definition</returns>
        protected int AddRowDefinition(double height)
        {
            if (height < 0.0) height = 0.0;
            GridLength glen = new GridLength(height);
            Grid.RowDefinitions.Add(new RowDefinition() { Height = glen });
            return Grid.RowDefinitions.Count - 1;
        }

        /// <summary>
        /// Adds a row definition of auto height and returns its index.
        /// </summary>
        /// <returns>The index of the newly added row definition</returns>
        protected int AddRowDefinition()
        {
            GridLength glen = new GridLength(1, GridUnitType.Auto);
            Grid.RowDefinitions.Add(new RowDefinition() { Height = glen });
            return Grid.RowDefinitions.Count - 1;
        }

        /// <summary>
        /// Adds a column definition and returns its index.
        /// </summary>
        /// <param name="width">The height. Pass 0 (the default) for Auto</param>
        /// <returns>The index of the newly added column definition</returns>
        protected int AddColumnDefinition(double width = 0.0)
        {
            GridLength glen = (width > 0) ? new GridLength(width) : new GridLength(1, GridUnitType.Auto);
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = glen });
            return Grid.ColumnDefinitions.Count - 1;
        }

        /// <summary>
        /// Adds the specifed UIElement
        /// </summary>
        /// <param name="element">The element to add</param>
        /// <param name="row">The row</param>
        /// <param name="column">The column</param>
        protected void AddElement(UIElement element, int row, int column)
        {
            row = Math.Max(row, 0);
            column = Math.Max(column, 0);
            if (element == null) throw new ArgumentNullException(nameof(element));
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
            Grid.Children.Add(element);
        }
        #endregion
    }
}