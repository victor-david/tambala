using System;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    public class MasterControl : ControlBase
    {
        #region Private
        private SongContainer owner;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterControl"/> class.
        /// </summary>
        internal MasterControl(SongContainer owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        static MasterControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterControl), new FrameworkPropertyMetadata(typeof(MasterControl)));
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
                nameof(Tempo), typeof(double), typeof(MasterControl), new PropertyMetadata(TrackVals.Tempo.Default, OnTempoChanged, OnTempoCoerce)
            );

        private static void OnTempoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MasterControl c)
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
                nameof(TempoText), typeof(string), typeof(MasterControl), new PropertyMetadata(TrackVals.Tempo.DefaultText)
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
        public override XElement GetXElement()
        {
            throw new NotImplementedException();
        }

        public override void RestoreFromXElement(XElement element)
        {
        }
        #endregion
    }
}
