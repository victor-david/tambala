/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;

namespace Restless.Tambala.Controls.Audio
{
    /// <summary>
    /// Provides parameters used to communicate the state of the rendering process.
    /// </summary>
    internal class AudioRenderStateParameters
    {
        private AudioRenderState state;
        private Action<AudioRenderState> stateChange;

        /// <summary>
        /// From this assembly, gets or sets a value that determines if rendering is in progress
        /// </summary>
        internal bool IsRendering;

        /// <summary>
        /// From this assembly, gets or sets the state change callback.
        /// </summary>
        /// <exception cref="ArgumentNullException">Attempt to set to null</exception>
        internal Action<AudioRenderState> StateChange
        {
            get => stateChange;
            set => stateChange = value ?? throw new ArgumentNullException(nameof(StateChange));
        }

        /// <summary>
        /// From this assmebly, gets or sets the current state.
        /// </summary>
        internal AudioRenderState State
        {
            get => state;
            set => SetState(value);
        }

        /// <summary>
        /// From this assembly, gets a boolean value that indicates if <see cref="State"/> is in a frame capturing state.
        /// </summary>
        internal bool IsInCaptureState
        {
            get => state == AudioRenderState.Render || state == AudioRenderState.Fade;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRenderState"/> class.
        /// </summary>
        internal AudioRenderStateParameters()
        {
            State = AudioRenderState.Idle;
        }

        private void SetState(AudioRenderState state)
        {
            if (this.state != state)
            {
                this.state = state;
                StateChange?.Invoke(state);
            }
        }
    }
}
