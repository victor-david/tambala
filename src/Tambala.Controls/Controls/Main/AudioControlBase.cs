/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Controls.Core;
using SharpDX.XAudio2;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Represents base class for controls that require volume, pitch, and panning. This class must be inherited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides the base methods and properties for all track related object.
    /// It provides <see cref="Volume"/>, <see cref="Pitch"/>, and <see cref="Panning"/>.
    /// </para>
    /// <para>
    /// Not all descendents make use of all properties.
    /// </para>
    /// </remarks>
    public abstract class AudioControlBase : ControlObject, IVolume
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of <see cref="AudioControlBase"/>.
        /// </summary>
        protected AudioControlBase()
        {
            MutedImageSource = new BitmapImage(new Uri("/Tambala.Controls;component/Resources/Images/Image.Track.Muted.64.png", UriKind.Relative));
            VoicedImageSource = new BitmapImage(new Uri("/Tambala.Controls;component/Resources/Images/Image.Track.Voiced.64.png", UriKind.Relative));
            ActiveMutedImageSource = VoicedImageSource;

            SoloYesImageSource = new BitmapImage(new Uri("/Tambala.Controls;component/Resources/Images/Image.Track.Solo.Yes.64.png", UriKind.Relative));
            SoloNoImageSource = new BitmapImage(new Uri("/Tambala.Controls;component/Resources/Images/Image.Track.Solo.No.64.png", UriKind.Relative));
            ActiveSoloImageSource = SoloNoImageSource;

            Commands.Add("ToggleMute", new RelayCommand((p) => IsMuted = !IsMuted));
            Commands.Add("ToggleSolo", new RelayCommand((p) => IsSolo = !IsSolo));

            /* 
             * The following are thread safe values used in real time from the play thread.
             * They get updated via the dependency properties and are set here to match the
             * DP default value. Although not strictly necessary to set VolumeRaw and
             * and VolumeBiasRaw (because their defaults are 0.0), we do so here in case we 
             * later decide to change TrackVals.Volume.Default and/or TrackVals.VolumeBias.Default
             */
            ThreadSafeVolumeRaw = Constants.Volume.Default;
            ThreadSafeVolume = XAudio2.DecibelsToAmplitudeRatio(Constants.Volume.Default);
            ThreadSafePitch = XAudio2.SemitonesToFrequencyRatio(Constants.Pitch.Default);
            VolumeDecibelText = (Volume <= Constants.Volume.Main.Min) ? "Off" : $"{Volume:N1}dB";

            SetPanningText();
        }
        #endregion

        /************************************************************************/

        #region Volume
        /// <summary>
        /// Gets or sets the volume.
        /// Volume is expressed as a dB value between <see cref="Constants.Volume.Main.Min"/> and <see cref="Constants.Volume.Main.Max"/>
        /// </summary>
        public float Volume
        {
            get => (float)GetValue(VolumeProperty);
            set => SetValue(VolumeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Volume"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register
            (
                nameof(Volume), typeof(float), typeof(AudioControlBase), new PropertyMetadata(Constants.Volume.Default, OnVolumeChanged, OnVolumeCoerce)
            );

        private static void OnVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AudioControlBase c)
            {
                // Save volume for later thread safe access.
                c.ThreadSafeVolumeRaw = (float)e.NewValue;
                c.ThreadSafeVolume = XAudio2.DecibelsToAmplitudeRatio(c.ThreadSafeVolumeRaw);
                c.VolumeDecibelText = (c.Volume <= Constants.Volume.Main.Min) ? "Off" : $"{c.Volume:N1}dB";
                c.IsAutoMuted = c.Volume == Constants.Volume.Main.Min;
                c.OnVolumeChanged();
                c.SetIsChanged();
            }
        }

        private static object OnVolumeCoerce(DependencyObject d, object baseValue)
        {
            float proposed = (float)baseValue;
            return Math.Min(Constants.Volume.Main.Max, Math.Max(Constants.Volume.Main.Min, proposed));
        }

        /// <summary>
        /// Gets the volume as a string expressed in decibels.
        /// </summary>
        public string VolumeDecibelText
        {
            get => (string)GetValue(VolumeDecibelTextProperty);
            private set => SetValue(VolumeDecibelTextPropertyKey, value);
        }

        private static readonly DependencyPropertyKey VolumeDecibelTextPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(VolumeDecibelText), typeof(string), typeof(AudioControlBase), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="VolumeDecibelText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeDecibelTextProperty = VolumeDecibelTextPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets the volume text
        /// </summary>
        public string VolumeText
        {
            get => (string)GetValue(VolumeTextProperty);
            set => SetValue(VolumeTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="VolumeText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeTextProperty = DependencyProperty.Register
            (
                nameof(VolumeText), typeof(string), typeof(AudioControlBase), new PropertyMetadata(Constants.Volume.DefaultText)
            );

        /// <summary>
        /// Gets or sets the short volume text.
        /// </summary>
        public string ShortVolumeText
        {
            get => (string)GetValue(ShortVolumeTextProperty);
            set => SetValue(ShortVolumeTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ShortVolumeText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShortVolumeTextProperty = DependencyProperty.Register
            (
                nameof(ShortVolumeText), typeof(string), typeof(AudioControlBase), new PropertyMetadata(Constants.Volume.DefaultShortText)
            );

        /// <summary>
        /// Gets the minimum volume allowed. Used for binding in the control template.
        /// </summary>
        public float MinVolume
        {
            get => Constants.Volume.Main.Min;
        }

        /// <summary>
        /// Gets the maximum volume allowed.Used for binding in the control template.
        /// </summary>
        public float MaxVolume
        {
            get => Constants.Volume.Main.Max;
        }
        #endregion

        /************************************************************************/

        #region Muted / Solo
        /// <summary>
        /// Gets or sets a boolean value that determines if audio is muted.
        /// </summary>
        public bool IsMuted
        {
            get => (bool)GetValue(IsMutedProperty);
            set => SetValue(IsMutedProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsMuted"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMutedProperty = DependencyProperty.Register
            (
                nameof(IsMuted), typeof(bool), typeof(AudioControlBase), new PropertyMetadata(false, OnIsMutedChanged)
            );

        private static void OnIsMutedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AudioControlBase c)
            {
                c.ThreadSafeIsMuted = (bool)e.NewValue;
                c.OnIsMutedChanged();
                c.SetIsChanged();
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that determines if audio is soloed.
        /// </summary>
        public bool IsSolo
        {
            get => (bool)GetValue(IsSoloProperty);
            set => SetValue(IsSoloProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSolo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSoloProperty = DependencyProperty.Register
            (
                nameof(IsSolo), typeof(bool), typeof(AudioControlBase), new PropertyMetadata(false, OnIsSoloChanged)
            );

        private static void OnIsSoloChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AudioControlBase c)
            {
                c.RaiseEvent(new RoutedEventArgs(IsSoloChangedEvent));
                c.OnIsSoloChanged();
                c.SetIsChanged();
            }
        }

        /// <summary>
        /// Provides notification when the <see cref="IsSolo"/> property changes.
        /// </summary>
        public event RoutedEventHandler IsSoloChanged
        {
            add => AddHandler(IsSoloChangedEvent, value);
            remove => RemoveHandler(IsSoloChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSoloChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent IsSoloChangedEvent = EventManager.RegisterRoutedEvent
            (
                nameof(IsSoloChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AudioControlBase)
            );
        #endregion

        /************************************************************************/

        #region Panning
        /// <summary>
        /// Gets or sets the panning.
        /// </summary>
        public float Panning
        {
            get => (float)GetValue(PanningProperty);
            set => SetValue(PanningProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Panning"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanningProperty = DependencyProperty.Register
            (
                nameof(Panning), typeof(float), typeof(AudioControlBase), new PropertyMetadata(Constants.Panning.Default, OnPanningChanged, OnPanningCoerce)
            );

        private static void OnPanningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AudioControlBase c)
            {
                c.SetPanningText();
                c.OnPanningChanged();
                c.SetIsChanged();
            }
        }

        private static object OnPanningCoerce(DependencyObject d, object baseValue)
        {
            float proposed = (float)baseValue;
            return Math.Min(Constants.Panning.Max, Math.Max(Constants.Panning.Min, proposed));
        }

        /// <summary>
        /// Gets the panning text expressed as a percentage left or right
        /// </summary>
        public string PanningText
        {
            get => (string)GetValue(PanningTextProperty);
            private set => SetValue(PanningTextPropertyKey, value);
        }

        private static readonly DependencyPropertyKey PanningTextPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(PanningText), typeof(string), typeof(AudioControlBase), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="PanningText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanningTextProperty = PanningTextPropertyKey.DependencyProperty;


        /// <summary>
        /// Gets the minimum panning allowed. Used for binding in the control template.
        /// </summary>
        public float MinPanning
        {
            get => Constants.Panning.Min;
        }

        /// <summary>
        /// Gets the maximum panning allowed. Used for binding in the control template.
        /// </summary>
        public float MaxPanning
        {
            get => Constants.Panning.Max;
        }

        #endregion

        /************************************************************************/

        #region Pitch
        /// <summary>
        /// Gets or sets the pitch. 
        /// Pitch is expressed as a semi tone value between <see cref="Constants.Pitch.Min"/> and <see cref="Constants.Pitch.Max"/>
        /// </summary>
        public float Pitch
        {
            get => (float)GetValue(PitchProperty);
            set => SetValue(PitchProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Pitch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PitchProperty = DependencyProperty.Register
            (
                nameof(Pitch), typeof(float), typeof(AudioControlBase), new PropertyMetadata(Constants.Pitch.Default, OnPitchChanged, OnPitchCoerce)
            );

        private static void OnPitchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AudioControlBase c)
            {
                c.ThreadSafePitch = XAudio2.SemitonesToFrequencyRatio((float)e.NewValue);
                c.OnPitchChanged();
                c.SetIsChanged();
            }
        }

        private static object OnPitchCoerce(DependencyObject d, object baseValue)
        {
            float proposed = (float)baseValue;
            return Math.Min(Constants.Pitch.Max, Math.Max(Constants.Pitch.Min, proposed));
        }

        /// <summary>
        /// Gets the pitch as a string expressed in semitones.
        /// </summary>
        public string PitchSemiToneText
        {
            get => (string)GetValue(PitchSemiToneTextProperty);
            private set => SetValue(PitchSemiToneTextPropertyKey, value);
        }

        private static readonly DependencyPropertyKey PitchSemiToneTextPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(PitchSemiToneText), typeof(string), typeof(AudioControlBase), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="PitchSemiToneText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PitchSemiToneTextProperty = PitchSemiToneTextPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the minimum pitch allowed. Used for binding in the control template.
        /// </summary>
        public float MinPitch
        {
            get => Constants.Pitch.Min;
        }

        /// <summary>
        /// Gets the maximum pitch allowed. Used for binding in the control template.
        /// </summary>
        public float MaxPitch
        {
            get => Constants.Pitch.Max;
        }
        #endregion

        /************************************************************************/

        #region Images (Muted, Voiced [has state change])
        /// <summary>
        /// Gets or sets the image source to use for the muted button (when muted).
        /// </summary>
        public ImageSource MutedImageSource
        {
            get => (ImageSource)GetValue(MutedImageSourceProperty);
            set => SetValue(MutedImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MutedImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MutedImageSourceProperty = DependencyProperty.Register
            (
                nameof(MutedImageSource), typeof(ImageSource), typeof(AudioControlBase), new PropertyMetadata(null, OnMutedImageSourceChanged)
            );

        /// <summary>
        /// Gets or sets the image source to use for the muted button (when not muted).
        /// </summary>
        public ImageSource VoicedImageSource
        {
            get => (ImageSource)GetValue(VoicedImageSourceProperty);
            set => SetValue(VoicedImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="VoicedImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VoicedImageSourceProperty = DependencyProperty.Register
            (
                nameof(VoicedImageSource), typeof(ImageSource), typeof(AudioControlBase), new PropertyMetadata(null, OnMutedImageSourceChanged)
            );

        private static void OnMutedImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AudioControlBase c)
            {
                c.OnMutedImageSourceChanged();
                c.OnIsMutedChanged();
            }
        }

        /// <summary>
        /// Gets the image source to use for the mute button.
        /// </summary>
        public ImageSource ActiveMutedImageSource
        {
            get => (ImageSource)GetValue(ActiveMutedImageSourceProperty);
            private set => SetValue(ActiveMutedImageSourcePropertyKey, value);
        }

        private static readonly DependencyPropertyKey ActiveMutedImageSourcePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ActiveMutedImageSource), typeof(ImageSource), typeof(AudioControlBase), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ActiveMutedImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveMutedImageSourceProperty = ActiveMutedImageSourcePropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Images (Solo Yes, No [has state change])
        /// <summary>
        /// Gets or sets the image source to use for the solo button (when soloed).
        /// </summary>
        public ImageSource SoloYesImageSource
        {
            get => (ImageSource)GetValue(SoloYesImageSourceProperty);
            set => SetValue(SoloYesImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SoloYesImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SoloYesImageSourceProperty = DependencyProperty.Register
            (
                nameof(SoloYesImageSource), typeof(ImageSource), typeof(AudioControlBase), new PropertyMetadata(null, OnSoloImageSourceChanged)
            );

        /// <summary>
        /// Gets or sets the image source to use for the solo button (when not soloed).
        /// </summary>
        public ImageSource SoloNoImageSource
        {
            get => (ImageSource)GetValue(SoloNoImageSourceProperty);
            set => SetValue(SoloNoImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SoloNoImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SoloNoImageSourceProperty = DependencyProperty.Register
            (
                nameof(SoloNoImageSource), typeof(ImageSource), typeof(AudioControlBase), new PropertyMetadata(null, OnSoloImageSourceChanged)
            );

        private static void OnSoloImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AudioControlBase c)
            {
                c.OnSoloImageSourceChanged();
                c.OnIsSoloChanged();
            }
        }

        /// <summary>
        /// Gets the image source to use for the solo button.
        /// </summary>
        public ImageSource ActiveSoloImageSource
        {
            get => (ImageSource)GetValue(ActiveSoloImageSourceProperty);
            private set => SetValue(ActiveSoloImageSourcePropertyKey, value);
        }

        private static readonly DependencyPropertyKey ActiveSoloImageSourcePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ActiveSoloImageSource), typeof(ImageSource), typeof(AudioControlBase), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ActiveSoloImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveSoloImageSourceProperty = ActiveSoloImageSourcePropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Protected / Internal properties
        /// <summary>
        /// Gets a thread safe boolean value that reflects the value of <see cref="IsMuted"/>.
        /// </summary>
        protected bool ThreadSafeIsMuted
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a thread safe boolean value that indicates if audio has been automatically muted
        /// due to the <see cref="Volume"/> having reached a value of <see cref="Constants.Volume.Main.Min"/>.
        /// </summary>
        protected bool IsAutoMuted
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the thread safe raw value of <see cref="Volume"/>. This value is expressed in dB.
        /// </summary>
        protected float ThreadSafeVolumeRaw
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the thread safe value that indicates if audio has been muted because other track(s) are in solo mode.
        /// </summary>
        internal bool IsSoloMuted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the thread safe volume value. 
        /// This value is calculated from <see cref="Volume"/>, which is expressed as a dB value
        /// </summary>
        /// <remarks>
        /// This value is used internally and represents the actual XAudio2 volume value to use when 
        /// setting the volume on a voice.
        /// </remarks>
        internal float ThreadSafeVolume
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the thread safe pitch value.
        /// This value is calculated from <see cref="Pitch"/>, which is expressed as a percentage.
        /// </summary>
        /// <remarks>
        /// This value is used internally and represents the actual XAudio2 frequency ratio value to use when 
        /// setting the pitch (aka frequency ratio) on a voice.
        /// </remarks>
        internal float ThreadSafePitch
        {
            get;
            private set;
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
            OnIsMutedChanged();
            OnIsSoloChanged();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when <see cref="Volume"/> is changed. A derived class can override this method to perform updates as needed.
        /// Before this method is called, all volume related properties such as <see cref="ThreadSafeVolume"/> have been updated.
        /// The base implementaion does nothing.
        /// </summary>
        protected virtual void OnVolumeChanged()
        {
        }

        /// <summary>
        /// Called when <see cref="Panning"/> is changed. A derived class can override this method to perform updates as needed.
        /// The base implementaion does nothing.
        /// </summary>
        protected virtual void OnPanningChanged()
        {
        }

        /// <summary>
        /// Called when <see cref="Pitch"/> is changed. A derived class can override this method to perform updates as needed.
        /// Before this method is called, all pitch related properties such as <see cref="ThreadSafePitch"/> have been updated.
        /// The base implementaion does nothing.
        /// </summary>
        protected virtual void OnPitchChanged()
        {
        }

        /// <summary>
        /// Called when either <see cref="MutedImageSource"/> or <see cref="VoicedImageSource"/> are changed.
        /// A devired class can override this method to perform other updates.
        /// The base method does nothing.
        /// </summary>
        protected virtual void OnMutedImageSourceChanged()
        {
        }

        /// <summary>
        /// Called when the <see cref="IsMuted"/> property is changed. A derived class can override this method to perform updates as needed.
        /// Before this method is called, all muted related properties such as <see cref="ThreadSafeIsMuted"/> have been updated.
        /// Always call the base method.
        /// </summary>
        protected virtual void OnIsMutedChanged()
        {
            ActiveMutedImageSource = IsMuted ? MutedImageSource : VoicedImageSource;
        }

        /// <summary>
        /// Called when either <see cref="SoloYesImageSource"/> or <see cref="SoloNoImageSource"/> are changed.
        /// A devired class can override this method to perform other updates.
        /// The base method does nothing.
        /// </summary>
        protected virtual void OnSoloImageSourceChanged()
        {
        }

        /// <summary>
        /// Called when the <see cref="IsSolo"/> property is changed. A derived class can override this method to perform updates as needed.
        /// Always call the base method.
        /// </summary>
        protected virtual void OnIsSoloChanged()
        {
            ActiveSoloImageSource = IsSolo ? SoloYesImageSource : SoloNoImageSource;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void SetPanningText()
        {
            // 0.0 = 100% left;
            // 0.5 = Center
            // 1.0 = 100% right
            float pan = Panning * 100.0f;
            if (pan == 50)
            {
                PanningText = "Center";
            }
            else if (pan < 50)
            {
                float pl = (50 - pan) / 50 * 100;
                double pdl = Math.Round(pl, 0);
                PanningText = $"{pdl}% L";
            }
            else
            {
                float pr = (pan - 50) / 50 * 100;
                double pdr = Math.Round(pr, 0);
                PanningText = $"{pdr}% R";
            }
        }
        #endregion
    }
}