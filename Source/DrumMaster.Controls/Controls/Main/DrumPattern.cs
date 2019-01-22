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
            DrumKit = Owner.DrumKits[DrumKitCollection.DrumKitDefaultId];
            Controller = ThreadSafeController = new DrumPatternController(this);
            Presenter = ThreadSafePresenter = new DrumPatternPresenter(this);
            AddHandler(ControlObjectSelector.IsSelectedChangedEvent, new RoutedEventHandler(SelectorIsSelectedChanged));
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

        #region DrumKit / DrumKitId
        /// <summary>
        /// Gets the drum kit associated with this drum pattern.
        /// </summary>
        public DrumKit DrumKit
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the drum kit id for this drum pattern.
        /// </summary>
        public string DrumKitId
        {
            get => (string)GetValue(DrumKitIdProperty);
            set => SetValue(DrumKitIdProperty, value);
        }

        public static readonly DependencyProperty DrumKitIdProperty = DependencyProperty.Register
            (
                nameof(DrumKitId), typeof(string), typeof(DrumPattern), new PropertyMetadata(DrumKitCollection.DrumKitDefaultId, OnDrumKitIdChanged)
            );

        private static void OnDrumKitIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrumPattern c)
            {
                if (c.Owner.DrumKits.ContainsDrumKit(c.DrumKitId))
                {
                    c.DrumKit = c.Owner.DrumKits[c.DrumKitId];
                    c.RaiseEvent(new RoutedEventArgs(DrumKitChangedEvent));
                    c.SetIsChanged();
                }
            }
        }

        /// <summary>
        /// Provides notification when the <see cref="DrumKit"/> property changes.
        /// </summary>
        public event RoutedEventHandler DrumKitChanged
        {
            add => AddHandler(DrumKitChangedEvent, value);
            remove => RemoveHandler(DrumKitChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="DrumKitChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DrumKitChangedEvent = EventManager.RegisterRoutedEvent
            (
                nameof(DrumKitChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DrumPattern)
            );
        #endregion

        /************************************************************************/

        #region Presenter
        /// <summary>
        /// Gets the <see cref="DrumPatternPresenter"/> object.
        /// </summary>
        internal DrumPatternPresenter Presenter
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
        internal static readonly DependencyProperty PresenterProperty = PresenterPropertyKey.DependencyProperty;

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

        /// <summary>
        /// Gets a thread safe reference to <see cref="Controller"/>.
        /// </summary>
        internal DrumPatternController ThreadSafeController
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region SelectedEventCount
        /// <summary>
        /// Gets the total number of events selected for this drum pattern
        /// </summary>
        public int SelectedEventCount
        {
            get => (int)GetValue(SelectedEventCountProperty);
            private set => SetValue(SelectedEventCountPropertyKey, value);
        }

        private static readonly DependencyPropertyKey SelectedEventCountPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(SelectedEventCount), typeof(int), typeof(DrumPattern), new PropertyMetadata(0, OnSelectedEventCountChanged)
            );

        /// <summary>
        /// Identifies the <see cref="SelectedEventCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedEventCountProperty = SelectedEventCountPropertyKey.DependencyProperty;

        private static void OnSelectedEventCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrumPattern c)
            {
                c.ThreadSafeSelectedEventCount = c.SelectedEventCount;
            }
        }

        /// <summary>
        /// Gets the thread safe value of <see cref="SelectedEventCount"/>.
        /// </summary>
        internal int ThreadSafeSelectedEventCount
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
            var element = new XElement(nameof(DrumPattern));
            element.Add(new XElement(nameof(DisplayName), DisplayName));
            element.Add(DrumKit.GetXElement());
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
            Presenter.Create();
            foreach (XElement e in ChildElementList(element))
            {
                if (e.Name == nameof(DisplayName)) SetDependencyProperty(DisplayNameProperty, e.Value);
                if (e.Name == nameof(DrumPatternController)) Controller.RestoreFromXElement(e);
                if (e.Name == nameof(DrumPatternPresenter)) Presenter.RestoreFromXElement(e);
                if (e.Name == nameof(Controls.DrumKit))
                {
                    foreach (XElement dke in ChildElementList(e))
                    {
                        if (dke.Name == nameof(Controls.DrumKit.Id))
                        {
                            DrumKitId = dke.Value;
                        }
                    }
                }
            }
            SelectedEventCount = Presenter.GetSelectedCount();
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

        private void SelectorIsSelectedChanged(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is PointSelector selector)
            {
                int delta = selector.IsSelected ? 1 : -1;
                SelectedEventCount += delta;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Gets the text for the specified tick value.
        /// </summary>
        /// <param name="tickValue">The tick value</param>
        /// <returns>The tick string</returns>
        private string TickValueToText(int tickValue)
        {
            switch (tickValue)
            {
                case Constants.DrumPattern.TicksPerQuarterNote.Eighth:
                    return "8th";

                case Constants.DrumPattern.TicksPerQuarterNote.EighthTriplet:
                    return "8th(t)";

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
