/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.Tambala.Controls.Core
{
    /// <summary>
    /// Provides enumerated values for selector units.
    /// </summary>
    public enum PointSelectorUnit
    {
        /// <summary>
        /// No unit is associated with the selector.
        /// </summary>
        None = 0,
        /// <summary>
        /// The unit is a quarter note.
        /// </summary>
        QuarterNote = 1,
        /// <summary>
        /// The unit is an eighth note.
        /// </summary>
        EighthNote = 2,
        /// <summary>
        /// The unit is an eighth note triplet.
        /// </summary>
        EighthNoteTriplet = 3,
        /// <summary>
        /// The unit is a sixteenth note.
        /// </summary>
        SixteenthNote = 4,
        /// <summary>
        /// The unit is a thirty second note.
        /// </summary>
        ThirtySecondNote = 8,
    }
}