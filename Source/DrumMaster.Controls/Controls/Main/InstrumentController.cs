using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using SharpDX.XAudio2;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a controll for a single instrument of a drum pattern.
    /// </summary>
    public class InstrumentController : AudioControlBase
    {
        #region Private
        private bool isAudioEnabled;
        private SubmixVoice submixVoice;
        private VoicePool voicePool;
        private readonly int channelCount;
        private readonly float[] channelVolumes;
        private readonly DrumPatternPresenter owner;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentController"/> class.
        /// </summary>
        internal InstrumentController(DrumPatternPresenter owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            submixVoice = new SubmixVoice(AudioHost.Instance.AudioDevice);
            submixVoice.SetOutputVoices(new VoiceSendDescriptor(owner.Owner.Controller.SubmixVoice));
            channelCount = submixVoice.VoiceDetails.InputChannelCount;
            channelVolumes = new float[channelCount];
            channelVolumes[0] = 1.0f;
            channelVolumes[1] = 1.0f;

            ExpandedImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Caret.Up.Blue.32.png", UriKind.Relative));
            CollapsedImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Caret.Down.Blue.32.png", UriKind.Relative));
        }

        static InstrumentController()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InstrumentController), new FrameworkPropertyMetadata(typeof(InstrumentController)));
        }
        #endregion

        /************************************************************************/

        #region Instrument
        /// <summary>
        /// Gets or sets the drum piece for this track.
        /// </summary>
        public Instrument Instrument
        {
            get => (Instrument)GetValue(InstrumentProperty);
            set => SetValue(InstrumentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Instrument"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InstrumentProperty = DependencyProperty.Register
            (
                nameof(Instrument), typeof(Instrument), typeof(InstrumentController), new PropertyMetadata(null, OnInstrumentChanged)
            );

        private static void OnInstrumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InstrumentController c)
            {
                c.OnInstrumentChanged();
            }
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
            var element = new XElement(nameof(InstrumentController));
            element.Add(new XElement(nameof(Volume), Volume));
            element.Add(new XElement(nameof(Panning), Panning));
            element.Add(new XElement(nameof(Pitch), Pitch));
            element.Add(new XElement(nameof(IsMuted), IsMuted));
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
        /// Called when the <see cref="AudioControlBase.Volume"/> property is changed.
        /// </summary>
        protected override void OnVolumeChanged()
        {
            if (submixVoice != null)
            {
                submixVoice.SetVolume(ThreadSafeVolume);
            }
        }

        /// <summary>
        /// Called when the <see cref="AudioControlBase.Panning"/> property is changed.
        /// </summary>
        protected override void OnPanningChanged()
        {
            if (channelCount == 2 && submixVoice != null)
            {
                // var p = GetLinearPanning(Panning);
                var p = GetSquareRootPanning(Panning);
                channelVolumes[0] = p.Item1;
                channelVolumes[1] = p.Item2;
                submixVoice.SetChannelVolumes(channelCount, channelVolumes);
            }
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        internal void Play(int operationSet)
        {
            if (isAudioEnabled && !IsUserMuted && !IsAutoMuted)
            {
                voicePool.Play(ThreadSafeVolume, ThreadSafePitch, operationSet);
            }
        }


        //internal void Play(int pass, int step, int operationSet)
        //{
        //    if (isAudioEnabled && !IsUserMuted && !IsAutoMuted)
        //    {
        //        voicePool.Play(ThreadSafeVolume, ThreadSafePitch, operationSet);
        //    }


        //    //if (isAudioEnabled && !IsUserMuted && !IsAutoMuted && step < owner.ThreadSafeBoxContainer.Boxes.Count)
        //    //{
        //    //    try
        //    //    {
        //    //        if (owner.ThreadSafeBoxContainer.CanPlay(pass, step))
        //    //        {
        //    //            owner.ThreadSafeBoxContainer.Boxes[step].ApplyHumanVolumeBias(random, humanVolumeBias);
        //    //            voicePool.Play(owner.ThreadSafeBoxContainer.Boxes[step].ThreadSafeVolume, ThreadSafePitch, operationSet);
        //    //        }
        //    //    }
        //    //    catch { }
        //    //}
        //}
        #endregion

        /************************************************************************/

        #region Private methods
        private void OnInstrumentChanged()
        {
            isAudioEnabled = Instrument != null && Instrument.IsAudioInitialized;
            if (isAudioEnabled)
            {
                AudioHost.Instance.DestroyVoicePool(voicePool);
                voicePool = AudioHost.Instance.CreateVoicePool(Instrument.DisplayName, Instrument.Audio, submixVoice, TrackVals.InitialVoicePool.Normal);
            }
        }

        private const float PanAdjust = 1.15f;

        private Tuple<float, float> GetLinearPanning(float pan)
        {
            return new Tuple<float, float>((1.0f - pan) * PanAdjust, pan * PanAdjust);
        }

        private Tuple<float, float> GetSquareRootPanning(float pan)
        {
            return new Tuple<float, float>((float)Math.Sqrt(1.0f - pan) * PanAdjust, (float)Math.Sqrt(pan) * PanAdjust);
        }

        private Tuple<float, float> GetSinusoidalPanning(float pan)
        {
            double f = pan * (Math.PI / 2);
            return new Tuple<float, float>((float)Math.Sin(f) * PanAdjust, (float)Math.Cos(f) * PanAdjust);
        }
        #endregion
    }
}
