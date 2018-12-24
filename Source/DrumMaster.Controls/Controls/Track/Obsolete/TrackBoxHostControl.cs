using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.App.DrumMaster.Controls
{
    ///// <summary>
    ///// Represents a track control that uses a set of <see cref="TrackBox"/> items.
    ///// </summary>
    //public abstract class TrackBoxHostControl : TrackControlBase
    //{
    //    #region Private
    //    #endregion

    //    /************************************************************************/

    //    #region Public properties (Sizes)
    //    /// <summary>
    //    /// Gets or sets the size of the boxes.
    //    /// </summary>
    //    public double BoxSize
    //    {
    //        get => (double)GetValue(BoxSizeProperty);
    //        set => SetValue(BoxSizeProperty, value);
    //    }

    //    public static readonly DependencyProperty BoxSizeProperty = DependencyProperty.Register
    //        (
    //            nameof(BoxSize), typeof(double), typeof(TrackBoxHostControl), new PropertyMetadata(TrackVals.BoxSize.Default, OnBoxSizeChanged, OnBoxSizeCoerce)
    //        );

    //    /// <summary>
    //    /// Gets or sets the text used to display the box size
    //    /// </summary>
    //    public string BoxSizeText
    //    {
    //        get => (string)GetValue(BoxSizeTextProperty);
    //        set => SetValue(BoxSizeTextProperty, value);
    //    }

    //    public static readonly DependencyProperty BoxSizeTextProperty = DependencyProperty.Register
    //        (
    //            nameof(BoxSizeText), typeof(string), typeof(TrackBoxHostControl), new PropertyMetadata(TrackVals.BoxSize.DefaultText)
    //        );

    //    /// <summary>
    //    /// Gets the minimum box size allowed. Used for binding in the control template.
    //    /// </summary>
    //    public double MinBoxSize
    //    {
    //        get => TrackVals.BoxSize.Min;
    //    }

    //    /// <summary>
    //    /// Gets the maximum box size allowed. Used for binding in the control template.
    //    /// </summary>
    //    public double MaxBoxSize
    //    {
    //        get => TrackVals.BoxSize.Max;
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Public properties (Brushes)
    //    /// <summary>
    //    /// Gets or sets the brush that is used when a header step is selected.
    //    /// </summary>
    //    public Brush HeaderBackgroundBrush
    //    {
    //        get => (Brush)GetValue(HeaderBackgroundBrushProperty);
    //        set => SetValue(HeaderBackgroundBrushProperty, value);
    //    }

    //    public static readonly DependencyProperty HeaderBackgroundBrushProperty = DependencyProperty.Register
    //        (
    //            nameof(HeaderBackgroundBrush), typeof(Brush), typeof(TrackBoxHostControl), new PropertyMetadata(new SolidColorBrush(Colors.Red), OnHeaderBackgroundBrushChanged)
    //        );

    //    /// <summary>
    //    /// Gets or sets the brush that is used when a track step is selected.
    //    /// </summary>
    //    public Brush StepBackgroundBrush
    //    {
    //        get => (Brush)GetValue(StepBackgroundBrushProperty);
    //        set => SetValue(StepBackgroundBrushProperty, value);
    //    }

    //    public static readonly DependencyProperty StepBackgroundBrushProperty = DependencyProperty.Register
    //        (
    //            nameof(StepBackgroundBrush), typeof(Brush), typeof(TrackBoxHostControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGreen), OnStepBackgroundBrushChanged)
    //        );
    //    #endregion

    //    /************************************************************************/

    //    #region Protected properties

    //    /// <summary>
    //    /// From a derived class, gets the control type.
    //    /// </summary>
    //    protected TrackBoxType ControlType
    //    {
    //        get;
    //    }
    //    /// <summary>
    //    /// Gets a list of boxes
    //    /// </summary>
    //    protected List<TrackBox> Boxes
    //    {
    //        get;
    //        private set;
    //    }

    //    /// <summary>
    //    /// Gets the total steps, i.e. the count of <see cref="Boxes"/>.
    //    /// </summary>
    //    protected int TotalSteps
    //    {
    //        get => Boxes.Count;
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Constructor
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    protected TrackBoxHostControl(TrackBoxType controlType)
    //    {
    //        ControlType = controlType;
    //        Boxes = new List<TrackBox>();
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Protected methods
    //    /// <summary>
    //    /// Called when the <see cref="BoxSize"/> property changes.
    //    /// A derived class may override this method to perform other updates.
    //    /// Always call the base method.
    //    /// </summary>
    //    protected virtual void OnBoxSizeChanged()
    //    {
    //        foreach (var box in Boxes)
    //        {
    //            box.Width = box.Height = BoxSize;
    //        }
    //    }


    //    protected virtual void OnTotalStepsChanged(Grid hostGrid, int totalSteps)
    //    {
    //        if (hostGrid != null)
    //        {
    //            int currentTotalSteps = hostGrid.ColumnDefinitions.Count;
    //            if (totalSteps > currentTotalSteps)
    //            {
    //                int stepsToAdd = totalSteps - currentTotalSteps;
    //                for (int k = 0; k < stepsToAdd; k++)
    //                {
    //                    hostGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

    //                    var box = CreateTrackBox();

    //                    Grid.SetColumn(box, hostGrid.ColumnDefinitions.Count - 1);
    //                    hostGrid.Children.Add(box);
    //                    Boxes.Add(box);
    //                }
    //            }
    //            else
    //            {
    //                while (currentTotalSteps > totalSteps)
    //                {
    //                    hostGrid.Children.RemoveAt(currentTotalSteps - 1);
    //                    hostGrid.ColumnDefinitions.RemoveAt(currentTotalSteps - 1);
    //                    Boxes.RemoveAt(currentTotalSteps - 1);
    //                    currentTotalSteps--;
    //                }
    //            }
    //        }
    //    }

    //    //protected virtual void OnBackgroundBrushChanged(TrackBoxHostControlType controlType)
    //    //{
    //    //    Brush brush = (controlType == TrackBoxHostControlType.Header) ? HeaderBackgroundBrush : StepBackgroundBrush;
    //    //    foreach (var box in Boxes)
    //    //    {
    //    //        box.SelectedBackgroundBrush = brush;
    //    //    }
    //    //}

    //    /// <summary>
    //    /// Deselects all boxes.
    //    /// </summary>
    //    protected void DeselectAllBoxes()
    //    {
    //        foreach (var box in Boxes)
    //        {
    //            box.IsSelected = false;
    //        }
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Private methods

    //    private TrackBox CreateTrackBox()
    //    {
    //        switch (ControlType)
    //        {
    //            //case TrackBoxType.Header:
    //            //    return new TrackHeaderBoxControl(this) { Height = BoxSize, Width = BoxSize, SelectedBackgroundBrush = HeaderBackgroundBrush };
    //            //case TrackBoxType.TrackStep:
    //            //    return new TrackStepBoxControl(this) { Height = BoxSize, Width = BoxSize, SelectedBackgroundBrush = StepBackgroundBrush }; 
    //            default:
    //                throw new ArgumentOutOfRangeException(nameof(ControlType));
    //        }
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Private methods (static)
    //    private static void OnBoxSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is TrackBoxHostControl c)
    //        {
    //            c.OnBoxSizeChanged();
    //        }
    //    }

    //    private static object OnBoxSizeCoerce(DependencyObject d, object baseValue)
    //    {
    //        double proposed = (double)baseValue;
    //        return Math.Min(TrackVals.BoxSize.Max, Math.Max(TrackVals.BoxSize.Min, proposed));
    //    }

    //    private static void OnHeaderBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is TrackContainer c)
    //        {
    //            //c.OnBackgroundBrushChanged(TrackBoxHostControlType.Header);
    //        }
    //    }

    //    private static void OnStepBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is TrackContainer c)
    //        {
    //            //c.OnBackgroundBrushChanged(TrackBoxHostControlType.TrackStep);
    //        }
    //    }

    //    #endregion
    //}
}
