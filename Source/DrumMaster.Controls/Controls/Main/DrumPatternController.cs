using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using SharpDX.XAudio2;
using System;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a controller for a drum pattern.
    /// </summary>
    public class DrumPatternController : AudioControlBase
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumPatternController"/> class.
        /// </summary>
        internal DrumPatternController(DrumPattern owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            SubmixVoice = new SubmixVoice(AudioHost.Instance.AudioDevice);
            SubmixVoice.SetOutputVoices(new VoiceSendDescriptor(AudioHost.Instance.SubmixVoice));
            TickValueText = TickValueToText(Constants.DrumPattern.TotalTick.Default);
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
            get;
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
                nameof(QuarterNoteCount), typeof(int), typeof(DrumPatternController), new PropertyMetadata(Constants.DrumPattern.QuarterNote.Default, OnQuarterNoteCountChanged, OnQuarterNoteCountCoerce)
            );

        private static object OnQuarterNoteCountCoerce(DependencyObject d, object baseValue)
        {
            int proposed = (int)baseValue;
            return Math.Min(Constants.DrumPattern.QuarterNote.Max, Math.Max(Constants.DrumPattern.QuarterNote.Min, proposed));
        }

        private static void OnQuarterNoteCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrumPatternController c)
            {
                c.SetIsChanged();
                RoutedEventArgs args = new RoutedEventArgs(QuarterNoteCountChangedEvent);
                c.RaiseEvent(args);
            }
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
            get => Constants.DrumPattern.QuarterNote.Min;
        }

        /// <summary>
        /// Gets the maximum allowed value for <see cref="QuarterNoteCount"/>.
        /// Used to bind to the control template.
        /// </summary>
        public int MaxQuarterNoteCount
        {
            get => Constants.DrumPattern.QuarterNote.Max;
        }
        #endregion

        /************************************************************************/

        #region TickValue
        /// <summary>
        /// Gets or sets the number of ticks per quarter note.
        /// </summary>
        public int TotalTicks
        {
            get => (int)GetValue(TotalTicksProperty);
            set => SetValue(TotalTicksProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TotalTicks"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TotalTicksProperty = DependencyProperty.Register
            (
                nameof(TotalTicks), typeof(int), typeof(DrumPatternController), new PropertyMetadata(Constants.DrumPattern.TotalTick.Default, OnOnTotalTicksChanged, OnTotalTicksCoerce)
            );

        private static object OnTotalTicksCoerce(DependencyObject d, object baseValue)
        {
            int proposed = (int)baseValue;
            return Math.Min(Constants.DrumPattern.TotalTick.Max, Math.Max(Constants.DrumPattern.TotalTick.Min, proposed));
        }

        private static void OnOnTotalTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrumPatternController c)
            {
                c.SetIsChanged();
                c.TickValueText = c.TickValueToText(c.TotalTicks);
                RoutedEventArgs args = new RoutedEventArgs(TotalTicksChangedEvent);
                c.RaiseEvent(args);
            }
        }

        /// <summary>
        /// Provides notification when the <see cref="TotalTicks"/> property is changed.
        /// </summary>
        public event RoutedEventHandler TotalTicksChanged
        {
            add => AddHandler(TotalTicksChangedEvent, value);
            remove => RemoveHandler(TotalTicksChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="TotalTicksChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TotalTicksChangedEvent = EventManager.RegisterRoutedEvent
            (
                nameof(TotalTicksChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DrumPatternController)
            );
        #endregion

        /************************************************************************/

        #region TickValueText
        /// <summary>
        /// Gets the text that corresponds to <see cref="TotalTicks"/>.
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
                RoutedEventArgs args = new RoutedEventArgs(ScaleChangedEvent);
                c.RaiseEvent(args);
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
            element.Add(new XElement(nameof(DisplayName), DisplayName));
            element.Add(new XElement(nameof(QuarterNoteCount), QuarterNoteCount));
            element.Add(new XElement(nameof(TotalTicks), TotalTicks));
            element.Add(new XElement(nameof(Scale), Scale));
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
            return $"{nameof(DrumPatternController)} {DisplayName} Q:{QuarterNoteCount} Tick:{TotalTicks} Scale:{Scale}";
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
        /// <summary>
        /// Gets the text for the specified tick value.
        /// </summary>
        /// <param name="totalTicks">The total number of ticks</param>
        /// <returns>The tick string</returns>
        private string TickValueToText(int totalTicks)
        {
            switch (totalTicks)
            {
                case Constants.DrumPattern.TotalTick.Eighth:
                    return "8th";

                case Constants.DrumPattern.TotalTick.EighthTriplet:
                    return "8th (t)";

                case Constants.DrumPattern.TotalTick.Sixteenth:
                    return "16th";

                case Constants.DrumPattern.TotalTick.ThirtySecond:
                    return "32nd";

                default:
                    return "--";
            }
        }
        #endregion
    }
}
