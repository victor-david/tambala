namespace Restless.App.DrumMaster.Core
{
    /// <summary>
    /// Represents a collection of <see cref="WorkspaceViewModel"/> objects.
    /// </summary>
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
