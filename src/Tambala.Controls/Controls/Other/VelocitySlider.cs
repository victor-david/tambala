using Restless.App.Tambala.Controls.Core;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.App.Tambala.Controls
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
            Background = new SolidColorBrush(GetBlendedColor(50));
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

        #region Protected methods
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
            AdjustBackGroundToValue(newValue);
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

        #region Private methods (Colors)
        private readonly Color highColor = Colors.Red;
        private readonly Color midColor = Colors.Green;
        private readonly Color lowColor = Colors.DarkGray;

        private void AdjustBackGroundToValue(double value)
        {
            double range = Maximum - Minimum;
            double diff = Math.Abs(Minimum) + value;
            int percentage = (int)Math.Round(diff / range * 100);
            Background = new SolidColorBrush(GetBlendedColor(percentage));
        }

        /// <summary>
        /// Blends colors according to a percentage.
        /// </summary>
        /// <param name="percentage">The percentage, expressed as an integer between 0-100</param>
        /// <returns>A blended color</returns>
        /// <remarks>
        /// See: https://stackoverflow.com/questions/6394304/algorithm-how-do-i-fade-from-red-to-green-via-yellow-using-rgb-values/7947812 
        /// </remarks>
        private Color GetBlendedColor(int percentage)
        {
            if (percentage < 50)
            {
                return Interpolate(lowColor, midColor, percentage / 50.0);
            }
            return Interpolate(midColor, highColor, (percentage - 50) / 50.0);
        }

        private Color Interpolate(Color color1, Color color2, double fraction)
        {
            double r = Interpolate(color1.R, color2.R, fraction);
            double g = Interpolate(color1.G, color2.G, fraction);
            double b = Interpolate(color1.B, color2.B, fraction);
            return Color.FromArgb(255, (byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b));
        }

        private double Interpolate(double d1, double d2, double fraction)
        {
            return d1 + (d2 - d1) * fraction;
        }
        #endregion
    }
}
