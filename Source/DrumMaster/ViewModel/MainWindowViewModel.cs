using Restless.App.DrumMaster.Core;
using Restless.App.DrumMaster.Resources;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Restless.App.DrumMaster.ViewModel
{
    /// <summary>
    /// Represents the view model for the main window.
    /// </summary>
    public class MainWindowViewModel : WindowViewModel
    {
        #region Private
        private SongContainerViewModel songContainer;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if the window that is bound to this view model stays on top.
        /// </summary>
        public bool IsTopMost
        {
            // TODO: Make this configurable
            get => false;
        }

        /// <summary>
        /// Gets the track container object.
        /// </summary>
        public SongContainerViewModel SongContainer
        {
            get => songContainer;
            private set => SetProperty(ref songContainer, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of <see cref="MainWindowViewModel"/>.
        /// </summary>
        /// <param name="owner">The owner of this view model.</param>
        public MainWindowViewModel(Window owner) : base (owner)
        {
            WindowOwner.Closing += MainWindowClosing;
            DisplayName = $"{ApplicationInfo.Instance.Title} {ApplicationInfo.Instance.VersionMajor}";
            Commands.Add("NewSong", RunNewSongCommand);
            Commands.Add("SaveSong", RunSaveSongCommand, CanRunSaveSongCommand);
            Commands.Add("OpenSong", RunOpenSongCommand);
            Commands.Add("CloseSong", RunCloseSongCommand, CanRunCloseSongCommand);
            Commands.Add("EditSettings", RunEditSettingsCommand);
            Commands.Add("CloseApp", (p) => WindowOwner.Close());
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunSaveSongCommand(object parm)
        {
            SongContainer.Save();
        }

        private bool CanRunSaveSongCommand(object parm)
        {
            return SongContainer != null && SongContainer.IsChanged;
        }

        private void RunNewSongCommand(object parm)
        {
            if (IsOkayToClose())
            {
                CloseSongContainer();
                CreateSong();
                SongContainer.Show();
            }
        }

        private async void RunOpenSongCommand(object parm)
        {
            if (IsOkayToClose())
            {
                CloseSongContainer();
                CreateSong();
                if (await SongContainer.Open())
                {
                    SongContainer.Show();
                }
                else
                {
                    CloseSongContainer();
                }
            }
        }

        private void RunCloseSongCommand(object parm)
        {
            if (IsOkayToClose())
            {
                CloseSongContainer();
            }
        }

        private bool CanRunCloseSongCommand(object parm)
        {
            return SongContainer != null;
        }

        private void RunEditSettingsCommand(object parm)
        {
            // TODO
        }

        private void CloseSongContainer()
        {
            if (SongContainer != null)
            {
                SongContainer.Deactivate();
                SongContainer = null;
            }
        }

        private bool IsOkayToClose()
        {
            bool isOkay = SongContainer == null || !SongContainer.IsChanged;
            if (!isOkay)
            {
                var result = MessageBox.Show($"{Strings.MessageConfirmSave} {SongContainer.Container.DisplayName}?", Strings.MessageDrumMaster, MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        isOkay = SongContainer.Save();
                        break;
                    case MessageBoxResult.No:
                        isOkay = true;
                        break;
                }
            }
            return isOkay;
        }

        private void CreateSong()
        {
            SongContainer = null;
            SongContainer = new SongContainerViewModel(this);
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                {
                    SongContainer.Activate();
                }));
        }

        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            if (IsOkayToClose())
            {
                CloseSongContainer();
            }
            else
            {
                e.Cancel = true;
            }
        }
        #endregion
    }
}