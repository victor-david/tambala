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
        private const int InitialVoicePoolSize = 16;
        private int voicePoolSize = InitialVoicePoolSize;
        private AudioBuffer audio;
        private Voice outputVoice;
        private VoiceSendDescriptor voiceSendDescriptor;
        private SourceVoice[] voices;
        private int highWaterIndex;
        private int increaseCount;
        #endregion

        /************************************************************************/

        #region Constructor (internal)
        /// <summary>
        /// Initializes a new instance of the <see cref="VoicePool"/> class
        /// </summary>
        internal VoicePool(AudioBuffer audio, Voice outputVoice)
        {
            this.audio = audio ?? throw new ArgumentNullException(nameof(audio));
            this.outputVoice = outputVoice ?? throw new ArgumentNullException(nameof(outputVoice));
            voiceSendDescriptor = new VoiceSendDescriptor(outputVoice);
            InitializeVoices();
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
            for (int k = 0; k < voicePoolSize; k++)
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

        private void InitializeVoices()
        {
            voices = new SourceVoice[InitialVoicePoolSize];

            for (int k = 0; k < voicePoolSize; k++)
            {
                voices[k] = CreateVoice();
            }
        }

        private SourceVoice GetAvailableVoice()
        {
            for (int k = 0; k < voicePoolSize; k++)
            {
                if (voices[k].State.BuffersQueued == 0)
                {
                    highWaterIndex = Math.Max(highWaterIndex, k);
                    voices[k].SubmitSourceBuffer(audio, audio.DecodedPacketsInfo);
                    return voices[k];
                }
            }
            increaseCount++;
            return IncreasePoolSize(3);
        }

        private SourceVoice IncreasePoolSize(int increase)
        {
            int oldSize = voices.Length;
            voicePoolSize = oldSize + increase;
            Array.Resize(ref voices, voicePoolSize);
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
