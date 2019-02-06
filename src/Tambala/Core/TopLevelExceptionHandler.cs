/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the MIT License
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Tambala.Resources;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Restless.App.Tambala.Core
{
    /// <summary>
    /// Represents a top level exception handler.
    /// </summary>
    public sealed class TopLevelExceptionHandler
    {
        #region Private fields
        private static TopLevelExceptionHandler instance;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes the TopLevelExceptionHandler object.
        /// </summary>
        /// <remarks>
        /// By calling this static method, any exception that is not handled
        /// in another portion of code is handled by this object. Unhandled
        /// exceptions are logged to the application's directory.
        /// </remarks>
        public static void Initialize()
        {
            if (instance == null)
            {
                instance = new TopLevelExceptionHandler();
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TopLevelExceptionHandler"/> class.
        /// </summary>
        private TopLevelExceptionHandler()
        {
            //AppDomain.CurrentDomain.FirstChanceException += CurrentDomainFirstChanceException;
            Application.Current.DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
        }
        #endregion

        /************************************************************************/

        #region Private methods (event handlers)
        /// <summary>
        /// Handles a first chance exception. If wired, this method is called before the CLR searches for possible exception handlers.
        /// For instance, this method will be called before the catch() of a try / catch.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments.</param>
        private void CurrentDomainFirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
        }

        /// <summary>
        /// Handles an otherwise unhandled exception.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args.</param>
        private void AppDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            HandleExceptionFully("AppDispatcher", e.Exception, true);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Handles an otherwise unhandled exception in the current domain.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args.</param>
        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                HandleExceptionFully("CurrentDomain", ex, e.IsTerminating);
            }
            else
            {
                MessageBox.Show(Strings.UnhandledExceptionCurrentDomain, Strings.UnhandledExceptionCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (e.IsTerminating)
            {
                // nothing for now.
            }
        }

        
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
        }

        private void HandleExceptionFully(string caller, Exception ex, bool isTerminating)
        {
            string fileName = GetExceptionLogFileName();
            string logFileMsg = $"Details in {fileName}";
            try
            {
                LogException(caller, ex, fileName);
            }
            catch (Exception logException)
            {
                logFileMsg = $"Could not write to log file ({logException.Message})";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Strings.UnhandledExceptionMessageHeader);
            sb.AppendLine(ex.Message);
            sb.AppendLine();
            sb.AppendLine(logFileMsg);
            sb.AppendLine();
            if (isTerminating)
            {
                sb.AppendLine(Strings.UnhandledExceptionMessageFooter);
            }
            MessageBox.Show(sb.ToString(), Strings.UnhandledExceptionCaption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private string GetExceptionLogFileName()
        {
            string dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return Path.Combine(dir, "exception.log");
        }

        private void LogException(string caller, Exception ex, string fileName)
        {
            StringBuilder sb = new StringBuilder();
            string header = $"{caller} unhandled exception: {DateTime.Now} local time";
            sb.AppendLine(header);
            sb.AppendLine(string.Empty.PadLeft(header.Length, '='));
            sb.Append(GetExceptionMessage(ex, 0));
            sb.AppendLine("Stack trace:");
            sb.AppendLine(ex.StackTrace);
            sb.AppendLine("===END");
            sb.AppendLine();
            File.AppendAllText(fileName, sb.ToString());
        }

        /// <summary>
        /// Recursively gets messages from all nested exceptions.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="level">The nesting level.</param>
        /// <returns>The exception message(s)</returns>
        private string GetExceptionMessage(Exception ex, int level)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Level {level} => {ex.GetType().FullName}");
            sb.AppendLine(ex.Message);
            sb.AppendLine();
            if (ex.InnerException != null)
            {
                sb.Append(GetExceptionMessage(ex.InnerException, level+1));
            }
            return sb.ToString();
        }
        #endregion

    }

}