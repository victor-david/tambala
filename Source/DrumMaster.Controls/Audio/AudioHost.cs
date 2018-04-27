﻿using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Restless.App.DrumMaster.Controls.Audio
{
    public class AudioHost
    {
        #region Private
        private static readonly AudioHost instance = new AudioHost();
        private MasteringVoice masteringVoice;
        private List<VoicePool> voicePools;
        private XAudio2 audioDevice;
        
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the audio device.
        /// </summary>
        public XAudio2 AudioDevice
        {
            get => audioDevice;
            private set
            {
                audioDevice = value;
            }
        }

        /// <summary>
        /// Gets the audio pieces
        /// </summary>
        public AudioPieceCollection AudioPieces
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Singleton access and constructor
        /// <summary>
        /// Gets the singleton instance of <see cref="AudioManager"/>
        /// </summary>
        public static AudioHost Instance
        {
            get => instance;
        }

        private AudioHost()
        {
            AudioDevice = new XAudio2();
            masteringVoice = new MasteringVoice(AudioDevice);
            AudioPieces = new AudioPieceCollection();
            voicePools = new List<VoicePool>();
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
        /// Initializes the audio manager.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Adds a audio piece that resides in the specified assembly.
        /// </summary>
        /// <param name="audioResourceName">The audio name</param>
        /// <param name="displayName">The display name</param>
        /// <param name="type">The type</param>
        /// <param name="sourceAssembly">The source assembly</param>
        public void AddPieceFromAssembly(string audioResourceName, string displayName, AudioPieceType type, Assembly sourceAssembly)
        {
            if (sourceAssembly == null) throw new ArgumentNullException(nameof(sourceAssembly));
            AudioPieces.Add(new AudioPiece(audioResourceName, displayName, type, sourceAssembly));
        }

        /// <summary>
        /// Adds a audio piece that resides in the specified assembly.
        /// The display name type will be detected automatically if possible.
        /// </summary>
        /// <param name="audioResourceName">The audio name</param>
        /// <param name="sourceAssembly">The source assembly</param>
        public void AddPieceFromAssembly(string audioResourceName, Assembly sourceAssembly)
        {
            if (sourceAssembly == null) throw new ArgumentNullException(nameof(sourceAssembly));
            AudioPieces.Add(new AudioPiece(audioResourceName, sourceAssembly));
        }

        /// <summary>
        /// Adds a audio piece that resides in the file system.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="type">The type.</param>
        public void AddPieceFromFileSystem(string fileName, string displayName, AudioPieceType type)
        {
            AudioPieces.Add(new AudioPiece(fileName, displayName, type, null));
        }

        /// <summary>
        /// Adds a audio piece that resides in the file system.
        /// The display name and type will be detected automatically if possible.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public void AddPieceFromFileSystem(string fileName)
        {
            AudioPieces.Add(new AudioPiece(fileName, null));
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
            SharpDX.Utilities.Dispose(ref masteringVoice);
            SharpDX.Utilities.Dispose(ref audioDevice);
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Gets an audio piece of the specified type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The default piece for the type, or the first piece of the type if none are marked default, or null.</returns>
        internal AudioPiece GetAudioPiece(AudioPieceType type)
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
        internal AudioPiece GetAudioPiece(string audioName)
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
        /// Creates a voice pool
        /// </summary>
        /// <param name="audio">The audio buffer.</param>
        /// <param name="outputVoice">The output voice for the new voice pool.</param>
        /// <returns>The newly created voice pool.</returns>
        internal VoicePool CreateVoicePool(AudioBuffer audio, Voice outputVoice)
        {
            var pool = new VoicePool(audio, outputVoice);
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
