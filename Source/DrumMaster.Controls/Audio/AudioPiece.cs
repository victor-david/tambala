using Restless.App.DrumMaster.Controls.Core;
using Restless.App.DrumMaster.Data.Core;
using SharpDX.IO;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.Reflection;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls.Audio
{
    /// <summary>
    /// Represents an audio piece including its type and audio buffer.
    /// </summary>
    public class AudioPiece : IXElement
    {
        #region Private
        private Assembly sourceAssembly;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the name of the audio file or resource that supplies the sound for this piece
        /// </summary>
        public string AudioName
        {
            get;
        }

        /// <summary>
        /// Gets the display name for this piece.
        /// </summary>
        public string DisplayName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a comment associated with this piece.
        /// </summary>
        public string Comment
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the drum piece type.
        /// </summary>
        public AudioPieceType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a boolean value that indicates if this piece is the default
        /// for its <see cref="Type"/>. Used when initially populating a track container.
        /// </summary>
        public bool IsDefault
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="Audio"/> has been successfully initialized.
        /// </summary>
        public bool IsAudioInitialized
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Internal properties
        /// <summary>
        /// From this assembly, gets the <see cref="AudioBuffer"/> associated with this piece.
        /// </summary>
        internal AudioBuffer Audio
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioPiece"/> class.
        /// </summary>
        /// <param name="audioName">The name of the audio file or resource that contains the sound to use.</param>
        /// <param name="displayName">The display name for the audio piece</param>
        /// <param name="type">The type for the audio piece.</param>
        /// <param name="sourceAssembly">The assembly when the audio piece is located.</param>
        internal AudioPiece(string audioName, string displayName, AudioPieceType type, Assembly sourceAssembly)
            : this (audioName, sourceAssembly, false)
        {
            DisplayName = displayName;
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioPiece"/> class.
        /// </summary>
        /// <param name="audioName">The name of the audio file or resource that contains the sound to use.</param>
        /// <param name="sourceAssembly">The assembly when the audio piece is located.</param>
        internal AudioPiece(string audioName, Assembly sourceAssembly)
            : this(audioName, sourceAssembly, true)
        {
        }

        private AudioPiece(string audioName, Assembly sourceAssembly, bool autoDetectProperties)
        {
            if (string.IsNullOrEmpty(audioName)) throw new ArgumentNullException(nameof(audioName));
            AudioName = audioName;
            this.sourceAssembly = sourceAssembly;
            Type = AudioPieceType.Unknown;
            InitializeAudio();
            if (IsAudioInitialized && autoDetectProperties)
            {
                InitializeProperties();
            }
        }
        #endregion

        /************************************************************************/

        #region IXElement 
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public XElement GetXElement()
        {
            var element = new XElement(nameof(AudioPiece));
            element.Add(new XElement(nameof(DisplayName), DisplayName));
            element.Add(new XElement(nameof(AudioName), AudioName));
            element.Add(new XElement(nameof(Type), Type));
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public void RestoreFromXElement(XElement element)
        {
        }
        #endregion

        /************************************************************************/

        #region Public Methods
        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return DisplayName;
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        #endregion

        /************************************************************************/

        #region Private methods

        private void InitializeAudio()
        {
            try
            {
                if (sourceAssembly != null)
                {
                    InitializeAudioFromResource();
                }
                else
                {
                    InitializeAudioFromFileSystem();
                }
                IsAudioInitialized = true;
            }
            catch
            {
                IsAudioInitialized = false;
            }
        }

        private void InitializeAudioFromFileSystem()
        {
            using (var nativefilestream = new NativeFileStream(AudioName, NativeFileMode.Open, NativeFileAccess.Read, NativeFileShare.Read))
            {
                InitializeAudio(nativefilestream);
            }
        }

        private void InitializeAudioFromResource()
        {
            using (System.IO.Stream stream = sourceAssembly.GetManifestResourceStream(AudioName))
            {
                InitializeAudio(stream);
            }
        }

        private void InitializeAudio(System.IO.Stream stream)
        {
            using (var soundStream = new SoundStream(stream))
            {
                Audio = new AudioBuffer()
                {
                    Stream = soundStream.ToDataStream(),
                    AudioBytes = (int)soundStream.Length,
                    Flags = BufferFlags.EndOfStream,
                    WaveFormat = soundStream.Format,
                    DecodedPacketsInfo = soundStream.DecodedPacketsInfo,
                };
            }
        }

        private void InitializeProperties()
        {
            if (sourceAssembly != null)
            {
                InitializePropertiesFromResource();
            }
            else
            {
                InitializePropertiesFromFileSystem();
            }

        }

        private void InitializePropertiesFromResource()
        {
            var data = new AudioResourceMetadata(sourceAssembly, AudioName);
            DisplayName = data.Name;
            Comment = data.Comment;
            IsDefault = data.TrackNumber == "1";

            AudioPieceType result = AudioPieceType.Unknown;

            if (Enum.TryParse(data.Album, out result))
            {
                Type = result;
            }
        }

        private void InitializePropertiesFromFileSystem()
        {
            DisplayName = "TODO: File Sys";
        }


        #endregion
    }
}
