﻿using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents the topmost container for a track layout control.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This control is the top level control for a series of tracks and controllers
    /// that represent a single drum beat. It contains the upper controls such as tempo, beats, steps per beat,
    /// master volume, etc. and a series of paired track controller / track box containers. Each track controller manages
    /// the instrument for the track, the track volume, pan, pitch, etc. Each track box container holds the individual
    /// beats.
    /// </para>
    /// <para>
    /// This control is responsible for creating the controllers and the track box containers. It creates a series
    /// of default tracks. The user can add more.
    /// </para>
    /// </remarks>
    [TemplatePart(Name = PartHeaderBoxes, Type = typeof(TrackBoxContainer))]
    public class TrackContainer : TrackSized
    {
        #region Private
        private const string PartHeaderBoxes = "PART_HEADER_BOXES";
        private const string DefaultCounterText = "00:00";
        private const string DefaultPassText = "000";
        private const int MilliSecondsPerMinute = 60000;
        private TrackBoxContainer headerBoxes;
        private AutoResetEvent playSignaler;
        private AutoResetEvent endPlaySignaler;
        private Thread playThread;
        private bool isStarted;
        private int totalSteps = TrackVals.TotalSteps.Default;
        private int sleepTime;
        private bool isControlClosing;
        private int operationSet;
        private Metronome metronome;
        #endregion

        /************************************************************************/

        #region Public fields
        #endregion

        /************************************************************************/

        #region Public properties (Tracks)
        /// <summary>
        /// Gets the track controllers
        /// </summary>
        public MaxSizeObservableCollection<TrackController> TrackControllers
        {
            get;
        }

        /// <summary>
        /// Gets the track boxes
        /// </summary>
        public MaxSizeObservableCollection<TrackBoxContainer> TrackBoxes
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Public properties (Tempo, beats, volume)
        /// <summary>
        /// Gets or sets the tempo
        /// </summary>
        public double Tempo
        {
            get => (double)GetValue(TempoProperty);
            set => SetValue(TempoProperty, value);
        }

        public static readonly DependencyProperty TempoProperty = DependencyProperty.Register
            (
                nameof(Tempo), typeof(double), typeof(TrackContainer), new PropertyMetadata(TrackVals.Tempo.Default, OnTempoChanged, OnTempoCoerce)
            );

        /// <summary>
        /// Gets or sets the tempo text descriptor
        /// </summary>
        public string TempoText
        {
            get => (string)GetValue(TempoTextProperty);
            set => SetValue(TempoTextProperty, value);
        }

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

        /// <summary>
        /// Gets or sets the number of beats
        /// </summary>
        public int Beats
        {
            get => (int)GetValue(BeatsProperty);
            set => SetValue(BeatsProperty, value);
        }

        public static readonly DependencyProperty BeatsProperty = DependencyProperty.Register
            (
                nameof(Beats), typeof(int), typeof(TrackContainer), new PropertyMetadata(TrackVals.Beats.Default, OnTotalStepsChanged, OnBeatsCoerce)
            );

        /// <summary>
        /// Gets or sets the beats text descriptor
        /// </summary>
        public string BeatsText
        {
            get => (string)GetValue(BeatsTextProperty);
            set => SetValue(BeatsTextProperty, value);
        }

        public static readonly DependencyProperty BeatsTextProperty = DependencyProperty.Register
            (
                nameof(BeatsText), typeof(string), typeof(TrackContainer), new PropertyMetadata(TrackVals.Beats.DefaultText)
            );

        /// <summary>
        /// Gets the minimum beats allowed. Used for binding in the control template.
        /// </summary>
        public int MinBeats
        {
            get => TrackVals.Beats.Min;
        }

        /// <summary>
        /// Gets the maximum beats allowed. Used for binding in the control template.
        /// </summary>
        public int MaxBeats
        {
            get => TrackVals.Beats.Max;
        }

        /// <summary>
        /// Gets or sets the number of steps per beat
        /// </summary>
        public int StepsPerBeat
        {
            get => (int)GetValue(StepsPerBeatProperty);
            set => SetValue(StepsPerBeatProperty, value);
        }

        public static readonly DependencyProperty StepsPerBeatProperty = DependencyProperty.Register
            (
                nameof(StepsPerBeat), typeof(int), typeof(TrackContainer), new PropertyMetadata(TrackVals.StepsPerBeat.Default, OnTotalStepsChanged, OnStepsPerBeatCoerce)
            );

        /// <summary>
        /// Gets or sets the steps per beat text descriptor
        /// </summary>
        public string StepsPerBeatText
        {
            get => (string)GetValue(StepsPerBeatTextProperty);
            set => SetValue(StepsPerBeatTextProperty, value);
        }

        public static readonly DependencyProperty StepsPerBeatTextProperty = DependencyProperty.Register
            (
                nameof(StepsPerBeatText), typeof(string), typeof(TrackContainer), new PropertyMetadata(TrackVals.StepsPerBeat.DefaultText)
            );

        /// <summary>
        /// Gets the minimum steps per beats allowed. Used for binding in the control template.
        /// </summary>
        public int MinStepsPerBeat
        {
            get => TrackVals.StepsPerBeat.Min;
        }

        /// <summary>
        /// Gets the maximum steps per beat allowed. Used for binding in the control template.
        /// </summary>
        public int MaxStepsPerBeat
        {
            get => TrackVals.StepsPerBeat.Max;
        }

        /// <summary>
        /// Gets the total number of steps.
        /// </summary>
        public int TotalSteps
        {
            get => (int)GetValue(TotalStepsProperty);
            private set => SetValue(TotalStepsPropertyKey, value);
        }

        private static readonly DependencyPropertyKey TotalStepsPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(TotalSteps), typeof(int), typeof(TrackContainer), new FrameworkPropertyMetadata(TrackVals.TotalSteps.Default)
            );

        public static readonly DependencyProperty TotalStepsProperty = TotalStepsPropertyKey.DependencyProperty;


        public bool IsMetronomeActive
        {
            get => (bool)GetValue(IsMetronomeActiveProperty);
            set => SetValue(IsMetronomeActiveProperty, value);
        }

        public static readonly DependencyProperty IsMetronomeActiveProperty = DependencyProperty.Register
            (
                nameof(IsMetronomeActive), typeof(bool), typeof(TrackContainer), new PropertyMetadata(false, OnIsMetronomeActiveChanged)
            );
        #endregion

        /************************************************************************/

        #region Public properties (Images)
        /// <summary>
        /// Gets or sets the image source to use for the play button when <see cref="IsStarted"/> is false.
        /// </summary>
        public ImageSource StartImageSource
        {
            get => (ImageSource)GetValue(StartImageSourceProperty);
            set => SetValue(StartImageSourceProperty, value);
        }

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

        public static readonly DependencyProperty StopImageSourceProperty = DependencyProperty.Register
            (
                nameof(StopImageSource), typeof(ImageSource), typeof(TrackContainer), new PropertyMetadata(null, OnPlayImageSourceChanged)
            );

        /// <summary>
        /// Gets or sets the image source to use for the add track button.
        /// </summary>
        public ImageSource AddTrackImageSource
        {
            get => (ImageSource)GetValue(AddTrackImageSourceProperty);
            set => SetValue(AddTrackImageSourceProperty, value);
        }

        public static readonly DependencyProperty AddTrackImageSourceProperty = DependencyProperty.Register
            (
                nameof(AddTrackImageSource), typeof(ImageSource), typeof(TrackContainer), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the image source to use for the close button
        /// </summary>
        public ImageSource CloseImageSource
        {
            get => (ImageSource)GetValue(CloseImageSourceProperty);
            set => SetValue(CloseImageSourceProperty, value);
        }

        public static readonly DependencyProperty CloseImageSourceProperty =
            DependencyProperty.Register
                (
                    nameof(CloseImageSource), typeof(ImageSource), typeof(TrackContainer), new PropertyMetadata(null)
                );

        /// <summary>
        /// Gets or sets the image to use for the metronone
        /// </summary>
        public ImageSource MetronomeImageSource
        {
            get => (ImageSource)GetValue(MetronomeImageSourceProperty);
            set => SetValue(MetronomeImageSourceProperty, value);
        }

        public static readonly DependencyProperty MetronomeImageSourceProperty = DependencyProperty.Register
            (
                nameof(MetronomeImageSource), typeof(ImageSource), typeof(TrackContainer), new PropertyMetadata(null)
            );


        /// <summary>
        /// Gets or sets the slash image
        /// </summary>
        public ImageSource SlashImageSource
        {
            get => (ImageSource)GetValue(SlashImageSourceProperty);
            set => SetValue(SlashImageSourceProperty, value);
        }

        public static readonly DependencyProperty SlashImageSourceProperty = DependencyProperty.Register
            (
                nameof(SlashImageSource), typeof(ImageSource), typeof(TrackContainer), new PropertyMetadata(null)
            );
        #endregion

        /************************************************************************/

        #region Public properties (Commands)
        ///// <summary>
        ///// Gets or sets the command to be executed when the user clicks the close button.
        ///// </summary>
        //public ICommand CloseCommand
        //{
        //    get => (ICommand)GetValue(CloseCommandProperty);
        //    set => SetValue(CloseCommandProperty, value);
        //}

        //public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register
        //    (
        //        nameof(CloseCommand), typeof(ICommand), typeof(TrackContainer), new PropertyMetadata(null)
        //    );
        #endregion

        /************************************************************************/

        #region Public properties (Other)
        /// <summary>
        /// Gets or sets the display name for this track layout
        /// </summary>
        public string DisplayName
        {
            get => (string)GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }

        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register
            (
                nameof(DisplayName), typeof(string), typeof(TrackContainer), new PropertyMetadata(null, OnDisplayNameChanged)
            );

        #endregion

        /************************************************************************/

        #region Public properties (read only)
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

        public static readonly DependencyProperty ActivePlayImageSourceProperty = ActivePlayImageSourcePropertyKey.DependencyProperty;


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

        public static readonly DependencyProperty IsStartedProperty = IsStartedPropertyKey.DependencyProperty;

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

        public static readonly DependencyProperty CounterTextProperty = CounterTextPropertyKey.DependencyProperty;

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

        #region Private properties
        /// <summary>
        /// Gets the current track count
        /// </summary>
        private int TrackCount
        {
            get => TrackControllers.Count;
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

        public static readonly RoutedEvent ClosingEvent = EventManager.RegisterRoutedEvent
            (
                nameof(Closing), RoutingStrategy.Bubble, typeof(CancelRoutedEventHandler), typeof(TrackContainer)
            );

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

            CloseImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Close.64.png", UriKind.Relative));
            MetronomeImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Metronome.64.png", UriKind.Relative));
            SlashImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Slash.64.png", UriKind.Relative));

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                TrackControllers = new MaxSizeObservableCollection<TrackController>(TrackVals.Track.Max);
                TrackBoxes = new MaxSizeObservableCollection<TrackBoxContainer>(TrackVals.Track.Max);
                SubmixVoice = new SubmixVoice(AudioHost.Instance.AudioDevice);

                metronome = new Metronome(this);

                
                Commands.Add("Close", new RelayCommand(RunCloseCommand));
                Commands.Add("AddTrack", new RelayCommand(RunAddTrackCommand, CanRunAddTrackCommand));
                Commands.Add("Play", new RelayCommand(RunPlayCommand));
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
            headerBoxes = GetTemplateChild(PartHeaderBoxes) as TrackBoxContainer;
            if (headerBoxes != null) headerBoxes.TotalStepsChanged -= HeaderBoxesTotalStepsChanged;
            if (headerBoxes != null) headerBoxes.TotalStepsChanged += HeaderBoxesTotalStepsChanged;
            IsChangedSet -= TrackContainerIsChangedSet;
            IsChangedReset -= TrackContainerIsChangedReset;
            IsChangedSet += TrackContainerIsChangedSet;
            IsChangedReset += TrackContainerIsChangedReset;

            if (TrackControllers != null && TrackControllers.Count == 0)
            {
                AddTrack(AudioPieceType.Cymbal);
                AddTrack(AudioPieceType.HighHat);
                AddTrack(AudioPieceType.Snare);
                AddTrack(AudioPieceType.Tom);
                AddTrack(AudioPieceType.Kick);
            }

            metronome.Piece = AudioHost.Instance.GetAudioPiece(AudioPieceType.Percussion);
            metronome.UpdateBeatValues(Beats, StepsPerBeat);

            IsStarted = false;
            CalculateThreadSafeValues();
            OnVolumeChanged();
            OnMutedImageSourceChanged();
        }


        /// <summary>
        /// Creates an XDocument representation of the container and saves it to the specified file.
        /// </summary>
        /// <returns>The XDocument object</returns>
        public void Save(string filename)
        {
            var doc = new XDocument(GetXElement());
            System.IO.File.WriteAllText(filename, doc.ToString());
            ResetIsChanged();
        }

        public void Open(string filename)
        {
            TrackControllers.Clear();
            TrackBoxes.Clear();
            XDocument doc = XDocument.Load(filename);
            RestoreFromXElement(doc.Root);
            ResetIsChanged();
        }

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

            foreach (var controller in TrackControllers)
            {
                element.Add(controller.GetXElement());
            }
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
                if (e.Name == nameof(TrackController))
                {
                    AddTrack(null);
                    OnBoxSizeChanged();
                    OnTotalStepsChanged();
                    TrackControllers[TrackControllers.Count - 1].RestoreFromXElement(e);
                }
            }


        }
        #endregion

        /************************************************************************/

        #region Protected methods
        protected override void OnVolumeChanged()
        {
            base.OnVolumeChanged();
            if (SubmixVoice != null)
            {
                SubmixVoice.SetVolume(VolumeInternal);
            }
        }

        protected override void OnMutedImageSourceChanged()
        {
            base.OnMutedImageSourceChanged();
            if (TrackControllers != null)
            {
                TrackControllers.DoForAll((item) =>
                {
                    item.MutedImageSource = MutedImageSource;
                    item.VoicedImageSource = VoicedImageSource;
                });
            }
        }

        protected override void OnIsMutedChanged()
        {
            base.OnIsMutedChanged();
        }

        protected override void OnBoxSizeChanged()
        {
            foreach (var boxes in TrackBoxes)
            {
                boxes.BoxSize = BoxSize;
            }

            foreach (var tc in TrackControllers)
            {
                tc.MinHeight = BoxSize + 18;
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
                AddTrack(AudioPieceType.Kick);
                SetIsChanged();
            }
        }

        private void AddTrack(AudioPiece piece)
        {
            TrackControllers.Add(new TrackController(this)
            {
                Margin = new Thickness(4),
                Padding = new Thickness(4),
                BorderThickness = new Thickness(1,1,0,1),
                BorderBrush = Brushes.LightSlateGray,
                MinHeight = BoxSize + 18,
                Piece = piece,
                MutedImageSource = MutedImageSource,
                VoicedImageSource = VoicedImageSource,
                IsPitchEnabled = TrackVals.Pitch.IsEnabledDefault,
                IsPanningEnabled = TrackVals.Panning.IsEnabledDefault,
            });

            TrackBoxes.Add(new TrackBoxContainer()
            {
                Margin = new Thickness(4),
                Padding = new Thickness(4),
                BorderThickness = new Thickness(0,1,1,1),
                BorderBrush = Brushes.LightSlateGray,
                BoxType = TrackBoxType.TrackStep,
                TotalSteps = TotalSteps,
                BoxSize = BoxSize,
                SelectedBackgroundBrush = new SolidColorBrush(Colors.LightBlue)
            });
            TrackBoxes[TrackCount - 1].SetController(TrackControllers[TrackCount - 1]);
            TrackControllers[TrackCount - 1].SetBoxContainer(TrackBoxes[TrackCount - 1]);
        }

        private void AddTrack(AudioPieceType type)
        {
            AudioPiece piece = AudioHost.Instance.GetAudioPiece(type);
            AddTrack(piece);
        }

        private bool CanRunAddTrackCommand(object parm)
        {
            return TrackCount < TrackVals.Track.Max;
        }

        private void RunPlayCommand(object parm)
        {
            IsStarted = !IsStarted;
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
                if (!isControlClosing)
                {
                    while (isStarted)
                    {
                        int step = 0;
                        while (isStarted && step < totalSteps)
                        {
                            
                            PlayOneStep(pass,step++);
                            Thread.Sleep(sleepTime);
                        }
                        ClearAllSteps();
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
                    headerBoxes.Boxes[step].PlayFrequency = StepPlayFrequency.EveryPass;
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
                TrackControllers.DoForAll((controller) =>
                {
                    controller.Play(pass, step, operationSet);
                });

                AudioHost.Instance.AudioDevice.CommitChanges(operationSet);
            }
        }

        private void ClearAllSteps()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback
                ((args) =>
                {
                    headerBoxes.DeselectAllBoxes();
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
            ActivePlayImageSource = (IsStarted) ? StopImageSource : StartImageSource;
        }

        private void OnTotalStepsChanged()
        {
            TotalSteps = totalSteps = Beats * StepsPerBeat;
            TrackBoxes.DoForAll((c) => 
            {
                c.TotalSteps = TotalSteps;
            });
            metronome.UpdateBeatValues(Beats, StepsPerBeat);
        }

        private void HeaderBoxesTotalStepsChanged(object sender, RoutedEventArgs e)
        {
            if (headerBoxes != null)
            {
                // Update the beat labels
                int beat = 1;
                for (int k = 0; k < headerBoxes.Boxes.Count; k++)
                {
                    headerBoxes.Boxes[k].Text = (k % StepsPerBeat == 0) ? $"{beat++}" : string.Empty;
                }
            }
        }

        private void CalculateThreadSafeValues()
        {
            sleepTime = (MilliSecondsPerMinute / (int)Tempo) / 4; //  StepsPerBeat;
            //Debug.WriteLine($"Sleep: {sleepTime}");
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
        #endregion

        /************************************************************************/

        #region Private methods (Static)

        private static void OnDisplayNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackContainer c)
            {
                c.SetIsChanged();
            }
        }

        private static void OnTempoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackContainer c)
            {
                c.CalculateThreadSafeValues();
                c.SetIsChanged();
            }
        }

        private static void OnTotalStepsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackContainer c)
            {
                c.OnTotalStepsChanged();
                c.CalculateThreadSafeValues();
                c.SetIsChanged();
            }
        }

        private static void OnIsStartedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackContainer c)
            {
                c.OnIsStartedChanged();
                c.OnPlayImageSourceChanged();
            }
        }

        private static void OnPlayImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackContainer c)
            {
                c.OnPlayImageSourceChanged();
            }
        }

        private static void OnIsMetronomeActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackContainer c)
            {
                c.metronome.IsActive = c.IsMetronomeActive;
            }
        }

        private static object OnTempoCoerce(DependencyObject d, object baseValue)
        {
            double proposed = (double)baseValue;
            return Math.Min(TrackVals.Tempo.Max, Math.Max(TrackVals.Tempo.Min, proposed));
        }

        private static object OnBeatsCoerce(DependencyObject d, object baseValue)
        {
            int proposed = (int)baseValue;
            return Math.Min(TrackVals.Beats.Max, Math.Max(TrackVals.Beats.Min, proposed));
        }

        private static object OnStepsPerBeatCoerce(DependencyObject d, object baseValue)
        {
            int proposed = (int)baseValue;
            return Math.Min(TrackVals.StepsPerBeat.Max, Math.Max(TrackVals.StepsPerBeat.Min, proposed));
        }
        #endregion
    }
}
