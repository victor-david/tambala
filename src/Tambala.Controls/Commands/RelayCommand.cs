/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Restless.App.Tambala.Controls
{
    /// <summary>
    /// Represents a command. This is the class from which all all application commands are created.
    /// </summary>
    public class RelayCommand : ICommand 
    { 
        #region Private 
        private Action<object> execute; 
        private Predicate<object> canExecute;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets a parameter that is associated with this command.
        /// </summary>
        public object Parameter
        {
            get;
            set;
        }
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The method that executes the command.</param>
        /// <param name="canExecute">The method that checks if this command can execute. If null, no check is performed.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
          {
            this.execute = execute ?? throw new ArgumentNullException("execute"); 
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The method that executes the command.</param>
        /// <remarks>This overload creates a command that has no corresponding command predicate.</remarks>
        public RelayCommand(Action<object> execute) : this(execute, null) 
        { 
        } 
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/
        
        #region ICommand Members
        /// <summary>
        /// Checks to see if this command can execute.
        /// </summary>
        /// <param name="parameter">An object to pass to the command evaulator method.</param>
        /// <returns>true if the command can excecute; otherwise, false.</returns>
        [DebuggerStepThrough] 
        public bool CanExecute(object parameter) 
        {
            parameter = Parameter ?? parameter;
            return canExecute == null ? true : canExecute(parameter); 
        } 
        
        /// <summary>
        /// Occurs when the conditions that affect whether a command may excute change.
        /// </summary>
        public event EventHandler CanExecuteChanged 
        { 
            add 
            { 
                CommandManager.RequerySuggested += value; 
            } 
            remove 
            { 
                CommandManager.RequerySuggested -= value; 
            } 
        } 
        
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">An object to pass to the command method.</param>
        public void Execute(object parameter) 
        {
            parameter = Parameter ?? parameter;
            execute(parameter);
        } 
        #endregion 
    }
}