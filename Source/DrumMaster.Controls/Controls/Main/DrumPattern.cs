using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Diagnostics;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a single drum pattern that is comprised of instruments, and the ability
    /// to select which ones play on the timeline.
    /// </summary>
    public class DrumPattern : AudioControlBase
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumPattern"/> class.
        /// </summary>
        internal DrumPattern(ProjectContainer owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Controller = new DrumPatternController(this);
            Presenter = new DrumPatternPresenter(this);
            AddHandler(DrumPatternController.QuarterNoteCountChangedEvent, new RoutedEventHandler((s, e) => {Presenter.QuarterNoteCount = Controller.QuarterNoteCount; e.Handled = true; }));
            AddHandler(DrumPatternController.TotalTicksChangedEvent, new RoutedEventHandler((s,e) => { Presenter.TotalTicks = Controller.TotalTicks; e.Handled = true; }));
            AddHandler(DrumPatternController.ScaleChangedEvent, new RoutedEventHandler((s,e) => { Presenter.Scale = Controller.Scale; e.Handled = true; }));
        }

        static DrumPattern()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrumPattern), new FrameworkPropertyMetadata(typeof(DrumPattern)));
        }
        #endregion

        /************************************************************************/

        #region Owner
        /// <summary>
        /// Gets the <see cref="ProjectContainer"/> that owns this instance.
        /// </summary>
        internal ProjectContainer Owner
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region QuarterNoteCount (commented out)
        ///// <summary>
        ///// Gets or sets the number of quarter notes
        ///// </summary>
        //public int QuarterNoteCount
        //{
        //    get => (int)GetValue(QuarterNoteCountProperty);
        //    set => SetValue(QuarterNoteCountProperty, value);
        //}

        ///// <summary>
        ///// Identifies the <see cref="QuarterNoteCount"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty QuarterNoteCountProperty = DependencyProperty.Register
        //    (
        //        nameof(QuarterNoteCount), typeof(int), typeof(DrumPattern), new PropertyMetadata(Constants.DrumPattern.QuarterNote.Default, OnQuarterNoteCountChanged, OnQuarterNoteCountCoerce)
        //    );

        //private static object OnQuarterNoteCountCoerce(DependencyObject d, object baseValue)
        //{
        //    int proposed = (int)baseValue;
        //    return Math.Min(Constants.DrumPattern.QuarterNote.Max, Math.Max(Constants.DrumPattern.QuarterNote.Min, proposed));
        //}

        //private static void OnQuarterNoteCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is DrumPattern c)
        //    {
        //        c.SetIsChanged();
        //        c.DrumPatternPresenter.SetPresenterParameters(c.QuarterNoteCount, c.TickValue, c.Scale);
        //    }
        //}

        ///// <summary>
        ///// Gets the minimum allowed value for <see cref="QuarterNoteCount"/>.
        ///// Used to bind to the control template.
        ///// </summary>
        //public int MinQuarterNoteCount
        //{
        //    get => Constants.DrumPattern.QuarterNote.Min;
        //}

        ///// <summary>
        ///// Gets the maximum allowed value for <see cref="QuarterNoteCount"/>.
        ///// Used to bind to the control template.
        ///// </summary>
        //public int MaxQuarterNoteCount
        //{
        //    get => Constants.DrumPattern.QuarterNote.Max;
        //}
        #endregion

        /************************************************************************/

        #region TickValue (commented out)
        ///// <summary>
        ///// Gets or sets the number of ticks per quarter note.
        ///// </summary>
        //public int TickValue
        //{
        //    get => (int)GetValue(TickValueProperty);
        //    set => SetValue(TickValueProperty, value);
        //}

        ///// <summary>
        ///// Identifies the <see cref="TickValue"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty TickValueProperty = DependencyProperty.Register
        //    (
        //        nameof(TickValue), typeof(int), typeof(DrumPattern), new PropertyMetadata(Constants.DrumPattern.Tick.Default, OnTickValueChanged, OnTickValueCoerce)
        //    );

        //private static object OnTickValueCoerce(DependencyObject d, object baseValue)
        //{
        //    int proposed = (int)baseValue;
        //    return Math.Min(Constants.DrumPattern.Tick.Max, Math.Max(Constants.DrumPattern.Tick.Min, proposed));
        //}

        //private static void OnTickValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is DrumPattern c)
        //    {
        //        c.SetIsChanged();
        //        c.TickValueText = c.TickValueToText(c.TickValue);
        //        c.DrumPatternPresenter.SetPresenterParameters(c.QuarterNoteCount, c.TickValue, c.Scale);
        //    }
        //}

        ///// <summary>
        ///// Gets the minimum allowed value for <see cref="TickValue"/>.
        ///// Used to bind to the control template.
        ///// </summary>
        //public int MinTickValue
        //{
        //    get => Constants.DrumPattern.Tick.Min;
        //}

        ///// <summary>
        ///// Gets the maximum allowed value for <see cref="TickValue"/>.
        ///// Used to bind to the control template.
        ///// </summary>
        //public int MaxTickValue
        //{
        //    get => Constants.DrumPattern.Tick.Max;
        //}
        #endregion

        /************************************************************************/

        #region TickValueText (commented out)
        ///// <summary>
        ///// Gets the text that corresponds to <see cref="TickValue"/>.
        ///// </summary>
        //public string TickValueText
        //{
        //    get => (string)GetValue(TickValueTextProperty);
        //    private set => SetValue(TickValueTextPropertyKey, value);
        //}

        //private static readonly DependencyPropertyKey TickValueTextPropertyKey = DependencyProperty.RegisterReadOnly
        //    (
        //        nameof(TickValueText), typeof(string), typeof(DrumPattern), new PropertyMetadata(null)
        //    );

        ///// <summary>
        ///// Identifies the <see cref="TickValueText"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty TickValueTextProperty = TickValueTextPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Scale (commented out)
        ///// <summary>
        ///// Gets or sets the scale
        ///// </summary>
        //public double Scale
        //{
        //    get => (double)GetValue(ScaleProperty);
        //    set => SetValue(ScaleProperty, value);
        //}

        ///// <summary>
        ///// Identifies the <see cref="Scale"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register
        //    (
        //        nameof(Scale), typeof(double), typeof(DrumPattern), new PropertyMetadata(Constants.DrumPattern.Scale.Default, OnScaleChanged, OnScaleCoerce)
        //    );


        //private static object OnScaleCoerce(DependencyObject d, object baseValue)
        //{
        //    double proposed = (double)baseValue;
        //    return Math.Min(Constants.DrumPattern.Scale.Max, Math.Max(Constants.DrumPattern.Scale.Min, proposed));
        //}

        //private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is DrumPattern c)
        //    {
        //        c.SetIsChanged();
        //        c.DrumPatternPresenter.SetPresenterParameters(c.QuarterNoteCount, c.TickValue, c.Scale);
        //    }
        //}

        ///// <summary>
        ///// Gets the minimum allowed value for <see cref="Scale"/>.
        ///// Used to bind to the control template.
        ///// </summary>
        //public double MinScale
        //{
        //    get => Constants.DrumPattern.Scale.Min;
        //}

        ///// <summary>
        ///// Gets the maximum allowed value for <see cref="Scale"/>.
        ///// Used to bind to the control template.
        ///// </summary>
        //public double MaxScale
        //{
        //    get => Constants.DrumPattern.Scale.Max;
        //}
        #endregion

        /************************************************************************/

        #region Presenter
        /// <summary>
        /// Gets the <see cref="DrumPatternPresenter"/> object.
        /// </summary>
        public DrumPatternPresenter Presenter
        {
            get => (DrumPatternPresenter)GetValue(PresenterProperty);
            private set => SetValue(PresenterPropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey PresenterPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(Presenter), typeof(DrumPatternPresenter), typeof(DrumPattern), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="Presenter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PresenterProperty = PresenterPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Controller
        /// <summary>
        /// Gets the <see cref="DrumPatternController"/> object.
        /// </summary>
        public DrumPatternController Controller
        {
            get => (DrumPatternController)GetValue(ControllerProperty);
            private set => SetValue(ControllerPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ControllerPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(Controller), typeof(DrumPatternController), typeof(DrumPattern), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="Controller"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ControllerProperty = ControllerPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region IXElement
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public override XElement GetXElement()
        {
            var element = new XElement(nameof(DrumPattern));
            element.Add(new XElement(nameof(DisplayName), DisplayName));
            //element.Add(new XElement(nameof(QuarterNoteCount), QuarterNoteCount));
            //element.Add(new XElement(nameof(TickValue), TickValue));
            //element.Add(new XElement(nameof(Scale), Scale));
            element.Add(Controller.GetXElement());
            element.Add(Presenter.GetXElement());
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
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string that describes this object</returns>
        public override string ToString()
        {
            return $"{nameof(DrumPattern)} {DisplayName}";
        }
        #endregion

        /************************************************************************/

        #region Private methods
        /// <summary>
        /// Gets the text for the specified tick value.
        /// </summary>
        /// <param name="tickValue">The tick value</param>
        /// <returns>The tick string</returns>
        private string TickValueToText(int tickValue)
        {
            switch (tickValue)
            {
                case Constants.DrumPattern.TotalTick.Eighth:
                    return "8th";

                case Constants.DrumPattern.TotalTick.EighthTriplet:
                    return "8th(t)";

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
