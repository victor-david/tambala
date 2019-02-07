/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Tambala.Controls.Core;
using System;
using System.Reflection;

namespace Restless.App.Tambala.Controls.Audio
{
    /// <summary>
    /// Represents a collection of <see cref="Instrument"/> objects.
    /// </summary>
    public class InstrumentCollection : GenericList<Instrument>
    {
        #region Internal methods
        /// <summary>
        /// Loads instruments from the specified directory.
        /// </summary>
        /// <param name="directory">The directory that contains the instrument files.</param>
        internal void LoadFromDirectory(string directory)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads instruments from the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly that contains the instruments.</param>
        /// <param name="resourcePath">The resource path to the collection of instruments to load.</param>
        internal void LoadFromAssembly(Assembly assembly, string resourcePath)
        {
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.Contains(resourcePath))
                {
                    Add(new Instrument(resourceName, assembly));
                }
            }
        }
        #endregion
    }
}