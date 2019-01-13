using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents the top most container for a project. A project includes song arrangement, drum kit patterns, master output controls, etc.
    /// </summary>
    /// <remarks>
    /// This class provides the container for all pieces that comprise a project. It holds the controls to play / pause,
    /// the controls for the master output (volume, pitch, tempo), the controls to create a song from multiple drum
    /// patterns, and the drum patterns.
    /// </remarks>
    public class ProjectContainer : ControlObject
    {
        #region Private
        private DrumKit drumKit;
        private bool isSongContainerChangeInProgress = false;
        private double songContainerLastManualHeight;
        private const double SongContainerDefaultHeight = 300.0;
        private const double SongContainerThresholdHeight = 106.0;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectContainer"/> class.
        /// </summary>
        public ProjectContainer()
        {
            MasterPlay = new MasterPlay(this);
            MasterOutput = new MasterOutput(this);
            SongContainer = new SongContainer(this);

            DrumKit = new DrumKit();
            DrumKit.LoadBuiltInInstruments();

            DrumPatterns = new List<DrumPattern>();
            for (int k=1; k <= Constants.DrumPattern.MaxCount; k++)
            {
                DrumPatterns.Add(new DrumPattern(this)
                {
                    DisplayName = $"Pattern {k}",
                });
            }

            ActiveDrumPattern = DrumPatterns[0];
            SongContainer.SongPresenter.HighlightSelectedPattern(0);

            AddHandler(IsChangedSetEvent, new RoutedEventHandler(IsChangedSetEventHandler));
            AddHandler(IsChangedResetEvent, new RoutedEventHandler(IsChangedResetEventHandler));
            AddHandler(LoadedEvent, new RoutedEventHandler((s,e)=> 
            {
                SongContainer.SongPresenter.HighlightSelectedPattern(0);
                e.Handled = true;
            }));

            songContainerLastManualHeight = 0.0;
        }

        static ProjectContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProjectContainer), new FrameworkPropertyMetadata(typeof(ProjectContainer)));
        }
        #endregion

        /************************************************************************/

        #region MasterOutput
        /// <summary>
        /// Gets the master output control for the container.
        /// </summary>
        public MasterOutput MasterOutput
        {
            get => (MasterOutput)GetValue(MasterOutputProperty);
            private set => SetValue(MasterOutputPropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey MasterOutputPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(MasterOutput), typeof(MasterOutput), typeof(ProjectContainer), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="MasterOutput"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MasterOutputProperty = MasterOutputPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region MasterPlay
        /// <summary>
        /// Gets the master play control for the container.
        /// </summary>
        public MasterPlay MasterPlay
        {
            get => (MasterPlay)GetValue(MasterPlayProperty);
            private set => SetValue(MasterPlayPropertyKey, value);
        }

        private static readonly DependencyPropertyKey MasterPlayPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(MasterPlay), typeof(MasterPlay), typeof(ProjectContainer), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="MasterPlay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MasterPlayProperty = MasterPlayPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region SongContainer
        /// <summary>
        /// Gets the song container.
        /// </summary>
        public SongContainer SongContainer
        {
            get => (SongContainer)GetValue(SongContainerProperty);
            set => SetValue(SongContainerPropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey SongContainerPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(SongContainer), typeof(SongContainer), typeof(ProjectContainer), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="SongContainer"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SongContainerProperty = SongContainerPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region SongContainerHeight
        /// <summary>
        /// Gets the height of the <see cref="SongContainer"/>.
        /// </summary>
        public GridLength SongContainerHeight
        {
            get => (GridLength)GetValue(SongContainerHeightProperty);
            set => SetValue(SongContainerHeightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SongContainerHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SongContainerHeightProperty = DependencyProperty.Register
            (
                nameof(SongContainerHeight), typeof(GridLength), typeof(ProjectContainer), new PropertyMetadata(new GridLength(SongContainerDefaultHeight, GridUnitType.Pixel), OnSongContainerHeightChanged)
            );

        private static void OnSongContainerHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ProjectContainer c)
            {
                if (!c.isSongContainerChangeInProgress) c.songContainerLastManualHeight = c.SongContainerHeight.Value;
            }
        }

        /// <summary>
        /// Gets the minimum height for <see cref="SongContainerHeight"/>. Used to bind to the control template.
        /// </summary>
        public double SongContainerMinHeight
        {
            get => 76.0;
        }
        #endregion

        /************************************************************************/

        #region DrumPatternContainer (commented out)
        ///// <summary>
        ///// Gets the drum kit container
        ///// </summary>
        //public DrumPatternContainer DrumPatternContainer
        //{
        //    get => (DrumPatternContainer)GetValue(DrumPatternContainerProperty);
        //    private set => SetValue(DrumPatternContainerPropertyKey, value);
        //}

        //private static readonly DependencyPropertyKey DrumPatternContainerPropertyKey = DependencyProperty.RegisterReadOnly
        //    (
        //        nameof(DrumPatternContainer), typeof(DrumPatternContainer), typeof(ProjectContainer), new PropertyMetadata(null)
        //    );

        ///// <summary>
        ///// Identifies the <see cref="DrumPatternContainer"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty DrumPatternContainerProperty = DrumPatternContainerPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region DrumPatterns
        /// <summary>
        /// Gets the list of drum patterms
        /// </summary>
        public List<DrumPattern> DrumPatterns
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region ActiveDrumPattern
        /// <summary>
        /// Gets the active <see cref="DrumPattern"/> object.
        /// </summary>
        public DrumPattern ActiveDrumPattern
        {
            get => (DrumPattern)GetValue(ActiveDrumPatternProperty);
            private set => SetValue(ActiveDrumPatternPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ActiveDrumPatternPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ActiveDrumPattern), typeof(DrumPattern), typeof(ProjectContainer), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ActiveDrumPattern"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveDrumPatternProperty = ActiveDrumPatternPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region DrumKit
        /// <summary>
        /// Gets or sets the drum kit for the project.
        /// </summary>
        public DrumKit DrumKit
        {
            get => drumKit;
            set => drumKit = value ?? throw new ArgumentNullException(nameof(DrumKit));
        }
        #endregion

        /************************************************************************/

        #region FileName
        /// <summary>
        /// Gets the current file name for the container, or null if none.
        /// This is the .xml file
        /// </summary>
        public string FileName
        {
            get;
            private set;
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
            var element = new XElement(nameof(ProjectContainer));
            element.Add(new XElement(nameof(DisplayName), DisplayName));
            element.Add(MasterOutput.GetXElement());
            element.Add(MasterPlay.GetXElement());
            element.Add(SongContainer.GetXElement());
            ///element.Add(DrumPatternContainer.GetXElement());
            //element.Add(new XElement(nameof(Volume), Volume));
            //element.Add(new XElement(nameof(Tempo), Tempo));
            //element.Add(new XElement(nameof(Beats), Beats));
            //element.Add(new XElement(nameof(StepsPerBeat), StepsPerBeat));
            //element.Add(new XElement(nameof(BoxSize), BoxSize));
            //element.Add(RenderParms.GetXElement());

            //Tracks.DoForAll((track) =>
            //{
            //    element.Add(track.GetXElement());
            //});
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
                if (e.Name == nameof(DisplayName)) SetDependencyProperty(DisplayNameProperty, e.Value);
                //if (e.Name == nameof(Volume)) SetDependencyProperty(VolumeProperty, e.Value);
                //if (e.Name == nameof(Tempo)) SetDependencyProperty(TempoProperty, e.Value);
                //if (e.Name == nameof(Beats)) SetDependencyProperty(BeatsProperty, e.Value);
                //if (e.Name == nameof(StepsPerBeat)) SetDependencyProperty(StepsPerBeatProperty, e.Value);
                //if (e.Name == nameof(BoxSize)) SetDependencyProperty(BoxSizeProperty, e.Value);
                //if (e.Name == nameof(AudioRenderParameters))
                //{
                //    RenderParms.RestoreFromXElement(e);
                //    AudioHost.Instance.AudioCapture.RenderParms = RenderParms;
                //}

                // For backward compatibility
                //if (e.Name == nameof(TrackController))
                //{
                //    AddTrack(null);
                //    OnBoxSizeChanged();
                //    OnTotalStepsChanged();
                //    Tracks[Tracks.Count - 1].Controller.RestoreFromXElement(e);
                //    IEnumerable<XElement> subChildList = from subE in e.Elements() select subE;
                //    foreach (XElement ee in subChildList)
                //    {
                //        if (ee.Name == "TrackBoxContainer")
                //        {
                //            Tracks[Tracks.Count - 1].BoxContainer.RestoreFromXElement(ee);
                //        }
                //    }
                //}

                // For new format
                //if (e.Name == nameof(CompositeTrack))
                //{
                //    AddTrack(null);
                //    Tracks[Tracks.Count - 1].RestoreFromXElement(e);
                //}
            }
        }
        #endregion

        /************************************************************************/

        #region Public methods
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
                //Tracks.Clear();
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
        /// Shuts down the song container.
        /// </summary>
        public void Shutdown()
        {
            // StopPlayThread();
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// From this assembly, activate the drum patterm at the specified index
        /// </summary>
        /// <param name="patternIdx">The index of the drum pattern to active.</param>
        internal void ActivateDrumPattern(int patternIdx)
        {
            if (patternIdx >=0 && patternIdx < DrumPatterns.Count)
            {
                ActiveDrumPattern = DrumPatterns[patternIdx];
                SongContainer.SongPresenter.HighlightSelectedPattern(patternIdx);
            }
        }

        internal void ChangeSongContainerHeight()
        {
            isSongContainerChangeInProgress = true;
            bool isOpen = SongContainerHeight.Value >= SongContainerThresholdHeight;

            if (isOpen)
            {
                SongContainerHeight = new GridLength(SongContainerMinHeight, GridUnitType.Pixel);
            }
            else
            {
                double newHeight = songContainerLastManualHeight > SongContainerThresholdHeight ? songContainerLastManualHeight : SongContainerDefaultHeight;
                SongContainerHeight = new GridLength(newHeight, GridUnitType.Pixel);
            }
            isSongContainerChangeInProgress = false;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void IsChangedSetEventHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource != this)
            {
                SetIsChanged();
            }
        }

        private void IsChangedResetEventHandler(object sender, RoutedEventArgs e)
        {
            if (e.Source != this)
            {
                ResetIsChanged();
            }
        }
        #endregion
    }
}
