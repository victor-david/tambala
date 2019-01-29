using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using Restless.App.DrumMaster.Controls.Resources;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents the top most container for a project. A project includes song arrangement, drum kit patterns, master output controls, etc.
    /// </summary>
    /// <remarks>
    /// This class provides the container for all pieces that comprise a project. It holds the controls to play / pause,
    /// the controls for the master output (volume, tempo), the controls to create a song from multiple drum
    /// patterns, and the drum patterns.
    /// </remarks>
    public sealed class ProjectContainer : ControlObject, IDisposable
    {
        #region Private
        private bool isSongContainerChangeInProgress = false;
        private double songContainerLastManualHeight;
        private const double SongContainerDefaultHeight = 250.0;
        private const double SongContainerThresholdHeight = 106.0;

        private bool isMixerContainerChangeInProgress = false;
        private double mixerContainerLastManualWidth;
        private const double MixerContainerDefaultWidth = 232.0;
        private const double MixerContainerThresholdWidth = 104.0;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectContainer"/> class.
        /// </summary>
        public ProjectContainer()
        {
            DisplayName = Strings.ProjectContainerDisplayName;
            MasterPlay = new MasterPlay(this);
            MasterOutput = ThreadSafeMasterOutput = new MasterOutput(this);
            SongContainer = new SongContainer(this);

            DrumPatterns = new DrumPatternCollection();
            for (int k=1; k <= Constants.DrumPattern.MaxCount; k++)
            {
                DrumPatterns.Add(new DrumPattern(this)
                {
                    DisplayName = $"Pattern {k}",
                });
            }

            // IsExpanded is used in the template to expand / contract the drum kit list.
            // Drum kits are assigned per pattern but the display is same for all.
            IsExpanded = false;

            ActivateDrumPattern(0);

            AddHandler(IsChangedSetEvent, new RoutedEventHandler(IsChangedSetEventHandler));
            AddHandler(IsChangedResetEvent, new RoutedEventHandler(IsChangedResetEventHandler));

            songContainerLastManualHeight = 0.0;
            Dispatcher.ShutdownStarted += DispatcherShutdownStarted;
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

        /// <summary>
        /// Gets a thread safe reference to <see cref="MasterOutput"/>.
        /// </summary>
        internal MasterOutput ThreadSafeMasterOutput
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region MasterPlay
        /// <summary>
        /// Gets the master play control for the container.
        /// </summary>
        internal MasterPlay MasterPlay
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
        internal static readonly DependencyProperty MasterPlayProperty = MasterPlayPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region SongContainer (CLR)
        /// <summary>
        /// Gets the song container.
        /// </summary>
        public SongContainer SongContainer
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region SongContainerHeight
        /// <summary>
        /// Gets ot sets the height of the <see cref="SongContainer"/>.
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
                nameof(SongContainerHeight), typeof(GridLength), typeof(ProjectContainer), new PropertyMetadata
                    (
                        new GridLength(SongContainerDefaultHeight, GridUnitType.Pixel), OnSongContainerHeightChanged
                    )
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

        #region MixerContainerWidth

        /// <summary>
        /// Gets or sets the width of the mixer container.
        /// </summary>
        public GridLength MixerContainerWidth
        {
            get => (GridLength)GetValue(MixerContainerWidthProperty);
            set => SetValue(MixerContainerWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MixerContainerWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MixerContainerWidthProperty = DependencyProperty.Register
            (
                nameof(MixerContainerWidth), typeof(GridLength), typeof(ProjectContainer), new PropertyMetadata
                    (
                        new GridLength(MixerContainerDefaultWidth, GridUnitType.Pixel), OnMixerContainerWidthChanged
                    )
            );

        private static void OnMixerContainerWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ProjectContainer c)
            {
                if (!c.isMixerContainerChangeInProgress) c.mixerContainerLastManualWidth = c.MixerContainerWidth.Value;
            }
        }
        #endregion

        /************************************************************************/

        #region DrumPatterns
        /// <summary>
        /// Gets the list of drum patterms
        /// </summary>
        public DrumPatternCollection DrumPatterns
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

        /// <summary>
        /// Gets a thread safe reference to <see cref="ActiveDrumPattern"/>.
        /// </summary>
        internal DrumPattern ThreadSafeActiveDrumPattern
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region DrumKit
        /// <summary>
        /// Gets the collection of available drum kits.
        /// This is a shortcut property to <see cref="AudioHost.DrumKits"/>.
        /// </summary>
        public DrumKitCollection DrumKits
        {
            get => AudioHost.Instance.DrumKits;
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
             
            DrumPatterns.DoForAll((pattern) =>
            {
                element.Add(pattern.GetXElement());
             });
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            int idx = 0;
            foreach (XElement e in ChildElementList(element))
            {
                if (e.Name == nameof(DisplayName)) SetDependencyProperty(DisplayNameProperty, e.Value);
                if (e.Name == nameof(MasterOutput)) MasterOutput.RestoreFromXElement(e);
                if (e.Name == nameof(MasterPlay)) MasterPlay.RestoreFromXElement(e);
                if (e.Name == nameof(SongContainer)) SongContainer.RestoreFromXElement(e);

                if (e.Name == nameof(DrumPattern))
                {
                    if (idx < DrumPatterns.Count)
                    {
                        DrumPatterns[idx].Create();
                        DrumPatterns[idx].RestoreFromXElement(e);
                    }
                    idx++;
                }
            }
        }
        #endregion

        /************************************************************************/

        #region IDisposable
        /// <summary>
        /// Disposes resources.
        /// </summary>
        public void Dispose()
        {
            MasterPlay.Dispose();
            Dispatcher.ShutdownStarted -= DispatcherShutdownStarted;
            GC.SuppressFinalize(this);
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
            MasterPlay.SetTempo(MasterOutput.Tempo);
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
        /// <returns>An exception object if an exception occurred; otherwise, null.</returns>
        public async Task<Exception> Open(string filename)
        {
            try
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    XDocument doc = XDocument.Load(filename);
                    RestoreFromXElement(doc.Root);
                    ResetIsChanged();
                    DisplayName = FileName = filename;
                    OnLoaded();
                }, DispatcherPriority.Loaded);
            }
            catch (Exception ex)
            {
                return new System.IO.IOException($"Dispatcher Unable to load {filename}", ex);
            }
            return null;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the control has been loaded.
        /// </summary>
        protected override void OnLoaded()
        {
            SongContainer.Presenter.HighlightSelectedPattern(0);
        }

        /// <summary>
        /// Called when the <see cref="ControlObject.IsSlidRight"/> property changes.
        /// </summary>
        protected override void OnIsSlidRightChanged()
        {
            isMixerContainerChangeInProgress = true;
            if (IsSlidRight)
            {
                MixerContainerWidth = new GridLength(0.0, GridUnitType.Pixel);
            }
            else
            {
                if (mixerContainerLastManualWidth < MixerContainerThresholdWidth)
                {
                    mixerContainerLastManualWidth = MixerContainerDefaultWidth;
                }
                MixerContainerWidth = new GridLength(mixerContainerLastManualWidth, GridUnitType.Pixel);
            }
            isMixerContainerChangeInProgress = false;
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// From this assembly, activate the drum pattern at the specified index
        /// </summary>
        /// <param name="patternIdx">The index of the drum pattern to activate.</param>
        internal void ActivateDrumPattern(int patternIdx)
        {
            if (patternIdx >=0 && patternIdx < DrumPatterns.Count)
            {
                ActiveDrumPattern = ThreadSafeActiveDrumPattern = DrumPatterns[patternIdx];
                SongContainer.Presenter.HighlightSelectedPattern(patternIdx);
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

        private void DispatcherShutdownStarted(object sender, EventArgs e)
        {
            // This event handler may not be needed. The important thing is to shutdown
            // the threads in MasterPlay and this is handled by the Shutdown() method
            // above. As long as the consumer of ProjectContainer calls Shutdown()
            // the threads will finish and this handler will be removed.
            MasterPlay.Dispose();
        }
        #endregion
    }
}
