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
        private bool isTopMost;
        private ProjectContainerViewModel projectContainer;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if the window that is bound to this view model stays on top.
        /// </summary>
        public bool IsTopMost
        {
            get => isTopMost;
            private set => SetProperty(ref isTopMost, value);
        }

        /// <summary>
        /// Gets the project container object.
        /// </summary>
        public ProjectContainerViewModel ProjectContainer
        {
            get => projectContainer;
            private set => SetProperty(ref projectContainer, value);
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
            Commands.Add("ViewAlwaysOnTop", (p)=> IsTopMost = !IsTopMost);
            Commands.Add("About", RunOpenAboutCommand);
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
            ProjectContainer.Save();
        }

        private bool CanRunSaveSongCommand(object parm)
        {
            return ProjectContainer != null && ProjectContainer.IsChanged;
        }

        private void RunNewSongCommand(object parm)
        {
            if (IsOkayToClose())
            {
                CloseSongContainer();
                CreateSong();
                ProjectContainer.Show();
            }
        }

        private async void RunOpenSongCommand(object parm)
        {
            if (IsOkayToClose())
            {
                CloseSongContainer();
                CreateSong();
                if (await ProjectContainer.Open())
                {
                    ProjectContainer.Show();
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
            return ProjectContainer != null;
        }

        private void RunEditSettingsCommand(object parm)
        {
            // TODO
        }

        private void CloseSongContainer()
        {
            if (ProjectContainer != null)
            {
                ProjectContainer.Deactivate();
                ProjectContainer = null;
            }
        }

        private void RunOpenAboutCommand(object parm)
        {
            WindowFactory.About.Create().ShowDialog();
        }

        private bool IsOkayToClose()
        {
            bool isOkay = ProjectContainer == null || !ProjectContainer.IsChanged;
            if (!isOkay)
            {
                var result = MessageBox.Show($"{Strings.MessageConfirmSave} {ProjectContainer.Container.DisplayName}?", Strings.MessageDrumMaster, MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        isOkay = ProjectContainer.Save();
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
            ProjectContainer = null;
            ProjectContainer = new ProjectContainerViewModel(this);
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                {
                    ProjectContainer.Activate();
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