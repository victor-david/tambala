using Microsoft.Win32;
using Restless.App.Tambala.Controls;
using Restless.App.Tambala.Controls.Audio;
using Restless.App.Tambala.Resources;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Restless.App.Tambala.ViewModel
{
    /// <summary>
    /// Represents the view model for handling the audio render window.
    /// </summary>
    public class AudioRenderWindowViewModel : WindowViewModel
    {
        #region Private
        private bool isRenderInProgress;
        private bool isRenderComplete;
        private string closeCaption;
        
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
            private set
            {
                SetProperty(ref isRenderInProgress, value);
                OnPropertyChanged(nameof(AreControlsEnabled));
                OnPropertyChanged(nameof(IsCloseEnabled));
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates if rendering is complete.
        /// </summary>
        public bool IsRenderComplete
        {
            get => isRenderComplete;
            private set
            {
                SetProperty(ref isRenderComplete, value);
                OnPropertyChanged(nameof(IsCloseEnabled));
            }
        }

        /// <summary>
        /// Gets the caption text for the close button
        /// </summary>
        public string CloseCaption
        {
            get => closeCaption;
            private set => SetProperty(ref closeCaption, value);
        }
        /// <summary>
        /// Gets a boolean value that indicates if controls are enabled.
        /// </summary>
        public bool AreControlsEnabled
        {
            get => !IsRenderInProgress && !IsRenderComplete;
        }

        /// <summary>
        /// Gets a boolean value that indicates if the window may be closed.
        /// </summary>
        public bool IsCloseEnabled
        {
            get => !IsRenderInProgress || IsRenderComplete;
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
        /// <param name="owner">The owner of this view model.</param>
        /// <param name="projectContainer">The project container.</param>
        public AudioRenderWindowViewModel(Window owner, ProjectContainer projectContainer) : base(owner)
        {
            Container = projectContainer ?? throw new ArgumentNullException(nameof(projectContainer));
            // Container.RenderCompleted += ContainerRenderCompleted;
            Commands.Add("Output", RunChangeOutputCommand);
            Commands.Add("Render", RunRenderCommand);
            Commands.Add("Close", RunCloseCommand);
            WindowOwner.Closing += WindowOwnerClosing;
            WindowOwner.Closed += WindowOwnerClosed;
            CloseCaption = "Cancel";
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

                //Container.RenderParms.FileName = fileName;
            }
        }

        private void RunRenderCommand(object parm)
        {
            IsRenderInProgress = true;
            //Container.StartRender();
        }

        private void ContainerRenderCompleted(object sender, AudioRenderEventArgs e)
        {
            IsRenderInProgress = false;
            IsRenderComplete = true;
            CloseCaption = "Close";
        }

        private void RunCloseCommand(object parm)
        {
            WindowOwner.Close();
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
