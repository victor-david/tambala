using Restless.App.DrumMaster.Core;
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

namespace Restless.App.DrumMaster.Drum
{
    public class DrumMatrixVoice : ObjectBase
    {
        #region Private
        private XAudio2 device;
        private AudioBuffer audio;
        private uint[] decodedPacketsInfo;
        private WaveFormat waveFormat;
        private DrumPiece drumPiece;
        private SourceVoice voice;
        private bool audioInitialized;
        private float volume;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the volume for this voice.
        /// </summary>
        public float Volume
        {
            get => volume;
            set => volume = value;
        }

        /// <summary>
        /// Gets or sets whether this voice is on, that is whether it particpates in the pattern.
        /// </summary>
        public bool IsOn
        {
            get;
            set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        public DrumMatrixVoice(XAudio2 device, DrumPiece drumPiece)
        {
            ValidateNull(device, nameof(device));
            ValidateNull(drumPiece, nameof(drumPiece));
            this.device = device;
            this.drumPiece = drumPiece;
            Volume = 1;
            InitializeAudio();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        public void Play(int operationSet)
        {
            if (operationSet == 0) throw new InvalidOperationException("Operation set is zero");
            if (IsOn && audioInitialized)
            {
                InitializeVoice();
                voice.Start(operationSet);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void InitializeAudio()
        {
            try
            {
                switch (drumPiece.AudioSource)
                {
                    case DrumPieceAudioSource.AppResource:
                        InitializeAudioFromAppResource();
                        break;
                    case DrumPieceAudioSource.FileSystem:
                        InitializeAudioFromFileSystem();
                        break;
                    default:
                        throw new Exception("Unsupported");
                }
                audioInitialized = true;
            }
            catch
            {
                audioInitialized = false;
            }
        }

        private void InitializeAudioFromFileSystem()
        {
            using (var nativefilestream = new NativeFileStream(drumPiece.AudioName, NativeFileMode.Open, NativeFileAccess.Read, NativeFileShare.Read))
            {
                InitializeAudio(nativefilestream);
            }
        }

        private void InitializeAudioFromAppResource()
        {
            var a = Assembly.GetExecutingAssembly();

            using (System.IO.Stream stream = a.GetManifestResourceStream(drumPiece.AudioName))
            {
                InitializeAudio(stream);
            }
        }

        private void InitializeAudio(System.IO.Stream stream)
        {
            using (var soundStream = new SoundStream(stream))
            {
                waveFormat = soundStream.Format;
                decodedPacketsInfo = soundStream.DecodedPacketsInfo;
                audio = new AudioBuffer()
                {
                    Stream = soundStream.ToDataStream(),
                    AudioBytes = (int)soundStream.Length,
                    Flags = BufferFlags.EndOfStream,
                };
            }
        }

        private void InitializeVoice()
        {
            if (voice != null)
            {
                voice.DestroyVoice();
                voice.Dispose();
            }
            voice = new SourceVoice(device, waveFormat);
            //voice.StreamEnd += VoiceStreamEnd;
            voice.SetVolume(Volume);
            voice.SubmitSourceBuffer(audio, decodedPacketsInfo);
        }
        #endregion
    }
}
