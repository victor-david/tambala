using System;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    public class MasterOutput : AudioControlBase
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterOutput"/> class.
        /// </summary>
        internal MasterOutput(ProjectContainer owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            DisplayName = "Master Output";
        }

        static MasterOutput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterOutput), new FrameworkPropertyMetadata(typeof(MasterOutput)));
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the <see cref="ProjectContainer"/> that owns this instance.
        /// </summary>
        private ProjectContainer Owner
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Tempo
        /// <summary>
        /// Gets or sets the tempo
        /// </summary>
        public double Tempo
        {
            get => (double)GetValue(TempoProperty);
            set => SetValue(TempoProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Tempo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TempoProperty = DependencyProperty.Register
            (
                nameof(Tempo), typeof(double), typeof(MasterOutput), new PropertyMetadata(TrackVals.Tempo.Default, OnTempoChanged, OnTempoCoerce)
            );

        private static void OnTempoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MasterOutput c)
            {
                // c.CalculateThreadSafeSleepTime();
                c.SetIsChanged();
            }
        }

        private static object OnTempoCoerce(DependencyObject d, object baseValue)
        {
            double proposed = (double)baseValue;
            return Math.Min(TrackVals.Tempo.Max, Math.Max(TrackVals.Tempo.Min, proposed));
        }

        /// <summary>
        /// Gets or sets the tempo text descriptor
        /// </summary>
        public string TempoText
        {
            get => (string)GetValue(TempoTextProperty);
            set => SetValue(TempoTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TempoText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TempoTextProperty = DependencyProperty.Register
            (
                nameof(TempoText), typeof(string), typeof(MasterOutput), new PropertyMetadata(TrackVals.Tempo.DefaultText)
            );


        /// <summary>
        /// Gets the minimum tempo allowed. Used for binding in the control template.
        /// </summary>
        public double MinTempo
        {
            get => TrackVals.Tempo.Min;
        }

        /// <summary>
        /// Gets the maximum tempo allowed. Used for binding in the control template.
        /// </summary>
        public double MaxTempo
        {
            get => TrackVals.Tempo.Max;
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
        }
        #endregion
    }
}
