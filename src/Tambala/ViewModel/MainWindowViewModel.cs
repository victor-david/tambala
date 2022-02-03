/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Core;
using Restless.Tambala.Resources;
using Restless.Toolkit.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Restless.Tambala.ViewModel
{
    /// <summary>
    /// Represents the view model for the main window.
    /// </summary>
    public class MainWindowViewModel : ApplicationViewModel
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
        public MainWindowViewModel()
        {
            DisplayName = $"{ApplicationInfo.Instance.Title} {ApplicationInfo.Instance.VersionMajor}";
            Commands.Add("NewSong", RunNewSongCommand);
            Commands.Add("SaveSong", RunSaveSongCommand, CanRunSaveSongCommand);
            Commands.Add("OpenSong", RunOpenSongCommand);
            Commands.Add("CloseSong", RunCloseSongCommand, CanRunCloseSongCommand);
            Commands.Add("EditSettings", RunEditSettingsCommand);
            Commands.Add("ViewAlwaysOnTop", (p)=> IsTopMost = !IsTopMost);
            Commands.Add("About", RunOpenAboutCommand);
            Commands.Add("ExitApp", p => Application.Current.MainWindow.Close());
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
                if (MessageWindow.ShowYesNo($"{Strings.MessageConfirmSave} {ProjectContainer.Container.DisplayName}?"))
                {
                    isOkay = ProjectContainer.Save();
                }
                else
                {
                    isOkay = true;
                }
            }
            return isOkay;
        }

        private void CreateSong()
        {
            ProjectContainer = null;
            ProjectContainer = new ProjectContainerViewModel();
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