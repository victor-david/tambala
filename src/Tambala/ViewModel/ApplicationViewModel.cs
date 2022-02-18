/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Controls;
using Restless.Toolkit.Mvvm;
using System;

namespace Restless.Tambala.ViewModel
{
    /// <summary>
    /// Provides properties that are common to all view models. This class must be inherited.
    /// </summary>
    public abstract class ApplicationViewModel : ViewModelBase
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        #endregion

        /************************************************************************/

        #region Protected properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationViewModel"/> class.
        /// </summary>
        /// <param name="owner">The owner of this VM.</param>
        protected ApplicationViewModel() : base()
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion
    }
}