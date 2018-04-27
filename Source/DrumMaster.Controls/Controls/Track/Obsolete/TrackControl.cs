using Restless.App.DrumMaster.Controls.Audio;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Restless.App.DrumMaster.Controls
{

    //[TemplatePart(Name = PartGridTrack, Type = typeof(Grid))]
    //[TemplatePart(Name = PartEnvelopeHost, Type = typeof(Grid))]
    //public class TrackControl : TrackBoxHostControl
    //{
    //    #region Private
    //    private const string PartGridTrack = "PART_GRID_TRACK";
    //    private const string PartEnvelopeHost = "PART_ENVELOPE_HOST";

    //    private Grid gridTrack;
    //    //private Grid envelopeHost;
    //    //private EnvelopeControl volEnvelop;

    //    private bool isAudioEnabled;
    //    private SubmixVoice submixVoice;
    //    private VoicePool voicePool;
    //    private ImageSource imageMuted;
    //    private ImageSource imageVoiced;


    //    #endregion

    //    /************************************************************************/

    //    #region Public Properties
    //    /// <summary>
    //    /// Gets or sets the drum piece for this track.
    //    /// </summary>
    //    public AudioPiece Piece
    //    {
    //        get => (AudioPiece)GetValue(PieceProperty);
    //        set => SetValue(PieceProperty, value);
    //    }

    //    public static readonly DependencyProperty PieceProperty = DependencyProperty.Register
    //        (
    //            nameof(Piece), typeof(AudioPiece), typeof(TrackControl), new PropertyMetadata(null, OnPieceChanged)
    //        );
    //    #endregion

    //    /************************************************************************/

    //    #region Public properties (read only)
    //    /// <summary>
    //    /// Gets the <see cref="TrackContainer"/> that owns this track.
    //    /// </summary>
    //    public TrackContainer Owner
    //    {
    //        get => (TrackContainer)GetValue(OwnerProperty);
    //        private set => SetValue(OwnerPropertyKey, value);
    //    }

    //    private static readonly DependencyPropertyKey OwnerPropertyKey = DependencyProperty.RegisterReadOnly
    //        (
    //            nameof(Owner), typeof(TrackContainer), typeof(TrackControl), new FrameworkPropertyMetadata(null)
    //        );

    //    public static readonly DependencyProperty OwnerProperty = OwnerPropertyKey.DependencyProperty;


    //    /// <summary>
    //    /// Gets a boolean value that indicates if audio is enabled for the track.
    //    /// </summary>
    //    public bool IsAudioEnabled
    //    {
    //        get => (bool)GetValue(IsAudioEnabledProperty);
    //        private set => SetValue(IsAudioEnabledPropertyKey, value);
    //    }

    //    private static readonly DependencyPropertyKey IsAudioEnabledPropertyKey = DependencyProperty.RegisterReadOnly
    //        (
    //            nameof(IsAudioEnabled), typeof(bool), typeof(TrackControl), new FrameworkPropertyMetadata(false)
    //        );

    //    public static readonly DependencyProperty IsAudioEnabledProperty = IsAudioEnabledPropertyKey.DependencyProperty;


    //    /// <summary>
    //    /// Gets the image source to use for the mute button.
    //    /// </summary>
    //    public ImageSource MuteImageSource
    //    {
    //        get => (ImageSource)GetValue(MuteImageSourceProperty);
    //        private set => SetValue(MuteImageSourcePropertyKey, value);
    //    }

    //    private static readonly DependencyPropertyKey MuteImageSourcePropertyKey = DependencyProperty.RegisterReadOnly
    //        (
    //            nameof(MuteImageSource), typeof(ImageSource), typeof(TrackControl), new FrameworkPropertyMetadata(null)
    //        );

    //    public static readonly DependencyProperty MuteImageSourceProperty = MuteImageSourcePropertyKey.DependencyProperty;

    //    public ImageSource ShiftLeftImageSource
    //    {
    //        get;
    //    }

    //    public ImageSource ShiftRightImageSource
    //    {
    //        get;
    //    }

    //    #endregion

    //    /************************************************************************/

    //    #region Internal Properties
    //    #endregion

    //    /************************************************************************/

    //    #region Constructors
    //    internal TrackControl(int beats, int stepsPerBeat, AudioPiece piece, TrackContainer owner)
    //        : base(TrackBoxType.TrackStep)
    //    {
    //        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
    //        submixVoice = new SubmixVoice(AudioHost.Instance.AudioDevice);
    //        submixVoice.SetOutputVoices(new VoiceSendDescriptor(Owner.SubmixVoice));
    //        Loaded += TrackControlLoaded;
    //        Piece = piece;

    //        Commands.Add("ToggleMute", new RelayCommand(RunToggleMuteCommand));
    //        Commands.Add("ShiftLeft", new RelayCommand(RunShiftLeftCommand));
    //        Commands.Add("ShiftRight", new RelayCommand(RunShiftRightCommand));
    //        Commands.Add("ViewTrackProps", new RelayCommand(RunViewTrackProps));

    //        MuteImageSource = imageVoiced = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Track.Voiced.64.png", UriKind.Relative));
    //        imageMuted = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Track.Muted.64.png", UriKind.Relative));

    //        ShiftLeftImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Shift.Left.64.png", UriKind.Relative));
    //        ShiftRightImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Shift.Right.64.png", UriKind.Relative));


    //    }

    //    static TrackControl()
    //    {
    //        DefaultStyleKeyProperty.OverrideMetadata(typeof(TrackControl), new FrameworkPropertyMetadata(typeof(TrackControl)));
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Public methods
    //    /// <summary>
    //    /// Called when the template is applied.
    //    /// </summary>
    //    public override void OnApplyTemplate()
    //    {
    //        base.OnApplyTemplate();
    //        gridTrack = GetTemplateChild(PartGridTrack) as Grid;

    //        //envelopeHost = GetTemplateChild(PartEnvelopeHost) as Grid;
    //        //if (envelopeHost != null)
    //        //{
    //        //    envelopeHost.Children.Clear();
    //        //    volEnvelop = new EnvelopeControl();
    //        //    envelopeHost.Children.Add(volEnvelop);
    //        //}
    //        OnBoxSizeChanged();
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Protected methods

    //    ///// <summary>
    //    ///// Called when the box size is changed.
    //    ///// </summary>
    //    //protected override void OnBoxSizeChanged()
    //    //{
    //    //    base.OnBoxSizeChanged();
    //    //    if (volEnvelop != null)
    //    //    {
    //    //        volEnvelop.BoxSize = BoxSize;
                
    //    //    }
    //    //}

    //    //protected override void OnTotalStepsChanged(Grid hostGrid, int totalSteps)
    //    //{
    //    //    base.OnTotalStepsChanged(hostGrid, totalSteps);
    //    //    if (volEnvelop != null)
    //    //    {
    //    //        volEnvelop.TotalSteps = TotalSteps;
    //    //    }
    //    //}

    //    /// <summary>
    //    /// Called when <see cref="Volume"/> is changed.
    //    /// </summary>
    //    protected override void OnVolumeChanged()
    //    {
    //        if (submixVoice != null)
    //        {
    //            submixVoice.SetVolume(VolumeInternal);
    //        }
    //    }

    //    /// <summary>
    //    /// Called when the <see cref="IsMuted"/> property is changed.
    //    /// </summary>
    //    protected override void OnIsMutedChanged()
    //    {
    //        MuteImageSource = (IsMuted) ? imageMuted : imageVoiced;
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Internal methods
    //    internal void UpdateTrack(int totalSteps)
    //    {
    //        OnTotalStepsChanged(gridTrack, totalSteps);
    //    }

    //    internal void Play(int step, int operationSet)
    //    {
    //        if (isAudioEnabled && !IsUserMuted && !IsAutoMuted && step < Boxes.Count)
    //        {
    //            try
    //            {
    //                if (Boxes[step].IsSelectedInternal)
    //                {
    //                    //float vol = (step % 5 == 0) ? 1.0F : 0.45F;
    //                    //piece.Play(steps[step].VolumeInternal, operationSet);
    //                    // vol = Boxes[step].VolumeInternal;
    //                    //voicePool.Play(vol, PitchInternal, operationSet);
    //                    voicePool.Play(Boxes[step].VolumeInternal, PitchInternal, operationSet);

    //                }
    //            }
    //            catch { }
    //        }
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Private methods (Instance)
    //    private void TrackControlLoaded(object sender, RoutedEventArgs e)
    //    {
    //        UpdateTrack(Owner.Beats * Owner.StepsPerBeat);
    //        //BoxSize = Owner.BoxSize;
    //    }

    //    private void RunToggleMuteCommand(object parm)
    //    {
    //        IsMuted = !IsMuted;
    //    }

    //    private void RunShiftLeftCommand(object parm)
    //    {
    //        bool firstBoxSelected = Boxes[0].IsSelected;
    //        for (int k = 0; k < TotalSteps - 1; k++)
    //        {
    //            Boxes[k].IsSelected = Boxes[k + 1].IsSelected;
    //        }
    //        Boxes[TotalSteps -1].IsSelected = firstBoxSelected;
    //    }

    //    private void RunShiftRightCommand(object parm)
    //    {
    //        bool lastBoxSelected = Boxes[TotalSteps - 1].IsSelected;
    //        for (int k= TotalSteps -1; k > 0; k--)
    //        {
    //            Boxes[k].IsSelected = Boxes[k - 1].IsSelected;
    //        }
    //        Boxes[0].IsSelected = lastBoxSelected;
    //    }

    //    private void RunViewTrackProps(object parm)
    //    {
    //        Debug.WriteLine("view props");
    //        Window window = new Window
    //        {
    //            ShowInTaskbar = false,
    //            ResizeMode = ResizeMode.NoResize,
    //            Height = 400,
    //            Width = 640,
    //            Topmost = true
               
    //        };
    //        window.Show();
    //    }

    //    private void OnPieceChanged()
    //    {
    //        IsAudioEnabled = isAudioEnabled = (Piece != null && Piece.IsAudioInitialized);
    //        if (IsAudioEnabled)
    //        {
    //            AudioHost.Instance.DestroyVoicePool(voicePool);
    //            voicePool = AudioHost.Instance.CreateVoicePool(Piece.Audio, submixVoice);
    //        }

    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Private methods (Static)

    //    private static void OnPieceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is TrackControl c)
    //        {
    //            c.OnPieceChanged();
    //        }
    //    }


    //    #endregion
    //}
}
