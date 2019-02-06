/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the MIT License
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Tambala.Controls.Audio;
using Restless.App.Tambala.Controls.Core;
using System;
using System.ComponentModel;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.Tambala.Controls
{
    /// <summary>
    /// Represents the master output control. Handles the final voice.
    /// </summary>
    public sealed class MasterOutput : AudioControlBase, INotifyPropertyChanged
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterOutput"/> class.
        /// </summary>
        /// <param name="owner">The project container that owns this instance.</param>
        internal MasterOutput(ProjectContainer owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            DisplayName = "Master Output";
        }

        static MasterOutput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterOutput), new FrameworkPropertyMetadata(typeof(MasterOutput)));
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
                nameof(Tempo), typeof(double), typeof(MasterOutput), new PropertyMetadata(Constants.Tempo.Default, OnTempoChanged, OnTempoCoerce)
            );

        private static void OnTempoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MasterOutput c)
            {
                c.RaiseEvent(new RoutedEventArgs(TempoChangedEvent));
                c.SetIsChanged();
            }
        }

        private static object OnTempoCoerce(DependencyObject d, object baseValue)
        {
            double proposed = (double)baseValue;
            return Math.Min(Constants.Tempo.Max, Math.Max(Constants.Tempo.Min, proposed));
        }

        /// <summary>
        /// Provides notification when the <see cref="Tempo"/> property is changed.
        /// </summary>
        public event RoutedEventHandler TempoChanged
        {
            add => AddHandler(TempoChangedEvent, value);
            remove => RemoveHandler(TempoChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="TempoChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TempoChangedEvent = EventManager.RegisterRoutedEvent
            (
                nameof(TempoChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MasterOutput)
            );


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
                nameof(TempoText), typeof(string), typeof(MasterOutput), new PropertyMetadata(Constants.Tempo.DefaultText)
            );

        /// <summary>
        /// Gets the minimum tempo allowed. Used for binding in the control template.
        /// </summary>
        public double MinTempo
        {
            get => Constants.Tempo.Min;
        }

        /// <summary>
        /// Gets the maximum tempo allowed. Used for binding in the control template.
        /// </summary>
        public double MaxTempo
        {
            get => Constants.Tempo.Max;
        }
        #endregion

        /************************************************************************/

        #region VolumePeak
        /// <summary>
        /// Gets the peak volume.
        /// </summary>
        public float VolumePeak
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
            var element = new XElement(nameof(MasterOutput));
            element.Add(new XElement(nameof(Tempo), Tempo));
            element.Add(new XElement(nameof(Volume), Volume));
            element.Add(new XElement(nameof(Panning), Panning));
            element.Add(new XElement(nameof(Pitch), Pitch));
            element.Add(new XElement(nameof(IsMuted), IsMuted));
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
                if (e.Name == nameof(Tempo)) SetDependencyProperty(TempoProperty, e.Value);
                if (e.Name == nameof(Volume)) SetDependencyProperty(VolumeProperty, e.Value);
                if (e.Name == nameof(Panning)) SetDependencyProperty(PanningProperty, e.Value);
                if (e.Name == nameof(Pitch)) SetDependencyProperty(PitchProperty, e.Value);
                if (e.Name == nameof(IsMuted)) SetDependencyProperty(IsMutedProperty, e.Value);
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when <see cref="AudioControlBase.Volume"/> is changed
        /// to set the volume on the mastering voice.
        /// </summary>
        protected override void OnVolumeChanged()
        {
            AudioHost.Instance.MasterVoice.SetVolume(ThreadSafeVolume);
        }
        #endregion

        /************************************************************************/

        #region INotifyPropertyChanged
        /// <summary>
        /// Enables listeners to respond to property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Sets the peak volume.
        /// </summary>
        /// <param name="peak">The peak</param>
        /// <remarks>
        /// The calling thread should limit how rapidly it callss this method
        /// </remarks>
        internal void SetVolumePeak(float peak)
        {
            VolumePeak = peak * 100f;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VolumePeak)));
        }
        #endregion
    }
}