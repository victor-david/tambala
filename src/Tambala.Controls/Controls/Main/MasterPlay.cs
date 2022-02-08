/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Controls.Core;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Represents a control that provides master play / stop services.
    /// </summary>
    [TemplatePart(Name = PartPlayMode, Type = typeof(OnOff))]
    internal sealed partial class MasterPlay : AudioControlBase, IShutdown
    {
        #region Private
        // All thread related fields and methods are in the partial.
        private const string PartPlayMode = "PART_PlayMode";
        private OnOff playModeControl;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPlay"/> class.
        /// </summary>
        /// <param name="owner">The project container that owns this instance.</param>
        internal MasterPlay(ProjectContainer owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            StartImageSource = new BitmapImage(new Uri("/Tambala.Controls;component/Resources/Images/Image.Start.64.png", UriKind.Relative));
            StopImageSource = new BitmapImage(new Uri("/Tambala.Controls;component/Resources/Images/Image.Stop.64.png", UriKind.Relative));
            ActivePlayImageSource = StartImageSource;
            Commands.Add("Play", new RelayCommand(RunPlayCommand));
            AddHandler(OnOff.ActiveValueChangedEvent, new RoutedEventHandler(OnOffActiveValueChanged));
            Owner.AddHandler(MasterOutput.TempoChangedEvent, new RoutedEventHandler(MasterOutputTempoChanged));
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
                nameof(PlayMode), typeof(PlayMode), typeof(MasterPlay), new FrameworkPropertyMetadata()
                {
                    DefaultValue = PlayMode.Pattern,
                    PropertyChangedCallback = OnPlayModeChanged
                }
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
                c.SetIsChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region CounterText
        /// <summary>
        /// Gets the counter text.
        /// </summary>
        public string CounterText
        {
            get => (string)GetValue(CounterTextProperty);
            private set => SetValue(CounterTextPropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey CounterTextPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(CounterText), typeof(string), typeof(MasterPlay), new PropertyMetadata()
                {
                    DefaultValue = null
                }
            );

        /// <summary>
        /// Identifies the <see cref="CounterText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CounterTextProperty = CounterTextPropertyKey.DependencyProperty;
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

        #region MetrononeFrequency
        /// <summary>
        /// Gets or sets the metronome frequency.
        /// </summary>
        public int MetronomeFrequency
        {
            get => (int)GetValue(MetronomeFrequencyProperty);
            set => SetValue(MetronomeFrequencyProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MetronomeFrequency"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MetronomeFrequencyProperty = DependencyProperty.Register
            (
                nameof(MetronomeFrequency), typeof(int), typeof(MasterPlay), new PropertyMetadata()
                {
                    DefaultValue =  Constants.Metronome.Frequency.Default, 
                    PropertyChangedCallback = OnMetronomeFrequencyChanged, 
                    CoerceValueCallback = OnMetronomeFrequencyCoerce
                }
            );

        private static object OnMetronomeFrequencyCoerce(DependencyObject d, object baseValue)
        {
            int proposed = (int)baseValue;
            return Math.Min(Constants.Metronome.Frequency.Max, Math.Max(Constants.Metronome.Frequency.Min, proposed));
        }

        private static void OnMetronomeFrequencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MasterPlay c)
            {
                c.SetIsChanged();
                c.metronome.SetFrequency (c.MetronomeFrequency);
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
            element.Add(new XElement(nameof(MetronomeFrequency), MetronomeFrequency));
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            foreach (XElement e in ChildElementList(element))
            {
                if (e.Name == nameof(MetronomeFrequency))
                {
                    SetDependencyProperty(MetronomeFrequencyProperty, e.Value);
                }

                if (e.Name == nameof(PlayMode))
                {
                    if (Enum.Parse(typeof(PlayMode), e.Value) is PlayMode playMode)
                    {
                        PlayMode = playMode;
                    }
                }
            }
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
            playModeControl = GetTemplateChild(PartPlayMode) as OnOff;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        protected override void OnLoaded()
        {
            if (playModeControl != null)
            {
                playModeControl.IsChecked = PlayMode == PlayMode.Song;
            }
        }

        /// <summary>
        /// Called when the volume is changed.
        /// </summary>
        protected override void OnVolumeChanged()
        {
            metronome.SetVolume(ThreadSafeVolume);
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
            if (e.Source is OnOff c)
            {
                if (c.ActiveValue is PlayMode mode)
                {
                    PlayMode = mode;
                    e.Handled = true;
                }

                if (c.Id == "Metronome")
                {
                    if (c.IsChecked.HasValue)
                    {
                        metronome.IsActive = c.IsChecked.Value;
                    }
                    e.Handled = true;
                }
            }
        }

        private void MasterOutputTempoChanged(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MasterOutput master)
            {
                SetTempo(master.Tempo);
                e.Handled = true;
            }
        }
        #endregion
    }
}