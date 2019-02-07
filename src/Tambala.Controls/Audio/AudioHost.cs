/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Tambala.Controls.Core;
using SharpDX.XAudio2;
using System.Collections.Generic;
using System.Linq;

namespace Restless.App.Tambala.Controls.Audio
{
    /// <summary>
    /// Manages the XAudio2 device, the mastering voice, and the voice pools for the audio.
    /// </summary>
    public sealed class AudioHost
    {
        #region Private
        private MasteringVoice masterVoice;
        private List<VoicePool> voicePools;
        private XAudio2 audioDevice;
        //private Reverb reverb;
        //private readonly EffectDescriptor reverbEffectDescriptor;
        //private AudioCaptureEffect audioCapture;
        //private readonly EffectDescriptor audioCaptureEffectDescriptor;
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
        /// Gets the audio pieces
        /// </summary>
        public InstrumentCollection AudioPieces
        {
            get;
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
        //public AudioCaptureEffect AudioCapture
        //{
        //    get;
        //    private set;
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

            AudioPieces = new InstrumentCollection();
            voicePools = new List<VoicePool>();

            //reverb = new Reverb(AudioDevice);
            //reverbEffectDescriptor = new EffectDescriptor(reverb);

            //audioCapture = new AudioCaptureEffect();
            //audioCaptureEffectDescriptor = new EffectDescriptor(audioCapture);
            
            //masterVoice.SetEffectChain(reverbEffectDescriptor);

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
            // Not doing anything right now, but reserved for future expansion. 
        }

        /// <summary>
        /// Shuts down the audio host. This method should be called when the application closes.
        /// Once this method is called, <see cref="AudioDevice"/> is not longer available.
        /// </summary>
        public void Shutdown()
        {
            foreach (VoicePool pool in voicePools)
            {
                pool.Destroy();
            }

            AudioDevice.StopEngine();
            SharpDX.Utilities.Dispose(ref masterVoice);
            SharpDX.Utilities.Dispose(ref audioDevice);
            // SharpDX.Utilities.Dispose(ref reverb);
            // Note. This will throw in SharpDx.CallbackBase if AudioCapture has not been inserted into the effect chain.
            //SharpDX.Utilities.Dispose(ref audioCapture);
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Gets an audio piece of the specified type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The default piece for the type, or the first piece of the type if none are marked default, or null.</returns>
        internal Instrument GetAudioPiece(InstrumentType type)
        {
            foreach (var piece in AudioPieces.Where((p)=>p.Type == type))
            {
                if (piece.IsDefault)
                {
                    return piece;
                }
            }
            return AudioPieces.Find((p) => p.Type == type);
        }

        /// <summary>
        /// Gets an audio piece by its audio name.
        /// </summary>
        /// <param name="audioName">The audio name</param>
        /// <returns>The specified piece, or null if not found.</returns>
        internal Instrument GetAudioPiece(string audioName)
        {
            foreach (var piece in AudioPieces)
            {
                if (piece.AudioName == audioName)
                {
                    return piece;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a voice pool.
        /// </summary>
        /// <param name="name">The voice pool name. Used in diagnositics, usually the instrument name.</param>
        /// <param name="audio">The audio buffer.</param>
        /// <param name="outputVoice">The output voice for the new voice pool.</param>
        /// <param name="initialSize">The initial size of the voice pool.</param>
        /// <returns>The newly created voice pool.</returns>
        internal VoicePool CreateVoicePool(string name, AudioBuffer audio, Voice outputVoice, int initialSize)
        {
            var pool = new VoicePool(name, audio, outputVoice, initialSize);
            voicePools.Add(pool);
            return pool;
        }

        /// <summary>
        /// Destroys the specified <see cref="VoicePool"/> and removes it from the list.
        /// </summary>
        /// <param name="pool">The pool. If null, this method does nothing.</param>
        internal void DestroyVoicePool(VoicePool pool)
        {
            if (pool != null && voicePools.Contains(pool))
            {
                pool.Destroy();
                voicePools.Remove(pool);
            }
        }
        #endregion
    }
}