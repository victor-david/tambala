using System;
using System.Collections.ObjectModel;

namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Represents an observable collection that can only contain a specified number of items.
    /// </summary>
    public class MaxSizeObservableCollection<T> : ObservableCollection<T>
    {
        #region Public properties
        /// <summary>
        /// Gets the maximum number of items that the collection may contain.
        /// </summary>
        public int Capacity
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxSizeObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="capacity"></param>
        public MaxSizeObservableCollection(int capacity)
        {
            if (capacity < 1) throw new ArgumentException(nameof(capacity));
            Capacity = capacity;

        }
        #endregion

        /************************************************************************/

        #region Public methods
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
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Inserts the item at the specified index.
        /// Will not allow the collection to grow beyond <see cref="Capacity"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        protected override void InsertItem(int index, T item)
        {
            if (Count < Capacity)
            {
                base.InsertItem(index, item);
            }
        }
        #endregion
    }
}
