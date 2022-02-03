/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Core;
using System.Diagnostics;
using System.Windows;

namespace Restless.Tambala.ViewModel
{
    /// <summary>
    /// Represents the view model for displaying the about window.
    /// </summary>
    public class AboutWindowViewModel : ApplicationViewModel
    {
        #region Private
        #endregion

        /************************************************************************/

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
            Commands.Add("ImageCredit", RunImageCreditCommand);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunImageCreditCommand(object parm)
        {
            if (parm is string user)
            {
                string url = $"https://www.flaticon.com/authors/{user}";
                Process.Start(url);
            }
        }
        #endregion
    }
}