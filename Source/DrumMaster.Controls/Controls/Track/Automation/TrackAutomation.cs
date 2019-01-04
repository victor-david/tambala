using Restless.App.DrumMaster.Controls.Core;
using System;
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
        /// Gets the controller that owns this track automation.
        /// </summary>
        protected TrackController Owner
        {
            get;
        }

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
        public TrackAutomationItemCollection Items
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackAutomation"/> class.
        /// </summary>
        /// <param name="owner">The controller that owns this automation</param>
        protected TrackAutomation(TrackController owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Items = new TrackAutomationItemCollection();
            IsEnabled = true;
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

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Applies the automation. A derived class can override this method.
        /// The base implementation does nothing.
        /// </summary>
        internal virtual void ApplyAutomation(int pass, int operationSet)
        {
            // Note: this is currently unused because automation currently
            // consists of voice automation only, which is strictly an 
            // on / off automation; it doesn't need to change any values.
            // Later, with pitch animation, we'll need to modify pitch
            // values of the owner controller.
        }

        /// <summary>
        /// Gets a boolean value that indicates if the automation allows
        /// the specified pass to play. Override this method to provide 
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        internal virtual bool CanPlay(int pass)
        {
            return true;
        }
        #endregion

    }
}
