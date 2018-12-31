using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a container for a series of <see cref="TrackBox"/> items.
    /// </summary>
    [TemplatePart(Name = PartHostGrid, Type = typeof(Grid))]
    public class TrackBoxContainer : TrackSized
    {
        #region Private
        private const string PartHostGrid = "PART_HOST_GRID";
        private Grid hostGrid;
        private TrackController controller;
        private XElement holdElement;
        #endregion

        /************************************************************************/

        #region Public properties (Type / Steps)
        /// <summary>
        /// Gets or sets the box type.
        /// </summary>
        public TrackBoxType BoxType
        {
            get => (TrackBoxType)GetValue(BoxTypeProperty);
            set => SetValue(BoxTypeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BoxType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BoxTypeProperty = DependencyProperty.Register
            (
                nameof(BoxType), typeof(TrackBoxType), typeof(TrackBoxContainer), new PropertyMetadata(TrackBoxType.Header)
            );

        /// <summary>
        /// Gets or sets the total steps, i.e. the total number of boxes
        /// </summary>
        public int TotalSteps
        {
            get => (int)GetValue(TotalStepsProperty);
            set => SetValue(TotalStepsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TotalSteps"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TotalStepsProperty = DependencyProperty.Register
            (
                nameof(TotalSteps), typeof(int), typeof(TrackBoxContainer), new PropertyMetadata(TrackVals.TotalSteps.Default, OnTotalStepsChanged, OnTotalStepsCoerce)
            );

        private static void OnTotalStepsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackBoxContainer c)
            {
                c.OnTotalStepsChanged();
                c.SetIsChanged();
            }
        }

        private static object OnTotalStepsCoerce(DependencyObject d, object baseValue)
        {
            int proposed = (int)baseValue;
            return Math.Min(TrackVals.TotalSteps.Max, Math.Max(TrackVals.TotalSteps.Min, proposed));
        }
        #endregion

        /************************************************************************/

        #region Public properties (Brushes)
        /// <summary>
        /// Gets or sets the brush that is used when a header step is selected.
        /// </summary>
        public Brush SelectedBackgroundBrush
        {
            get => (Brush)GetValue(SelectedBackgroundBrushProperty);
            set => SetValue(SelectedBackgroundBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectedBackgroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundBrushProperty = DependencyProperty.Register
            (
                nameof(SelectedBackgroundBrush), typeof(Brush), typeof(TrackBoxContainer), new PropertyMetadata(new SolidColorBrush(Colors.Red), OnSelectedBackgroundBrushChanged)
            );

        private static void OnSelectedBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion

        /************************************************************************/

        #region Routed Events
        /// <summary>
        /// Provides notification when the <see cref="TotalSteps"/> property is changed.
        /// </summary>
        public event RoutedEventHandler TotalStepsChanged
        {
            add => AddHandler(TotalStepsChangedEvent, value);
            remove => RemoveHandler(TotalStepsChangedEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="TotalStepsChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TotalStepsChangedEvent = EventManager.RegisterRoutedEvent
            (
                nameof(TotalStepsChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TrackBoxContainer)
            );
        #endregion

        /************************************************************************/

        #region Internal properties
        /// <summary>
        /// Gets a collection of track boxes
        /// </summary>
        internal TrackBoxCollection Boxes
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackBoxContainer"/> class.
        /// </summary>
        public TrackBoxContainer()
        {
            Boxes = new TrackBoxCollection();
        }

        static TrackBoxContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TrackBoxContainer), new FrameworkPropertyMetadata(typeof(TrackBoxContainer)));
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
            hostGrid = GetTemplateChild(PartHostGrid) as Grid;
            OnTotalStepsChanged();
            if (holdElement != null)
            {
                RestoreFromXElement(holdElement);
            }
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
            var element = new XElement(nameof(TrackBoxContainer));
            foreach (var box in Boxes)
            {
                element.Add(box.GetXElement());
            }
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            holdElement = element;
            if (IsTemplateApplied)
            {
                IEnumerable<XElement> childList = from el in element.Elements() select el;
                int boxIndex = 0;
                foreach (XElement e in childList)
                {
                    if (e.Name == nameof(TrackBox))
                    {
                        if (boxIndex < Boxes.Count)
                        {
                            Boxes[boxIndex].RestoreFromXElement(e);
                        }
                        boxIndex++;
                    }
                }
                ResetIsChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the box size is changed
        /// </summary>
        protected override void OnBoxSizeChanged()
        {
            base.OnBoxSizeChanged();
            foreach (var box in Boxes)
            {
                box.Width = box.Height = BoxSize;
            }
        }

        /// <summary>
        /// Deselects all boxes.
        /// </summary>
        public void DeselectAllBoxes()
        {
            Boxes.SetAllTo(StepPlayFrequency.None);
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// From this assembly, sets the track controller
        /// </summary>
        /// <param name="controller">The controller.</param>
        internal void SetController(TrackController controller)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            controller.SizeChanged += ControllerSizeChanged;
        }

        /// <summary>
        /// From this assembly, gets a value that indicates if the specified
        /// pass and step can play.
        /// </summary>
        /// <param name="pass">The pass</param>
        /// <param name="step">The step</param>
        /// <returns>true if pass/step can play; otherwise, false.</returns>
        internal bool CanPlay(int pass, int step)
        {
            if (step < Boxes.Count)
            {
                return Boxes[step].CanPlay(pass);
            }
            return false;
        }

        /// <summary>
        /// From this assembly, removes any human volume bias from each <see cref="TrackBox"/> in the container.
        /// </summary>
        internal void RemoveHumanVolumeBias()
        {
            foreach (TrackBox box in Boxes)
            {
                box.RemoveHumanVolumeBias();
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (Instance)
        private void OnTotalStepsChanged()
        {
            if (hostGrid != null)
            {
                int currentTotalSteps = hostGrid.ColumnDefinitions.Count;
                if (TotalSteps > currentTotalSteps)
                {
                    int stepsToAdd = TotalSteps - currentTotalSteps;
                    for (int k = 0; k < stepsToAdd; k++)
                    {
                        hostGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                        var box = new TrackBox(this)
                        {
                            BoxType = BoxType,
                            Height = BoxSize,
                            Width = BoxSize,
                            SelectedBackgroundBrush = SelectedBackgroundBrush,
                        };
                        Grid.SetColumn(box, hostGrid.ColumnDefinitions.Count - 1);
                        hostGrid.Children.Add(box);
                        Boxes.Add(box);
                    }
                }
                else
                {
                    while (currentTotalSteps > TotalSteps)
                    {
                        hostGrid.Children.RemoveAt(currentTotalSteps - 1);
                        hostGrid.ColumnDefinitions.RemoveAt(currentTotalSteps - 1);
                        Boxes.RemoveAt(currentTotalSteps - 1);
                        currentTotalSteps--;
                    }
                }
                
                RoutedEventArgs args = new RoutedEventArgs(TotalStepsChangedEvent);
                RaiseEvent(args);
            }
        }

        private void ControllerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                Height = e.NewSize.Height;
            }
        }
        #endregion
    }
}
