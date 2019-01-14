using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a control that provides master play / stop services.
    /// </summary>
    public sealed partial class MasterPlay : AudioControlBase
    {
        #region Private
        // All thread related fields and methods are in the partial.
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPlay"/> class.
        /// </summary>
        internal MasterPlay(ProjectContainer owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            StartImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Start.64.png", UriKind.Relative));
            StopImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Stop.64.png", UriKind.Relative));
            ActivePlayImageSource = StartImageSource;
            Commands.Add("Play", new RelayCommand(RunPlayCommand));
            AddHandler(OnOff.ActiveValueChangedEvent, new RoutedEventHandler(OnOffActiveValueChanged));
            InitializeThreads();
        }

        static MasterPlay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterPlay), new FrameworkPropertyMetadata(typeof(MasterPlay)));
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the <see cref="ProjectContainer"/> that owns this instance.
        /// </summary>
        private ProjectContainer Owner
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region PlayMode
        /// <summary>
        /// Gets the play mode
        /// </summary>
        public PlayMode PlayMode
        {
            get => (PlayMode)GetValue(PlayModeProperty);
            private set => SetValue(PlayModePropertyKey, value);
        }

        private static readonly DependencyPropertyKey PlayModePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(PlayMode), typeof(PlayMode), typeof(MasterPlay), new PropertyMetadata(PlayMode.Pattern, OnPlayModeChanged)
            );

        /// <summary>
        /// Identifies the <see cref="PlayMode"/> dependency property
        /// </summary>
        public static readonly DependencyProperty PlayModeProperty = PlayModePropertyKey.DependencyProperty;

        private static void OnPlayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MasterPlay c)
            {
                c.OnPlayModeChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region IsStarted
        /// <summary>
        /// Gets a boolean value that indicates if the song or pattern is playing
        /// </summary>
        public bool IsStarted
        {
            get => (bool)GetValue(IsStartedProperty);
            private set => SetValue(IsStartedPropertyKey, value);
        }

        private static readonly DependencyPropertyKey IsStartedPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(IsStarted), typeof(bool), typeof(MasterPlay), new FrameworkPropertyMetadata(false, OnIsStartedChanged)
            );

        /// <summary>
        /// Identifies the <see cref="IsStarted"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsStartedProperty = IsStartedPropertyKey.DependencyProperty;

        private static void OnIsStartedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MasterPlay c)
            {
                c.OnIsStartedChanged();
                c.OnPlayImageSourceChanged();
            }
        }
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
                nameof(StartImageSource), typeof(ImageSource), typeof(MasterPlay), new PropertyMetadata(null, OnPlayImageSourceChanged)
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
                nameof(StopImageSource), typeof(ImageSource), typeof(MasterPlay), new PropertyMetadata(null, OnPlayImageSourceChanged)
            );

        private static void OnPlayImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MasterPlay c)
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
                nameof(ActivePlayImageSource), typeof(ImageSource), typeof(MasterPlay), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ActivePlayImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActivePlayImageSourceProperty = ActivePlayImageSourcePropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region IXElement
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public override XElement GetXElement()
        {
            var element = new XElement(nameof(MasterPlay));
            element.Add(new XElement(nameof(PlayMode), PlayMode));
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

        #region Private methods
        private void OnPlayImageSourceChanged()
        {
            ActivePlayImageSource = IsStarted ? StopImageSource : StartImageSource;
        }

        private void RunPlayCommand(object parm)
        {
            IsStarted = !IsStarted;
        }

        private void OnOffActiveValueChanged(object sender, RoutedEventArgs e)
        {
            if (e.Source is OnOff c && c.ActiveValue is PlayMode mode)
            {
                PlayMode = mode;
                e.Handled = true;
            }
        }
        #endregion
    }
}