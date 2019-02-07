/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Collections.Generic;

namespace Restless.App.Tambala.Controls.Core
{
    /// <summary>
    /// Extends List to provide additional functionality.
    /// </summary>
    /// <typeparam name="T">The type of objects for the list</typeparam>
    public class GenericList<T> : List<T>
    {
        /// <summary>
        /// Performs the specified operation on each element of the collection.
        /// </summary>
        /// <param name="action">The action</param>
        public void DoForAll(Action<T> action)
        {
            if (action != null)
            {
                foreach (T element in this)
                {
                    action(element);
                }
            }
        }
    }
}