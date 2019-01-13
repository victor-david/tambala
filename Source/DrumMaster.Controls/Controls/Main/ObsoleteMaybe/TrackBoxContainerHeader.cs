using System;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls.Obsolete
{
    /// <summary>
    /// Represents a container for a series of <see cref="TrackBox"/> items
    /// that represent the header for a series of tracks.
    /// </summary>
    public class TrackBoxContainerHeader : TrackBoxContainerBase
    {
        #region Private
        private readonly TrackContainer owner;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackBoxContainerHeader"/> class.
        /// </summary>
        internal TrackBoxContainerHeader(TrackContainer owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            BoxType = TrackBoxType.Header;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region IXElement
        /// <summary>
        /// This method is not supported for this type. It always throws an exception.
        /// </summary>
        /// <returns>This method is not supported for this type. It always throws an exception.</returns>
        public override XElement GetXElement()
        {
            throw new InvalidOperationException($"{nameof(TrackBoxContainerHeader)} does not support this operation");
        }

        /// <summary>
        /// This method is not supported for this type. It always throws an exception.
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            throw new InvalidOperationException($"{nameof(TrackBoxContainerHeader)} does not support this operation");
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the <see cref="TrackStepControl.TotalSteps"/> property changes.
        /// Places the beat markers in the header.
        /// </summary>
        protected override void OnTotalStepsChanged()
        {
            // The base method adjusts the visual grid
            base.OnTotalStepsChanged();
            // Update the beat labels
            int beat = 1;
            for (int k = 0; k < Boxes.Count; k++)
            {
                Boxes[k].Text = (k % StepsPerBeat == 0) ? $"{beat++}" : string.Empty;
            }
        }
        #endregion
    }
}
