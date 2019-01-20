using System;
using System.Collections.Generic;

namespace Restless.App.DrumMaster.Controls.Core
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
