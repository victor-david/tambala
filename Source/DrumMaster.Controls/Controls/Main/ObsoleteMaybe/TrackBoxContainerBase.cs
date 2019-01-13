using Restless.App.DrumMaster.Controls.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.App.DrumMaster.Controls.Obsolete
{
    /// <summary>
    /// Represents the base class for a container that contains for a series of <see cref="TrackBox"/> items.
    /// This class must be inherited.
    /// </summary>
    [TemplatePart(Name = PartHostGrid, Type = typeof(Grid))]
    public abstract class TrackBoxContainerBase : TrackStepControl
    {
        #region Private
        private const string PartHostGrid = "PART_HOST_GRID";
        private Grid hostGrid;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or (from a derived class) sets the box type.
        /// </summary>
        public TrackBoxType BoxType
        {
            get;
            protected set;
        }
        #endregion

        /************************************************************************/

        #region Public properties (Brushes)
        /// <summary>
        /// Gets or sets the brush that is used when a step is selected.
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
                nameof(SelectedBackgroundBrush), typeof(Brush), typeof(TrackBoxContainerBase), new PropertyMetadata(new SolidColorBrush(Colors.Red), OnSelectedBackgroundBrushChanged)
            );

        private static void OnSelectedBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
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
        /// Initializes a new instance of the <see cref="TrackBoxContainerBase"/> class.
        /// </summary>
        internal TrackBoxContainerBase()
        {
            Boxes = new TrackBoxCollection();
            BoxType = TrackBoxType.None;
        }

        static TrackBoxContainerBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TrackBoxContainerBase), new FrameworkPropertyMetadata(typeof(TrackBoxContainerBase)));
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
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the <see cref="TrackStepControl.TotalSteps"/> property changes.
        /// Adjusts the visual grid for the track boxes.
        /// </summary>
        protected override void OnTotalStepsChanged()
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
            }
        }

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
        #endregion

        /************************************************************************/

        #region Private methods (Instance)
        #endregion
    }
}
