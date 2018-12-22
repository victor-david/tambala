using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Restless.App.DrumMaster.Controls
{
    [TemplatePart(Name = PartHostGrid, Type = typeof(Grid))]
    internal class EnvelopeControl : ContentControl
    {
        private const string PartHostGrid = "PART_HOST_GRID";
        private Grid hostGrid;

        public int TotalSteps
        {
            get => (int)GetValue(TotalStepsProperty);
            set => SetValue(TotalStepsProperty, value);
        }

        public static readonly DependencyProperty TotalStepsProperty = DependencyProperty.Register
            (
                nameof(TotalSteps), typeof(int), typeof(EnvelopeControl), new PropertyMetadata(0, OnTotalStepsChanged)
            );

        public double BoxSize
        {
            get => (double)GetValue(BoxSizeProperty);
            set => SetValue(BoxSizeProperty, value);
        }

        public static readonly DependencyProperty BoxSizeProperty = DependencyProperty.Register
            (
                nameof(BoxSize), typeof(double), typeof(EnvelopeControl), new PropertyMetadata(TrackVals.BoxSize.Default, OnBoxSizeChanged)
            );


        public float Minimum
        {
            get => (float)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register
            (
                nameof(Minimum), typeof(float), typeof(EnvelopeControl), new PropertyMetadata(-1.0f)
            );

        public float Maximum
        {
            get => (float)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register
            (
                nameof(Maximum), typeof(float), typeof(EnvelopeControl), new PropertyMetadata(1.0f)
            );
        

        /************************************************************************/

        #region Constructors
        internal EnvelopeControl()
        {
        }

        static EnvelopeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnvelopeControl), new FrameworkPropertyMetadata(typeof(EnvelopeControl)));
        }
        #endregion

        /************************************************************************/

        #region Public methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            hostGrid = GetTemplateChild(PartHostGrid) as Grid;

        }
        #endregion

        /************************************************************************/

        #region Private methods (Instance)

        private void OnTotalStepsChanged()
        {
            Debug.WriteLine($"Envelope. Total steps changed to: {TotalSteps}");
            if (hostGrid != null)
            {
                int currentTotalSteps = hostGrid.ColumnDefinitions.Count;
                if (TotalSteps > currentTotalSteps)
                {
                    int stepsToAdd = TotalSteps - currentTotalSteps;
                    double myWidth = BoxSize;
                    for (int k = 0; k < stepsToAdd; k++)
                    {
                        hostGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(myWidth, GridUnitType.Pixel) });
                        Line line = new Line
                        {
                            X1 = 0,
                            X2 = myWidth - 2
                        };
                        line.Y1 = line.Y2 = hostGrid.ActualHeight / 2;
                        line.Stroke = Brushes.Red;
                        line.StrokeThickness = 2;
                        Grid.SetColumn(line, k);
                        hostGrid.Children.Add(line);
                        //var box = CreateTrackBox();

                        //Grid.SetColumn(box, hostGrid.ColumnDefinitions.Count - 1);
                        //hostGrid.Children.Add(box);
                        //Boxes.Add(box);
                    }
                }
                else
                {
                    while (currentTotalSteps > TotalSteps)
                    {
                        hostGrid.Children.RemoveAt(currentTotalSteps - 1);
                        hostGrid.ColumnDefinitions.RemoveAt(currentTotalSteps - 1);
                        // Boxes.RemoveAt(currentTotalSteps - 1);
                        currentTotalSteps--;
                    }
                }
            }
        }

        private void OnBoxSizeChanged()
        {
            string gh = (hostGrid == null) ? "NULL" : "NOT NULL";
            Debug.WriteLine($"Envelope. BoxSize changed to: {BoxSize} {gh}");
            if (hostGrid != null)
            {
                hostGrid.ColumnDefinitions.Clear();
                for (int k =0; k < TotalSteps; k++)
                {
                    hostGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(BoxSize, GridUnitType.Pixel) });
                }
            }

        }

        #endregion

        /************************************************************************/

        #region Private methods (Static)

        private static void OnTotalStepsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EnvelopeControl c)
            {
                c.OnTotalStepsChanged();
            }
        }

        private static void OnBoxSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EnvelopeControl c)
            {
                c.OnBoxSizeChanged();
            }
        }

        //private static object OnTotalStepsCoerce(DependencyObject d, object baseValue)
        //{
        //    int proposed = (int)baseValue;
        //    int r =  Math.Min(TrackVals.TotalSteps.Max, Math.Max(TrackVals.TotalSteps.Min, proposed));
        //    return r;

        //}
        #endregion












    }
}
