using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls.Obsolete
{
    /// <summary>
    /// Represents the topmost container for a track layout control.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This control is the top level control for a series of tracks and controllers that represent a single drum beat
    /// comprised of different instruments. It contains the upper controls such as tempo, beats, steps per beat,
    /// master volume, etc. and a series of tracks. Each track contains a track controller that manages the instrument
    /// for the track, the track volume, pan, pitch, etc., and a track box container that holds the individual
    /// beats the user can select.
    /// </para>
    /// <para>
    /// This control is responsible for creating tracks, for managing the background thread that plays the audio, and for providing
    /// rendering services, i.e, saving the rhythm as a looped wave file.
    /// </para>
    /// <para>
    /// Upon initialization, this control creates a series of default tracks. The user can add more.
    /// </para>
    /// </remarks>
    [TemplatePart(Name = PartHeaderScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PartTrackScrollViewer, Type = typeof(ScrollViewer))]
    public class TrackContainer : TrackStepControl
    {
        #region Private
        private const string PartHeaderScrollViewer = "PART_HeaderScrollViewer";
        private const string PartTrackScrollViewer = "PART_TrackScrollViewer";
        private const string DefaultCounterText = "00:00";
        private const string DefaultPassText = "000";
        private const int MilliSecondsPerMinute = 60000;
        private AutoResetEvent playSignaler;
        private AutoResetEvent endPlaySignaler;
        private Thread playThread;
        private bool isStarted;
        private int totalSteps = TrackVals.TotalSteps.Default;
        private int sleepTime;
        private bool isControlClosing;
        private int operationSet;
        private Metronome metronome;
        private int maxRenderPass;
        private bool isRendering;
        private Stopwatch loopTimer;
        private ScrollViewer headerScrollViewer;
        private ScrollViewer trackScrollViewer;
        #endregion

        /************************************************************************/

        #region Tracks
        /// <summary>
        /// Gets the collection of <see cref="CompositeTrack"/> objects.
        /// </summary>
        public MaxSizeObservableCollection<CompositeTrack> Tracks
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Header boxes
        /// <summary>
        /// Gets the header boxes for the container
        /// </summary>
        public TrackBoxContainerHeader HeaderBoxes
        {
            get => (TrackBoxContainerHeader)GetValue(HeaderBoxesProperty);
            private set => SetValue(HeaderBoxesPropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey HeaderBoxesPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(HeaderBoxes), typeof(TrackBoxContainerHeader), typeof(TrackContainer), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="HeaderBoxes"/> dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderBoxesProperty = HeaderBoxesPropertyKey.DependencyProperty;
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
                nameof(Tempo), typeof(double), typeof(TrackContainer), new PropertyMetadata(TrackVals.Tempo.Default, OnTempoChanged, OnTempoCoerce)
            );

        private static void OnTempoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackContainer c)
            {
                c.CalculateThreadSafeSleepTime();
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
                nameof(TempoText), typeof(string), typeof(TrackContainer), new PropertyMetadata(TrackVals.Tempo.DefaultText)
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

        #region Metronome
        /// <summary>
        /// Gets or sets a value that indicates if the metronome is active.
        /// </summary>
        public bool IsMetronomeActive
        {
            get => (bool)GetValue(IsMetronomeActiveProperty);
            set => SetValue(IsMetronomeActiveProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsMetronomeActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMetronomeActiveProperty = DependencyProperty.Register
            (
                nameof(IsMetronomeActive), typeof(bool), typeof(TrackContainer), new PropertyMetadata(false, OnIsMetronomeActiveChanged)
            );

        private static void OnIsMetronomeActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackContainer c)
            {
                c.metronome.IsActive = c.IsMetronomeActive;
            }
        }
        #endregion

        /************************************************************************/

        #region Render
        /// <summary>
        /// Gets a value that determines if render is requested.
        /// </summary>
        public bool IsRenderRequestMode
        {
            get => (bool)GetValue(IsRenderRequestModeProperty);
            private set => SetValue(IsRenderRequestModePropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey IsRenderRequestModePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(IsRenderRequestMode), typeof(bool), typeof(TrackContainer), new PropertyMetadata(false, OnIsRenderRequestModeChanged)
            );

        /// <summary>
        /// Identifies the <see cref="IsRenderRequestMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRenderRequestModeProperty = IsRenderRequestModePropertyKey.DependencyProperty;


        private static void OnIsRenderRequestModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackContainer c)
            {
                c.OnRenderImageSourceChanged();
                c.OnIsRenderRequestModeChanged();
            }
        }

        /// <summary>
        /// Gets or sets a command that is executed when a render has been requested.
        /// See remarks for more.
        /// </summary>
        /// <remarks>
        /// Assign a command to this property to start the rending process by creating the 
        /// rendering parms and then calling <see cref="StartRender()"/>.
        /// If your command handler decides not to render (for instance, user cancels), simply 
        /// return without starting the render. Once a render is started, it runs to completion
        /// at which time the <see cref="RenderCompleted"/> event is raised.
        /// </remarks>
        public ICommand RequestRenderCommand
        {
            get => (ICommand)GetValue(RequestRenderCommandProperty);
            set => SetValue(RequestRenderCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RequestRenderCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RequestRenderCommandProperty = DependencyProperty.Register
            (
                nameof(RequestRenderCommand), typeof(ICommand), typeof(TrackContainer), new PropertyMetadata(null)
            );
        #endregion

        /************************************************************************/

        #region Images (Start / stop [has state change])
        /// <summary>
        /// Gets or sets the image source to use for the play button when <see cref="IsStarted"/> is false.
        /// </summary>
        public ImageSource StartImageSource
        {
            get => (ImageSource)GetValue(StartImageSourceProperty);
            set => SetValue(StartImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StartImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartImageSourceProperty = DependencyProperty.Register
            (
                nameof(StartImageSource), typeof(ImageSource), typeof(TrackContainer), new PropertyMetadata(null, OnPlayImageSourceChanged)
            );

        /// <summary>
        /// Gets or sets the image source to use for the play button when <see cref="IsStarted"/> is true.
        /// </summary>
        public ImageSource StopImageSource
        {
            get => (ImageSource)GetValue(StopImageSourceProperty);
            set => SetValue(StopImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StopImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StopImageSourceProperty = DependencyProperty.Register
            (
                nameof(StopImageSource), typeof(ImageSource), typeof(TrackContainer), new PropertyMetadata(null, OnPlayImageSourceChanged)
            );

        private static void OnPlayImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackContainer c)
            {
                c.OnPlayImageSourceChanged();
            }
        }

        /// <summary>
        /// Gets image source that is currently active (depends on started/stopped state)
        /// </summary>
        public ImageSource ActivePlayImageSource
        {
            get => (ImageSource)GetValue(ActivePlayImageSourceProperty);
            private set => SetValue(ActivePlayImageSourcePropertyKey, value);
        }

        private static readonly DependencyPropertyKey ActivePlayImageSourcePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ActivePlayImageSource), typeof(ImageSource), typeof(TrackContainer), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ActivePlayImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActivePlayImageSourceProperty = ActivePlayImageSourcePropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Images (Add track)
        /// <summary>
        /// Gets or sets the image source to use for the add track button.
        /// </summary>
        public ImageSource AddTrackImageSource
        {
            get => (ImageSource)GetValue(AddTrackImageSourceProperty);
            set => SetValue(AddTrackImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="AddTrackImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AddTrackImageSourceProperty = DependencyProperty.Register
            (
                nameof(AddTrackImageSource), typeof(ImageSource), typeof(TrackContainer), new PropertyMetadata(null)
            );
        #endregion

        /************************************************************************/

        #region Images (Close)
        /// <summary>
        /// Gets or sets the image source to use for the close button
        /// </summary>
        public ImageSource CloseImageSource
        {
            get => (ImageSource)GetValue(CloseImageSourceProperty);
            set => SetValue(CloseImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CloseImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CloseImageSourceProperty =
            DependencyProperty.Register
                (
                    nameof(CloseImageSource), typeof(ImageSource), typeof(TrackContainer), new PropertyMetadata(null)
                );
        #endregion

        /************************************************************************/

        #region Images (Render [has state change])
        /// <summary>
        /// Gets or sets the image source to use for the render button.
        /// </summary>
        public ImageSource RenderImageSource
        {
            get => (ImageSource)GetValue(RenderImageSourceProperty);
            set => SetValue(RenderImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RenderImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RenderImageSourceProperty = DependencyProperty.Register
            (
                nameof(RenderImageSource), typeof(ImageSource), typeof(TrackContainer), new PropertyMetadata(null, OnRenderImageSourceChanged)
            );
        
        /// <summary>
        /// Gets or sets the image source to use for thr render button when the render functionality has been activated.
        /// </summary>
        public ImageSource ActivatedRenderImageSource
        {
            get => (ImageSource)GetValue(ActivatedRenderImageSourceProperty);
            set => SetValue(ActivatedRenderImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ActivatedRenderImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActivatedRenderImageSourceProperty = DependencyProperty.Register
            (
                nameof(ActivatedRenderImageSource), typeof(ImageSource), typeof(TrackContainer), new PropertyMetadata(null, OnRenderImageSourceChanged)
            );

        /// <summary>
        /// Gets image source that is currently active for render (depends on render state)
        /// </summary>
        public ImageSource ActiveRenderImageSource
        {
            get => (ImageSource)GetValue(ActiveRenderImageSourceProperty);
            private set => SetValue(ActiveRenderImageSourcePropertyKey, value);
        }

        private static readonly DependencyPropertyKey ActiveRenderImageSourcePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ActiveRenderImageSource), typeof(ImageSource), typeof(TrackContainer), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ActiveRenderImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveRenderImageSourceProperty = ActiveRenderImageSourcePropertyKey.DependencyProperty;

        private static void OnRenderImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackContainer c)
            {
                c.OnRenderImageSourceChanged();
            }
        }





        #endregion

        /************************************************************************/

        #region Images (Metronone)
        /// <summary>
        /// Gets or sets the image to use for the metronone
        /// </summary>
        public ImageSource MetronomeImageSource
        {
            get => (ImageSource)GetValue(MetronomeImageSourceProperty);
            set => SetValue(MetronomeImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MetronomeImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MetronomeImageSourceProperty = DependencyProperty.Register
            (
                nameof(MetronomeImageSource), typeof(ImageSource), typeof(TrackContainer), new PropertyMetadata(null)
            );
        #endregion

        /************************************************************************/

        #region Public properties (Other)
        ///// <summary>
        ///// Gets or sets the display name for this track layout
        ///// </summary>
        //public string DisplayName
        //{
        //    get => (string)GetValue(DisplayNameProperty);
        //    set => SetValue(DisplayNameProperty, value);
        //}

        ///// <summary>
        ///// Identifies the <see cref="DisplayName"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register
        //    (
        //        nameof(DisplayName), typeof(string), typeof(TrackContainer), new PropertyMetadata(null, OnDisplayNameChanged)
        //    );

        //private static void OnDisplayNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is TrackContainer c)
        //    {
        //        c.SetIsChanged();
        //    }
        //}

        /// <summary>
        /// Gets the current file name for the container, or null if none.
        /// This is the .xml file
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the audio render parameters
        /// </summary>
        public AudioRenderParameters RenderParms
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Public properties (read only)
        /// <summary>
        /// Gets a boolean value that indicates if the track layout is started (i.e. playing its pattern)
        /// </summary>
        public bool IsStarted
        {
            get => (bool)GetValue(IsStartedProperty);
            private set => SetValue(IsStartedPropertyKey, value);
        }

        private static readonly DependencyPropertyKey IsStartedPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(IsStarted), typeof(bool), typeof(TrackContainer), new FrameworkPropertyMetadata(false, OnIsStartedChanged)
            );

        /// <summary>
        /// Identifies the <see cref="IsStarted"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsStartedProperty = IsStartedPropertyKey.DependencyProperty;

        private static void OnIsStartedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackContainer c)
            {
                c.OnIsStartedChanged();
                c.OnPlayImageSourceChanged();
            }
        }

        /// <summary>
        /// Gets the counter text
        /// </summary>
        public string CounterText
        {
            get => (string)GetValue(CounterTextProperty);
            private set => SetValue(CounterTextPropertyKey, value);
        }

        private static readonly DependencyPropertyKey CounterTextPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(CounterText), typeof(string), typeof(TrackContainer), new FrameworkPropertyMetadata(DefaultCounterText)
            );

        /// <summary>
        /// Identifies the <see cref="CounterText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CounterTextProperty = CounterTextPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the margin for the header boxes
        /// </summary>
        public Thickness HeaderMargin
        {
            get => (Thickness)GetValue(HeaderMarginProperty);
            private set => SetValue(HeaderMarginPropertyKey, value);
        }

        private static readonly DependencyPropertyKey HeaderMarginPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(HeaderMargin), typeof(Thickness), typeof(TrackContainer), new PropertyMetadata(new Thickness(0))
            );

        /// <summary>
        /// Identifies the <see cref="HeaderMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderMarginProperty = HeaderMarginPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the pass text
        /// </summary>
        public string PassText
        {
            get => (string)GetValue(PassTextProperty);
            private set => SetValue(PassTextPropertyKey, value);
        }

        private static readonly DependencyPropertyKey PassTextPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(PassText), typeof(string), typeof(TrackContainer), new FrameworkPropertyMetadata(DefaultPassText)
            );

        /// <summary>
        /// Identifies the <see cref="PassText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PassTextProperty = PassTextPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Internal properties
        /// <summary>
        /// From this assembly, gets the track container's submix voice. All tracks route through this voice.
        /// </summary>
        internal SubmixVoice SubmixVoice
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Routed events
        /// <summary>
        /// Represents a routed event that is raised when the <see cref="TrackContainer"/> is closing.
        /// </summary>
        public event CancelRoutedEventHandler Closing
        {
            add => AddHandler(ClosingEvent, value);
            remove => RemoveHandler(ClosingEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="Closing"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ClosingEvent = EventManager.RegisterRoutedEvent
            (
                nameof(Closing), RoutingStrategy.Bubble, typeof(CancelRoutedEventHandler), typeof(TrackContainer)
            );

        #endregion

        /************************************************************************/

        #region CLR events
        /// <summary>
        /// Raised when a render operation has completed.
        /// This event is raised on the background play thread.
        /// </summary>
        public event EventHandler<AudioRenderEventArgs> RenderCompleted;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of <see cref="TrackContainer"/>.
        /// </summary>
        public TrackContainer()
        {
            AddTrackImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Track.Add.64.png", UriKind.Relative));
            StartImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Start.64.png", UriKind.Relative));
            StopImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Stop.64.png", UriKind.Relative));
            ActivePlayImageSource = StartImageSource;

            RenderImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Render.64.png", UriKind.Relative));
            ActivatedRenderImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Render.Active.64.png", UriKind.Relative));
            ActiveRenderImageSource = RenderImageSource;

            CloseImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Close.64.png", UriKind.Relative));
            MetronomeImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Metronome.64.png", UriKind.Relative));

            RenderParms = AudioRenderParameters.CreateDefault();

            loopTimer = new Stopwatch();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                HeaderBoxes = new TrackBoxContainerHeader(this)
                {
                    Padding = new Thickness(4),
                    Beats = Beats,
                    StepsPerBeat = StepsPerBeat,
                };

                Tracks = new MaxSizeObservableCollection<CompositeTrack>(TrackVals.Track.Max);
                SubmixVoice = new SubmixVoice(AudioHost.Instance.AudioDevice);

                metronome = new Metronome(this);

                Commands.Add("Close", new RelayCommand(RunCloseCommand));
                Commands.Add("AddTrack", new RelayCommand(RunAddTrackCommand, CanRunAddTrackCommand));
                Commands.Add("Play", new RelayCommand(RunPlayCommand));
                Commands.Add("Render", new RelayCommand(RunRequestRenderCommand));
                Commands.Add("ToggleMute", new RelayCommand(RunToggleMuteCommand));
                Commands.Add("ToggleMetronome", new RelayCommand(RunToggleMetronomeCommand));

                endPlaySignaler = new AutoResetEvent(false);
                playSignaler = new AutoResetEvent(false);
                playThread = new Thread(PlayThreadHandler)
                {
                    Name = "TrackContainerPlay"
                };
                playThread.Start();
                Dispatcher.ShutdownStarted += DispatcherShutdownStarted;
            }
        }

        static TrackContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TrackContainer), new FrameworkPropertyMetadata(typeof(TrackContainer)));
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
            // Get the references from the template
            headerScrollViewer = GetTemplateChild(PartHeaderScrollViewer) as ScrollViewer;
            trackScrollViewer = GetTemplateChild(PartTrackScrollViewer) as ScrollViewer;

            if (trackScrollViewer != null) trackScrollViewer.ScrollChanged -= TrackScrollViewerScrollChanged;
            if (trackScrollViewer != null) trackScrollViewer.ScrollChanged += TrackScrollViewerScrollChanged;

            IsChangedSet -= TrackContainerIsChangedSet;
            IsChangedReset -= TrackContainerIsChangedReset;
            IsChangedSet += TrackContainerIsChangedSet;
            IsChangedReset += TrackContainerIsChangedReset;

            if (Tracks != null && Tracks.Count == 0)
            {
                AddTrack(InstrumentType.Cymbal);
                AddTrack(InstrumentType.HighHat);
                AddTrack(InstrumentType.Snare);
                AddTrack(InstrumentType.Tom);
                AddTrack(InstrumentType.Kick);
            }

            metronome.Piece = AudioHost.Instance.GetAudioPiece(InstrumentType.Percussion);
            metronome.UpdateBeatValues(Beats, StepsPerBeat);

            IsStarted = false;
            CalculateThreadSafeSleepTime();
            OnVolumeChanged();
            OnMutedImageSourceChanged();
        }

        /// <summary>
        /// Creates an XDocument representation of the container and saves it to the specified file.
        /// </summary>
        /// <returns>The XDocument object</returns>
        public void Save(string filename)
        {
            try
            {
                DisplayName = FileName = filename;
                var xe = GetXElement();
                System.IO.File.WriteAllText(filename, xe.ToString());
                ResetIsChanged();
            }
            catch (Exception ex)
            {
                throw new System.IO.IOException($"Unable to save {filename}", ex);
            }
        }

        /// <summary>
        /// Opens the specified file and sets all tracks and values according to the contents.
        /// </summary>
        /// <param name="filename">The file name</param>
        public void Open(string filename)
        {
            try
            {
                Tracks.Clear();
                XDocument doc = XDocument.Load(filename);
                RestoreFromXElement(doc.Root);
                ResetIsChanged();
                DisplayName = FileName = filename;
            }
            catch (Exception ex)
            {
                throw new System.IO.IOException($"Unable to load {filename}", ex);
            }
        }

        /// <summary>
        /// Starts the rendering process. This method may only be called when <see cref="IsRenderRequestMode"/> equals true.
        /// </summary>
        public void StartRender() 
        {
            if (!IsRenderRequestMode) throw new InvalidOperationException("Render request mode must be activated before calling this method");
            RenderParms.Validate();

            if (RenderParms.IsChanged)
            {
                SetIsChanged();
            }

            AudioHost.Instance.AudioCapture.RenderParms = RenderParms;

            maxRenderPass = GetMaxRenderPass();
            isRendering = true;
            IsStarted = true;
        }

        /// <summary>
        /// Shuts down the track container.
        /// </summary>
        public void Shutdown()
        {
            StopPlayThread();
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
            var element = new XElement(nameof(TrackContainer));
            element.Add(new XElement(nameof(DisplayName), DisplayName));
            element.Add(new XElement(nameof(Volume), Volume));
            element.Add(new XElement(nameof(Tempo), Tempo));
            element.Add(new XElement(nameof(Beats), Beats));
            element.Add(new XElement(nameof(StepsPerBeat), StepsPerBeat));
            element.Add(new XElement(nameof(BoxSize), BoxSize));
            element.Add(RenderParms.GetXElement());

            Tracks.DoForAll((track) =>
            {
                element.Add(track.GetXElement());
            });
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            IEnumerable<XElement> childList = from el in element.Elements()  select el;

            foreach (XElement e in childList)
            {
                if (e.Name == nameof(DisplayName)) SetDependencyProperty(DisplayNameProperty, e.Value);
                if (e.Name == nameof(Volume)) SetDependencyProperty(VolumeProperty, e.Value);
                if (e.Name == nameof(Tempo)) SetDependencyProperty(TempoProperty, e.Value);
                if (e.Name == nameof(Beats)) SetDependencyProperty(BeatsProperty, e.Value);
                if (e.Name == nameof(StepsPerBeat)) SetDependencyProperty(StepsPerBeatProperty, e.Value);
                if (e.Name == nameof(BoxSize)) SetDependencyProperty(BoxSizeProperty, e.Value);
                if (e.Name == nameof(AudioRenderParameters))
                {
                    RenderParms.RestoreFromXElement(e);
                    AudioHost.Instance.AudioCapture.RenderParms = RenderParms;
                }

                // For backward compatibility
                if (e.Name == nameof(TrackController))
                {
                    AddTrack(null);
                    OnBoxSizeChanged();
                    OnTotalStepsChanged();
                    Tracks[Tracks.Count - 1].Controller.RestoreFromXElement(e);
                    IEnumerable<XElement> subChildList = from subE in e.Elements() select subE;
                    foreach (XElement ee in subChildList)
                    {
                        if (ee.Name == "TrackBoxContainer")
                        {
                            Tracks[Tracks.Count - 1].BoxContainer.RestoreFromXElement(ee);
                        }
                    }
                }

                // For new format
                if (e.Name == nameof(CompositeTrack))
                {
                    AddTrack(null);
                    Tracks[Tracks.Count - 1].RestoreFromXElement(e);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when <see cref="AudioControlBase.Volume"/> property is changed.
        /// </summary>
        protected override void OnVolumeChanged()
        {
            base.OnVolumeChanged();
            if (SubmixVoice != null)
            {
                SubmixVoice.SetVolume(ThreadSafeVolume);
            }
        }

        /// <summary>
        /// Called when either <see cref="AudioControlBase.MutedImageSource"/> or <see cref="AudioControlBase.VoicedImageSource"/> are changed.
        /// </summary>
        protected override void OnMutedImageSourceChanged()
        {
            base.OnMutedImageSourceChanged();
            if (Tracks != null)
            {
                Tracks.DoForAll((track) =>
                {
                    track.Controller.MutedImageSource = MutedImageSource;
                    track.Controller.VoicedImageSource = VoicedImageSource;
                });
            }
        }

        /// <summary>
        /// Called when the <see cref="ControlObject.IsExpanded"/> property changes.
        /// Expands / contracts all tracks
        /// </summary>
        protected override void OnIsExpandedChanged()
        {
            Tracks.DoForAll((track) =>
            {
                track.Controller.IsExpanded = IsExpanded;
            });
        }

        /// <summary>
        /// Called when the <see cref="TrackStepControl.TotalSteps"/> property changes.
        /// </summary>
        protected override void OnTotalStepsChanged()
        {
            // save a thread safe local value of total steps
            totalSteps = TotalSteps;

            HeaderBoxes.SetBeats(Beats, StepsPerBeat);

            Tracks.DoForAll((track) =>
            {
                track.BoxContainer.SetBeats(Beats, StepsPerBeat);
            });
            metronome.UpdateBeatValues(Beats, StepsPerBeat);
        }

        /// <summary>
        /// Called when the <see cref="TrackStepControl.BoxSize"/> property changes.
        /// </summary>
        protected override void OnBoxSizeChanged()
        {
            HeaderBoxes.BoxSize = BoxSize;

            Tracks.DoForAll((track) =>
            {
                track.BoxContainer.BoxSize = BoxSize;
            });
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Removes the specified track.
        /// </summary>
        /// <param name="track">The track controller</param>
        internal void RemoveTrack(CompositeTrack track)
        {
            if (track == null) throw new ArgumentNullException(nameof(track));
            int idx = Tracks.IndexOf(track);
            if (idx < 0) throw new ArgumentException("The specified track is not included in the collection");
            Tracks.RemoveAt(idx);
            SetIsChanged();
        }

        /// <summary>
        /// Moves the specified tracks
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="target">The target</param>
        internal void MoveTracks(CompositeTrack source, CompositeTrack target)
        {
            if (source != null && target != null)
            {
                int idxSource = Tracks.IndexOf(source);
                int idxTarget = Tracks.IndexOf(target);
                if (idxSource >= 0 && idxTarget >= 0)
                {
                    Tracks.Move(idxSource, idxTarget);
                    SetIsChanged();
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (Instance)

        private void RunCloseCommand(object parm)
        {
            CancelRoutedEventArgs args = new CancelRoutedEventArgs(ClosingEvent);
            RaiseEvent(args);
            if (!args.Cancel)
            {
                StopPlayThread();
            }
        }

        private void RunAddTrackCommand(object parm)
        {
            if (CanRunAddTrackCommand(null))
            {
                // TODO - Add a track by selecting a type first?
                AddTrack(InstrumentType.Kick);
                SetIsChanged();
            }
        }

        private void AddTrack(Instrument piece)
        {
            Tracks.Add(new CompositeTrack(this, piece));
        }

        private void AddTrack(InstrumentType type)
        {
            Instrument piece = AudioHost.Instance.GetAudioPiece(type);
            AddTrack(piece);
        }

        private bool CanRunAddTrackCommand(object parm)
        {
            return Tracks.Count < TrackVals.Track.Max;
        }

        private void RunPlayCommand(object parm)
        {
            IsStarted = !IsStarted;
        }

        private void RunRequestRenderCommand(object parm)
        {
            // Simply toggles the property. All other action is handled by OnIsRenderRequestModeChanged()
            IsRenderRequestMode = !IsRenderRequestMode;
        }

        private void OnRenderImageSourceChanged()
        {
            ActiveRenderImageSource = IsRenderRequestMode ? ActivatedRenderImageSource : RenderImageSource;
        }

        private void OnIsRenderRequestModeChanged()
        {
            IsStarted = false;
            if (IsRenderRequestMode)
            {
                if (RequestRenderCommand != null && RequestRenderCommand.CanExecute(null))
                {
                    RequestRenderCommand.Execute(null);
                }
                IsRenderRequestMode = false;
            }
        }

        /// <summary>
        /// Gets the number of passes needed to perform a complete render.
        /// </summary>
        /// <returns></returns>
        private int GetMaxRenderPass()
        {
            int maxPass = 1;
            foreach (CompositeTrack track in Tracks)
            {
                if (!track.Controller.IsMuted)
                {
                    foreach (TrackBox box in track.BoxContainer.Boxes)
                    {
                        switch (box.PlayFrequency)
                        {
                            case StepPlayFrequency.OddPass:
                            case StepPlayFrequency.SecondPass:
                                maxPass = Math.Max(maxPass, 2);
                                break;
                            case StepPlayFrequency.ThirdPass3:
                                maxPass = Math.Max(maxPass, 3);
                                break;
                            case StepPlayFrequency.ThirdPass4:
                            case StepPlayFrequency.FourthPass:
                                maxPass = Math.Max(maxPass, 4);
                                break;
                        }
                    }
                }
            }
            return maxPass;
        }

        private void RunToggleMuteCommand(object parm)
        {
            IsMuted = !IsMuted;
        }

        private void RunToggleMetronomeCommand(object parm)
        {
            IsMetronomeActive = !IsMetronomeActive;
        }

        private void PlayThreadHandler()
        {
            while (!isControlClosing)
            {
                playSignaler.WaitOne();
                int pass = 1;

                if (isRendering)
                {
                    AudioHost.Instance.AudioCapture.StartCapture();
                }

                if (!isControlClosing)
                {
                    while (isStarted)
                    {
                        loopTimer.Restart();
                        int step = 0;
                        while (isStarted && step < totalSteps)
                        {
                            PlayOneStep(pass,step++);
                            Thread.Sleep(sleepTime);
                        }

                        ClearAllSteps();

                        loopTimer.Stop();
                        Debug.WriteLine($"{pass}. {loopTimer.ElapsedMilliseconds}");

                        if (isRendering && pass == maxRenderPass)
                        {
                            isStarted = false;
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback
                                ((args) =>
                                {
                                    IsStarted = false;
                                    return null;
                                }), null);

                            AudioHost.Instance.AudioCapture.FadeAndStopCapture();
                            isRendering = false;
                            RenderCompleted?.Invoke(this, new AudioRenderEventArgs(AudioHost.Instance.AudioCapture.RenderParms));
                        }

                        pass++;
                    }
                }
            }
            endPlaySignaler.Set();
        }

        private void PlayOneStep(int pass, int step)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback
                ((args) =>
                {
                    HeaderBoxes.Boxes[step].PlayFrequency = StepPlayFrequency.EveryPass;
                    int subDiv = (step % StepsPerBeat) + 1;
                    int beat = (step / StepsPerBeat) + 1;
                    PassText = $"00{pass}";
                    CounterText = $"0{beat}:0{subDiv}";
                    return null;
                }), null);

            if (!IsUserMuted && !IsAutoMuted)
            {
                operationSet++;
                metronome.Play(step, operationSet);

                Tracks.DoForAll((track) =>
                {
                    track.ThreadSafeController.Play(pass, step, operationSet);
                });

                AudioHost.Instance.AudioDevice.CommitChanges(operationSet);
            }
        }

        private void ClearAllSteps()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback
                ((args) =>
                {
                    HeaderBoxes.DeselectAllBoxes();
                    PassText = DefaultPassText;
                    CounterText = DefaultCounterText;
                    return null;
                }), null);
        }

        private void StopPlayThread()
        {
            IsStarted = false;
            isControlClosing = true;
            bool isClosed = playSignaler.SafeWaitHandle.IsClosed;
            if (!playSignaler.SafeWaitHandle.IsClosed)
            {
                playSignaler.Set();
                playSignaler.Dispose();
            }
            if (endPlaySignaler != null)
            {
                endPlaySignaler.WaitOne();
                endPlaySignaler.Dispose();
                endPlaySignaler = null;
            }
        }

        private void OnIsStartedChanged()
        {
            isStarted = IsStarted;
            if (isStarted)
            {
                playSignaler.Set();
            }
        }

        private void OnPlayImageSourceChanged()
        {
            ActivePlayImageSource = IsStarted ? StopImageSource : StartImageSource;
        }

        private void CalculateThreadSafeSleepTime()
        {
            sleepTime = MilliSecondsPerMinute / (int)Tempo / 4; 
        }

        private void DispatcherShutdownStarted(object sender, EventArgs e)
        {
            StopPlayThread();
        }

        private void TrackContainerIsChangedSet(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() != GetType())
            {
                SetIsChanged();
            }
        }

        private void TrackContainerIsChangedReset(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() != GetType())
            {
                ResetIsChanged();
            }
        }

        private void TrackScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (headerScrollViewer != null)
            {
                if (trackScrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
                {
                    HeaderMargin = new Thickness(0, 0, 17, 0);
                }
                else
                {
                    HeaderMargin = new Thickness(0);
                }

                headerScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
        }
        #endregion
    }
}
