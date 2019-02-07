/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Tambala.Controls;
using System;

namespace Restless.App.Tambala.Core
{
    /// <summary>
    /// Provides properties that are common to all view models. This class must be inherited.
    /// </summary>
    public abstract class WorkspaceViewModel : ObservableObject
    {
        #region Private
        private string displayName;
        private bool isActivated;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the view model that owns this view model, or null if none.
        /// </summary>
        public WorkspaceViewModel Owner
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the display name for this instance.
        /// </summary>
        public string DisplayName
        {
            get => displayName;
            protected set => SetProperty(ref displayName, value);
        }

        /// <summary>
        /// Gets a dictionary of commands. 
        /// </summary>
        public CommandDictionary Commands
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a boolean value that indicates if this VM is active.
        /// </summary>
        /// <remarks>
        /// The VM can be signaled via this property. When this property is set to true,
        /// the <see cref="OnActivated"/> method is called. When set to false, 
        /// the <see cref="OnDeactivated"/> method is called.
        /// </remarks>
        public bool IsActivated
        {
            get => isActivated;
            private set
            {
                if (SetProperty(ref isActivated, value))
                {
                    if (isActivated)
                        OnActivated();
                    else
                        OnDeactivated();
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Protected properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceViewModel"/> class.
        /// </summary>
        /// <param name="owner">The owner of this VM.</param>
        protected WorkspaceViewModel(WorkspaceViewModel owner)
        {
            Owner = owner;
            Commands = new CommandDictionary();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Activates the view model.
        /// </summary>
        public void Activate()
        {
            IsActivated = true;
        }

        /// <summary>
        /// Deactivates the view model.
        /// </summary>
        public void Deactivate()
        {
            IsActivated = false;
        }

        /// <summary>
        /// Toggles the view model between activated and deactivated.
        /// </summary>
        public void ToggleActivation()
        {
            if (!IsActivated)
                Activate();
            else
                Deactivate();
        }

        /// <summary>
        /// Toggles the specifed view model between activated and deactivated.
        /// </summary>
        /// <param name="vm">The view model to toggle.</param>
        public void ToggleActivation(WorkspaceViewModel vm)
        {
            if (vm != null)
            {
                vm.ToggleActivation();
                OnActivationToggled(vm);
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Gets the <see cref="Owner"/> property as the specified type
        /// </summary>
        /// <typeparam name="T">The type that derives from <see cref="WorkspaceViewModel"/>.</typeparam>
        /// <returns>The owner as the specified type, or null if owner not set or can't be cast.</returns>
        protected T GetOwner<T>() where T : WorkspaceViewModel
        {
            return Owner as T;
        }

        /// <summary>
        /// Attempts to set the passed item owner to the specified type. Throws an exception if <see cref="Owner"/> is not <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type reference.</typeparam>
        /// <param name="item">The item to set.</param>
        /// <remarks>
        /// This is a convienence method that allows the caller to set a local var to the more specific type
        /// rather than needing to call <see cref="GetOwner{T}"/>. If <see cref="Owner"/> is not type <typeparamref name="T"/>, 
        /// this method throws an exception.
        /// </remarks>
        protected void SetLocalOwner<T>(ref T item) where T: WorkspaceViewModel
        {
            item = GetOwner<T>() ?? throw new InvalidCastException();
        }

        /// <summary>
        /// Called when this view model becomes active.
        /// A derived class can override this method to perform initialization actions.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void OnActivated()
        {
        }

        /// <summary>
        /// Called when this view model becomes inactive.
        /// A derived class can override this method to perform cleanup actions.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void OnDeactivated()
        {
        }

        /// <summary>
        /// Called after <see cref="ToggleActivation(WorkspaceViewModel)"/> has toggled the activation state of the specified <see cref="WorkspaceViewModel"/>.
        /// A derived class can override this method to perform update actions.
        /// The base implementation does nothing.
        /// </summary>
        /// <param name="vm">The view model that is being toggled.</param>
        protected virtual void OnActivationToggled(WorkspaceViewModel vm)
        {
        }
        #endregion
    }
}