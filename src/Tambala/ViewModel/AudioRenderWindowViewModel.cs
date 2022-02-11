/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Microsoft.Win32;
using Restless.Tambala.Controls;
using Restless.Tambala.Controls.Audio;
using Restless.Tambala.Resources;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Restless.Tambala.ViewModel
{
    /// <summary>
    /// Represents the view model for handling the audio render window.
    /// </summary>
    public class AudioRenderWindowViewModel : ApplicationViewModel
    {
        #region Private
        private bool isRenderInProgress;
        
        private const string FileExtension = "wav";
        private const string DottedFileExtension = ".wav";
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
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRenderWindowViewModel"/> class.
        /// </summary>
        /// <param name="projectContainer">The project container.</param>
        public AudioRenderWindowViewModel(ProjectContainer projectContainer)
        {
            Container = projectContainer ?? throw new ArgumentNullException(nameof(projectContainer));
            // Container.RenderCompleted += ContainerRenderCompleted;
            Commands.Add("ChangeOutput", RunChangeOutputCommand);
            Commands.Add("PerformRender", RunRenderCommand);
            Commands.Add("Close", RunCloseCommand);
            //WindowOwner.Closing += WindowOwnerClosing;
            //WindowOwner.Closed += WindowOwnerClosed;
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
                //InitialDirectory = Path.GetDirectoryName(Container.RenderParms.FileName)
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
            }
        }

        private void RunRenderCommand(object parm)
        {
            try
            {
                IsRenderInProgress = true;
                Container.StartRender(() =>
                {
                    IsRenderInProgress = false;
                });
            }
            catch (Exception ex)
            {
                IsRenderInProgress = false;
            }
        }

        private void ContainerRenderCompleted(object sender, AudioRenderEventArgs e)
        {
            IsRenderInProgress = false;
        }

        private void RunCloseCommand(object parm)
        {
            //WindowOwner.Close();
        }

        private void WindowOwnerClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = IsRenderInProgress;
        }

        private void WindowOwnerClosed(object sender, EventArgs e)
        {
            //Container.RenderCompleted -= ContainerRenderCompleted;
        }
        #endregion
    }
}