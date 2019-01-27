using SharpDX.XAudio2;
using SharpDX.XAudio2.Fx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Restless.App.DrumMaster.Controls.Audio
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
        //private EffectDescriptor reverbEffectDescriptor;
        //private AudioCaptureEffect audioCapture;
        private readonly EffectDescriptor audioCaptureEffectDescriptor;
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
            get;
            private set;
        }

        /// <summary>
        /// From this assembly, gets the audio capture effect.
        /// </summary>
        internal AudioCaptureEffect AudioCapture
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
            MasterVoice = new MasteringVoice(AudioDevice);

            //SubmixVoice = new SubmixVoice(AudioDevice);
            //SubmixVoice.SetOutputVoices(new VoiceSendDescriptor(masteringVoice));

            AudioPieces = new InstrumentCollection();
            voicePools = new List<VoicePool>();

            //reverb = new Reverb(AudioDevice);
            //reverbEffectDescriptor = new EffectDescriptor(reverb);

            AudioCapture = new AudioCaptureEffect();
            audioCaptureEffectDescriptor = new EffectDescriptor(AudioCapture);

            // TODO
            //masterVoice.SetEffectChain(audioCaptureEffectDescriptor);
            //masterVoice.DisableEffect(0);

            AudioDevice.StartEngine();
        }

        /// <summary>
        /// Static constructor. Tells C# compiler not to mark type as beforefieldinit.
        /// </summary>
        static AudioHost()
        {
            // not sure if this is still needed in .NET 4.x
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Initializes the audio host.
        /// </summary>
        public void Initialize()
        {
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
            // This throws an exception from SharpDx.CallbackBase
            // AudioCapture.Dispose();
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
