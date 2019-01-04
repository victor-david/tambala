using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls.Automation
{
    /// <summary>
    /// Represents a track automation item
    /// </summary>
    public class TrackAutomationItem : IXElement
    {
        #region Private
        #endregion
        
        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the automation type
        /// </summary>
        public TrackAutomationType AutomationType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the pass number when this automation item begins.
        /// </summary>
        public int FirstPass
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of passes that this automation item lasts.
        /// </summary>
        public int Duration
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackAutomationItem"/> class.
        /// </summary>
        /// <param name="firstPass">The first pass.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="automationType">the type of automation.</param>
        internal TrackAutomationItem(int firstPass, int duration, TrackAutomationType automationType)
        {
            if (firstPass < 1) throw new ArgumentException($"{nameof(FirstPass)} must be one or greater");
            if (duration < 1) throw new ArgumentException($"{nameof(Duration)} must be one or greater");
            FirstPass = firstPass;
            Duration = duration;
            AutomationType = automationType;
        }
        #endregion


        /************************************************************************/

        #region IXElement
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public XElement GetXElement()
        {
            var element = new XElement(nameof(TrackAutomationItem));
            element.Add(new XElement(nameof(AutomationType), AutomationType));
            element.Add(new XElement(nameof(FirstPass), FirstPass));
            element.Add(new XElement(nameof(Duration), Duration));
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public void RestoreFromXElement(XElement element)
        {
            IEnumerable<XElement> childList = from el in element.Elements() select el;

            foreach (XElement e in childList)
            {
                if (e.Name == nameof(AutomationType))
                {
                    if (Enum.TryParse(e.Value, out TrackAutomationType result)) AutomationType = result;
                }

                if (e.Name == nameof(FirstPass))
                {
                    if (int.TryParse(e.Value, out int result)) FirstPass = result;
                }

                if (e.Name == nameof(Duration))
                {
                    if (int.TryParse(e.Value, out int result)) Duration = result;
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets a string representation of this item.
        /// </summary>
        /// <returns>A string that describes the item</returns>
        public override string ToString()
        {
            return $"TrackAutomationItem. Type: {AutomationType} Start: {FirstPass} Duration: {Duration}";
        }
        #endregion
    }
}
