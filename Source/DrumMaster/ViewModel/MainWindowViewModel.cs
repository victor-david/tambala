using Restless.App.DrumMaster.Controls.Core;
using Restless.App.DrumMaster.Core;
using Restless.App.DrumMaster.Resources;
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
        private int songNumber;
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
            Commands.Add("EditSettings", RunEditSettingsCommand);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Closes the track container.
        /// </summary>
        /// <param name="e">The event args</param>
        public void CloseSongContainer(CancelRoutedEventArgs e)
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

        private void RunOpenSongCommand(object parm)
        {
            if (IsOkayToClose())
            {
                CloseSongContainer();
                CreateSong();
                if (SongContainer.Open())
                {
                    SongContainer.Show();
                }
                else
                {
                    CloseSongContainer();
                }
            }
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
            songNumber++;
            SongContainer = null;
            SongContainer = new SongContainerViewModel($"Song #{songNumber}", this);
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new DispatcherOperationCallback
                ((args) =>
                {
                    SongContainer.Activate();
                    return null;
                }), null);
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