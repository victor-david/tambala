/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the MIT License
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Tambala.Controls.Core;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Restless.App.Tambala.Controls.Audio
{
    /// <summary>
    /// Provides a metronome
    /// </summary>
    internal sealed class Metronome : IDisposable
    {
        #region Private
        // TODO: Make instrument configurable.
        private Instrument instrument;
        private bool isAudioEnabled;
        private SubmixVoice submixVoice;
        private VoicePool voicePool;
        // TODO Make volume adjustable.
        private readonly float volume;
        private readonly float pitchNormal;
        private readonly float pitchAccent;
        private int frequency;
        private readonly List<int> supportedFrequency;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Metronome"/> class.
        /// </summary>
        internal Metronome()
        {
            submixVoice = new SubmixVoice(AudioHost.Instance.AudioDevice);
            //submixVoice.SetOutputVoices(new VoiceSendDescriptor(this.owner.SubmixVoice));
            volume = XAudio2.DecibelsToAmplitudeRatio(0f);
            pitchNormal = XAudio2.SemitonesToFrequencyRatio(Constants.Pitch.Default);
            pitchAccent = XAudio2.SemitonesToFrequencyRatio(1.5f);
            supportedFrequency = new List<int>()
            {
                Constants.Metronome.Frequency.Quarter,
                Constants.Metronome.Frequency.Eighth,
                Constants.Metronome.Frequency.EighthTriplet,
                Constants.Metronome.Frequency.Sixteenth
            };
            Frequency = Constants.Metronome.Frequency.Default;
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets the instrument to use for the metronome.
        /// </summary>
        internal Instrument Instrument
        {
            get => instrument;
            set
            {
                instrument = value;
                OnInstrumentChanged();
            }
        }

        /// <summary>
        /// Gets or sets the frequency (how often the metronome sounds)
        /// </summary>
        /// <remarks>
        /// This value comes from <see cref="Constants.Metronome.Frequency"/>
        /// and is expressed as divisions (or ticks) within a quarter note.
        /// For example, 1 (every quarter note), 2 (eighth notes), 4 (sixteenth notes).
        /// The metronome always plays on the quarter note.
        /// </remarks>
        internal int Frequency
        {
            get => frequency;
            set
            {
                if (!supportedFrequency.Contains(value))
                {
                    value = Constants.Metronome.Frequency.Default;
                }
                frequency = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the metronome is active.
        /// </summary>
        internal bool IsActive
        {
            get;
            set;
        }
        #endregion

        /************************************************************************/

        #region IDisposable
        /// <summary>
        /// Disposes resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        /// <param name="disposing">true if disposing</param>
        [SuppressMessage("Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification="Disposal happens via SharpDx.Utilities")]
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (submixVoice != null)
                {
                    SharpDX.Utilities.Dispose(ref submixVoice);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// 
        /// Plays the metronome for the specified position of the quarter note.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="operationSet">The operation set.</param>
        internal void Play(int position, int operationSet)
        {
            if (IsActive & isAudioEnabled)
            {
                if (position == 0 || (frequency > Constants.Metronome.Frequency.Quarter && Ticks.FullTickPositionMap[frequency].Contains(position)))
                {
                    float pitch = (position == 0) ? pitchAccent : pitchNormal;
                    voicePool.Play(volume, pitch, operationSet);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void OnInstrumentChanged()
        {
            isAudioEnabled = (Instrument != null && Instrument.IsAudioInitialized);
            if (isAudioEnabled)
            {
                AudioHost.Instance.DestroyVoicePool(voicePool);
                voicePool = AudioHost.Instance.CreateVoicePool("Metronome", Instrument.Audio, submixVoice, Constants.InitialVoicePool.Normal);
            }
        }
        #endregion
    }
}