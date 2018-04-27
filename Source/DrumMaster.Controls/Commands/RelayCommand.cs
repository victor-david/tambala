using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Restless.App.DrumMaster.Controls
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
        /// Gets or sets a value that determines if the command is supported.
        /// </summary>
        public CommandSupported Supported
        {
            get;
            set;
        }

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
        /// <param name="supported">A value that determines if the command is supported.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute, CommandSupported supported) 
        {
            this.execute = execute ?? throw new ArgumentNullException("execute"); 
            this.canExecute = canExecute;
            Supported = supported;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The method that executes the command.</param>
        /// <param name="canExecute">The method that checks if this command can execute. If null, no check is performed.</param>
        /// <remarks>This overload creates a command that is marked as supported.</remarks>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            :this(execute, canExecute, CommandSupported.Yes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The method that executes the command.</param>
        /// <remarks>This overload creates a command that has no corresponding command predicate and is marked as supported.</remarks>
        public RelayCommand(Action<object> execute) 
            : this(execute, null) 
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

            switch (Supported)
            {
                case CommandSupported.No:
                    return false;
                default:
                    return canExecute == null ? true : canExecute(parameter); 
            }
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
            switch (Supported)
            {
                case CommandSupported.Yes:
                    parameter = Parameter ?? parameter;
                    execute(parameter);
                    break;
                case CommandSupported.No:
                    break;
                case CommandSupported.NoWithException:
                    throw new NotSupportedException("The command is not supported.");
            }
            
        } 
        #endregion 
    }
}
