/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Microsoft.Win32;
using NAudio.Wave;
using NAudio.WaveFormRenderer;
using Restless.Tambala.Controls;
using Restless.Tambala.Controls.Audio;
using Restless.Tambala.Core;
using Restless.Tambala.Resources;
using Restless.Toolkit.Controls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Restless.Tambala.ViewModel
{
    /// <summary>
    /// Represents the view model for handling the audio render window.
    /// </summary>
    public class AudioRenderWindowViewModel : ApplicationViewModel
    {
        #region Private
        private bool isRenderInProgress;
        private string renderMessage;
        private const string FileExtension = "wav";
        private const string DottedFileExtension = ".wav";
        private bool haveRenderedFile;
        private readonly WaveFormRenderer waveFormRenderer;
        private readonly IPeakProvider peakProvider;
        private readonly WaveFormRendererSettings waveSettings;
        private ImageSource fileVisual;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private string playButtonText;
        private Window window;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a value that indicates if rendering is in progress.
        /// </summary>
        public bool IsRenderInProgress
        {
            get => isRenderInProgress;
            private set => SetProperty(ref isRenderInProgress, value);
        }

        /// <summary>
        /// Gets the <see cref="ProjectContainer"/> object associated with this render window.
        /// </summary>
        public ProjectContainer Container
        {
            get;
        }

        /// <summary>
        /// Gets a message for the interface, Render complete, or exception if one occurs
        /// </summary>
        public string RenderMessage
        {
            get => renderMessage;
            private set => SetProperty(ref renderMessage, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if a rendered file is present.
        /// </summary>
        public bool HaveRenderedFile
        {
            get => haveRenderedFile;
            private set => SetProperty(ref haveRenderedFile, value);
        }

        /// <summary>
        /// Gets the image source for the visual representation of the rendered file.
        /// </summary>
        public ImageSource FileVisual
        {
            get => fileVisual;
            private set => SetProperty(ref fileVisual, value);
        }

        /// <summary>
        /// Gets the text for the play button, i.e. Play or Stop
        /// </summary>
        public string PlayButtonText
        {
            get => playButtonText;
            private set => SetProperty(ref playButtonText, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRenderWindowViewModel"/> class.
        /// </summary>
        /// <param name="projectContainer">The project container.</param>
        public AudioRenderWindowViewModel(Window window, ProjectContainer projectContainer)
        {
            this.window = window ?? throw new ArgumentNullException(nameof(window));
            Container = projectContainer ?? throw new ArgumentNullException(nameof(projectContainer));
            Commands.Add("ChangeOutput", RunChangeOutputCommand);
            Commands.Add("PerformRender", RunRenderCommand);
            Commands.Add("PlayFile", RunPlayFileCommand);
            PlayButtonText = "Play";
            projectContainer.AudioRenderParameters.PropertyChanged += AudioRenderParametersPropertyChanged;
            window.Closing += WindowOwnerClosing;
            waveFormRenderer = new WaveFormRenderer();
            peakProvider = new MaxPeakProvider();
            waveSettings = GetRendererSettings();
            outputDevice = new WaveOutEvent();
            outputDevice.PlaybackStopped += OutputDevicePlaybackStopped;
            SetHaveRenderedFile();
            CreateVisualization();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunChangeOutputCommand(object parm)
        {
            var dialog = new SaveFileDialog
            {
                Title = Strings.DialogTitleSaveFile,
                AddExtension = true,
                DefaultExt = DottedFileExtension,
                Filter = $"{Strings.CaptionWaveFile} | *{DottedFileExtension}",
                OverwritePrompt = true,
                InitialDirectory = Path.GetDirectoryName(Container.AudioRenderParameters.FileName)
            };

            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                string ext = Path.GetExtension(fileName);

                if (ext != DottedFileExtension)
                {
                    fileName = Path.ChangeExtension(fileName, FileExtension);
                }

                Container.AudioRenderParameters.FileName = fileName;
                SetHaveRenderedFile();
                CreateVisualization();
            }
        }

        private void RunRenderCommand(object parm)
        {
            try
            {
                StopPlayback();
                IsRenderInProgress = true;
                RenderMessage = Strings.TextRenderInProgress;
                Container.StartRender(RenderStateChange);
            }
            catch (Exception ex)
            {
                IsRenderInProgress = false;
                RenderMessage = ex.Message;
            }
        }

        private void RenderStateChange(AudioRenderState state, Exception exception)
        {
            if (state == AudioRenderState.Complete)
            {
                SetHaveRenderedFile();
                CreateVisualization();
                IsRenderInProgress = false;
                RenderMessage = exception == null ? Strings.TextRenderComplete : exception.Message;
            }
        }

        private void CreateVisualization()
        {
            if (HaveRenderedFile)
            {
                Task.Factory.StartNew(() => CreateVisualization(peakProvider, waveSettings));
            }
        }

        private WaveFormRendererSettings GetRendererSettings()
        {
            return new SoundCloudBlockWaveFormSettings(Color.DarkBlue, Color.Transparent, Color.CadetBlue,Color.Transparent)
            {
                TopHeight = 68,
                BottomHeight = 68,
                Width = 756,
                DecibelScale = true,
                BackgroundColor = Color.Transparent
            };
        }

        private void CreateVisualization(IPeakProvider peakProvider, WaveFormRendererSettings settings)
        {
            Image image = null;
            try
            {
                using (var waveStream = new AudioFileReader(Container.AudioRenderParameters.RenderFileName))
                {
                    image = waveFormRenderer.Render(waveStream, peakProvider, settings);
                }
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(() => 
                {
                    FileVisual = CreateBitmapSourceFromGdiBitmap((Bitmap)image);
                    SetHaveRenderedFile();
                }));

            }
            catch (Exception ex)
            {
                MessageWindow.ShowError(ex.Message);
            }
        }

        private BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            Rectangle rect = new (0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            try
            {
                int size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        private void SetHaveRenderedFile()
        {
            HaveRenderedFile = File.Exists(Container.AudioRenderParameters.RenderFileName);
        }

        private void RunPlayFileCommand(object parm)
        {
            if (HaveRenderedFile)
            {
                CreateAudioFileReaderIf();
                TogglePlayback();
            }
        }

        private void TogglePlayback()
        {
            if (outputDevice.PlaybackState == PlaybackState.Stopped)
            {
                StartPlayback();
            }
            else
            {
                StopPlayback();
            }
        }
    

        private void StartPlayback()
        {
            if (outputDevice.PlaybackState == PlaybackState.Stopped)
            {
                outputDevice.Play();
                PlayButtonText = "Stop";
            }
        }

        private void StopPlayback()
        {
            if (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
                PlayButtonText = "Play";
            }
        }

        private void OutputDevicePlaybackStopped(object sender, StoppedEventArgs e)
        {
            audioFile.Position = 0;
            if (!HaveRenderedFile)
            {
                DestroyAudioFileReader();
            }
        }

        private void CreateAudioFileReaderIf()
        {
            if (audioFile == null)
            {
                audioFile = new AudioFileReader(Container.AudioRenderParameters.RenderFileName);
                LoopStream loop = new LoopStream(audioFile);
                outputDevice.Init(loop);
            }
        }

        private void DestroyAudioFileReader()
        {
            if (audioFile != null)
            {
                audioFile.Dispose();
            }
            audioFile = null;
        }

        private void AudioRenderParametersPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            StopPlayback();
            SetHaveRenderedFile();
            if (!HaveRenderedFile)
            {
                StopPlayback();
            }
        }

        private void WindowOwnerClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = IsRenderInProgress;
            if (!e.Cancel)
            {
                Container.AudioRenderParameters.PropertyChanged -= AudioRenderParametersPropertyChanged;
                window.Closing -= WindowOwnerClosing;
                outputDevice.PlaybackStopped -= OutputDevicePlaybackStopped;
                outputDevice.Dispose();
                DestroyAudioFileReader();
                outputDevice = null;
            }
        }
        #endregion
    }
}