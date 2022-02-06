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
    /// Represents a pool of SourceVoice objects.
    /// </summary>
    /// <remarks>
    /// A voice pool holds a series of SourceVoice objects that route to a specified submix voice.
    /// Each instrument uses a voice pool to play its sound. An instrument may be playing the sound
    /// from a previous spot on the timeline when it needs to play the sound again; the sounds may overlap,
    /// such as with cymbals that delay. The instrument plays via its voice pool <see cref="Play(float, float, int)"/>
    /// method which grabs an idle voice from the pool to submit to the audio engine. If no idle voices are available,
    /// the pool increases in size.
    /// 
    /// Instruments must obtain their voice pool via <see cref="VoicePools.Create(string, AudioBuffer, SubmixVoice, int)"/>
    /// instead of creating one directly. VoicePools keeps track of allocated voice pools for proper shutdown.
    /// </remarks>
    internal class VoicePool
    {
        #region Private
        private AudioBuffer audio;
        private VoiceSendDescriptor voiceSendDescriptor;
        private SourceVoice[] voices;
        private SubmixVoice outputVoice;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the name assigned to this voice pool.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets the size of this voice pool.
        /// </summary>
        public int Size
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the highest index within the voice pool that has been used.
        /// </summary>
        public int HighWaterIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of times the voice pool had to be expanded.
        /// </summary>
        public int IncreaseCount
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor (internal)
        /// <summary>
        /// Initializes a new instance of the <see cref="VoicePool"/> class
        /// </summary>
        /// <param name="name">The name assigned to this voice pool. Used in diagnostics.</param>
        /// <param name="audio">The auido source for the voice pool</param>
        /// <param name="outputVoice">The output voice.</param>
        /// <param name="initialSize">The intial size. Clamped to 16-48</param>
        internal VoicePool(string name, AudioBuffer audio, SubmixVoice outputVoice, int initialSize)
        {
            this.audio = audio ?? throw new ArgumentNullException(nameof(audio));
            this.outputVoice = outputVoice ?? throw new ArgumentNullException(nameof(outputVoice));
            Name = name;
            initialSize = Math.Max(Math.Min(initialSize, Constants.InitialVoicePool.High), Constants.InitialVoicePool.Normal);
            voiceSendDescriptor = new VoiceSendDescriptor(outputVoice);
            Size = initialSize;
            InitializeVoices(initialSize);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>A friendly string that describes the object.</returns>
        public override string ToString()
        {
            return $"Voice pool {Name} Size: {Size} Highwater: {HighWaterIndex} Increases: {IncreaseCount}";
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Plays the voice at the specified volume using the specified operation set.
        /// </summary>
        /// <param name="volume">The volume</param>
        /// <param name="pitch">The pitch (aka frequency ratio)</param>
        /// <param name="operationSet">The operation set.</param>
        internal void Play(float volume, float pitch, int operationSet)
        {
            SourceVoice voice = GetAvailableVoice();
            if (voice != null)
            {
                voice.SubmitSourceBuffer(audio, audio.DecodedPacketsInfo);
                voice.SetFrequencyRatio(pitch, operationSet);
                voice.SetVolume(volume, operationSet);
                voice.Start(operationSet);
            }
        }

        /// <summary>
        /// Destroys all voices in the pool.
        /// Only <see cref="VoicePools"/> calls this method.
        /// Consumers must always call <see cref="VoicePools.Destroy(VoicePool)"/>,
        /// not this method directly.
        /// </summary>
        internal void DestroyVoices()
        {
            for (int k = 0; k < Size; k++)
            {
                voices[k]?.DestroyVoice();
                SharpDX.Utilities.Dispose(ref voices[k]);
            }
        }

        /// <summary>
        /// Destroys the output voice associated with this pool.
        /// Only <see cref="VoicePools"/> calls this method.
        /// Consumers must always call <see cref="VoicePools.Shutdown(VoicePool)"/>,
        /// not this method directly.
        /// </summary>
        internal void DestroyOutputVoice()
        {
            outputVoice?.DestroyVoice();
            SharpDX.Utilities.Dispose(ref outputVoice);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void InitializeVoices(int initialSize)
        {
            voices = new SourceVoice[initialSize];

            for (int k = 0; k < Size; k++)
            {
                voices[k] = CreateVoice();
            }
        }

        private SourceVoice GetAvailableVoice()
        {
            for (int k = 0; k < Size; k++)
            {
                if (voices[k].State.BuffersQueued == 0)
                {
                    HighWaterIndex = Math.Max(HighWaterIndex, k);
                    return voices[k];
                }
            }
            IncreaseCount++;
            return IncreasePoolSize(6);
        }

        private SourceVoice IncreasePoolSize(int increase)
        {
            int oldSize = voices.Length;
            Size = oldSize + increase;
            
            Array.Resize(ref voices, Size);
            for (int k = 0; k < increase; k++)
            {
                voices[oldSize + k] = CreateVoice();
            }

            return voices[oldSize];
        }

        private SourceVoice CreateVoice()
        {
            SourceVoice voice = new SourceVoice(AudioHost.Instance.AudioDevice, audio.WaveFormat, VoiceFlags.None, Constants.Pitch.Max);
            voice.SetOutputVoices(voiceSendDescriptor);
            voice.SetVolume(1);
            return voice;
        }
        #endregion
    }
}