/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the MIT License
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.Tambala.Controls.Core
{
    /// <summary>
    /// Defines properties for controls that implement volume bias
    /// </summary>
    internal interface IVolume
    {
        /// <summary>
        /// Gets or sets the volume. This value is expressed in decibels.
        /// </summary>
        float Volume { get; set; }
    }
}