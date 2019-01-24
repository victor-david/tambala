using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a controller for a single instrument of a drum pattern.
    /// </summary>
    public class InstrumentController : AudioControlBase, ISelectable
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
            IsEnabledForPlay = true;
            submixVoice = new SubmixVoice(AudioHost.Instance.AudioDevice);
            submixVoice.SetOutputVoices(new VoiceSendDescriptor(owner.Owner.Controller.SubmixVoice));
            channelCount = submixVoice.VoiceDetails.InputChannelCount;
            channelVolumes = new float[channelCount];
            channelVolumes[0] = 1.0f;
            channelVolumes[1] = 1.0f;

            PatternQuarters = new Dictionary<int, DrumPatternQuarter>();
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

        #region Quarters
        /// <summary>
        /// Gets the dictionary of pattern quarters that are controlled by this controller.
        /// </summary>
        internal Dictionary<int, DrumPatternQuarter> PatternQuarters
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region IsEnabledForPlay
        /// <summary>
        /// Gets a value that indicate if this instrument controller
        /// is enabled for play.
        /// </summary>
        /// <remarks>
        /// This property is set when switching drum kits. If a drum kit has fewer
        /// instruments than the previous (or default) drum kit, the extra controllers
        /// are hidden and this property is set to false so that any notes that may
        /// have been set before the drum kit switch aren't played.Cannot use the regular
        /// IsEnabled property because that is a dependency property and attempting to
        /// access if from the play thread throws an exception.
        /// </remarks>
        public bool IsEnabledForPlay
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region IsSelected
        /// <summary>
        /// Gets or sets a value that indicates if the <see cref="InstrumentController"/> is selected.
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        /// <summary>
        /// Identifes the <see cref="IsSelected"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register
            (
                nameof(IsSelected), typeof(bool), typeof(InstrumentController), new PropertyMetadata(false, OnIsSelectedChanged)
            );

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InstrumentController c)
            {
                c.RaiseEvent(new RoutedEventArgs(IsSelectedChangedEvent));
            }
        }

        /// <summary>
        /// Provides notification when the <see cref="IsSelected"/> property changes.
        /// </summary>
        public event RoutedEventHandler IsSelectedChanged
        {
            add => AddHandler(IsSelectedChangedEvent, value);
            remove => RemoveHandler(IsSelectedChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSelectedChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent IsSelectedChangedEvent = EventManager.RegisterRoutedEvent
            (
                nameof(IsSelectedChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(InstrumentController)
            );
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
            element.Add(new XElement(nameof(DisplayName), DisplayName));
            element.Add(new XElement(nameof(Volume), Volume));
            element.Add(new XElement(nameof(Panning), Panning));
            element.Add(new XElement(nameof(Pitch), Pitch));
            element.Add(new XElement(nameof(IsMuted), IsMuted));
            element.Add(Instrument.GetXElement());

            foreach(var item in PatternQuarters)
            {
                element.Add(item.Value.GetXElement());
            }
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            int quarterIdx = 1;

            foreach (XElement e in ChildElementList(element))
            {
                // Leave DisplayName as it comes from the instrument.
                //if (e.Name == nameof(DisplayName)) SetDependencyProperty(DisplayNameProperty, e.Value);
                if (e.Name == nameof(Volume)) SetDependencyProperty(VolumeProperty, e.Value);
                if (e.Name == nameof(Panning)) SetDependencyProperty(PanningProperty, e.Value);
                if (e.Name == nameof(Pitch)) SetDependencyProperty(PitchProperty, e.Value);
                if (e.Name == nameof(IsMuted)) SetDependencyProperty(IsMutedProperty, e.Value);
                if (e.Name == nameof(Instrument)) Instrument.RestoreFromXElement(e);

                if (e.Name == nameof(DrumPatternQuarter))
                {
                    if (PatternQuarters.ContainsKey(quarterIdx))
                    {
                        PatternQuarters[quarterIdx].Create();
                        PatternQuarters[quarterIdx].RestoreFromXElement(e);
                    }
                    quarterIdx++;
                }
            }
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

        /// <summary>
        /// Called when the left mouse button is released.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            IsSelected = !IsSelected;
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Submits this instrument's voice to the voice pool 
        /// if the specified position within the specified quarter note
        /// is selected (and if the entire instrument is not muted)
        /// </summary>
        /// <param name="songUnit">The song unit.</param>
        /// <param name="quarterNote">The quarter note.</param>
        /// <param name="position">The position within the quarter note.</param>
        /// <param name="operationSet">The operation set. Used only when submitting the voice to the pool.</param>
        internal void Play(PointSelectorSongUnit songUnit, int quarterNote, int position, int operationSet)
        {
            if (PatternQuarters.ContainsKey(quarterNote) &&
                PatternQuarters[quarterNote].IsSelected(songUnit, position) &&
                isAudioEnabled && !IsUserMuted && !IsAutoMuted && IsEnabledForPlay)
            {
                float dbVol = PatternQuarters[quarterNote].GetSelectorVolume(position);
                voicePool.Play(dbVol, ThreadSafePitch, operationSet);
            }
        }

        /// <summary>
        /// Sets whether this controller is enabled for play. When <paramref name="isEnabled"/>
        /// is false, the controller is disbled for play and is hidden.
        /// </summary>
        /// <param name="isEnabled">true if enabled for play.</param>
        /// <param name="quarterNoteCount">The quarter note count.</param>
        internal void SetIsEnabledForPlay(bool isEnabled, int quarterNoteCount)
        {
            IsEnabledForPlay = isEnabled;
            SetIsVisible(isEnabled, quarterNoteCount);
        }

        /// <summary>
        /// Sets whether this controller is visible. When <paramref name="isVisible"/>
        /// is false, the controller is hidden.
        /// </summary>
        /// <param name="isVisible">true if visible.</param>
        /// <param name="quarterNoteCount">The quarter note count.</param>
        internal void SetIsVisible(bool isVisible, int quarterNoteCount)
        {
            Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;

            foreach (var item in PatternQuarters)
            {
                item.Value.Visibility = item.Value.QuarterNote <= quarterNoteCount ? Visibility : Visibility.Collapsed;
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void OnInstrumentChanged()
        {
            isAudioEnabled = Instrument != null && Instrument.IsAudioInitialized;
            if (isAudioEnabled)
            {
                AudioHost.Instance.DestroyVoicePool(voicePool);
                voicePool = AudioHost.Instance.CreateVoicePool(Instrument.DisplayName, Instrument.Audio, submixVoice, Constants.InitialVoicePool.Normal);
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
