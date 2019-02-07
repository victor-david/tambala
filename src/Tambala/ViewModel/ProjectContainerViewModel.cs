/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Microsoft.Win32;
using Restless.App.Tambala.Controls;
using Restless.App.Tambala.Core;
using Restless.App.Tambala.Resources;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Restless.App.Tambala.ViewModel
{
    /// <summary>
    /// Provides interaction logic for the main drum track container.
    /// </summary>
    public class ProjectContainerViewModel : WorkspaceViewModel
    {
        #region Private
        private ProjectContainer container;
        private bool isChanged;
        private const string FileExtension = "xml";
        private const string DottedFileExtension = ".xml";
        #endregion

        /************************************************************************/

        #region Public Properties
        /// <summary>
        /// Gets the container that belongs to this layout.
        /// </summary>
        public ProjectContainer Container
        {
            get => container;
            private set => SetProperty(ref container, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if the project container is changed.
        /// </summary>
        public bool IsChanged
        {
            get => isChanged;
            private set => SetProperty(ref isChanged, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectContainerViewModel"/> class.
        /// </summary>
        /// <param name="owner">The owner of this VM.</param>
        public ProjectContainerViewModel(WorkspaceViewModel owner) : base(owner)
        {
            Container = new ProjectContainer()
            {
                Visibility = Visibility.Collapsed,
            };

            //Container.RequestRenderCommand = new RelayCommand(RunRequestRenderCommand);
            Container.IsChangedSet += ContainerIsChangedSet;
            Container.IsChangedReset += ContainerIsChangedReset;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Shows the project container control.
        /// </summary>
        public void Show()
        {
            Container.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Opens the specified file
        /// </summary>
        /// <returns>true if opened successfully; otherwise, false.</returns>
        public async Task<bool> Open()
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog()
                {
                    Title = Strings.DialogTitleOpenFile,
                    DefaultExt = DottedFileExtension,
                    Filter = $"{Strings.CaptionXmlFile} | *{DottedFileExtension}",
#if DEBUG
                    InitialDirectory = @"D:\vds\Music\Drum Patterns\Xml",
#endif
                };

                if (dialog.ShowDialog() == true)
                {
                    var result = await Container.Open(dialog.FileName);
                    if (result != null) throw result;
                    DisplayName = dialog.FileName;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ContainerIOException(Strings.MessageOpenFailure, ex);
                return false;
            }
        }

        /// <summary>
        /// Saves the project container
        /// </summary>
        /// <returns>true if saved; otherwise, false.</returns>
        public bool Save()
        {
            try
            {
                string fileName = GetFileName();
                if (!string.IsNullOrEmpty(fileName))
                {
                    Container.Save(fileName);
                    DisplayName = fileName;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ContainerIOException(Strings.MessageSaveFailure, ex);
                return false;
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the vm has been deativated.
        /// </summary>
        protected override void OnDeactivated()
        {
            container.Dispose();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void ContainerIsChangedSet(object sender, RoutedEventArgs e)
        {
            IsChanged = true;
        }

        private void ContainerIsChangedReset(object sender, RoutedEventArgs e)
        {
            IsChanged = false;
        }

        private void RunRequestRenderCommand(object parm)
        {
            try
            {
                //var window = WindowFactory.AudioRender.Create(Container);
                //window.ShowDialog();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string GetFileName()
        {
            if (!string.IsNullOrEmpty(Container.FileName))
            {
                return Container.FileName;
            }

            var dialog = new SaveFileDialog
            {
                Title = Strings.DialogTitleSaveFile,
                AddExtension = true,
                DefaultExt = DottedFileExtension,
                Filter = $"{Strings.CaptionXmlFile} | *{DottedFileExtension}",
                OverwritePrompt = true,
#if DEBUG
                InitialDirectory = @"D:\vds\Music\Drum Patterns\Xml",
#endif
            };

            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                string ext = Path.GetExtension(fileName);

                if (ext != DottedFileExtension)
                {
                    fileName = Path.ChangeExtension(fileName, FileExtension);
                }
                return fileName;
            }
            return null;
        }

        private void ContainerIOException(string mainMessage, Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(mainMessage);
            sb.AppendLine();
            sb.AppendLine(ex.Message);
            if (ex.InnerException != null)
            {
                sb.AppendLine();
                sb.AppendLine(ex.InnerException.Message);
            }
            MessageBox.Show(sb.ToString(), Strings.ApplicationName);
        }
        #endregion
    }
}