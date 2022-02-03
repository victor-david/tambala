/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
namespace Restless.Tambala.Controls.Audio
{
    /// <summary>
    /// Provides an enumeration that is used to categorize the type of a drum piece.
    /// </summary>
    public enum InstrumentType
    {
        /// <summary>
        /// The drum piece is a cymbal.
        /// </summary>
        Cymbal,
        /// <summary>
        /// The drum piece is a high hat.
        /// </summary>
        HighHat,
        /// <summary>
        /// The drum piece is a kick drum.
        /// </summary>
        Kick,
        /// <summary>
        /// The drum piece is a snare drum.
        /// </summary>
        Snare,
        /// <summary>
        /// The drum piece is a tom.
        /// </summary>
        Tom,
        /// <summary>
        /// The drum piece is percussion
        /// </summary>
        Percussion,
        /// <summary>
        /// The drum piece type is unknown.
        /// </summary>
        Unknown,
    }
}