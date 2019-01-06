using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a track box (header or step).
    /// </summary>
    public class TrackBox : ControlBase
    {
        #region Private
        private TrackBoxContainerBase owner;
        private StepPlayFrequency playFrequency;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the box type.
        /// </summary>
        public TrackBoxType BoxType
        {
            get => (TrackBoxType)GetValue(BoxTypeProperty);
            set => SetValue(BoxTypeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BoxType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BoxTypeProperty = DependencyProperty.Register
            (
                nameof(BoxType), typeof(TrackBoxType), typeof(TrackBox), new PropertyMetadata(TrackBoxType.Header)
            );


        /// <summary>
        /// Gets or sets the text associated with this box
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register
            (
                nameof(Text), typeof(string), typeof(TrackBox), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the play frequency.
        /// </summary>
        public StepPlayFrequency PlayFrequency
        {
            get => (StepPlayFrequency)GetValue(PlayFrequencyProperty);
            set => SetValue(PlayFrequencyProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PlayFrequency"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlayFrequencyProperty = DependencyProperty.Register
            (
                nameof(PlayFrequency), typeof(StepPlayFrequency), typeof(TrackBox), new PropertyMetadata(StepPlayFrequency.None, OnPlayFrequencyChanged )
            );


        private static void OnPlayFrequencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackBox c)
            {
                c.playFrequency = (StepPlayFrequency)e.NewValue;
                if (c.owner.BoxType == TrackBoxType.TrackStep)
                {
                    c.SetIsChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the brush that is used when <see cref="PlayFrequency"/> is a value other than None.
        /// </summary>
        public Brush SelectedBackgroundBrush
        {
            get => (Brush)GetValue(SelectedBackgroundBrushProperty);
            set => SetValue(SelectedBackgroundBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectedBackgroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundBrushProperty = DependencyProperty.Register
            (
                nameof(SelectedBackgroundBrush), typeof(Brush), typeof(TrackBox), new PropertyMetadata(new SolidColorBrush(Colors.RosyBrown))
            );

        /// <summary>
        /// Gets or sets a value that determines if the volume bias control for the <see cref="TrackBox"/> is visible.
        /// </summary>
        public bool IsVolumeVisible
        {
            get => (bool)GetValue(IsVolumeVisibleProperty);
            set => SetValue(IsVolumeVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsVolumeVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVolumeVisibleProperty = DependencyProperty.Register
            (
                nameof(IsVolumeVisible), typeof(bool), typeof(TrackBox), new PropertyMetadata(false)
            );
        #endregion

        /************************************************************************/

        #region Protected properties

        #endregion

        /************************************************************************/

        #region Internal properties
        #endregion

        /************************************************************************/

        #region Constructors (Internal / Static)
        /// <summary>
        /// Creates a new instance of <see cref="TrackBox"/>
        /// </summary>
        /// <param name="owner">The owner of this control.</param>
        internal TrackBox(TrackBoxContainerBase owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Height = Width = TrackVals.BoxSize.Default;
            Commands.Add("ToggleIsSelected", new RelayCommand(RunToggleIsSelectedCommand));
            Commands.Add("SwitchFrequency", new RelayCommand(RunSwitchStepFrequencyCommand));
            playFrequency = StepPlayFrequency.None;
        }

        static TrackBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TrackBox), new FrameworkPropertyMetadata(typeof(TrackBox)));
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
            var element = new XElement(nameof(TrackBox));
            element.Add(new XElement(nameof(PlayFrequency), PlayFrequency));
            element.Add(new XElement(nameof(VolumeBias), VolumeBias));
            element.Add(new XElement(nameof(IsVolumeVisible), IsVolumeVisible));
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            IEnumerable<XElement> childList = from el in element.Elements() select el;
            foreach (XElement e in childList)
            {
                if (e.Name == nameof(PlayFrequency))
                {
                    StepPlayFrequency result = StepPlayFrequency.None;

                    if (Enum.TryParse(e.Value, out result))
                    {
                        PlayFrequency = result;
                    }
                }

                if (e.Name == nameof(VolumeBias))
                {
                    float volBias = TrackVals.VolumeBias.Default;
                    if (float.TryParse(e.Value, out volBias))
                    {
                        VolumeBias = volBias;
                    }
                }

                if (e.Name == nameof(IsVolumeVisible))
                {
                    if (bool.TryParse(e.Value, out bool result))
                    {
                        IsVolumeVisible = result;
                    }
                }
            }
            ResetIsChanged();
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Returns a boolean value that indicates if this beat
        /// can play for the specified pass.
        /// </summary>
        /// <param name="pass">The pass</param>
        /// <returns>true if this beat can play on the pass; otherwise, false.</returns>
        internal bool CanPlay(int pass)
        {
            switch (playFrequency)
            {
                case StepPlayFrequency.None:
                    return false;
                case StepPlayFrequency.EveryPass:
                    return true;
                case StepPlayFrequency.SecondPass:
                    return pass % 2 == 0;
                case StepPlayFrequency.OddPass:
                    return (pass + 1) % 2 == 0;
                case StepPlayFrequency.ThirdPass3:
                    return pass % 3 == 0;
                case StepPlayFrequency.ThirdPass4:
                    return pass % 4 == 3;
                case StepPlayFrequency.FourthPass:
                    return pass % 4 == 0;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Applies the human volume bias according to the value specified
        /// by <paramref name="biasFactor"/>. If 0.0, this method returns
        /// without doing anything.
        /// </summary>
        /// <param name="random">The random generator object.</param>
        /// <param name="biasFactor">The bias factor to apply.</param>
        internal void ApplyHumanVolumeBias(Random random, float biasFactor)
        {
            if (biasFactor > TrackVals.HumanVolumeBias.Min)
            {
                // biasFactor is expressed as a value between zero and HumanVolumeBias.Max (7.5)
                float minValue = -biasFactor;
                float result = (float)random.NextDouble() * (biasFactor - minValue) + minValue;
                float dbVol = ThreadSafeVolumeRaw + ThreadSafeVolumeBiasRaw + result;
                ThreadSafeVolume = XAudio2.DecibelsToAmplitudeRatio(dbVol);
            }
        }

        /// <summary>
        /// From this assembly, removes any human volume bias that may be present.
        /// </summary>
        internal void RemoveHumanVolumeBias()
        {
            float dbVol = ThreadSafeVolumeRaw + ThreadSafeVolumeBiasRaw;
            ThreadSafeVolume = XAudio2.DecibelsToAmplitudeRatio(dbVol);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion

        /************************************************************************/

        #region Private methods (Instance)

        private void RunToggleIsSelectedCommand(object parm)
        {
            PlayFrequency = (playFrequency == StepPlayFrequency.None) ? StepPlayFrequency.EveryPass : StepPlayFrequency.None;
        }

        private void RunSwitchStepFrequencyCommand(object parm)
        {
            if (parm is StepPlayFrequency frequency)
            {
                PlayFrequency = frequency;
            }
        }
        #endregion
    }
}
