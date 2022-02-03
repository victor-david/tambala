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
    /// Extends Border to provide a selector <see cref="SongPresenter"/>
    /// </summary>
    internal class VisualSelector : VisualBorder
    {
        #region Private
        private bool isSelected;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualSelector"/> class.
        /// </summary>
        /// <param name="position">The position</param>
        internal VisualSelector(int position) : base(position)
        {
            Padding = new Thickness(3, 0, 3, 0);
            Margin = new Thickness(2);
            CornerRadius = new CornerRadius(1);
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets a boolean vlaue that indicates whether this item is selected.
        /// When setting to true, <see cref="IsSelectedBrush"/> is applied.
        /// </summary>
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                Background = isSelected ? IsSelectedBrush : null;
            }
        }

        /// <summary>
        /// Gets or sets the brush to be applied when <see cref="IsSelected"/> is true;
        /// </summary>
        public Brush IsSelectedBrush
        {
            get; set;
        }
        #endregion
    }
}