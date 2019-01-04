using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Automation;
using Restless.App.DrumMaster.Controls.Resources;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a track controller. This control manages the track with volume, pitch, panning, etc.
    /// </summary>
    [TemplatePart(Name = PartGridTrack, Type = typeof(Grid))]
    public class TrackController : TrackControlBase
    {
        #region Private
        private const string PartGridTrack = "PART_GRID_TRACK";
        private const string PartEnvelopeHost = "PART_ENVELOPE_HOST";
        private readonly CompositeTrack owner;
        private bool isAudioEnabled;
        private SubmixVoice submixVoice;
        private VoicePool voicePool;
        private readonly int channelCount;
        private readonly float[] channelVolumes;
        private float humanVolumeBias;
        private readonly Random random;
        #endregion

        /************************************************************************/

        #region Public Properties
        /// <summary>
        /// Gets the available audio pieces.
        /// </summary>
        public AudioPieceCollection AudioPieces
        {
            get => AudioHost.Instance.AudioPieces;
        }

        /// <summary>
        /// Gets or sets the drum piece for this track.
        /// </summary>
        public AudioPiece Piece
        {
            get => (AudioPiece)GetValue(PieceProperty);
            set => SetValue(PieceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Piece"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PieceProperty = DependencyProperty.Register
            (
                nameof(Piece), typeof(AudioPiece), typeof(TrackController), new PropertyMetadata(null, OnPieceChanged)
            );

        private static void OnPieceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackController c)
            {
                c.OnPieceChanged();
                c.SetIsChanged();
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates if the controller is editing its properties.
        /// </summary>
        public bool IsEditPropertyMode
        {
            get => (bool)GetValue(IsEditPropertyModeProperty);
            set => SetValue(IsEditPropertyModeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsEditPropertyMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEditPropertyModeProperty = DependencyProperty.Register
            (
                nameof(IsEditPropertyMode), typeof(bool), typeof(TrackController), new PropertyMetadata(false)
            );

        /// <summary>
        /// Gets or sets a value that determines if the track box volume controls for this track controller are visible.
        /// </summary>
        public bool IsTrackBoxVolumeVisible
        {
            get => (bool)GetValue(IsTrackBoxVolumeVisibleProperty);
            set => SetValue(IsTrackBoxVolumeVisibleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsTrackBoxVolumeVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTrackBoxVolumeVisibleProperty = DependencyProperty.Register
            (
                nameof(IsTrackBoxVolumeVisible), typeof(bool), typeof(TrackController), new PropertyMetadata(false, OnIsTrackBoxVisibleChanged)
            );

        private static void OnIsTrackBoxVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackController c)
            {
                c.owner.BoxContainer.Boxes.SetVolumeVisibility(c.IsTrackBoxVolumeVisible);
            }
        }

        /// <summary>
        /// Gets or sets a value that determines the volume variance to apply during playback.
        /// This property applies to individual beats
        /// </summary>
        public float HumanVolumeBias
        {
            get => (float)GetValue(HumanVolumeBiasProperty);
            set => SetValue(HumanVolumeBiasProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="HumanVolumeBias"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HumanVolumeBiasProperty = DependencyProperty.Register
            (
                nameof(HumanVolumeBias), typeof(float), typeof(TrackController), new PropertyMetadata
                    (
                        TrackVals.HumanVolumeBias.Default, OnHumanVolumeBiasChanged, OnHumanVolumeBiasCoerce
                    )
            );

        private static void OnHumanVolumeBiasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackController c)
            {
                // Save human volume bias for later thread safe access.
                c.humanVolumeBias = (float)e.NewValue;
                if (c.humanVolumeBias == TrackVals.HumanVolumeBias.Min)
                {
                    c.owner.BoxContainer.Boxes.RemoveHumanVolumeBias();
                }
                c.SetIsChanged();
            }
        }

        private static object OnHumanVolumeBiasCoerce(DependencyObject d, object baseValue)
        {
            float proposed = (float)baseValue;
            return Math.Min(TrackVals.HumanVolumeBias.Max, Math.Max(TrackVals.HumanVolumeBias.Min, proposed));
        }

        /// <summary>
        /// Gets the minimum human volume bias allowed. Used for binding in the control template.
        /// </summary>
        public float MinHumanVolumeBias
        {
            get => TrackVals.HumanVolumeBias.Min;
        }

        /// <summary>
        /// Gets the maximum human volume bias allowed. Used for binding in the control template.
        /// </summary>
        public float MaxHumanVolumeBias
        {
            get => TrackVals.HumanVolumeBias.Max;
        }
        #endregion

        /************************************************************************/

        #region Public properties (read only)
        /// <summary>
        /// Gets a boolean value that indicates if audio is enabled for the track.
        /// </summary>
        public bool IsAudioEnabled
        {
            get => (bool)GetValue(IsAudioEnabledProperty);
            private set => SetValue(IsAudioEnabledPropertyKey, value);
        }

        private static readonly DependencyPropertyKey IsAudioEnabledPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(IsAudioEnabled), typeof(bool), typeof(TrackController), new FrameworkPropertyMetadata(false)
            );

        /// <summary>
        /// Identifies the <see cref="IsAudioEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAudioEnabledProperty = IsAudioEnabledPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the image used on the shift left button.
        /// </summary>
        public ImageSource ShiftLeftImageSource
        {
            get => (ImageSource)GetValue(ShiftLeftImageSourceProperty);
            private set => SetValue(ShiftLeftImageSourcePropertyKey, value);
        }

        private static readonly DependencyPropertyKey ShiftLeftImageSourcePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ShiftLeftImageSource), typeof(ImageSource), typeof(TrackController), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ShiftLeftImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShiftLeftImageSourceProperty = ShiftLeftImageSourcePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the image used on the shift right button.
        /// </summary>
        public ImageSource ShiftRightImageSource
        {
            get => (ImageSource)GetValue(ShiftRightImageSourceProperty);
            private set => SetValue(ShiftRightImageSourcePropertyKey, value);
        }

        private static readonly DependencyPropertyKey ShiftRightImageSourcePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ShiftRightImageSource), typeof(ImageSource), typeof(TrackController), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ShiftRightImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShiftRightImageSourceProperty = ShiftRightImageSourcePropertyKey.DependencyProperty;

        /// <summary>
        /// Get the tooltip text for the shift left command
        /// </summary>
        public string ShiftLeftToolTip
        {
            get => (string)GetValue(ShiftLeftToolTipProperty);
            private set => SetValue(ShiftLeftToolTipPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ShiftLeftToolTipPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ShiftLeftToolTip), typeof(string), typeof(TrackController), new PropertyMetadata(Strings.ToolTipShiftLeft)
            );

        /// <summary>
        /// Identifies the <see cref="ShiftLeftToolTip"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShiftLeftToolTipProperty = ShiftLeftToolTipPropertyKey.DependencyProperty;

        /// <summary>
        /// Get the tooltip text for the shift right command
        /// </summary>
        public string ShiftRightToolTip
        {
            get => (string)GetValue(ShiftRightToolTipProperty);
            private set => SetValue(ShiftRightToolTipPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ShiftRightToolTipPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ShiftRightToolTip), typeof(string), typeof(TrackController), new PropertyMetadata(Strings.ToolTipShiftRight)
            );

        /// <summary>
        /// Identifies the <see cref="ShiftRightToolTip"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShiftRightToolTipProperty = ShiftRightToolTipPropertyKey.DependencyProperty;

        #endregion

        /************************************************************************/

        #region CLR properties
        /// <summary>
        /// Gets the track's voice automation object.
        /// </summary>
        public TrackVoiceAutomation VoiceAutomation
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructors (Internal / Static)
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackController"/> class.
        /// </summary>
        /// <param name="owner">The composite track that owns this controller</param>
        internal TrackController(CompositeTrack owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            submixVoice = new SubmixVoice(AudioHost.Instance.AudioDevice);
            submixVoice.SetOutputVoices(new VoiceSendDescriptor(this.owner.Owner.SubmixVoice));
            channelCount = submixVoice.VoiceDetails.InputChannelCount;
            channelVolumes = new float[channelCount];
            channelVolumes[0] = 1.0f;
            channelVolumes[1] = 1.0f;
            random = new Random(DateTime.Now.Minute * DateTime.Now.Minute);

            Commands.Add("ToggleMute", new RelayCommand(RunToggleMuteCommand));
            Commands.Add("ShiftLeft", new RelayCommand(RunShiftLeftCommand));
            Commands.Add("ShiftRight", new RelayCommand(RunShiftRightCommand));
            Commands.Add("ToggleTrackProp", new RelayCommand(RunToggleTrackProps));
            Commands.Add("RemoveTrack", new RelayCommand(RunRemoveTrackCommand));
            Commands.Add("ToggleBeatVolume", new RelayCommand(RunToggleBeatVolumeCommand));
            Commands.Add("ResetBeatVolume", new RelayCommand(RunResetBeatVolumeCommand));

            ShiftLeftImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Shift.Left.64.png", UriKind.Relative));
            ShiftRightImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Shift.Right.64.png", UriKind.Relative));

            MinimizeImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Minimize.Blue.64.png", UriKind.Relative));
            MaximizeImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Maximize.Blue.64.png", UriKind.Relative));

            VoiceAutomation = new TrackVoiceAutomation(this);
        }

        static TrackController()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TrackController), new FrameworkPropertyMetadata(typeof(TrackController)));
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Called when the template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnPanningChanged();
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
            var element = new XElement(nameof(TrackController));
            element.Add(new XElement(nameof(Volume), Volume));
            element.Add(new XElement(nameof(Panning), Panning));
            element.Add(new XElement(nameof(Pitch), Pitch));
            element.Add(new XElement(nameof(HumanVolumeBias), HumanVolumeBias));
            element.Add(new XElement(nameof(IsMuted), IsMuted));
            element.Add(new XElement(nameof(IsTrackBoxVolumeVisible), IsTrackBoxVolumeVisible));
            element.Add(Piece.GetXElement());
            element.Add(VoiceAutomation.GetXElement());
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
                if (e.Name == nameof(Volume)) SetDependencyProperty(VolumeProperty, e.Value);
                if (e.Name == nameof(Panning)) SetDependencyProperty(PanningProperty, e.Value);
                if (e.Name == nameof(Pitch)) SetDependencyProperty(PitchProperty, e.Value);
                if (e.Name == nameof(HumanVolumeBias)) SetDependencyProperty(HumanVolumeBiasProperty, e.Value);
                if (e.Name == nameof(IsMuted)) SetDependencyProperty(IsMutedProperty, e.Value);
                if (e.Name == nameof(IsTrackBoxVolumeVisible)) SetDependencyProperty(IsTrackBoxVolumeVisibleProperty, e.Value);
                if (e.Name == nameof(AudioPiece))
                {
                    IEnumerable<XElement> audioList = from el in e.Elements() select el;
                    foreach (XElement ae in audioList)
                    {
                        if (ae.Name == nameof(AudioPiece.AudioName))
                        {
                            Piece = AudioHost.Instance.GetAudioPiece(ae.Value);
                        }
                    }
                }
                if (e.Name == nameof(TrackVoiceAutomation)) VoiceAutomation.RestoreFromXElement(e);
            }

            ResetIsChanged();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the <see cref="TrackControlBase.Volume"/> property is changed.
        /// </summary>
        protected override void OnVolumeChanged()
        {
            if (submixVoice != null)
            {
                submixVoice.SetVolume(ThreadSafeVolume);
            }
        }

        /// <summary>
        /// Called when the <see cref="TrackControlBase.Panning"/> property is changed.
        /// </summary>
        protected override void OnPanningChanged()
        {
            if (channelCount == 2)
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

        internal void Play(int pass, int step, int operationSet)
        {
            if (isAudioEnabled && !IsUserMuted && !IsAutoMuted && step < owner.ThreadSafeBoxContainer.Boxes.Count)
            {
                try
                {
                    if (VoiceAutomation.CanPlay(pass) && owner.ThreadSafeBoxContainer.CanPlay(pass, step))
                    {
                        owner.ThreadSafeBoxContainer.Boxes[step].ApplyHumanVolumeBias(random, humanVolumeBias);
                        voicePool.Play(owner.ThreadSafeBoxContainer.Boxes[step].ThreadSafeVolume, ThreadSafePitch, operationSet);
                    }
                }
                catch { }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (Instance)

        private void RunToggleMuteCommand(object parm)
        {
            IsMuted = !IsMuted;
        }

        private void RunShiftLeftCommand(object parm)
        {
            bool ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool alt = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);

            owner.BoxContainer.Boxes.ShiftLeft(!alt, !ctrl || alt);
        }

        private void RunShiftRightCommand(object parm)
        {
            bool ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool alt = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);

            owner.BoxContainer.Boxes.ShiftRight(!alt, !ctrl || alt);
        }

        private void RunRemoveTrackCommand(object parm)
        {
            owner.Owner.RemoveTrack(owner);
        }

        private void RunToggleBeatVolumeCommand(object parm)
        {
            IsTrackBoxVolumeVisible = !IsTrackBoxVolumeVisible;
            SetIsChanged();
        }

        private void RunResetBeatVolumeCommand(object parm)
        {
            owner.BoxContainer.Boxes.ResetVolumeBias();
            SetIsChanged();
        }

        private void RunToggleTrackProps(object parm)
        {
            IsEditPropertyMode = !IsEditPropertyMode;
        }

        private void OnPieceChanged()
        {
            IsAudioEnabled = isAudioEnabled = (Piece != null && Piece.IsAudioInitialized);
            if (IsAudioEnabled)
            {
                AudioHost.Instance.DestroyVoicePool(voicePool);
                voicePool = AudioHost.Instance.CreateVoicePool(Piece.Audio, submixVoice);
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
            return new Tuple<float, float>((float)Math.Sin(f)*PanAdjust, (float)Math.Cos(f)*PanAdjust);
        }
        #endregion
    }
}
