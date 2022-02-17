using NAudio.Wave;
using System;

namespace Restless.Tambala.Core
{
    /// <summary>
    /// Extends wave stream to provide a stream that loops continuously.
    /// </summary>
    /// <remarks>
    /// https://markheath.net/post/looped-playback-in-net-with-naudio
    /// </remarks>
    public class LoopStream : WaveStream
    {
        #region Private
        private WaveStream sourceStream;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LoopStream"/> class.
        /// </summary>
        /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
        /// or else we will not loop to the start again.</param>
        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream ?? throw new ArgumentNullException(nameof(sourceStream));
            EnableLooping = true;
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets whether looping is enabled.
        /// </summary>
        public bool EnableLooping 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets the wave format of the stream.
        /// </summary>
        public override WaveFormat WaveFormat
        {
            get => sourceStream.WaveFormat;
        }

        /// <summary>
        /// Gets the length of the stream.
        /// </summary>
        public override long Length
        {
            get => sourceStream.Length;
        }

        /// <summary>
        /// Gets or sets the position of the stream.
        /// </summary>
        public override long Position
        {
            get => sourceStream.Position;
            set => sourceStream.Position = value;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (sourceStream.Position == 0 || !EnableLooping)
                    {
                        // something wrong with the source stream
                        break;
                    }
                    // loop
                    sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
        #endregion
    }
}