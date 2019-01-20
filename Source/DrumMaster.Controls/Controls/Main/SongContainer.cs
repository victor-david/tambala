using Restless.App.DrumMaster.Controls.Core;
using Restless.App.DrumMaster.Controls.Resources;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a control that presents and manages a series of patterns to incorporate into a song.
    /// </summary>
    /// <remarks>
    /// This control provides the ability to select and manage which individual drum kit patterns
    /// that comprise a song and the timeline for selecting which drum kit patterns play and for how
    /// many times.
    /// </remarks>
    public class SongContainer : ControlObjectSelector
    {
        #region Private
        #endregion
        
        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SongContainer"/> class.
        /// </summary>
        internal SongContainer(ProjectContainer owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            DisplayName = Strings.SongContainerDisplayName;
            Presenter = new SongPresenter(this);
            ChangeContainerHeightImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Caret.Up.Down.White.32.png", UriKind.Relative));
            Commands.Add("ChangeContainerHeight", new RelayCommand(RunChangeContainerHeightCommand));
            AddHandler(IsSelectedChangedEvent, new RoutedEventHandler(IsSelectedChangedEventHandler));
        }

        static SongContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SongContainer), new FrameworkPropertyMetadata(typeof(SongContainer)));
        }
        #endregion

        /************************************************************************/

        #region SongPresenter (CLR)
        /// <summary>
        /// Gets the song presenter
        /// </summary>
        public SongPresenter Presenter
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
                nameof(SelectedEventCount), typeof(int), typeof(SongContainer), new PropertyMetadata(0, OnSelectedEventCountChanged)
            );

        /// <summary>
        /// Identifies the <see cref="SelectedEventCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedEventCountProperty = SelectedEventCountPropertyKey.DependencyProperty;

        private static void OnSelectedEventCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SongContainer c)
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

        #region ChangeContainerHeightImageSource
        /// <summary>
        /// Gets the image source for the change container height button.
        /// </summary>
        public ImageSource ChangeContainerHeightImageSource
        {
            get => (ImageSource)GetValue(ChangeContainerHeightImageSourceProperty);
            private set => SetValue(ChangeContainerHeightImageSourcePropertyKey, value);
        }

        private static readonly DependencyPropertyKey ChangeContainerHeightImageSourcePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ChangeContainerHeightImageSource), typeof(ImageSource), typeof(SongContainer), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ChangeContainerHeightImageSource"/> dependency property,
        /// </summary>
        public static readonly DependencyProperty ChangeContainerHeightImageSourceProperty = ChangeContainerHeightImageSourcePropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Owner (CLR)
        /// <summary>
        /// Gets the <see cref="ProjectContainer"/> that owns this instance.
        /// </summary>
        internal ProjectContainer Owner
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region IXElement
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public override XElement GetXElement()
        {
            var element = new XElement(nameof(SongContainer));
            element.Add(new XElement(nameof(SelectorType), SelectorType));
            element.Add(new XElement(nameof(SelectorSize), SelectorSize));
            element.Add(new XElement(nameof(DivisionCount), DivisionCount));
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
                if (e.Name == nameof(SelectorSize)) SetDependencyProperty(SelectorSizeProperty, e.Value);
                if (e.Name == nameof(DivisionCount)) SetDependencyProperty(DivisionCountProperty, e.Value);
                if (e.Name == nameof(SongPresenter))
                {
                    Presenter.RestoreFromXElement(e);
                }
            }

            SelectedEventCount = Presenter.SongSelectors.GetSelectedCount();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when <see cref="ControlObjectSelector.SelectorSize"/> changes.
        /// </summary>
        protected override void OnSelectorSizeChanged()
        {
            Presenter.SelectorSize = SelectorSize;
            SetIsChanged();
        }

        /// <summary>
        /// Called when <see cref="ControlObjectSelector.DivisionCount"/> changes.
        /// </summary>
        protected override void OnDivisionCountChanged()
        {
            Presenter.DivisionCount = DivisionCount;
            SetIsChanged();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunChangeContainerHeightCommand(object parm)
        {
            Owner.ChangeSongContainerHeight();
        }

        private void IsSelectedChangedEventHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is PointSelector selector && selector.SelectorType == PointSelectorType.SongRow)
            {
                int delta = selector.IsSelected ? 1 : -1;
                SelectedEventCount += delta;
                e.Handled = true;
            }
        }
        #endregion

    }
}
