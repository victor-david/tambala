/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Controls.Audio;
using Restless.Tambala.Core;
using System;
using System.Windows;

namespace Restless.App.Tambala
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
                Toolkit.Controls.MessageWindow.ShowError(ex.Message, null, false);
                Environment.Exit(0);
            }
#else
            RunApplication(e);
#endif
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
            TopLevelExceptionHandler.Initialize();
#endif
            // Validations.ThrowIfNotWindows7();
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            Dispatcher.ShutdownFinished += DispatcherShutdownFinished;
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

        /// <summary>
        /// Called when the dispatcher has completed its shutdown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// This event runs after the dispatcher has finished shutting down in order 
        /// to shutdown the audio host. Cannot shutdown audio host in the OnExit() method 
        /// because that runs before the dispatcher shuts down. 
        /// 
        /// In that case, if a project container is open, it attempts to shutdown AFTER
        /// the audio host, which throws exceptions when destroying audio voices and the voice pools.
        /// 
        /// Project container runs a shutdown when it is explictly closed. If the main window is closed
        /// while a project container is still open, it handles Dispatcher.ShutdownStarted to perform
        /// the shutdown cleanup. Then, this method is called to finalize the shutdown of the audio host.
        /// </remarks>
        private void DispatcherShutdownFinished(object sender, EventArgs e)
        {
            AudioHost.Instance.Shutdown();
        }
        #endregion
    }
}