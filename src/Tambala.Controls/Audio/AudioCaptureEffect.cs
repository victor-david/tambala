/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using NAudio.Wave;
using SharpDX;
using SharpDX.XAPO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Restless.Tambala.Controls.Audio
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
        private CaptureState captureState;
        private List<float> captureSamples;
        private int fadeSampleCount;
        private int frameCaptureCount;

        private enum CaptureState
        {
            Off,
            On,
            Fade,
            Save,
        }
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

            captureState = CaptureState.Off;
            captureSamples = new List<float>();
            frameCaptureCount = 0;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /*
         * Processing the Audio Data - Charles Petzold
         * https://msdn.microsoft.com/en-us/magazine/dn201755.aspx
         * 
         * The LockForProcess method can spend whatever time it needs for initialization, but the Process method runs on the audio-processing thread,
         * and it must not dawdle.You’ll discover that for a sampling rate of 44,100 Hz, the ValidFrameCount field of the buffer parameters equals 441,
         * indicating that Process is called 100 times per second, each time with 10 ms of audio data.
         * For two-channel stereo, the buffer contains 882 float values with the channels interleaved: left channel followed by right channel.
        */
        /// <summary>
        /// Processes the incoming buffer, capturing the values for later save.
        /// </summary>
        /// <param name="inputProcessParameters">The input process parameters</param>
        /// <param name="outputProcessParameters">The output process parameters. Not used.</param>
        /// <param name="isEnabled">Whether the effect is enabled. Not used.</param>
        public override void Process(BufferParameters[] inputProcessParameters, BufferParameters[] outputProcessParameters, bool isEnabled)
        {
            if (captureState == CaptureState.On || captureState == CaptureState.Fade)
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

                    if (captureState == CaptureState.Fade)
                    {
                        fadeSampleCount += RenderParms.Channels;
                        // Note: FadeSamples must always be an even number.
                        // This is handled in the AudioRenderParameters class.
                        if (fadeSampleCount == RenderParms.FadeSamples)
                        {
                            captureState = CaptureState.Save;
                            break;
                        }
                    }
                    frameCaptureCount++;

                    if (frameCaptureCount == RenderParms.FramesToCapture)
                    {
                        captureState = RenderParms.FadeSamples > 0 ? CaptureState.Fade : CaptureState.Save;
                    }
                }
            }

            if (captureState == CaptureState.Save)
            {
                captureState = CaptureState.Off;
                PerformFinalRendering();
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
            frameCaptureCount = 0;
            fadeSampleCount = 0;
            captureState = CaptureState.On;
        }

        private void PerformFinalRendering()
        {
            float[] samples = captureSamples.ToArray();
            if (RenderParms.FadeSamples > 0)
            {
                MixLoopFadeSamples(samples);
            }

            WaveFormat format;
            if (RenderParms.BitDepth == 32)
            {
                format = WaveFormat.CreateIeeeFloatWaveFormat(RenderParms.SampleRate, RenderParms.Channels);
            }
            else
            {
                format = new WaveFormat(RenderParms.SampleRate, RenderParms.BitDepth, RenderParms.Channels);
            }

            using (var writer = new WaveFileWriter(RenderParms.RenderFileName, format))
            {
                writer.WriteSamples(samples, 0, samples.Length - RenderParms.FadeSamples);
            }
        }

        private void MixLoopFadeSamples(float[] samples)
        {
            int fadeSamples = RenderParms.FadeSamples;
            if (samples.Length > fadeSamples * 2)
            {
                int len = samples.Length;
                for (int k=0; k < fadeSamples; k++)
                {
                    float front = samples[k];
                    float back = samples[len - fadeSamples + k];
                    float mixed = front + back;
                    mixed *= 0.95f;
                    if (mixed > 1.0f) mixed = 1.0f;
                    if (mixed < -1.0f) mixed = -1.0f;
                    samples[k] = mixed;
                }
            }
        }
        #endregion
    }
}