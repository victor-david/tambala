/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Controls.Audio;
using Restless.Tambala.Controls.Core;
using SharpDX.XAPO.Fx;
using SharpDX.XAudio2;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Xml.Linq;

namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Represents a controller for a drum pattern.
    /// </summary>
    public sealed class DrumPatternController : AudioControlBase, IQuarterNote, IDisposable
    {
        #region Private
        private SubmixVoice submixVoice;
        private EqualizerParameters equalizerParameters;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumPatternController"/> class.
        /// </summary>
        /// <param name="owner">The drum pattern that owns this instance.</param>
        internal DrumPatternController(DrumPattern owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            SubmixVoice = new SubmixVoice(AudioHost.Instance.AudioDevice);

            Equalizer equalizer = new Equalizer(AudioHost.Instance.AudioDevice);
            EffectDescriptor equalizerEffectDescriptor = new EffectDescriptor(equalizer);
            submixVoice.SetEffectChain(equalizerEffectDescriptor);

            equalizerParameters = new EqualizerParameters()
            {
                Bandwidth0 = Equalizer.DefaultBandwidth,
                Bandwidth1 = Equalizer.DefaultBandwidth,
                Bandwidth2 = Equalizer.DefaultBandwidth,
                Bandwidth3 = Equalizer.DefaultBandwidth,
                FrequencyCenter0 = Equalizer.DefaultFrequencyCenter0,
                FrequencyCenter1 = Equalizer.DefaultFrequencyCenter1,
                FrequencyCenter2 = Equalizer.DefaultFrequencyCenter2,
                FrequencyCenter3 = Equalizer.DefaultFrequencyCenter3,
                Gain0 = Equalizer.DefaultGain,
                Gain1 = Equalizer.DefaultGain,
                Gain2 = Equalizer.DefaultGain,
                Gain3 = Equalizer.DefaultGain,
            };


            TickValueText = TickValueToText(Constants.DrumPattern.TicksPerQuarterNote.Default);
            ThreadSafeQuarterNoteCount = Constants.DrumPattern.QuarterNoteCount.Default;

            Owner.EqualizerController.AddHandler(EqualizerController.IsActiveChangedEvent, new RoutedEventHandler(EqualizerControllerIsActiveChanged));
            Owner.EqualizerController.AddHandler(EqualizerController.GainChangedEvent, new EqualizerRoutedEventHandler(EqualizerControllerGainChanged));
        }

        static DrumPatternController()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrumPatternController), new FrameworkPropertyMetadata(typeof(DrumPatternController)));
        }
        #endregion

        /************************************************************************/

        #region Owner
        /// <summary>
        /// Gets the <see cref="DrumPattern"/> that owns this instance.
        /// </summary>
        internal DrumPattern Owner
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region SubmixVoice
        /// <summary>
        /// From this assembly, gets the controller's submix voice. All instruments in the pattern route through this voice.
        /// </summary>
        internal SubmixVoice SubmixVoice
        {
            get => submixVoice;
            private set => submixVoice = value;
        }
        #endregion

        /************************************************************************/

        #region QuarterNoteCount
        /// <summary>
        /// Gets or sets the number of quarter notes
        /// </summary>
        public int QuarterNoteCount
        {
            get => (int)GetValue(QuarterNoteCountProperty);
            set => SetValue(QuarterNoteCountProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="QuarterNoteCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty QuarterNoteCountProperty = DependencyProperty.Register
            (
                nameof(QuarterNoteCount), typeof(int), typeof(DrumPatternController), new PropertyMetadata(Constants.DrumPattern.QuarterNoteCount.Default, OnQuarterNoteCountChanged, OnQuarterNoteCountCoerce)
            );

        private static object OnQuarterNoteCountCoerce(DependencyObject d, object baseValue)
        {
            int proposed = (int)baseValue;
            return Math.Min(Constants.DrumPattern.QuarterNoteCount.Max, Math.Max(Constants.DrumPattern.QuarterNoteCount.Min, proposed));
        }

        private static void OnQuarterNoteCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrumPatternController c)
            {
                c.ThreadSafeQuarterNoteCount = c.QuarterNoteCount;
                c.SetIsChanged();
                c.RaiseEvent(new RoutedEventArgs(QuarterNoteCountChangedEvent));
            }
        }

        /// <summary>
        /// Gets a thread safe value for <see cref="QuarterNoteCount"/>.
        /// </summary>
        internal int ThreadSafeQuarterNoteCount
        {
            get;
            private set;
        }
        /// <summary>
        /// Provides notification when the <see cref="QuarterNoteCount"/> property is changed.
        /// </summary>
        public event RoutedEventHandler QuarterNoteCountChanged
        {
            add => AddHandler(QuarterNoteCountChangedEvent, value);
            remove => RemoveHandler(QuarterNoteCountChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="QuarterNoteCountChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent QuarterNoteCountChangedEvent = EventManager.RegisterRoutedEvent
            (
                nameof(QuarterNoteCountChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DrumPatternController)
            );

        /// <summary>
        /// Gets the minimum allowed value for <see cref="QuarterNoteCount"/>.
        /// Used to bind to the control template.
        /// </summary>
        public int MinQuarterNoteCount
        {
            get => Constants.DrumPattern.QuarterNoteCount.Min;
        }

        /// <summary>
        /// Gets the maximum allowed value for <see cref="QuarterNoteCount"/>.
        /// Used to bind to the control template.
        /// </summary>
        public int MaxQuarterNoteCount
        {
            get => Constants.DrumPattern.QuarterNoteCount.Max;
        }
        #endregion

        /************************************************************************/

        #region TicksPerQuarterNote
        /// <summary>
        /// Gets or sets the number of ticks per quarter note.
        /// </summary>
        public int TicksPerQuarterNote
        {
            get => (int)GetValue(TicksPerQuarterNoteProperty);
            set => SetValue(TicksPerQuarterNoteProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TicksPerQuarterNote"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TicksPerQuarterNoteProperty = DependencyProperty.Register
            (
                nameof(TicksPerQuarterNote), typeof(int), typeof(DrumPatternController), new PropertyMetadata
                    (
                        Constants.DrumPattern.TicksPerQuarterNote.Default, OnTicksPerQuarterNoteChanged, OnTicksPerQuarterNoteCoerce
                    )
            );

        private static object OnTicksPerQuarterNoteCoerce(DependencyObject d, object baseValue)
        {
            int proposed = (int)baseValue;
            return Math.Min(Constants.DrumPattern.TicksPerQuarterNote.Max, Math.Max(Constants.DrumPattern.TicksPerQuarterNote.Min, proposed));
        }

        private static void OnTicksPerQuarterNoteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrumPatternController c)
            {
                c.SetIsChanged();
                c.TickValueText = c.TickValueToText(c.TicksPerQuarterNote);
                c.RaiseEvent(new RoutedEventArgs(TicksPerQuarterNoteChangedEvent));
            }
        }

        /// <summary>
        /// Provides notification when the <see cref="TicksPerQuarterNote"/> property is changed.
        /// </summary>
        public event RoutedEventHandler TicksPerQuarterNoteChanged
        {
            add => AddHandler(TicksPerQuarterNoteChangedEvent, value);
            remove => RemoveHandler(TicksPerQuarterNoteChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="TicksPerQuarterNoteChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TicksPerQuarterNoteChangedEvent = EventManager.RegisterRoutedEvent
            (
                nameof(TicksPerQuarterNoteChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DrumPatternController)
            );
        #endregion

        /************************************************************************/

        #region TickValueText
        /// <summary>
        /// Gets the text that corresponds to <see cref="TicksPerQuarterNote"/>.
        /// </summary>
        public string TickValueText
        {
            get => (string)GetValue(TickValueTextProperty);
            private set => SetValue(TickValueTextPropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey TickValueTextPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(TickValueText), typeof(string), typeof(DrumPatternController), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="TickValueText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickValueTextProperty = TickValueTextPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Scale
        /// <summary>
        /// Gets or sets the scale
        /// </summary>
        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Scale"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register
            (
                nameof(Scale), typeof(double), typeof(DrumPatternController), new PropertyMetadata(Constants.DrumPattern.Scale.Default, OnScaleChanged, OnScaleCoerce)
            );

        private static object OnScaleCoerce(DependencyObject d, object baseValue)
        {
            double proposed = (double)baseValue;
            return Math.Min(Constants.DrumPattern.Scale.Max, Math.Max(Constants.DrumPattern.Scale.Min, proposed));
        }

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrumPatternController c)
            {
                c.SetIsChanged();
                c.RaiseEvent(new RoutedEventArgs(ScaleChangedEvent));
            }
        }

        /// <summary>
        /// Provides notification when the <see cref="Scale"/> property is changed.
        /// </summary>
        public event RoutedEventHandler ScaleChanged
        {
            add => AddHandler(ScaleChangedEvent, value);
            remove => RemoveHandler(ScaleChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="ScaleChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ScaleChangedEvent = EventManager.RegisterRoutedEvent
            (
                nameof(ScaleChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DrumPatternController)
            );

        /// <summary>
        /// Gets the minimum allowed value for <see cref="Scale"/>.
        /// Used to bind to the control template.
        /// </summary>
        public double MinScale
        {
            get => Constants.DrumPattern.Scale.Min;
        }

        /// <summary>
        /// Gets the maximum allowed value for <see cref="Scale"/>.
        /// Used to bind to the control template.
        /// </summary>
        public double MaxScale
        {
            get => Constants.DrumPattern.Scale.Max;
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
            var element = new XElement(nameof(DrumPatternController));
            element.Add(new XElement(nameof(Volume), Volume));
            element.Add(new XElement(nameof(QuarterNoteCount), QuarterNoteCount));
            element.Add(new XElement(nameof(TicksPerQuarterNote), TicksPerQuarterNote));
            element.Add(new XElement(nameof(Scale), Scale));
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
                if (e.Name == nameof(Volume)) SetDependencyProperty(VolumeProperty, e.Value);
                if (e.Name == nameof(QuarterNoteCount)) SetDependencyProperty(QuarterNoteCountProperty, e.Value);
                if (e.Name == nameof(TicksPerQuarterNote))
                {
                    SetDependencyProperty(TicksPerQuarterNoteProperty, e.Value);
                }
                if (e.Name == nameof(Scale)) SetDependencyProperty(ScaleProperty, e.Value);
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        /// <param name="disposing">true if disposing</param>
        [SuppressMessage("Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "Disposal happens via SharpDx.Utilities")]
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (submixVoice != null)
                {
                    SharpDX.Utilities.Dispose(ref submixVoice);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Called when the template has been applied
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string that describes this object</returns>
        public override string ToString()
        {
            return $"{nameof(DrumPatternController)} {DisplayName} Q:{QuarterNoteCount} Tick:{TicksPerQuarterNote} Scale:{Scale}";
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when <see cref="AudioControlBase.Volume"/> property is changed.
        /// </summary>
        protected override void OnVolumeChanged()
        {
            if (SubmixVoice != null)
            {
                SubmixVoice.SetVolume(ThreadSafeVolume);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void EqualizerControllerIsActiveChanged(object sender, RoutedEventArgs e)
        {
            if (Owner.EqualizerController.IsActive)
                submixVoice.EnableEffect(0);
            else
                submixVoice.DisableEffect(0);

            e.Handled = true;
        }

        private void EqualizerControllerGainChanged(object sender, EqualizerRoutedEventArgs e)
        {
            SetEqualizerGain(e.Band, e.Gain);
            e.Handled = true;
        }

        private void SetEqualizerGain(int band, float gain)
        {
            switch (band)
            {
                case 0:
                    equalizerParameters.Gain0 = gain;
                    break;
                case 1:
                    equalizerParameters.Gain1 = gain;
                    break;
                case 2:
                    equalizerParameters.Gain2 = gain;
                    break;
                case 3:
                    equalizerParameters.Gain3 = gain;
                    break;
            }
            submixVoice.SetEffectParameters(0, equalizerParameters);
        }

        /// <summary>
        /// Gets the text for the specified tick value.
        /// </summary>
        /// <param name="totalTicks">The total number of ticks</param>
        /// <returns>The tick string</returns>
        private string TickValueToText(int totalTicks)
        {
            switch (totalTicks)
            {
                case Constants.DrumPattern.TicksPerQuarterNote.Eighth:
                    return "8th";

                case Constants.DrumPattern.TicksPerQuarterNote.EighthTriplet:
                    return "8th (t)";

                case Constants.DrumPattern.TicksPerQuarterNote.Sixteenth:
                    return "16th";

                case Constants.DrumPattern.TicksPerQuarterNote.ThirtySecond:
                    return "32nd";

                default:
                    return "--";
            }
        }
        #endregion
    }
}