/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
namespace Restless.App.Tambala.Controls.Core
{
    /// <summary>
    /// Defines properties for selectors.
    /// </summary>
    internal interface ISelector : ISelectable
    {
        /// <summary>
        /// Gets or sets the selector size.
        /// </summary>
        double SelectorSize { get; set; }
        /// <summary>
        /// Gets or sets the division count
        /// </summary>
        int DivisionCount { get; set; }
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        int Position { get; set; }
    }
}