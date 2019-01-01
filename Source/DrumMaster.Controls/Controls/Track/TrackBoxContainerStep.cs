using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a container for a series of <see cref="TrackBox"/> items
    /// that represent track steps.
    /// </summary>
    //[TemplatePart(Name = PartHostGrid, Type = typeof(Grid))]
    public class TrackBoxContainerStep : TrackBoxContainerBase
    {
        #region Private
        //private const string PartHostGrid = "PART_HOST_GRID";
        //private Grid hostGrid;
        private readonly CompositeTrack owner;
        //private readonly TrackContainer trackContainerOwner;
        //private TrackController controller;
        private XElement holdElement;
        #endregion

        /************************************************************************/

        #region Public properties (Type / Steps)
        #endregion

        /************************************************************************/

        #region Public properties (Brushes)
        #endregion

        /************************************************************************/

        #region Routed Events
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackBoxContainerStep"/> class.
        /// </summary>
        internal TrackBoxContainerStep(CompositeTrack owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            BoxType = TrackBoxType.TrackStep;
        }

        static TrackBoxContainerStep()
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        ///// <summary>
        ///// Called when the template is applied.
        ///// </summary>
        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //    hostGrid = GetTemplateChild(PartHostGrid) as Grid;
        //    OnTotalStepsChanged();
        //    if (holdElement != null)
        //    {
        //        RestoreFromXElement(holdElement);
        //    }
        //}
        #endregion

        /************************************************************************/

        #region IXElement
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public override XElement GetXElement()
        {
            var element = new XElement(nameof(TrackBoxContainerStep));
            foreach (var box in Boxes)
            {
                element.Add(box.GetXElement());
            }
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            holdElement = element;
            if (IsTemplateApplied)
            {
                IEnumerable<XElement> childList = from el in element.Elements() select el;
                int boxIndex = 0;
                foreach (XElement e in childList)
                {
                    if (e.Name == nameof(TrackBox))
                    {
                        if (boxIndex < Boxes.Count)
                        {
                            Boxes[boxIndex].RestoreFromXElement(e);
                        }
                        boxIndex++;
                    }
                }
                ResetIsChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the <see cref="TrackStepControl.TotalSteps"/> property changes.
        /// Ensures that the step volume control is visible for any newly added boxes.
        /// </summary>
        protected override void OnTotalStepsChanged()
        {
            // The base method adjusts the visual grid
            base.OnTotalStepsChanged();
            Boxes.SetVolumeVisibility(owner.Controller.IsTrackBoxVolumeVisible);
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        #endregion

        /************************************************************************/

        #region Private methods (Instance)
        #endregion
    }
}
