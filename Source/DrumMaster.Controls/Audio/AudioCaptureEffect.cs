using NAudio.Wave;
using SharpDX;
using SharpDX.XAPO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Restless.App.DrumMaster.Controls.Audio
{
    /// <summary>
    /// Satisfies the implementation needed for <see cref="AudioCaptureEffect"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct AudioCaptureParameters
    {
    }

    /// <summary>
    /// Represents an audio effect that is used to capture the audio and save it to a file.
    /// </summary>
    internal class AudioCaptureEffect : AudioProcessorBase<AudioCaptureParameters>
    {
        #region Private
        private bool isCaptureEnabled;
        private List<float> captureSamples;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// From this assembly, gets or sets the audio render parameters
        /// </summary>
        internal AudioRenderParameters RenderParms
        {
            get;
            set;
        }
        #endregion

        /************************************************************************/

        #region Constructor (internal)
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioCaptureEffect"/> class.
        /// </summary>
        internal AudioCaptureEffect()
        {
            RegistrationProperties = new RegistrationProperties()
            {
                Clsid = Utilities.GetGuidFromType(typeof(AudioCaptureEffect)),
                CopyrightInfo = "Copyright",
                FriendlyName = "Audio Capture",
                MaxInputBufferCount = 1,
                MaxOutputBufferCount = 1,
                MinInputBufferCount = 1,
                MinOutputBufferCount = 1,
                Flags = PropertyFlags.Default
            };


            captureSamples = new List<float>();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Processes the incoming buffer, capturing the values for later save.
        /// </summary>
        /// <param name="inputProcessParameters">The input process parameters</param>
        /// <param name="outputProcessParameters">The output process parameters. Not used.</param>
        /// <param name="isEnabled">Whether the effect is enabled. Not used.</param>
        public override void Process(BufferParameters[] inputProcessParameters, BufferParameters[] outputProcessParameters, bool isEnabled)
        {
            if (isCaptureEnabled)
            {
                int frameCount = inputProcessParameters[0].ValidFrameCount;

                long streamLen = frameCount * InputFormatLocked.BlockAlign;
                DataStream input = new DataStream(inputProcessParameters[0].Buffer, streamLen, true, true);

                for (int i = 0; i < frameCount; i++)
                {
                    float left = input.Read<float>();
                    float right = input.Read<float>();
                    if (RenderParms.Channels == 2)
                    {
                        captureSamples.Add(left);
                        captureSamples.Add(right);
                    }
                    else
                    {
                        float sum = left + right;
                        captureSamples.Add(sum / 2);
                    }
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Starts the capturing process
        /// </summary>
        internal void StartCapture()
        {
            captureSamples.Clear();
            isCaptureEnabled = true;
        }

        /// <summary>
        /// Stops the capturing process and saves the .wav file.
        /// </summary>
        internal void StopCapture()
        {
            
#if DEBUG
            //RenderParms.FileName = @"F:\Temp\DrumMasterTest\drum.wav";
            //RenderParms.ParmsInFileName = true;
#endif
            WaveFormat format;
            if (RenderParms.BitDepth == 32)
            {
                format = WaveFormat.CreateIeeeFloatWaveFormat(RenderParms.SampleRate, RenderParms.Channels);
            }
            else
            {
                format = new WaveFormat(RenderParms.SampleRate, RenderParms.BitDepth, RenderParms.Channels);
            }

            float[] samples = captureSamples.ToArray();
            Debug.WriteLine($"DONE. Have {samples.Length} samples");

            using (var writer = new WaveFileWriter(RenderParms.RenderFileName, format))
            {
                writer.WriteSamples(samples, 0, samples.Length);
            }
        }
        #endregion
    }
}
