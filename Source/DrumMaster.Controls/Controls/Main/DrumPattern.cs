using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using SharpDX.XAudio2;
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
            Presenter = ThreadSafePresenter = new DrumPatternPresenter(this);
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

        /// <summary>
        /// Gets a thread safe reference to <see cref="Presenter"/>.
        /// </summary>
        internal DrumPatternPresenter ThreadSafePresenter
        {
            get;
        }
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
