using SharpDX.XAudio2;
using System;

namespace Restless.App.DrumMaster.Controls.Audio
{
    /// <summary>
    /// Represents a pool of SourceVoice objects.
    /// </summary>
    internal class VoicePool
    {
        #region Private
        private AudioBuffer audio;
        private readonly Voice outputVoice;
        private VoiceSendDescriptor voiceSendDescriptor;
        private SourceVoice[] voices;
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
        internal VoicePool(string name, AudioBuffer audio, Voice outputVoice, int initialSize)
        {
            this.audio = audio ?? throw new ArgumentNullException(nameof(audio));
            this.outputVoice = outputVoice ?? throw new ArgumentNullException(nameof(outputVoice));
            Name = name;
            initialSize = Math.Max(Math.Min(initialSize, TrackVals.InitialVoicePool.High), TrackVals.InitialVoicePool.Normal);
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
            var voice = GetAvailableVoice();
            if (voice != null)
            {
                voice.SetFrequencyRatio(pitch, operationSet);
                voice.SetVolume(volume, operationSet);
                voice.Start(operationSet);
            }
        }

        /// <summary>
        /// Destroys all voices in the pool.
        /// </summary>
        internal void Destroy()
        {
            for (int k = 0; k < Size; k++)
            {
                if (voices[k] != null)
                {
                    voices[k].DestroyVoice();
                    SharpDX.Utilities.Dispose(ref voices[k]);
                }
            }
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
                    voices[k].SubmitSourceBuffer(audio, audio.DecodedPacketsInfo);
                    return voices[k];
                }
            }
            IncreaseCount++;
            return IncreasePoolSize(4);
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
            SourceVoice voice = new SourceVoice(AudioHost.Instance.AudioDevice, audio.WaveFormat, VoiceFlags.None, TrackVals.Pitch.Max);
            voice.SetOutputVoices(voiceSendDescriptor);
            voice.SetVolume(1);
            return voice;
        }
        #endregion
    }
}
