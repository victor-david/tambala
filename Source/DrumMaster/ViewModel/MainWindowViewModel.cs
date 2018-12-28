using Restless.App.DrumMaster.Controls.Core;
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
        private const int MaxWorkspace = 2;
        private const int OtherWorkspaceIndex = 2;
        private const int ContainerWorkspaceIndex = 1;
        private int layoutNumber;
        private TrackContainerViewModel TrackContainer
        {
            get => (TrackContainerViewModel)Pages[ContainerWorkspaceIndex];
            set => Pages[ContainerWorkspaceIndex] = value;
        }
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
        /// Gets the collection of workspace view models
        /// </summary>
        public WorkspaceViewModelCollection Pages
        {
            get;
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
            DisplayName = "Drum Master 3.0";
            Pages = new WorkspaceViewModelCollection(MaxWorkspace);
            Commands.Add("SaveLayout", RunSaveLayoutCommand, CanRunSaveLayoutCommand);
            Commands.Add("AddLayout", RunAddLayoutCommand);
            Commands.Add("OpenLayout", RunOpenLayoutCommand);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Closes the track container.
        /// </summary>
        /// <param name="e">Not used</param>
        public void CloseTrackContainer(CancelRoutedEventArgs e)
        {
            if (IsOkayToClose())
            {
                CloseTrackContainer();
            }
            else
            {
                e.Cancel = true;
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunSaveLayoutCommand(object parm)
        {
            TrackContainer.Save();
        }

        private bool CanRunSaveLayoutCommand(object parm)
        {
            return TrackContainer != null && TrackContainer.IsChanged;
        }

        private void RunAddLayoutCommand(object parm)
        {
            if (IsOkayToClose())
            {
                CloseTrackContainer();
                CreateLayout();
                TrackContainer.Show();
            }
        }


        private void RunOpenLayoutCommand(object parm)
        {
            if (IsOkayToClose())
            {
                CloseTrackContainer();
                CreateLayout();
                if (TrackContainer.Open())
                {
                    TrackContainer.Show();
                }
                else
                {
                    CloseTrackContainer();
                }
            }
            
            //OpenFileDialog dialog = new OpenFileDialog();
            //dialog.ShowDialog();
        }

        private void CloseTrackContainer()
        {
            if (TrackContainer != null)
            {
                TrackContainer.Deactivate();
                TrackContainer = null;
            }
        }

        private bool IsOkayToClose()
        {
            bool isOkay = TrackContainer == null || !TrackContainer.IsChanged;
            if (!isOkay)
            {
                var result = MessageBox.Show($"{Strings.MessageConfirmSave} {TrackContainer.DisplayName}?", Strings.MessageDrumMaster, MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        isOkay = TrackContainer.Save();
                        break;
                    case MessageBoxResult.No:
                        isOkay = true;
                        break;
                }
            }
            return isOkay;
        }


        private void CreateLayout()
        {
            layoutNumber++;
            TrackContainer = null;
            TrackContainer = new TrackContainerViewModel($"Pattern #{layoutNumber}", this);
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new DispatcherOperationCallback
                ((args) =>
                {
                    TrackContainer.Activate();
                    return null;
                }), null);
        }

        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            if (IsOkayToClose())
            {
                CloseTrackContainer();
            }
            else
            {
                e.Cancel = true;
            }
        }
        #endregion
    }
}
