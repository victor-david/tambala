/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the MIT License
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
namespace Restless.App.Tambala.Controls.Core
{
    /// <summary>
    /// Defines properties for classes that implement selector units.
    /// </summary>
    internal interface ISelectorUnit
    {
        /// <summary>
        /// Gets or sets the selector unit.
        /// </summary>
        PointSelectorUnit SelectorUnit { get; set; }
    }
}