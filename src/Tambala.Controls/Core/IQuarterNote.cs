/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the MIT License
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
namespace Restless.App.Tambala.Controls.Core
{
    /// <summary>
    /// Defines properties for classes that implement quarter note partitioning
    /// </summary>
    internal interface IQuarterNote
    {
        /// <summary>
        /// Gets or sets the number of quarter notes.
        /// </summary>
        int QuarterNoteCount { get; set; }

        /// <summary>
        /// Gets or sets the number of ticks per quarter note.
        /// </summary>
        int TicksPerQuarterNote { get; set; }
        
        /// <summary>
        /// Gets or sets the scale to use when presenting the quarter notes.
        /// </summary>
        double Scale { get; set; }
    }
}