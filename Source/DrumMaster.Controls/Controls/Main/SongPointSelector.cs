using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a single song box selector
    /// </summary>
    public class SongPointSelector : PatternSelector
    {
        #region Private
        private readonly SongPatternSelector owner;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SongPointSelector"/> class.
        /// </summary>
        internal SongPointSelector(SongPatternSelector owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            if (owner.SelectorType == SongPatternSelectorType.Standard)
            {
                Commands.Add("ToggleSelect", new RelayCommand((p) => IsSelected = !IsSelected));
            }
        }

        static SongPointSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SongPointSelector), new FrameworkPropertyMetadata(typeof(SongPointSelector)));
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Called when the IsSelected property changes.
        /// </summary>
        protected override void OnIsSelectedChanged()
        {
            SetIsChanged();
            Debug.WriteLine($"Pattern {owner.DisplayName} Position {Position}");
        }
        #endregion
    }
}
