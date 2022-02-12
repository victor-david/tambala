/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using NAudio.Wave;
using SharpDX;
using SharpDX.XAPO;
using System;
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
        private AudioRenderParameters renderParms;
        private AudioRenderStateParameters renderState;
        private List<float> captureSamples;
        private int fadeSampleCount;
        private int frameCaptureCount;
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

            renderState = new AudioRenderStateParameters();
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
            if (renderState.IsInCaptureState)
            {
                int frameCount = inputProcessParameters[0].ValidFrameCount;

                long streamLen = frameCount * InputFormatLocked.BlockAlign;
                DataStream input = new DataStream(inputProcessParameters[0].Buffer, streamLen, true, true);

                for (int i = 0; i < frameCount; i++)
                {
                    float left = input.Read<float>();
                    float right = input.Read<float>();
                    if (renderParms.Channels == 2)
                    {
                        captureSamples.Add(left);
                        captureSamples.Add(right);
                    }
                    else
                    {
                        float sum = left + right;
                        captureSamples.Add(sum / 2);
                    }

                    if (renderState.State == AudioRenderState.Fade)
                    {
                        fadeSampleCount += renderParms.Channels;
                        // Note: FadeSamples must always be an even number.
                        // This is handled in the AudioRenderParameters class.
                        if (fadeSampleCount == renderParms.FadeSamples)
                        {
                            renderState.State = AudioRenderState.Save;
                            break;
                        }
                    }
                    frameCaptureCount++;

                    if (frameCaptureCount == renderParms.FramesToCapture)
                    {
                        renderState.State = renderParms.FadeSamples > 0 ? AudioRenderState.Fade : AudioRenderState.Save;
                    }
                }
            }

            if (renderState.State == AudioRenderState.Save)
            {
                PerformFinalRendering();
                renderState.State = AudioRenderState.Complete;
            }
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Starts the capturing process
        /// </summary>
        internal void StartCapture(AudioRenderParameters renderParms, AudioRenderStateParameters processParms)
        {
            captureSamples.Clear();
            frameCaptureCount = 0;
            fadeSampleCount = 0;
            this.renderParms = renderParms;
            this.renderState.StateChange = processParms.StateChange;
            this.renderState.State = AudioRenderState.Render;
        }

        private void PerformFinalRendering()
        {
            float[] samples = captureSamples.ToArray();
            if (renderParms.FadeSamples > 0)
            {
                MixLoopFadeSamples(samples);
            }

            WaveFormat format;
            if (renderParms.BitDepth == 32)
            {
                format = WaveFormat.CreateIeeeFloatWaveFormat(renderParms.SampleRate, renderParms.Channels);
            }
            else
            {
                format = new WaveFormat(renderParms.SampleRate, renderParms.BitDepth, renderParms.Channels);
            }

            using (var writer = new WaveFileWriter(renderParms.RenderFileName, format))
            {
                writer.WriteSamples(samples, 0, samples.Length - renderParms.FadeSamples);
            }
        }

        private void MixLoopFadeSamples(float[] samples)
        {
            int fadeSamples = renderParms.FadeSamples;

            if (samples.Length <= fadeSamples * 2)
            {
                fadeSamples = Math.Max((samples.Length / 2) - 2, 0);
            }

            int len = samples.Length;
            for (int k = 0; k < fadeSamples; k++)
            {
                float front = samples[k];
                float back = samples[len - fadeSamples + k];
                float mixed = front + back;
                /* prevent clipping */
                samples[k] = Math.Clamp(mixed * 0.975f, -1.0f, 1.0f);
            }
        }
        #endregion
    }
}