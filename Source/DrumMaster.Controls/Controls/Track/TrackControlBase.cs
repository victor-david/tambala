using Restless.App.DrumMaster.Controls.Core;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents the base class for track controls. This class must be inherited.
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
    public abstract class TrackControlBase : ContentControl, IXElement
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties (Volume)
        /// <summary>
        /// Gets or sets the volume.
        /// Volume is expressed as a dB value between <see cref="TrackVals.Volume.Min"/> and <see cref="TrackVals.Volume.Max"/>
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
                nameof(Volume), typeof(float), typeof(TrackControlBase), new PropertyMetadata(TrackVals.Volume.Default, OnVolumeChanged, OnVolumeCoerce)
            );

        private static void OnVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackControlBase c)
            {
                // Save volume for later thread safe access.
                c.VolumeRaw = (float)e.NewValue;
                c.ThreadSafeVolume = XAudio2.DecibelsToAmplitudeRatio(c.VolumeRaw);
                c.VolumeDecibelText = (c.Volume <= TrackVals.Volume.Min) ? "Off" : $"{c.Volume:N1}dB";
                c.IsAutoMuted = c.Volume == TrackVals.Volume.Min;
                c.OnVolumeChanged();
                c.SetIsChanged();
            }
        }

        private static object OnVolumeCoerce(DependencyObject d, object baseValue)
        {
            float proposed = (float)baseValue;
            return Math.Min(TrackVals.Volume.Max, Math.Max(TrackVals.Volume.Min, proposed));
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
                nameof(VolumeDecibelText), typeof(string), typeof(TrackControlBase), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="VolumeDecibelText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeDecibelTextProperty = VolumeDecibelTextPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets the volume bias.
        /// Volume bias is expressed as a dB value between <see cref="TrackVals.VolumeBias.Min"/> and <see cref="TrackVals.VolumeBias.Max"/>
        /// </summary>
        public float VolumeBias
        {
            get => (float)GetValue(VolumeBiasProperty);
            set => SetValue(VolumeBiasProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="VolumeBias"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VolumeBiasProperty = DependencyProperty.Register
            (
                nameof(VolumeBias), typeof(float), typeof(TrackControlBase), new PropertyMetadata(TrackVals.VolumeBias.Default, OnVolumeBiasChanged, OnVolumeBiasCoerce)
            );

        private static void OnVolumeBiasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackControlBase c)
            {
                // Save volume bias in private var for later thread safe access.
                c.VolumeBiasRaw = (float)e.NewValue;
                float dbVol = c.VolumeRaw + c.VolumeBiasRaw;
                c.ThreadSafeVolume = XAudio2.DecibelsToAmplitudeRatio(dbVol);
                c.OnVolumeChanged();
                c.SetIsChanged();
            }
        }

        private static object OnVolumeBiasCoerce(DependencyObject d, object baseValue)
        {
            float proposed = (float)baseValue;
            return Math.Min(TrackVals.VolumeBias.Max, Math.Max(TrackVals.VolumeBias.Min, proposed));
        }

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
                nameof(VolumeText), typeof(string), typeof(TrackControlBase), new PropertyMetadata(TrackVals.Volume.DefaultText)
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
                nameof(ShortVolumeText), typeof(string), typeof(TrackControlBase), new PropertyMetadata(TrackVals.Volume.DefaultShortText)
            );

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
                nameof(IsMuted), typeof(bool), typeof(TrackControlBase), new PropertyMetadata(false, OnIsMutedChanged)
            );

        private static void OnIsMutedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackControlBase c)
            {
                c.IsUserMuted = (bool)e.NewValue;
                c.OnIsMutedChanged();
                c.SetIsChanged();
            }
        }

        /// <summary>
        /// Gets the minimum volume allowed. Used for binding in the control template.
        /// </summary>
        public float MinVolume
        {
            get => TrackVals.Volume.Min;
        }

        /// <summary>
        /// Gets the maximum volume allowed.Used for binding in the control template.
        /// </summary>
        public float MaxVolume
        {
            get => TrackVals.Volume.Max;
        }

        /// <summary>
        /// Gets the minimum volume bias allowed. Used for binding in the control template.
        /// </summary>
        public float MinVolumeBias
        {
            get => TrackVals.VolumeBias.Min;
        }

        /// <summary>
        /// Gets the maximum volume bias allowed. Used for binding in the control template.
        /// </summary>
        public float MaxVolumeBias
        {
            get => TrackVals.VolumeBias.Max;
        }
        #endregion

        /************************************************************************/

        #region Public properties (Panning)
        /// <summary>
        /// Gets or sets the track panning.
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
                nameof(Panning), typeof(float), typeof(TrackControlBase), new PropertyMetadata(TrackVals.Panning.Default, OnPanningChanged, OnPanningCoerce)
            );

        private static void OnPanningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackControlBase c)
            {
                c.SetPanningText();
                c.OnPanningChanged();
                c.SetIsChanged();
            }
        }

        private static object OnPanningCoerce(DependencyObject d, object baseValue)
        {
            float proposed = (float)baseValue;
            return Math.Min(TrackVals.Panning.Max, Math.Max(TrackVals.Panning.Min, proposed));
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
                nameof(PanningText), typeof(string), typeof(TrackControlBase), new FrameworkPropertyMetadata(null)
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
            get => TrackVals.Panning.Min;
        }

        /// <summary>
        /// Gets the maximum panning allowed. Used for binding in the control template.
        /// </summary>
        public float MaxPanning
        {
            get => TrackVals.Panning.Max;
        }

        #endregion

        /************************************************************************/

        #region Public properties (Pitch)
        /// <summary>
        /// Gets or sets the pitch. 
        /// Pitch is expressed as a semi tone value between <see cref="TrackVals.Pitch.Min"/> and <see cref="TrackVals.Pitch.Max"/>
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
                nameof(Pitch), typeof(float), typeof(TrackControlBase), new PropertyMetadata(TrackVals.Pitch.Default, OnPitchChanged, OnPitchCoerce)
            );

        private static void OnPitchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackControlBase c)
            {
                c.ThreadSafePitch = XAudio2.SemitonesToFrequencyRatio((float)e.NewValue);
                c.OnPitchChanged();
                c.SetIsChanged();
            }
        }

        private static object OnPitchCoerce(DependencyObject d, object baseValue)
        {
            float proposed = (float)baseValue;
            return Math.Min(TrackVals.Pitch.Max, Math.Max(TrackVals.Pitch.Min, proposed));
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
                nameof(PitchSemiToneText), typeof(string), typeof(TrackControlBase), new FrameworkPropertyMetadata(null)
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
            get => TrackVals.Pitch.Min;
        }

        /// <summary>
        /// Gets the maximum pitch allowed. Used for binding in the control template.
        /// </summary>
        public float MaxPitch
        {
            get => TrackVals.Pitch.Max;
        }
        #endregion

        /************************************************************************/

        #region  Public properties (IsExpanded)
        /// <summary>
        /// Gets a boolean value that indicates if the control is expanded (true) or collapsed (false).
        /// </summary>
        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            private set => SetValue(IsExpandedPropertyKey, value);
        }

        private static readonly DependencyPropertyKey IsExpandedPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(IsExpanded), typeof(bool), typeof(TrackControlBase), new FrameworkPropertyMetadata(true)
            );

        /// <summary>
        /// Identifies the <see cref="IsExpanded"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = IsExpandedPropertyKey.DependencyProperty;

        #endregion

        /************************************************************************/

        #region Public properties (Image)
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
                nameof(MutedImageSource), typeof(ImageSource), typeof(TrackControlBase), new PropertyMetadata(null, OnMutedImageSourceChanged)
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
                nameof(VoicedImageSource), typeof(ImageSource), typeof(TrackControlBase), new PropertyMetadata(null, OnMutedImageSourceChanged)
            );

        private static void OnMutedImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackControlBase c)
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
                nameof(ActiveMutedImageSource), typeof(ImageSource), typeof(TrackControlBase), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ActiveMutedImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveMutedImageSourceProperty = ActiveMutedImageSourcePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets the image source to use for the minimize button
        /// </summary>
        public ImageSource MinimizeImageSource
        {
            get => (ImageSource)GetValue(MinimizeImageSourceProperty);
            set => SetValue(MinimizeImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MinimizeImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimizeImageSourceProperty = DependencyProperty.Register
            (
                nameof(MinimizeImageSource), typeof(ImageSource), typeof(TrackControlBase), new PropertyMetadata(null, OnIsExpandedImageSourceChanged)
            );

        /// <summary>
        /// Gets or sets the image source to use for the maximize button
        /// </summary>
        public ImageSource MaximizeImageSource
        {
            get => (ImageSource)GetValue(MaximizeImageSourceProperty);
            set => SetValue(MaximizeImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MaximizeImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximizeImageSourceProperty = DependencyProperty.Register
            (
                nameof(MaximizeImageSource), typeof(ImageSource), typeof(TrackControlBase), new PropertyMetadata(null, OnIsExpandedImageSourceChanged)
            );

        private static void OnIsExpandedImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackControlBase control)
            {
                control.OnIsExpandedImageSourceChanged();
            }
        }

        /// <summary>
        /// Gets image source that is currently for the mimimized / maximized state
        /// </summary>
        public ImageSource ActiveExpandedStateImageSource
        {
            get => (ImageSource)GetValue(ActiveExpandedStateImageSourceProperty);
            private set => SetValue(ActiveExpandedStateImageSourcePropertyKey, value);
        }

        private static readonly DependencyPropertyKey ActiveExpandedStateImageSourcePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ActiveExpandedStateImageSource), typeof(ImageSource), typeof(TrackControlBase), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ActiveExpandedStateImageSource"/> dependency property,
        /// </summary>
        public static readonly DependencyProperty ActiveExpandedStateImageSourceProperty = ActiveExpandedStateImageSourcePropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Public properties (read only)
        /// <summary>
        /// Gets a dictionary of commands. Used internally by the control template
        /// </summary>
        public Dictionary<string, ICommand> Commands
        {
            get => (Dictionary<string, ICommand>)GetValue(CommandsProperty);
            private set => SetValue(CommandsPropertyKey, value);
        }

        private static readonly DependencyPropertyKey CommandsPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(Commands), typeof(Dictionary<string, ICommand>), typeof(TrackControlBase), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="Commands"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandsProperty = CommandsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a boolean value that indicates if changes have occured since this object was established.
        /// </summary>
        public bool IsChanged
        {
            get => (bool)GetValue(IsChangedProperty);
            private set => SetValue(IsChangedPropertyKey, value);
        }

        private static readonly DependencyPropertyKey IsChangedPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(IsChanged), typeof(bool), typeof(TrackContainer), new FrameworkPropertyMetadata(false)
            );

        /// <summary>
        /// Identifies the <see cref="IsChanged"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsChangedProperty = IsChangedPropertyKey.DependencyProperty;

        #endregion

        /************************************************************************/

        #region Protected / Internal properties
        /// <summary>
        /// Gets a thread safe boolean value that reflects the value of <see cref="IsMuted"/>.
        /// </summary>
        protected bool IsUserMuted
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a thread safe boolean value that indicates if audio has been automatically muted
        /// due to the <see cref="Volume"/> having reached a value of <see cref="TrackVals.Volume.Min"/>.
        /// </summary>
        protected bool IsAutoMuted
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="OnApplyTemplate"/> has been called.
        /// </summary>
        protected bool IsTemplateApplied
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the thread safe raw value of <see cref="Volume"/>. This value is expressed in dB.
        /// </summary>
        protected float VolumeRaw
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the thread value of <see cref="VolumeBias"/>. This value is expressed in dB.
        /// </summary>
        protected float VolumeBiasRaw
        {
            get;
            private set;
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

        #region Routed Events
        /// <summary>
        /// Provides notification when the <see cref="IsChanged"/> property is set to true.
        /// </summary>
        public event RoutedEventHandler IsChangedSet
        {
            add => AddHandler(IsChangedSetEvent, value);
            remove => RemoveHandler(IsChangedSetEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsChangedSet"/> routed event.
        /// </summary>
        public static readonly RoutedEvent IsChangedSetEvent = EventManager.RegisterRoutedEvent
            (
                nameof(IsChangedSet), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TrackControlBase)
            );

        /// <summary>
        /// Provides notification when the <see cref="IsChanged"/> property is set to false.
        /// </summary>
        public event RoutedEventHandler IsChangedReset
        {
            add => AddHandler(IsChangedResetEvent, value);
            remove => RemoveHandler(IsChangedResetEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsChangedReset"/> routed event.
        /// </summary>
        public static readonly RoutedEvent IsChangedResetEvent = EventManager.RegisterRoutedEvent
            (
                nameof(IsChangedReset), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TrackControlBase)
            );
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of <see cref="TrackControlBase"/>.
        /// </summary>
        protected TrackControlBase()
        {
            MutedImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Track.Muted.64.png", UriKind.Relative));
            VoicedImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Track.Voiced.64.png", UriKind.Relative));
            ActiveMutedImageSource = VoicedImageSource;

            MinimizeImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Minimize.64.png", UriKind.Relative));
            MaximizeImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Maximize.64.png", UriKind.Relative));
            ActiveExpandedStateImageSource = MinimizeImageSource;

            Commands = new Dictionary<string, ICommand>();
            Commands.Add("ToggleExpanded", new RelayCommand(RunToggleExpandedCommand));

            /* 
             * The following are thread safe values used in real time from the play thread.
             * They get updated via the dependency properties and are set here to match the
             * DP default value. Although not strictly necessary to set VolumeRaw and
             * and VolumeBiasRaw (because their defaults are 0.0), we do so here in case we 
             * later decide to change TrackVals.Volume.Default and/or TrackVals.VolumeBias.Default
             */
            VolumeRaw = TrackVals.Volume.Default;
            VolumeBiasRaw = TrackVals.VolumeBias.Default;
            ThreadSafeVolume = XAudio2.DecibelsToAmplitudeRatio(TrackVals.Volume.Default);
            ThreadSafePitch = XAudio2.SemitonesToFrequencyRatio(TrackVals.Pitch.Default);

            VolumeDecibelText = (Volume <= TrackVals.Volume.Min) ? "Off" : $"{Volume:N1}dB";
            SetPanningText();
            OnVolumeChanged();
            OnPanningChanged();
            OnPitchChanged();
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
            IsTemplateApplied = true;
        }
        #endregion

        /************************************************************************/

        #region IXElement 
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public abstract XElement GetXElement();
        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public abstract void RestoreFromXElement(XElement element);
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
        /// Before this method is called, all muted related properties such as <see cref="IsUserMuted"/> have been updated.
        /// Always call the base method.
        /// </summary>
        protected virtual void OnIsMutedChanged()
        {
            ActiveMutedImageSource = IsMuted ? MutedImageSource : VoicedImageSource;
        }

        /// <summary>
        /// Sets the <see cref="IsChanged"/> property to true and raises the <see cref="IsChangedSetEvent"/>.
        /// </summary>
        protected void SetIsChanged()
        {
            IsChanged = true;
            RoutedEventArgs args = new RoutedEventArgs(IsChangedSetEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Sets the <see cref="IsChanged"/> property to false and raises the <see cref="IsChangedResetEvent"/>.
        /// </summary>
        protected void ResetIsChanged()
        {
            IsChanged = false;
            RoutedEventArgs args = new RoutedEventArgs(IsChangedResetEvent);
            RaiseEvent(args);
        }
        
        /// <summary>
        /// Sets the specified dependency property to the specified string.
        /// </summary>
        /// <param name="prop">The dependency property.</param>
        /// <param name="val">The value</param>
        protected void SetDependencyProperty(DependencyProperty prop, string val)
        {
            if (prop == null) throw new ArgumentNullException(nameof(prop));

            if (prop.PropertyType == typeof(string))
            {
                SetValue(prop, val);
            }

            if (prop.PropertyType == typeof(int))
            {
                if (int.TryParse(val, out int result))
                {
                    SetValue(prop, result);
                }
            }

            if (prop.PropertyType == typeof(double))
            {
                if (double.TryParse(val, out double result))
                {
                    SetValue(prop, result);
                }
            }

            if (prop.PropertyType == typeof(float))
            {
                if (float.TryParse(val, out float result))
                {
                    SetValue(prop, result);
                }
            }

            if (prop.PropertyType == typeof(bool))
            {
                if (bool.TryParse(val, out bool result))
                {
                    SetValue(prop, result);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (Instance)

        private void RunToggleExpandedCommand(object parm)
        {
            IsExpanded = !IsExpanded;
            OnIsExpandedImageSourceChanged();
        }

        private void OnIsExpandedImageSourceChanged()
        {
            ActiveExpandedStateImageSource = (IsExpanded) ? MinimizeImageSource : MaximizeImageSource;
        }


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
                PanningText = $"{pdl}% left";
            }
            else
            {
                float pr = (pan - 50) / 50 * 100;
                double pdr = Math.Round(pr, 0);
                PanningText = $"{pdr}% right";
            }
        }
        #endregion
    }
}
