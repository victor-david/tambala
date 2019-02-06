/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the MIT License
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Tambala.Controls.Audio;
using Restless.App.Tambala.Core;
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
            TopLevelExceptionHandler.Initialize();
#endif
            // Validations.ThrowIfNotWindows7();
            ShutdownMode = ShutdownMode.OnMainWindowClose;
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
        #endregion
    }
}