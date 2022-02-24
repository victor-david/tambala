/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Microsoft.Win32;
using NAudio.Wave;
using Restless.Tambala.Controls;
using Restless.Tambala.Controls.Audio;
using Restless.Tambala.Core;
using Restless.Tambala.Resources;
using Restless.Toolkit.Controls;
using Restless.WaveForm.Calculators;
using Restless.WaveForm.Renderer;
using Restless.WaveForm.Settings;
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
        private const int BaseRenderHeight = 128;
        private bool isRenderInProgress;
        private string renderMessage;
        private const string FileExtension = "wav";
        private const string DottedFileExtension = ".wav";
        private bool haveRenderedFile;
        private readonly IRenderer renderer;
        private readonly RenderSettings renderSettings;
        private readonly ISampleCalculator calculator;
        private ImageSource fileVisualLeft;
        private ImageSource fileVisualRight;
        private GridLength fileVisualRightRow;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private string playButtonText;
        private long fileLength;
        private long filePosition;
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
        /// Gets the length of the audio file
        /// </summary>
        public long FileLength
        {
            get => fileLength;
            private set => SetProperty(ref fileLength, value);
        }

        /// <summary>
        /// Gets the current playing position of the audio file
        /// </summary>
        public long FilePosition
        {
            get => filePosition;
            private set => SetProperty(ref filePosition, value);
        }

        /// <summary>
        /// Gets the image source for the visual representation of the rendered file (left channel).
        /// </summary>
        public ImageSource FileVisualLeft
        {
            get => fileVisualLeft;
            private set => SetProperty(ref fileVisualLeft, value);
        }

        /// <summary>
        /// Gets the image source for the visual representation of the rendered file (right channel).
        /// </summary>
        public ImageSource FileVisualRight
        {
            get => fileVisualRight;
            private set
            {
                SetProperty(ref fileVisualRight, value);
                FileVisualRightRow = fileVisualRight != null ? new GridLength(1.0, GridUnitType.Star) : new GridLength(0.0, GridUnitType.Pixel);
            }
        }

        /// <summary>
        /// Gets the row height for the right channel, either * (when 2 channels) or zero pixels (when single channel)
        /// </summary>
        public GridLength FileVisualRightRow
        {
            get => fileVisualRightRow;
            private set => SetProperty(ref fileVisualRightRow, value);
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
            Commands.Add("CloseWindow", p => window.Close());
            PlayButtonText = "Play";
            projectContainer.AudioRenderParameters.PropertyChanged += AudioRenderParametersPropertyChanged;
            window.Closing += WindowOwnerClosing;
            renderer = new BarRenderer();
            calculator = new FirstCalculator();
            renderSettings = new BarSettings()
            {
                Height = BaseRenderHeight,
                Width = RenderSettings.MinWidth,
                AutoWidth = false,
                PrimaryLineColor = Color.DarkBlue,
                SecondaryLineColor = Color.CadetBlue
            };

            /* Is default null, but the setter sets the right row height to zero when null */
            FileVisualRight = null;

            outputDevice = new WaveOutEvent();
            outputDevice.PlaybackStopped += OutputDevicePlaybackStopped;
            FileLength = 1000;
            FilePosition = 0;
            SetHaveRenderedFile();
            CreateVisualizationIf();
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
                InitialDirectory = Container.AudioRenderParameters.FileDirectory
            };

            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                string ext = Path.GetExtension(fileName);

                if (ext != DottedFileExtension)
                {
                    fileName = Path.ChangeExtension(fileName, FileExtension);
                }

                Container.AudioRenderParameters.SetOutputFileName(fileName);
                SetHaveRenderedFile();
                CreateVisualizationIf();
            }
        }

        private void RunRenderCommand(object parm)
        {
            try
            {
                StopPlayback();
                DestroyAudioFileReader();
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
                CreateVisualizationIf();
                IsRenderInProgress = false;
                RenderMessage = exception == null ? Strings.TextRenderComplete : exception.Message;
            }
        }

        private void CreateVisualizationIf()
        {
            if (HaveRenderedFile)
            {
                Task.Factory.StartNew(() => CreateVisualization());
            }
        }

        private void CreateVisualization()
        {
            RenderResult result = null;
            try
            {
                using (var waveStream = new AudioFileReader(Container.AudioRenderParameters.RenderFileName))
                {
                    renderSettings.Height = BaseRenderHeight / waveStream.WaveFormat.Channels;
                    result = WaveFormRenderer.Create(renderer, waveStream, calculator, renderSettings);
                }
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(() => 
                {
                    FileVisualLeft = CreateBitmapSourceFromGdiBitmap((Bitmap)result.ImageLeft);
                    FileVisualRight = result.Channels == 2 ? CreateBitmapSourceFromGdiBitmap((Bitmap)result.ImageRight) : null;
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
                CreateAudioFileReader();
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
            if (audioFile != null)
            {
                audioFile.Position = 0;
            }
            if (!HaveRenderedFile)
            {
                DestroyAudioFileReader();
            }
        }

        /// <summary>
        /// Creates the audio file reader if it's not already created.
        /// </summary>
        private void CreateAudioFileReader()
        {
            if (audioFile == null)
            {
                audioFile = new AudioFileReader(Container.AudioRenderParameters.RenderFileName);
                FileLength = audioFile.Length;
                FilePosition = 0;
                LoopStream loop = new LoopStream(audioFile);
                loop.PositionChanged += PlaybackPositionChanged;
                outputDevice.Init(loop);
            }
        }

        private void PlaybackPositionChanged(object sender, long position)
        {
            FilePosition = position;
        }

        /// <summary>
        /// Destroys <see cref="audioFile"/> if it isn't null.
        /// </summary>
        /// <remarks>
        /// The audio file reader must be destroyed before a render so that it's
        /// not holding the file open.Also used when window is closed.
        /// </remarks>
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