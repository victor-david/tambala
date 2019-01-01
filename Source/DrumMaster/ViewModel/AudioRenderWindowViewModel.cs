using Restless.App.DrumMaster.Controls;
using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Restless.App.DrumMaster.Resources;
using System.IO;

namespace Restless.App.DrumMaster.ViewModel
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
        /// Gets the <see cref="TrackContainer"/> object associated with this render window.
        /// </summary>
        public TrackContainer Container
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
        /// <param name="trackContainer">The track container.</param>
        public AudioRenderWindowViewModel(Window owner, TrackContainer trackContainer) : base(owner)
        {
            Container = trackContainer ?? throw new ArgumentNullException(nameof(trackContainer));
            Container.RenderCompleted += ContainerRenderCompleted;
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
                InitialDirectory = Path.GetDirectoryName(Container.RenderParms.FileName)
            };

            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                string ext = Path.GetExtension(fileName);

                if (ext != DottedFileExtension)
                {
                    fileName = Path.ChangeExtension(fileName, FileExtension);
                }

                Container.RenderParms.FileName = fileName;
            }
        }

        private void RunRenderCommand(object parm)
        {
            IsRenderInProgress = true;
            Container.StartRender();
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
            Container.RenderCompleted -= ContainerRenderCompleted;
        }
        #endregion
    }
}
