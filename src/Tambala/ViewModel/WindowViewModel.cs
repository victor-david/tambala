using Restless.App.Tambala.Core;
using System;
using System.Windows;

namespace Restless.App.Tambala.ViewModel
{
    /// <summary>
    /// Represents a view model used by a window. This class must be inherited.
    /// </summary>
    public abstract class WindowViewModel : WorkspaceViewModel
    {
        #region Public properties
        /// <summary>
        /// Gets the window that owns this VM
        /// </summary>
        public Window WindowOwner
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowViewModel"/> class.
        /// </summary>
        /// <param name="owner">The owner of this view model.</param>
        public WindowViewModel(Window owner) : base(null)
        {
            WindowOwner = owner ?? throw new ArgumentNullException(nameof(owner));
        }
        #endregion
    }
}
