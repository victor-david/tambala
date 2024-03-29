/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Provides values for <see cref="PointSelector"/>.
    /// </summary>
    public enum PointSelectorType
    {
        /// <summary>
        /// The selector type is undefined
        /// </summary>
        None,
        /// <summary>
        /// Song header selector
        /// </summary>
        SongHeader,
        /// <summary>
        /// Song row selector
        /// </summary>
        SongRow,
        /// <summary>
        /// Drum pattern header selector
        /// </summary>
        PatternHeader,
        /// <summary>
        /// Drum pattern row selector
        /// </summary>
        PatternRow
    }
}