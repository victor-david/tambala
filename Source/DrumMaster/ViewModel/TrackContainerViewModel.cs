using Microsoft.Win32;
using Restless.App.DrumMaster.Controls;
using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using Restless.App.DrumMaster.Core;
using Restless.App.DrumMaster.Resources;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace Restless.App.DrumMaster.ViewModel
{
    /// <summary>
    /// Provides interaction logic for the main drum track container.
    /// </summary>
    public class TrackContainerViewModel : WorkspaceViewModel
    {
        #region Private
        private TrackContainer container;
        private bool isChanged;
        private const string FileExtension = "xml";
        private const string DottedFileExtension = ".xml";
        #endregion

        /************************************************************************/

        #region Public Properties
        /// <summary>
        /// Gets the container that belongs to this layout.
        /// </summary>
        public TrackContainer Container
        {
            get => container;
            private set => SetProperty(ref container, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if the track container is changed.
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
        /// Initializes a new instance of the <see cref="TrackContainerViewModel"/> class.
        /// </summary>
        /// <param name="displayName">The display name for the track container</param>
        /// <param name="owner">The owner of this VM.</param>
        public TrackContainerViewModel(string displayName, WorkspaceViewModel owner) : base(owner)
        {
            DisplayName = displayName;
            Container = new TrackContainer()
            {
                DisplayName = displayName,
                Visibility = Visibility.Collapsed,
            };

            Container.RequestRenderCommand = new RelayCommand(RunRequestRenderCommand);
            Container.Closing += ContainerClosing;
            Container.IsChangedSet += ContainerIsChangedSet;
            Container.IsChangedReset += ContainerIsChangedReset;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Shows the track container control.
        /// </summary>
        public void Show()
        {
            Container.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Opens the specified file
        /// </summary>
        /// <returns>true if opened successfully; otherwise, false.</returns>
        public bool Open()
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
                    Container.Open(dialog.FileName);
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
        /// Saves the track container
        /// </summary>
        /// <returns>true if saved; otherwise, false.</returns>
        public bool Save()
        {
            try
            {
                string filename = GetFileName();
                if (!string.IsNullOrEmpty(filename))
                {
                    Container.Save(filename);
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
            container.Shutdown();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void ContainerClosing(object sender, CancelRoutedEventArgs e)
        {
            var main = GetOwner<MainWindowViewModel>();
            if (main != null)
            {
                main.CloseTrackContainer(e);
            }
        }

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
                var window = WindowFactory.AudioRender.Create(Container);
                window.ShowDialog();
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
            MessageBox.Show(sb.ToString(), Strings.MessageDrumMaster);
        }
        #endregion
    }
}
