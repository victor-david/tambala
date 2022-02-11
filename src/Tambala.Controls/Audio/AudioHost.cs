/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Controls.Core;
using SharpDX.XAudio2;
using System;

namespace Restless.Tambala.Controls.Audio
{
    /// <summary>
    /// Manages the XAudio2 device, the mastering voice, and the voice pools for the audio.
    /// </summary>
    public sealed class AudioHost
    {
        #region Private
        private MasteringVoice masterVoice;
        private XAudio2 audioDevice;
        private AudioCaptureEffect audioCapture;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the audio device.
        /// </summary>
        public XAudio2 AudioDevice
        {
            get => audioDevice;
            private set => audioDevice = value;
        }

        /// <summary>
        /// Gets the mastering voice
        /// </summary>
        internal MasteringVoice MasterVoice
        {
            get => masterVoice;
        }

        ///// <summary>
        ///// From this assembly, gets the audio capture effect.
        ///// </summary>
        //internal AudioCaptureEffect AudioCapture
        //{
        //    get => audioCapture;
        //}

        /// <summary>
        /// Gets the collection of drum kits.
        /// </summary>
        internal DrumKitCollection DrumKits
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Singleton access and constructor
        /// <summary>
        /// Gets the singleton instance of <see cref="AudioHost"/>
        /// </summary>
        public static AudioHost Instance { get; } = new AudioHost();

        private AudioHost()
        {
            AudioDevice = new XAudio2();
            masterVoice = new MasteringVoice(AudioDevice);


            audioCapture = new AudioCaptureEffect();
            masterVoice.SetEffectChain(new EffectDescriptor(audioCapture));
            masterVoice.DisableEffect(0);

            AudioDevice.StartEngine();

            DrumKits = new DrumKitCollection();
        }

        static AudioHost()
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Initializes the audio host.
        /// You must call this method at application startup.
        /// </summary>
        public void Initialize()
        {
            VoicePools.Instance.Initialize();
        }

        /// <summary>
        /// Shuts down the audio host. This method should be called when the application closes.
        /// Once this method is called, <see cref="AudioDevice"/> is not longer available.
        /// </summary>
        public void Shutdown()
        {
            VoicePools.Instance.ShutdownAll();
            AudioDevice.StopEngine();
            masterVoice.DestroyVoice();
            SharpDX.Utilities.Dispose(ref masterVoice);
            SharpDX.Utilities.Dispose(ref audioDevice);
            // Note. This will throw in SharpDx.CallbackBase if AudioCapture has not been inserted into the effect chain.
            SharpDX.Utilities.Dispose(ref audioCapture);
        }
        #endregion


        ///// <summary>
        ///// Sets the rendering parameters.
        ///// </summary>
        ///// <param name="parms">The rendering parms</param>
        //internal void SetRenderingParameters(AudioRenderParameters parms)
        //{
        //    if (parms == null) throw new ArgumentNullException(nameof(parms));
        //    parms.Validate();
        //    audioCapture.RenderParms = parms;
        //}

        internal void StartCapture(AudioRenderParameters parms)
        {
            audioCapture.RenderParms = parms;
            masterVoice.EnableEffect(0);
            audioCapture.StartCapture();
        }

        //internal void EndCapture()
        //{
        //    masterVoice.DisableEffect(0);
        //    audioCapture.StopCapture();
        //}
    }
}