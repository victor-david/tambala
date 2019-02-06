/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the MIT License
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.Tambala.Controls.Audio
{
    /// <summary>
    /// Provides event arguments for the for audio render.
    /// </summary>
    public class AudioRenderEventArgs : EventArgs
    {
        #region Public properties
        /// <summary>
        /// Gets the audio render parameters associated with the event.
        /// </summary>
        public AudioRenderParameters AudioParms
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor (internal)
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRenderParameters"/> class.
        /// </summary>
        /// <param name="audioParms">The audio parameters</param>
        internal AudioRenderEventArgs(AudioRenderParameters audioParms)
        {
            AudioParms = audioParms ?? throw new ArgumentNullException(nameof(audioParms));
        }
        #endregion
    }
}