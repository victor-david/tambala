using System;
using System.Diagnostics;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a single point selector
    /// </summary>
    public class PointSelector : ControlObjectSelector
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PointSelector"/> class.
        /// </summary>
        internal PointSelector(PointSelectorType type) : base(type)
        {
            if (SelectorType == PointSelectorType.SongRow)
            {
                Commands.Add("ToggleSelect", new RelayCommand((p) => IsSelected = !IsSelected));
            }
        }

        static PointSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PointSelector), new FrameworkPropertyMetadata(typeof(PointSelector)));
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
