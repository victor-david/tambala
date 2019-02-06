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
    /// Represents the arguments used for an equalizer routed event.
    /// </summary>
    public class EqualizerRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets The band
        /// </summary>
        public int Band
        {
            get;
        }

        /// <summary>
        /// Gets the gain.
        /// </summary>
        public float Gain
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualizerRoutedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event</param>
        /// <param name="band">The band that gain applies to</param>
        /// <param name="gain">The gain value</param>
        internal EqualizerRoutedEventArgs(RoutedEvent routedEvent, int band, float gain) : base(routedEvent)
        {
            Band = band;
            Gain = gain;
        }
    }

    /// <summary>
    /// Delegate for <see cref="CancelRoutedEventArgs"/>
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">The args</param>
    public delegate void EqualizerRoutedEventHandler(object sender, EqualizerRoutedEventArgs e);
}