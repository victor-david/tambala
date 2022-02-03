/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Controls.Core;
using SharpDX.XAPO.Fx;
using System;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Represents a 4 band equalizer controller.
    /// </summary>
    public class EqualizerController : ControlElement
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualizerController"/> class
        /// </summary>
        internal EqualizerController()
        {
            ResetCommand = new RelayCommand(RunResetCommand);
        }

        static EqualizerController()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EqualizerController), new FrameworkPropertyMetadata(typeof(EqualizerController)));
        }
        #endregion

        /************************************************************************/

        #region IsActive
        /// <summary>
        /// Gets or sets a value that indicates if the equalizer is active.
        /// </summary>
        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register
            (
                nameof(IsActive), typeof(bool), typeof(EqualizerController), new PropertyMetadata(true, OnIsActiveChanged)
            );

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EqualizerController c)
            {
                c.SetIsChanged();
                c.RaiseEvent(new RoutedEventArgs(IsActiveChangedEvent));
            }
        }
        #endregion

        /************************************************************************/

        #region Gain0
        /// <summary>
        /// Gets or sets the gain for band 0.
        /// </summary>
        public float Gain0
        {
            get => (float)GetValue(Gain0Property);
            set => SetValue(Gain0Property, value);
        }

        /// <summary>
        /// Identifies the <see cref="Gain0"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Gain0Property = DependencyProperty.Register
            (
                nameof(Gain0), typeof(float), typeof(EqualizerController), new PropertyMetadata(Constants.Equalizer.Gain.Default, OnGainChanged, OnGainCoerce)
            );
        #endregion

        /************************************************************************/

        #region Gain1
        /// <summary>
        /// Gets or sets the gain for band 1.
        /// </summary>
        public float Gain1
        {
            get => (float)GetValue(Gain1Property);
            set => SetValue(Gain1Property, value);
        }

        /// <summary>
        /// Identifies the <see cref="Gain1"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Gain1Property = DependencyProperty.Register
            (
                nameof(Gain1), typeof(float), typeof(EqualizerController), new PropertyMetadata(Constants.Equalizer.Gain.Default, OnGainChanged, OnGainCoerce)
            );
        #endregion

        /************************************************************************/

        #region Gain2
        /// <summary>
        /// Gets or sets the gain for band 2.
        /// </summary>
        public float Gain2
        {
            get => (float)GetValue(Gain2Property);
            set => SetValue(Gain2Property, value);
        }

        /// <summary>
        /// Identifies the <see cref="Gain2"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Gain2Property = DependencyProperty.Register
            (
                nameof(Gain2), typeof(float), typeof(EqualizerController), new PropertyMetadata(Constants.Equalizer.Gain.Default, OnGainChanged, OnGainCoerce)
            );
        #endregion

        /************************************************************************/

        #region Gain3
        /// <summary>
        /// Gets or sets the gain for band 3.
        /// </summary>
        public float Gain3
        {
            get => (float)GetValue(Gain3Property);
            set => SetValue(Gain3Property, value);
        }

        /// <summary>
        /// Identifies the <see cref="Gain3"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Gain3Property = DependencyProperty.Register
            (
                nameof(Gain3), typeof(float), typeof(EqualizerController), new PropertyMetadata(Constants.Equalizer.Gain.Default, OnGainChanged, OnGainCoerce)
            );
        #endregion

        /************************************************************************/

        #region Gain support methods
        private static object OnGainCoerce(DependencyObject d, object baseValue)
        {
            float proposed = (float)baseValue;
            return Math.Min(Constants.Equalizer.Gain.Max, Math.Max(Constants.Equalizer.Gain.Min, proposed));
        }

        private static void OnGainChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EqualizerController c)
            {
                c.SetIsChanged();
                c.RaiseGainEvent(e.Property.Name);
            }
        }
        #endregion

        /************************************************************************/

        #region Routed events
        /// <summary>
        /// Provides notification when the <see cref="IsActive"/> property changes
        /// </summary>
        public event RoutedEventHandler IsActiveChanged
        {
            add => AddHandler(IsActiveChangedEvent, value);
            remove => RemoveHandler(IsActiveChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsActiveChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent IsActiveChangedEvent = EventManager.RegisterRoutedEvent
            (
                nameof(IsActiveChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EqualizerController)
            );

        /// <summary>
        /// Provides notification when the one of the gain properties changes.
        /// </summary>
        public event EqualizerRoutedEventHandler GainChanged
        {
            add => AddHandler(GainChangedEvent, value);
            remove => RemoveHandler(GainChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="GainChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent GainChangedEvent = EventManager.RegisterRoutedEvent
            (
                nameof(GainChanged), RoutingStrategy.Bubble, typeof(EqualizerRoutedEventHandler), typeof(EqualizerController)
            );
        #endregion

        /************************************************************************/

        #region Min/Max Gain
        /// <summary>
        /// Get the minimum gain allowed. Used to bind to the control template.
        /// </summary>
        public float MinGain
        {
            get => Constants.Equalizer.Gain.Min;
        }

        /// <summary>
        /// Get the maximum gain allowed. Used to bind to the control template.
        /// </summary>
        public float MaxGain
        {
            get => Constants.Equalizer.Gain.Max;
        }
        #endregion

        /************************************************************************/

        #region ResetCommand
        /// <summary>
        /// Gets the command used to reset all equalizer gain values.
        /// </summary>
        public ICommand ResetCommand
        {
            get;
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
            var element = new XElement(nameof(EqualizerController));
            element.Add(new XElement(nameof(IsActive), IsActive));
            element.Add(new XElement(nameof(Gain0), Gain0));
            element.Add(new XElement(nameof(Gain1), Gain1));
            element.Add(new XElement(nameof(Gain2), Gain2));
            element.Add(new XElement(nameof(Gain3), Gain3));
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
                if (e.Name == nameof(IsActive)) SetDependencyProperty(IsActiveProperty, e.Value);
                if (e.Name == nameof(Gain0)) SetDependencyProperty(Gain0Property, e.Value);
                if (e.Name == nameof(Gain1)) SetDependencyProperty(Gain1Property, e.Value);
                if (e.Name == nameof(Gain2)) SetDependencyProperty(Gain2Property, e.Value);
                if (e.Name == nameof(Gain3)) SetDependencyProperty(Gain3Property, e.Value);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RaiseGainEvent(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Gain0):
                    RaiseGainEvent(0, GainToEqualizerGain(Gain0));
                    break;
                case nameof(Gain1):
                    RaiseGainEvent(1, GainToEqualizerGain(Gain1));
                    break;
                case nameof(Gain2):
                    RaiseGainEvent(2, GainToEqualizerGain(Gain2));
                    break;
                case nameof(Gain3):
                    RaiseGainEvent(3, GainToEqualizerGain(Gain3));
                    break;
            }
        }


        private void RaiseGainEvent(int band, float gain)
        {
            RaiseEvent(new EqualizerRoutedEventArgs(GainChangedEvent, band, gain));
        }

        private float GainToEqualizerGain(float gain)
        {
            float egain = Equalizer.DefaultGain;

            if (gain > 0)
            {
                float highRange = Equalizer.MaximumGain - Equalizer.DefaultGain;
                egain = Equalizer.DefaultGain + (gain * highRange);
            }

            if (gain < 0)
            {
                float lowRange = Equalizer.DefaultGain - Equalizer.MinimumGain;
                egain = Equalizer.DefaultGain - (Math.Abs(gain) * lowRange);
            }

            return egain;
        }

        private void RunResetCommand(object parm)
        {
            Gain0 = Gain1 = Gain2 = Gain3 = Constants.Equalizer.Gain.Default;
        }
        #endregion
    }
}