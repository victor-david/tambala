using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a track box (header or step).
    /// </summary>
    public class TrackBox : TrackControlBase
    {
        #region Private
        private TrackBoxContainer owner;
        private StepPlayFrequency playFrequency;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the box type.
        /// </summary>
        public TrackBoxType BoxType
        {
            get => (TrackBoxType)GetValue(BoxTypeProperty);
            set => SetValue(BoxTypeProperty, value);
        }

        public static readonly DependencyProperty BoxTypeProperty = DependencyProperty.Register
            (
                nameof(BoxType), typeof(TrackBoxType), typeof(TrackBox), new PropertyMetadata(TrackBoxType.Header)
            );


        /// <summary>
        /// Gets or sets the text associated with this box
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register
            (
                nameof(Text), typeof(string), typeof(TrackBox), new PropertyMetadata(null)
            );


        ///// <summary>
        ///// Gets or sets a value that indicates if this box is selected.
        ///// </summary>
        //public bool IsSelected
        //{
        //    get => (bool)GetValue(IsSelectedProperty);
        //    set => SetValue(IsSelectedProperty, value);
        //}

        //public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register
        //    (
        //        nameof(IsSelected), typeof(bool), typeof(TrackBox), new PropertyMetadata(false, OnIsSelectedChanged)
        //    );

        /// <summary>
        /// Gets or sets the play frequency.
        /// </summary>
        public StepPlayFrequency PlayFrequency
        {
            get => (StepPlayFrequency)GetValue(PlayFrequencyProperty);
            set => SetValue(PlayFrequencyProperty, value);
        }

        public static readonly DependencyProperty PlayFrequencyProperty = DependencyProperty.Register
            (
                nameof(PlayFrequency), typeof(StepPlayFrequency), typeof(TrackBox), new PropertyMetadata(StepPlayFrequency.Default, OnPlayFrequencyChanged )
            );

        /// <summary>
        /// Gets or sets the brush that is used when <see cref="IsSelected"/> is true.
        /// </summary>
        public Brush SelectedBackgroundBrush
        {
            get => (Brush)GetValue(SelectedBackgroundBrushProperty);
            set => SetValue(SelectedBackgroundBrushProperty, value);
        }

        public static readonly DependencyProperty SelectedBackgroundBrushProperty = DependencyProperty.Register
            (
                nameof(SelectedBackgroundBrush), typeof(Brush), typeof(TrackBox), new PropertyMetadata(new SolidColorBrush(Colors.RosyBrown))
            );
        #endregion

        /************************************************************************/

        #region Protected properties

        #endregion

        /************************************************************************/

        #region Internal properties
        ///// <summary>
        ///// Gets the thread safe value of <see cref="IsSelected"/>.
        ///// </summary>
        //internal bool IsSelectedInternal
        //{
        //    get;
        //    private set;
        //}
        #endregion

        /************************************************************************/

        #region Constructors (Internal / Static)
        /// <summary>
        /// Creates a new instance of <see cref="TrackBox"/>
        /// </summary>
        /// <param name="owner">The owner of this control.</param>
        internal TrackBox(TrackBoxContainer owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Height = Width = TrackVals.BoxSize.Default;
            Commands.Add("ToggleIsSelected", new RelayCommand(RunToggleIsSelectedCommand));
            playFrequency = StepPlayFrequency.Default;
        }

        static TrackBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TrackBox), new FrameworkPropertyMetadata(typeof(TrackBox)));
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
            var element = new XElement(nameof(TrackBox));
            element.Add(new XElement(nameof(PlayFrequency), PlayFrequency));
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {

            IEnumerable<XElement> childList = from el in element.Elements() select el;
            foreach (XElement e in childList)
            {
                if (e.Name == nameof(PlayFrequency))
                {
                    StepPlayFrequency result = StepPlayFrequency.Default;

                    if (Enum.TryParse(e.Value, out result))
                    {
                        PlayFrequency = result;
                    }
                }
            }
            ResetIsChanged();
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        internal bool CanPlay(int pass)
        {
            switch (playFrequency)
            {
                case StepPlayFrequency.None:
                    return false;
                case StepPlayFrequency.EveryPass:
                    return true;
                case StepPlayFrequency.EvenPassOnly:
                    return pass % 2 == 0;
                case StepPlayFrequency.OddPassOnly:
                    return (pass + 1) % 2 == 0;
                default:
                    return false;
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        //protected override void OnBoxSizeChanged()
        //{
        //    Height = BoxSize;
        //    Width = BoxSize;
        //}
        #endregion

        /************************************************************************/

        #region Private methods (Instance)

        private void RunToggleIsSelectedCommand(object parm)
        {
            switch (playFrequency)
            {
                case StepPlayFrequency.None:
                    PlayFrequency = StepPlayFrequency.EveryPass;
                    break;
                case StepPlayFrequency.EveryPass:
                    PlayFrequency = StepPlayFrequency.OddPassOnly;
                    break;
                case StepPlayFrequency.OddPassOnly:
                    PlayFrequency = StepPlayFrequency.EvenPassOnly;
                    break;
                case StepPlayFrequency.EvenPassOnly:
                    PlayFrequency = StepPlayFrequency.None;
                    break;
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (Static)

        //private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is TrackBox c)
        //    {
        //        c.IsSelectedInternal = (bool)e.NewValue;
        //    }
        //}

        private static void OnPlayFrequencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackBox c)
            {
                c.playFrequency = (StepPlayFrequency)e.NewValue;
                if (c.owner.BoxType == TrackBoxType.TrackStep)
                {
                    c.SetIsChanged();
                }
            }
        }
        #endregion
    }
}
