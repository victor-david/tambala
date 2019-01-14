using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Core;
using System;
using System.Reflection;
using System.Windows;

namespace Restless.App.DrumMaster
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region Protected methods
        /// <summary>
        /// Called when the application is starting.
        /// </summary>
        /// <param name="e">The starup event args.</param>
        /// <remarks>
        /// This method sets up the audio drivers and creates the main application window. 
        /// </remarks>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
#if !DEBUG
            try
            {
                RunApplication(e);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
#else
            RunApplication(e);
#endif
        }

        /// <summary>
        /// Called when the application is exiting.
        /// </summary>
        /// <param name="e">The exit event args</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            AudioHost.Instance.Shutdown();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        /// <summary>
        /// Called from OnStartup(e) separately so we can catch an assembly missing.
        /// OnStartup() runs, then the runtime does a JIT for this method which needs other assemblies.
        /// If something is missing, the try/catch in OnStartup() handles it gracefully.
        /// </summary>
        /// <param name="e">The same parameter passed to OnStartup(e)</param>
        private void RunApplication(StartupEventArgs e)
        {
#if !DEBUG
            //TopLevelExceptionHandler.Initialize();
#endif
            // Validations.ThrowIfNotWindows7();
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            // InitializeAudioHost();
            AudioHost.Instance.Initialize();
            Window main = WindowFactory.Main.Create();
            main.MinWidth = 960;
            main.MinHeight = 600;
            main.Width = 1600;
            main.Height = 900;
            main.WindowStartupLocation = WindowStartupLocation.CenterScreen;
#if !DEBUG
            main.WindowState = WindowState.Maximized;
#endif
            main.Show();
        }

        //private void InitializeAudioHost()
        //{
        //    AudioHost.Instance.Initialize();
        //    //var a = Assembly.GetExecutingAssembly();

        //    //foreach (string resourceName in a.GetManifestResourceNames())
        //    //{
        //    //    if (resourceName.Contains("Resources.Audio."))
        //    //    {
        //    //        AudioHost.Instance.AddPieceFromAssembly(resourceName, a);
        //    //    }
        //    //}
        //}
#endregion
    }
}
