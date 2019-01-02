using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls.Automation
{
    /// <summary>
    /// Represents the base class for track automation operations.
    /// This class must be inherited.
    /// </summary>
    public abstract class TrackAutomation : IXElement
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets a value that indicates if the automation is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the dictionary of automation items.
        /// </summary>
        public Dictionary<int, TrackAutomationItem> Items
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackAutomation"/> class.
        /// </summary>
        protected TrackAutomation()
        {
            Items = new Dictionary<int, TrackAutomationItem>();
        }
        #endregion

        /************************************************************************/

        #region IXElement 
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public abstract XElement GetXElement();
        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public abstract void RestoreFromXElement(XElement element);
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        #region Internal methods
        internal virtual bool IsAllowedToPlay(int pass)
        {
            return true;
        }
        #endregion

    }
}
