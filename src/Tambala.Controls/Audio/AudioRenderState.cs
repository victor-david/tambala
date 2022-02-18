/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
namespace Restless.Tambala.Controls.Audio
{
    /// <summary>
    /// Provides an enumeration of values that indicate the state of an audio render operation
    /// </summary>
    public enum AudioRenderState
    {
        /// <summary>
        /// No render is occurring.
        /// </summary>
        Idle,
        /// <summary>
        /// Main render is in progress.
        /// </summary>
        Render,
        /// <summary>
        /// Fade render is in progress
        /// </summary>
        Fade,
        /// <summary>
        /// Saving of the rendered audio file is in progress.
        /// </summary>
        Save,
        /// <summary>
        /// Render is complete.
        /// </summary>
        Complete
    }
}