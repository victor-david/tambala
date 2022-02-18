/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Windows;
using System.Windows.Media;

namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Extends Border to provide a divider for a <see cref="SongPresenter"/>
    /// </summary>
    internal class VisualDivider : VisualBorder
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualDivider"/> class.
        /// </summary>
        /// <param name="position">The position</param>
        internal VisualDivider(int position) : base(position)
        {
            Width = 3.0;
            Margin = new Thickness(1);
            Background = new SolidColorBrush(Colors.DarkGray);
            VerticalAlignment = VerticalAlignment.Stretch;
        }
        #endregion
    }
}