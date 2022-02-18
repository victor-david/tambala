/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Windows;
using System.Windows.Controls;

namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Represents a volume meter.
    /// </summary>
    public class VolumeMeter : ProgressBar
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeMeter"/> class.
        /// </summary>
        public VolumeMeter()
        {
        }

        static VolumeMeter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VolumeMeter), new FrameworkPropertyMetadata(typeof(VolumeMeter)));
        }
        #endregion
    }
}