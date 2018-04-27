using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Core
{
    /// <summary>
    /// Represents a fixed size collection of <see cref="GridValue"/> objects.
    /// </summary>
    public class GridValueCollection : FixedSizeObservableCollection<GridValue>
    {
        #region Private
        private WorkspaceViewModelCollection workspaces;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridValueCollection"/> class.
        /// </summary>
        /// <param name="workspaces"></param>
        /// <param name="capacity"></param>
        public GridValueCollection(WorkspaceViewModelCollection workspaces, int capacity) : base(capacity)
        {
            this.workspaces = workspaces ?? throw new ArgumentNullException(nameof(workspaces));
            if (workspaces.Capacity != Capacity) throw new ArgumentException("Capacity mismatch");
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Adjusts the grid values according to the current state of the workspaces.
        /// </summary>
        public void Adjust()
        {
            int liveNumber = 1;
            int totalLive = workspaces.NonDefaultCount;
            for (int index = 0; index < Capacity; index++)
            {
                var l = workspaces[index];
                if (l != null)
                {
                    this[index].Row = GetRow(liveNumber, totalLive);
                    this[index].Col = GetCol(liveNumber, totalLive);
                    this[index].RowSpan = GetRowSpan(liveNumber, totalLive);
                    this[index].ColSpan = GetColSpan(liveNumber, totalLive);
                    liveNumber++;
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private int GetRow(int liveNumber, int totalLive)
        {
            switch (totalLive)
            {
                case 1:
                    return 0;
                case 2:
                    return (liveNumber == 1) ? 0 : 1;
                case 3:
                case 4:
                    return (liveNumber == 1 || liveNumber == 3) ? 0 : 1;
            }
            return 0;
        }

        private int GetCol(int liveNumber, int totalLive)
        {
            switch (totalLive)
            {
                case 1:
                case 2:
                    return 0;
                case 3:
                case 4:
                    return (liveNumber < 3) ? 0 : 1;
            }
            return 0;
        }

        private int GetRowSpan(int liveNumber, int totalLive)
        {
            switch (totalLive)
            {
                case 1:
                    return 2;
                case 2:
                case 4:
                    return 1;
                case 3:
                    return (liveNumber == 3) ? 2 : 1;

            }
            return 0;
        }

        private int GetColSpan(int liveNumber, int totalLive)
        {
            switch (totalLive)
            {
                case 1:
                case 2:
                    return 2;
                case 3:
                case 4:
                    return 1;
            }
            return 1;
        }
        #endregion
    }
}
