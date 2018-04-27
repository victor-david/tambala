using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Restless.App.DrumMaster.Core
{
    /// <summary>
    /// Represents an Observable collection that is used at a fixed capacity.
    /// </summary>
    /// <typeparam name="T">The data type that the collection holds.</typeparam>
    public class FixedSizeObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Gets the capacity that was used to create the collection.
        /// </summary>
        public int Capacity
        {
            get;
        }

        /// <summary>
        /// Gets a count of items that aren't set to the default of T
        /// </summary>
        public int NonDefaultCount
        {
            get => this.Count((item) => !EqualityComparer<T>.Default.Equals(item, default(T)));
        }

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedSizeObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity for the collection.</param>
        public FixedSizeObservableCollection(int capacity) : base()
        {
            if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity));
            Capacity = capacity;
            for (int index = 0; index < capacity; index++)
            {
                Add(default(T));
            }
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when an attempt is made to add an item. Used to enforce <see cref="Capacity"/>.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="item">The item.</param>
        protected override void InsertItem(int index, T item)
        {
            if (Count < Capacity)
            {
                base.InsertItem(index, item);
            }
            else
            {
                int indexOfDefault = IndexOf(default(T));
                if (indexOfDefault != -1)
                {
                    SetItem(indexOfDefault, item);
                }
            }
        }

        /// <summary>
        /// Called when items are cleared. 
        /// This method does not clear the items from the collection, but rather sets each item to the default of T.
        /// </summary>
        protected override void ClearItems()
        {
            for (int index = 0; index < Capacity; index++)
            {
                SetItem(index, default(T));
            }
        }

        /// <summary>
        /// Called when an item is requested to be removed.
        /// This method does not remove the item from the collection, but rather sets the item at <paramref name="index"/> to the default of T.
        /// </summary>
        protected override void RemoveItem(int index)
        {
            SetItem(index, default(T));
        }
        #endregion

    }
}
