using Restless.App.DrumMaster.Drum;
using SharpDX.IO;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Audio
{
    /// <summary>
    /// Provides audio management
    /// </summary>
    public class AudioManager
    {
        #region Private
        private static readonly AudioManager instance = new AudioManager();
        private bool initialized;

        private MasteringVoice masteringVoice;

        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the audio device.
        /// </summary>
        public XAudio2 AudioDevice
        {
            get;
        }

        /// <summary>
        /// Get the available drum pieces
        /// </summary>
        public DrumPieceCollection DrumPieces
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Singleton access and constructor
        /// <summary>
        /// Gets the singleton instance of <see cref="AudioManager"/>
        /// </summary>
        public static AudioManager Instance
        {
            get => instance;
        }

        private AudioManager()
        {
            AudioDevice = new XAudio2();
            masteringVoice = new MasteringVoice(AudioDevice);
            masteringVoice.SetVolume(1.0f, 0);
            DrumPieces = new DrumPieceCollection();
        }

        /// <summary>
        /// Static constructor. Tells C# compiler not to mark type as beforefieldinit.
        /// </summary>
        static AudioManager()
        {
            // not sure if this is still needed in .NET 4.x
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Initializes the audio manager
        /// </summary>
        public void Initialize()
        {
            if (!initialized)
            {
                AudioDevice.StartEngine();
                var a = Assembly.GetExecutingAssembly();

                foreach (string resourceName in a.GetManifestResourceNames())
                {
                    if (resourceName.Contains("Resources.Audio."))
                    {
                        DrumPieces.Add(new DrumPiece(resourceName, DrumPieceAudioSource.AppResource, GetDrumPieceType(resourceName)));
                    }
                }
                initialized = true;
            }
        }

        public void Shutdown()
        {
            if (initialized)
            {
                AudioDevice.StopEngine();
                AudioDevice.Dispose();
                initialized = false;
            }
        }

        public void Load(string fileName)
        {
            //if (!audioBuffers.Contains(fileName))
            //{
            //    audioBuffers.Add(fileName);
            //}
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private DrumPieceType GetDrumPieceType(string resourceName)
        {
            if (resourceName.Contains("Resources.Audio.Cymbal")) return DrumPieceType.Cymbal;
            if (resourceName.Contains("Resources.Audio.HiHat")) return DrumPieceType.HiHat;
            if (resourceName.Contains("Resources.Audio.Kick")) return DrumPieceType.Kick;
            if (resourceName.Contains("Resources.Audio.Snare")) return DrumPieceType.Snare;
            if (resourceName.Contains("Resources.Audio.Tom")) return DrumPieceType.Tom;
            return DrumPieceType.Unknown;
        }
        #endregion
    }
}
