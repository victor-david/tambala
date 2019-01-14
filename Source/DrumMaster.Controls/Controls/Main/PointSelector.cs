using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Diagnostics;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a single point selector
    /// </summary>
    internal class PointSelector : ControlObjectSelector, ISelectorUnit
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PointSelector"/> class.
        /// </summary>
        public PointSelector()
        {
        }

        static PointSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PointSelector), new FrameworkPropertyMetadata(typeof(PointSelector)));
        }
        #endregion

        /************************************************************************/

        #region ISelectorUnit
        /// <summary>
        /// Gets or sets the selector unit.
        /// </summary>
        public PointSelectorUnit SelectorUnit
        {
            get;
            set;
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
            var element = new XElement(nameof(PointSelector));
            element.Add(new XAttribute(nameof(Position), Position));
            element.Add(new XAttribute(nameof(IsSelected), IsSelected));
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

        #region Public methods
        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(PointSelector)} Unit:{SelectorUnit} Position:{ThreadSafePosition}";
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when <see cref="ControlObjectSelector.Position"/> changes.
        /// </summary>
        protected override void OnPositionChanged()
        {
            if (SelectorType == PointSelectorType.SongHeader)
            {
                DisplayName = $"{Position}";
            }
        }

        /// <summary>
        /// Called when <see cref="ControlObject.IsSelected"/> changes.
        /// </summary>
        protected override void OnIsSelectedChanged()
        {
            SetIsChanged();
            Debug.WriteLine($"{DisplayName} Position {Position}");
        }
        #endregion
    }
}
