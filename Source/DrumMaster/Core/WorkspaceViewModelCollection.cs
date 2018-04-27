using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Core
{
    public class WorkspaceViewModelCollection : FixedSizeObservableCollection<WorkspaceViewModel>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceViewModelCollection"/> class.
        /// </summary>
        /// <param name="capacity">The capacity</param>
        public WorkspaceViewModelCollection(int capacity) : base(capacity)
        {
        }
        #endregion
    }
}
