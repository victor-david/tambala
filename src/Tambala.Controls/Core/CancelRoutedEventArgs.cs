/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the MIT License
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Windows;

namespace Restless.App.Tambala.Controls.Core
{
    /// <summary>
    /// Represents the arguments used for a routed event that can ve canceled.
    /// </summary>
    public class CancelRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets or sets a value that indicates if the event should be canceled.
        /// </summary>
        public bool Cancel
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelRoutedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event</param>
        internal CancelRoutedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {

        }
    }

    /// <summary>
    /// Delegate for <see cref="CancelRoutedEventArgs"/>
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">The args</param>
    public delegate void CancelRoutedEventHandler(object sender, CancelRoutedEventArgs e);
}