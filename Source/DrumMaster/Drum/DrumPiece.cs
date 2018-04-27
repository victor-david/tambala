using Restless.App.DrumMaster.Audio;
using Restless.App.DrumMaster.Core;
using SharpDX.IO;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Drum
{
    /// <summary>
    /// Represents a drum piece including its type and audio buffer.
    /// </summary>
    public class DrumPiece : ObjectBase
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        ///// <summary>
        ///// Gets the audio buffer for this drum piece.
        ///// </summary>
        //public AudioBuffer Audio
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// Gets the name of the audio file or resource that supplies the sound for this piece
        /// </summary>
        public string AudioName
        {
            get;
        }

        /// <summary>
        /// Gets the source of this piece.
        /// </summary>
        public DrumPieceAudioSource AudioSource
        {
            get;
        }

        ///// <summary>
        ///// Gets the wave format for this piece.
        ///// </summary>
        //public WaveFormat WaveFormat
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// Gets the drum piece type.
        /// </summary>
        public DrumPieceType Type
        {
            get;
        }

        ///// <summary>
        ///// Gets the decoded packets. This is pushed to the device queue at play time.
        ///// </summary>
        //public uint[] DecodedPacketsInfo
        //{
        //    get;
        //    private set;
        //}

        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumPiece"/> class.
        /// </summary>
        /// <param name="audioName">The name of the audio file that contains the sound to use.</param>
        /// <param name="audioSource">The audio source for this piece.</param>
        /// <param name="type">The drum piece classification.</param>
        public DrumPiece(string audioName, DrumPieceAudioSource audioSource, DrumPieceType type)
        {
            ValidateNull(audioName, nameof(audioName));
            //ValidateOperation(type == DrumPieceType.None, "Invalid value");
            AudioName = audioName;
            AudioSource = audioSource;
            Type = type;

            //try
            //{
            //    InitializeAudio();
            //}
            //catch
            //{
            //    Type = DrumPieceType.None;
            //}
        }

        //public DrumPiece(string audioName, System.IO.Stream stream, DrumPieceType type)
        //    : this(audioName, type)
        //{
        //    ValidateNull(stream, nameof(stream));
            
        //    //try
        //    //{
        //    //    InitializeAudio(stream);
        //    //}
        //    //catch
        //    //{
        //    //    Type = DrumPieceType.None;
        //    //}
        //}
        #endregion

        /************************************************************************/

        #region Public Methods
        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"{Type} {AudioName}";
        }
        #endregion

        /************************************************************************/

        #region Private methods
        //private void InitializeAudio()
        //{
        //    using (var nativefilestream = new NativeFileStream(AudioFileName, NativeFileMode.Open, NativeFileAccess.Read, NativeFileShare.Read))
        //    {
        //        var soundStream = new SoundStream(nativefilestream);
        //        WaveFormat = soundStream.Format;
        //        DecodedPacketsInfo = soundStream.DecodedPacketsInfo;
        //        Audio = new AudioBuffer()
        //        {
        //            Stream = soundStream.ToDataStream(),
        //            AudioBytes = (int)soundStream.Length,
        //            Flags = BufferFlags.EndOfStream,
        //        };
        //    }
        //}



        //private void InitializeAudio(System.IO.Stream stream)
        //{
        //    var soundStream = new SoundStream(stream);
        //    WaveFormat = soundStream.Format;
        //    DecodedPacketsInfo = soundStream.DecodedPacketsInfo;
        //    Audio = new AudioBuffer()
        //    {
        //        Stream = soundStream.ToDataStream(),
        //        AudioBytes = (int)soundStream.Length,
        //        Flags = BufferFlags.EndOfStream,
        //    };
        //}
        #endregion
    }
}
