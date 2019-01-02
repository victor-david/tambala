using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls.Automation
{
    /// <summary>
    /// Represents track automation that automates the voiced/muted portion of the track.
    /// </summary>
    public class TrackVoiceAutomation : TrackAutomation
    {
        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackVoiceAutomation"/> class.
        /// </summary>
        internal TrackVoiceAutomation()
        {
            Items.Add(4, new TrackAutomationItem());
        }
        #endregion

        /************************************************************************/

        #region IXElement 
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public override XElement GetXElement()
        {
            var element = new XElement(nameof(TrackVoiceAutomation));
            element.Add(new XElement(nameof(IsEnabled), IsEnabled));
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
        }
        #endregion

        internal override bool IsAllowedToPlay(int pass)
        {
            return pass >=4 && pass < 8;

        }
    }
}
