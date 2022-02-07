/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Core;
using Restless.Toolkit.Controls;
using System;
using System.Diagnostics;
using System.Windows;

namespace Restless.Tambala.ViewModel
{
    /// <summary>
    /// Represents the view model for displaying the about window.
    /// </summary>
    public class AboutWindowViewModel : ApplicationViewModel
    {
        #region Public properties
        /// <summary>
        /// Gets the application info object.
        /// </summary>
        public static ApplicationInfo AppInfo
        {
            get => ApplicationInfo.Instance;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRenderWindowViewModel"/> class.
        /// </summary>
        public AboutWindowViewModel()
        {
            DisplayName = $"About {ApplicationInfo.Instance.Title} {ApplicationInfo.Instance.VersionMajor}";
            Commands.Add("Repository", RunRepositoryCommand);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunRepositoryCommand(object parm)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = AppInfo.RepositoryUrl,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageWindow.ShowError(ex.Message);
            }
        }
        #endregion
    }
}