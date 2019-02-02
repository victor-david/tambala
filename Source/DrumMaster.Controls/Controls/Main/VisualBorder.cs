using Restless.App.DrumMaster.Controls.Core;
using System.Windows.Controls;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Extends Border to provide a lightweight visual. This class must be inherited.
    /// </summary>
    internal abstract class VisualBorder : Border 
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualBorder"/> class.
        /// </summary>
        /// <param name="position">The position</param>
        internal VisualBorder(int position)
        {
            Position = position;
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the position.
        /// </summary>
        public int Position { get; }
        #endregion
    }
}
