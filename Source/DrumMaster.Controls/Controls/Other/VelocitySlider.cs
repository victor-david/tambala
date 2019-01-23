using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Extends Slider to provide additional properties to control velocity.
    /// </summary>
    public class VelocitySlider : Slider, ISelectorUnit
    {
        #region Private
        private PointSelector selector;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="VelocitySlider"/> class.
        /// </summary>
        internal VelocitySlider()
        {
        }

        static VelocitySlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VelocitySlider), new FrameworkPropertyMetadata(typeof(VelocitySlider)));
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Gets or sets the selector unit.
        /// </summary>
        public PointSelectorUnit SelectorUnit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="PointSelector"/> associated with this velocity slider.
        /// </summary>
        internal PointSelector Selector
        {
            get => selector;
            set
            {
                selector = value ?? throw new ArgumentNullException(nameof(Selector));
                Value = selector.Volume;
            }
        }
        #endregion
        
        /************************************************************************/

        #region Proptected methods
        /// <summary>
        /// Called when the value changes.
        /// </summary>
        /// <param name="oldValue">The old value, not used.</param>
        /// <param name="newValue">The new value. Updates <see cref="Selector"/></param>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            if (Selector != null)
            {
                Selector.Volume = (float)newValue;
            }
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string that describes this object.</returns>
        public override string ToString()
        {
            return $"{nameof(VelocitySlider)} SelectorUnit: {SelectorUnit}";
        }
        #endregion
    }
}
