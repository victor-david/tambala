/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
//using Restless.Tambala.Core;
//using System;
//using System.Windows;

//namespace Restless.Tambala.ViewModel
//{
//    /// <summary>
//    /// Represents a view model used by a window. This class must be inherited.
//    /// </summary>
//    public abstract class WindowViewModel : ApplicationViewModel
//    {
//        #region Public properties
//        /// <summary>
//        /// Gets the window that owns this VM
//        /// </summary>
//        public Window WindowOwner
//        {
//            get;
//        }
//        #endregion

//        /************************************************************************/

//        #region Constructor
//        /// <summary>
//        /// Initializes a new instance of the <see cref="WindowViewModel"/> class.
//        /// </summary>
//        /// <param name="owner">The owner of this view model.</param>
//        public WindowViewModel(Window owner) : base(null)
//        {
//            WindowOwner = owner ?? throw new ArgumentNullException(nameof(owner));
//        }
//        #endregion
//    }
//}