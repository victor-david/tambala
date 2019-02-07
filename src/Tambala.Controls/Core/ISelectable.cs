/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
namespace Restless.App.Tambala.Controls.Core
{
    /// <summary>
    /// Defines properties for a selectable object.
    /// </summary>
    internal interface ISelectable
    {
        /// <summary>
        /// Gets or sets a value that indicates if the object is selected.
        /// </summary>
        bool IsSelected { get; set; }
    }
}