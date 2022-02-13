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
            Commands.Add("NewProject", RunNewProjectCommand);
            Commands.Add("SaveProject", RunSaveProjectCommand, CanRunSaveProjectCommand);
            Commands.Add("OpenProject", RunOpenProjectCommand);
            Commands.Add("CloseProject", RunCloseProjectCommand, HaveProjectContainer);
            Commands.Add("EditSettings", RunEditSettingsCommand);
            Commands.Add("ViewAlwaysOnTop", (p)=> IsTopMost = !IsTopMost);
            Commands.Add("OpenAbout", RunOpenAboutCommand);
            Commands.Add("OpenRender", RunOpenRenderCommand, HaveProjectContainer);
            Commands.Add("ExitApp", p => Application.Current.MainWindow.Close());
            Application.Current.MainWindow.Closing += MainWindowClosing;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunSaveProjectCommand(object parm)
        {
            ProjectContainer.Save();
        }

        private bool CanRunSaveProjectCommand(object parm)
        {
            return ProjectContainer != null && ProjectContainer.IsChanged;
        }

        private void RunNewProjectCommand(object parm)
        {
            if (IsOkayToClose())
            {
                CloseProjectContainer();
                CreateProject();
                ProjectContainer.Show();
            }
        }

        private async void RunOpenProjectCommand(object parm)
        {
            if (IsOkayToClose())
            {
                CloseProjectContainer();
                CreateProject();
                if (await ProjectContainer.Open())
                {
                    ProjectContainer.Show();
                }
                else
                {
                    CloseProjectContainer();
                }
            }
        }

        private void RunCloseProjectCommand(object parm)
        {
            if (IsOkayToClose())
            {
                CloseProjectContainer();
            }
        }

        private bool HaveProjectContainer(object parm)
        {
            return ProjectContainer != null;
        }

        private void RunEditSettingsCommand(object parm)
        {
            // TODO
        }

        private void CloseProjectContainer()
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

        private void RunOpenRenderCommand(object parm)
        {
            if (ProjectContainer != null)
            {
                ProjectContainer.Container.StopPlay();
                WindowFactory.AudioRender.Create(ProjectContainer.Container).ShowDialog();
            }
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

        private void CreateProject()
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
                CloseProjectContainer();
            }
            else
            {
                e.Cancel = true;
            }
        }
        #endregion
    }
}