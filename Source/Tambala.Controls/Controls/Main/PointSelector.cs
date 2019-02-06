using Restless.App.Tambala.Controls.Core;
using SharpDX.XAudio2;
using System;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.Tambala.Controls
{
    /// <summary>
    /// Represents a single point selector
    /// </summary>
    internal class PointSelector : ControlObjectSelector, ISelectorUnit, IVolume, IXElement
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
            ThreadSafeVolume = XAudio2.DecibelsToAmplitudeRatio(Constants.Volume.Default);
        }

        static PointSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PointSelector), new FrameworkPropertyMetadata(typeof(PointSelector)));
        }
        #endregion

        /************************************************************************/

        #region Volume
        /// <summary>
        /// Gets or sets the volume bias.
        /// Volume is expressed as a dB value between <see cref="Constants.Volume.Selector.Min"/> and <see cref="Constants.Volume.Selector.Max"/>
        /// </summary>
        public float Volume
        {
            get => (float)GetValue(VolumeProperty);
            set => SetValue(VolumeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Volume"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register
            (
                nameof(Volume), typeof(float), typeof(PointSelector), new PropertyMetadata(Constants.Volume.Default, OnVolumeChanged, OnVolumeCoerce)
            );

        private static void OnVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PointSelector c)
            {
                // Save volume or later thread safe access.
                c.ThreadSafeVolume = XAudio2.DecibelsToAmplitudeRatio(c.Volume);
                c.SetIsChanged();
            }
        }

        private static object OnVolumeCoerce(DependencyObject d, object baseValue)
        {
            float proposed = (float)baseValue;
            return Math.Min(Constants.Volume.Selector.Max, Math.Max(Constants.Volume.Selector.Min, proposed));
        }

        /// <summary>
        /// Gets a thread safe value of <see cref="Volume"/> expressed as an XAudio2 amplitude ratio.
        /// </summary>
        internal float ThreadSafeVolume
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the selector unit.
        /// </summary>
        public PointSelectorUnit SelectorUnit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the selector song unit.
        /// </summary>
        public PointSelectorSongUnit SelectorSongUnit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the selector row
        /// </summary>
        public int Row
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
            element.Add(new XAttribute(nameof(Row), Row));
            element.Add(new XAttribute(nameof(Position), Position));
            element.Add(new XAttribute(nameof(Volume), Volume));
            element.Add(new XAttribute(nameof(IsSelected), IsSelected));
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            XAttribute volume = element.Attribute(nameof(Volume));
            XAttribute isSelected = element.Attribute(nameof(IsSelected));

            if (volume != null)
            {
                SetDependencyProperty(VolumeProperty, volume.Value);
            }
            if (isSelected != null)
            {
                SetDependencyProperty(IsSelectedProperty, isSelected.Value);
            }
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>A string that describes this object.</returns>
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
        /// Called when <see cref="ControlObjectSelector.IsSelected"/> changes.
        /// </summary>
        protected override void OnIsSelectedChanged()
        {
            if (SelectorType == PointSelectorType.SongRow || SelectorType == PointSelectorType.PatternRow)
            {
                SetIsChanged();
            }
        }
        #endregion
    }
}
