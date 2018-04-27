using Restless.App.DrumMaster.Controls.Audio;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{

    [TemplatePart(Name = PartGridTrack, Type = typeof(Grid))]
    public class TrackController : TrackControlBase
    {
        #region Private
        private const string PartGridTrack = "PART_GRID_TRACK";
        private const string PartEnvelopeHost = "PART_ENVELOPE_HOST";
        private TrackContainer owner;
        private bool isAudioEnabled;
        private TrackBoxContainer boxContainer;
        private SubmixVoice submixVoice;
        private VoicePool voicePool;
        private int channelCount;
        private float[] channelVolumes;
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

        public static readonly DependencyProperty PieceProperty = DependencyProperty.Register
            (
                nameof(Piece), typeof(AudioPiece), typeof(TrackController), new PropertyMetadata(null, OnPieceChanged)
            );


        /// <summary>
        /// Gets or sets a boolean value that indicates if the controller is editing its properties.
        /// </summary>
        public bool IsEditPropertyMode
        {
            get => (bool)GetValue(IsEditPropertyModeProperty);
            set => SetValue(IsEditPropertyModeProperty, value);
        }

        public static readonly DependencyProperty IsEditPropertyModeProperty = DependencyProperty.Register
            (
                nameof(IsEditPropertyMode), typeof(bool), typeof(TrackController), new PropertyMetadata(false)
            );
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

        public static readonly DependencyProperty IsAudioEnabledProperty = IsAudioEnabledPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the image used on the shift left button.
        /// </summary>
        public ImageSource ShiftLeftImageSource
        {
            get;
        }

        /// <summary>
        /// Gets the image used on the shift right button.
        /// </summary>
        public ImageSource ShiftRightImageSource
        {
            get;
        }

        #endregion

        /************************************************************************/

        #region Internal Properties
        #endregion

        /************************************************************************/

        #region Constructors (Internal / Static)
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackController"/> class.
        /// </summary>
        /// <param name="owner">The container that owns this instance</param>
        internal TrackController(TrackContainer owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            submixVoice = new SubmixVoice(AudioHost.Instance.AudioDevice);
            submixVoice.SetOutputVoices(new VoiceSendDescriptor(this.owner.SubmixVoice));
            channelCount = submixVoice.VoiceDetails.InputChannelCount;
            channelVolumes = new float[channelCount];
            channelVolumes[0] = 1.0f;
            channelVolumes[1] = 1.0f;

            Commands.Add("ToggleMute", new RelayCommand(RunToggleMuteCommand));
            Commands.Add("ShiftLeft", new RelayCommand(RunShiftLeftCommand));
            Commands.Add("ShiftRight", new RelayCommand(RunShiftRightCommand));
            Commands.Add("ToggleTrackProp", new RelayCommand(RunToggleTrackProps));

            ShiftLeftImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Shift.Left.64.png", UriKind.Relative));
            ShiftRightImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Shift.Right.64.png", UriKind.Relative));
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
            //gridTrack = GetTemplateChild(PartGridTrack) as Grid;

            //envelopeHost = GetTemplateChild(PartEnvelopeHost) as Grid;
            //if (envelopeHost != null)
            //{
            //    envelopeHost.Children.Clear();
            //    volEnvelop = new EnvelopeControl();
            //    envelopeHost.Children.Add(volEnvelop);
            //}
            //OnBoxSizeChanged();
            // OnIsMutedChanged();
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
            element.Add(new XElement(nameof(IsMuted), IsMuted));
            element.Add(new XElement(nameof(IsPanningEnabled), IsPanningEnabled));
            element.Add(new XElement(nameof(IsPitchEnabled), IsPitchEnabled));
            element.Add(Piece.GetXElement());
            element.Add(boxContainer.GetXElement());
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
                //Debug.WriteLine($"Name of node:{e.Name}");
                if (e.Name == nameof(Volume)) SetDependencyProperty(VolumeProperty, e.Value);
                if (e.Name == nameof(Panning)) SetDependencyProperty(PanningProperty, e.Value);
                if (e.Name == nameof(Pitch)) SetDependencyProperty(PitchProperty, e.Value);
                if (e.Name == nameof(IsMuted)) SetDependencyProperty(IsMutedProperty, e.Value);
                if (e.Name == nameof(IsPanningEnabled)) SetDependencyProperty(IsPanningEnabledProperty, e.Value);
                if (e.Name == nameof(IsPitchEnabled)) SetDependencyProperty(IsPitchEnabledProperty, e.Value);
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
                if (e.Name == nameof(TrackBoxContainer))
                {
                    boxContainer.RestoreFromXElement(e);
                }
            }
            ResetIsChanged();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when <see cref="Volume"/> is changed.
        /// </summary>
        protected override void OnVolumeChanged()
        {
            if (submixVoice != null)
            {
                submixVoice.SetVolume(VolumeInternal);
            }
        }

        protected override void OnPanningChanged()
        {
            if (IsPanningEnabled && channelCount == 2)
            {
                // var p = GetLinearPanning(Panning);
                var p = GetSquareRootPanning(Panning);
                channelVolumes[0] = p.Item1;
                channelVolumes[1] = p.Item2;
                submixVoice.SetChannelVolumes(channelCount, channelVolumes);
            }
        }

        /// <summary>
        /// Called when the <see cref="IsMuted"/> property is changed.
        /// </summary>
        protected override void OnIsMutedChanged()
        {
            base.OnIsMutedChanged();
            //MuteImageSource = (IsMuted) ? imageMuted : imageVoiced;
        }
        #endregion

        /************************************************************************/

        #region Internal methods

        internal void SetBoxContainer(TrackBoxContainer boxContainer)
        {
            this.boxContainer = boxContainer ?? throw new ArgumentNullException(nameof(boxContainer));
        }

        internal void Play(int pass, int step, int operationSet)
        {
            if (isAudioEnabled && !IsUserMuted && !IsAutoMuted && step < boxContainer.Boxes.Count)
            {
                try
                {
                    //if (boxContainer.Boxes[step].IsSelectedInternal)
                    if (boxContainer.CanPlay(pass, step))
                    {
                        //float vol = (step % 5 == 0) ? 1.0F : 0.45F;
                        //piece.Play(steps[step].VolumeInternal, operationSet);
                        // vol = Boxes[step].VolumeInternal;
                        //voicePool.Play(vol, PitchInternal, operationSet);
                        voicePool.Play(boxContainer.Boxes[step].VolumeInternal, PitchInternal, operationSet);

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
            //var boxes = boxContainer.Boxes;
            //bool firstBoxSelected = boxes[0].IsSelected;
            //for (int k = 0; k < boxes.Count - 1; k++)
            //{
            //    boxes[k].IsSelected = boxes[k + 1].IsSelected;
            //}
            //boxes[boxes.Count - 1].IsSelected = firstBoxSelected;
        }

        private void RunShiftRightCommand(object parm)
        {
            //var boxes = boxContainer.Boxes;
            //bool lastBoxSelected = boxes[boxes.Count - 1].IsSelected;
            //for (int k = boxes.Count - 1; k > 0; k--)
            //{
            //    boxes[k].IsSelected = boxes[k - 1].IsSelected;
            //}
            //boxes[0].IsSelected = lastBoxSelected;
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

        /************************************************************************/

        #region Private methods (Static)

        private static void OnPieceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackController c)
            {
                c.OnPieceChanged();
                c.SetIsChanged();
            }
        }
        #endregion
    }
}
