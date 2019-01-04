using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls.Automation
{
    /// <summary>
    /// Represents track automation that automates the voiced/muted portion of the track.
    /// </summary>
    public class TrackVoiceAutomation : TrackAutomation
    {
        #region Private
        private int inPass;
        private bool inPassDecision;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackVoiceAutomation"/> class.
        /// </summary>
        /// <param name="owner">The controller that owns this automation</param>
        internal TrackVoiceAutomation(TrackController owner) : base(owner)
        {
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

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Gets a boolean value that indicates if the automation allows
        /// the specified pass to play.
        /// </summary>
        /// <param name="pass">The pass</param>
        /// <returns>true if the automation allows the pass to play (or no automation applies to the pass);otherwise, false.</returns>
        internal override bool CanPlay(int pass)
        {
            if (pass == inPass) return inPassDecision;
            inPass = pass;

            Items.SetActiveItemIfStartPass(pass);

            if (Items.ActiveItem != null)
                inPassDecision = Items.ActiveItem.AutomationType == TrackAutomationType.Voice;
            else
                inPassDecision = true;

            Items.DeactivateActiveItemIfLastPass(pass);
            return inPassDecision;
        }

        internal void SetAutomationForTest()
        {
            Items.AddNextSequence(4, TrackAutomationType.Silence);
            Items.AddNextSequence(4, TrackAutomationType.Voice);
            Items.AddNextSequence(4, TrackAutomationType.Silence);
        }
        #endregion
    }
}
