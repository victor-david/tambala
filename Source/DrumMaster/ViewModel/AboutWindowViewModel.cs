using Microsoft.Win32;
using Restless.App.DrumMaster.Controls;
using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Core;
using Restless.App.DrumMaster.Resources;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Restless.App.DrumMaster.ViewModel
{
    /// <summary>
    /// Represents the view model for displaying the about window.
    /// </summary>
    public class AboutWindowViewModel : WindowViewModel
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the application info object.
        /// </summary>
        public ApplicationInfo AppInfo
        {
            get => ApplicationInfo.Instance;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRenderWindowViewModel"/> class.
        /// </summary>
        /// <param name="owner">The owner of this view model.</param>
        public AboutWindowViewModel(Window owner) : base(owner)
        {
            DisplayName = $"About {ApplicationInfo.Instance.Title} {ApplicationInfo.Instance.VersionMajor}";
            Commands.Add("Close", (p)=>WindowOwner.Close());
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
