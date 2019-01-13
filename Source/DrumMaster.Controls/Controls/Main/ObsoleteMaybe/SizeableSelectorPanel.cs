using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    ///// <summary>
    ///// Represents the base class for a sizeable visual display panel. This class must be inherited.
    ///// </summary>
    //public abstract class SizeableSelectorPanel : PanelBase, IXElement
    //{
    //    #region Private
    //    #endregion

    //    /************************************************************************/

    //    #region Constructors
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="SizeableSelectorPanel"/> class.
    //    /// </summary>
    //    internal SizeableSelectorPanel()
    //    {
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region SelectorSize
    //    /// <summary>
    //    /// Gets or sets the size of the selector
    //    /// </summary>
    //    public double SelectorSize
    //    {
    //        get => (double)GetValue(SelectorSizeProperty);
    //        set => SetValue(SelectorSizeProperty, value);
    //    }

    //    /// <summary>
    //    /// Identifies the <see cref="SelectorSize"/> dependency property.
    //    /// </summary>
    //    public static readonly DependencyProperty SelectorSizeProperty = DependencyProperty.Register
    //        (
    //            nameof(SelectorSize), typeof(double), typeof(SizeableSelectorPanel), new PropertyMetadata(Constants.SelectorPanel.Size.Default, OnSelectorSizeChanged, OnSelectorSizeCoerce)
    //        );

    //    private static object OnSelectorSizeCoerce(DependencyObject d, object baseValue)
    //    {
    //        double proposed = (double)baseValue;
    //        return Math.Min(Constants.SelectorPanel.Size.Max, Math.Max(Constants.SelectorPanel.Size.Min, proposed));
    //    }

    //    private static void OnSelectorSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is SizeableSelectorPanel c)
    //        {
    //            c.OnSelectorSizeChanged();
    //        }
    //    }

    //    /// <summary>
    //    /// Gets the minimum allowed value for <see cref="SelectorSize"/>.
    //    /// Used to bind to the control template.
    //    /// </summary>
    //    public double MinSelectorSize
    //    {
    //        get => Constants.SelectorPanel.Size.Min;
    //    }
        
    //    /// <summary>
    //    /// Gets the maximum allowed value for <see cref="SelectorSize"/>.
    //    /// Used to bind to the control template.
    //    /// </summary>
    //    public double MaxSelectorSize
    //    {
    //        get => Constants.SelectorPanel.Size.Max;
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region DivisionCount
    //    /// <summary>
    //    /// Gets or sets the division count, i.e. how many
    //    /// selector points in each division.
    //    /// </summary>
    //    public int DivisionCount
    //    {
    //        get => (int)GetValue(DivisionCountProperty);
    //        set => SetValue(DivisionCountProperty, value);
    //    }

    //    /// <summary>
    //    /// Identifies the <see cref="DivisionCount"/> dependency property.
    //    /// </summary>
    //    public static readonly DependencyProperty DivisionCountProperty = DependencyProperty.Register
    //        (
    //            nameof(DivisionCount), typeof(int), typeof(SizeableSelectorPanel), new PropertyMetadata(Constants.SongSelector.Division.Default, OnDivisionChanged, OnDivisionCountCoerce)
    //        );

    //    private static object OnDivisionCountCoerce(DependencyObject d, object baseValue)
    //    {
    //        int proposed = (int)baseValue;
    //        return Math.Min(Constants.SongSelector.Division.Max, Math.Max(Constants.SongSelector.Division.Min, proposed));
    //    }

    //    private static void OnDivisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is SizeableSelectorPanel c)
    //        {
    //            c.OnDivisionCountChanged();
    //        }
    //    }

    //    /// <summary>
    //    /// Gets the minimum allowed value for <see cref="DivisionCount"/>.
    //    /// Used to bind to the control template.
    //    /// </summary>
    //    public int MinDivisionCount
    //    {
    //        get => Constants.SongSelector.Division.Min;
    //    }

    //    /// <summary>
    //    /// Gets the maximum allowed value for <see cref="DivisionCount"/>.
    //    /// Used to bind to the control template.
    //    /// </summary>
    //    public int MaxDivisionCount
    //    {
    //        get => Constants.SongSelector.Division.Max;
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region IXElement 
    //    /// <summary>
    //    /// Gets the XElement for this object.
    //    /// </summary>
    //    /// <returns>The XElement that describes the state of this object.</returns>
    //    public abstract XElement GetXElement();
    //    /// <summary>
    //    /// Restores the object from the specified XElement
    //    /// </summary>
    //    /// <param name="element">The element</param>
    //    public abstract void RestoreFromXElement(XElement element);
    //    #endregion

    //    /************************************************************************/

    //    #region Protected methods
    //    /// <summary>
    //    /// Called when the <see cref="SelectorSize"/> property changes.
    //    /// A derived class can override this method to provide further processing.
    //    /// The base implementation does nothing.
    //    /// </summary>
    //    protected virtual void OnSelectorSizeChanged()
    //    {
    //    }

    //    /// <summary>
    //    /// Called when the <see cref="DivisionCount"/> property changes.
    //    /// A derived class can override this method to provide further processing.
    //    /// The base implementation does nothing.
    //    /// </summary>
    //    protected virtual void OnDivisionCountChanged()
    //    {
    //    }

    //    ///// <summary>
    //    ///// Gets a division border object.
    //    ///// </summary>
    //    ///// <param name="topMargin"></param>
    //    ///// <returns></returns>
    //    //protected PanelDivider GetPanelDivider(int topMargin)
    //    //{
    //    //    return new PanelDivider(0)
    //    //    {
    //    //        Margin = new Thickness(1, topMargin, 1, 0),
    //    //    };
    //    //}

    //    ///// <summary>
    //    ///// Changes the dividers for the specified grid.
    //    ///// </summary>
    //    ///// <param name="grid">The grid</param>
    //    //protected void ChangeDivisions(Grid grid)
    //    //{
    //    //    int topMargin = 0;
    //    //    if (grid is HeaderRow) topMargin = 1;

    //    //    foreach (var currentSep in grid.Children.OfType<PanelDivider>().ToList())
    //    //    {
    //    //        grid.Children.Remove(currentSep);
    //    //    }

    //    //    // All columns defs (except the first one) are Width=Auto
    //    //    // so we can just trim off excess defs from the end
    //    //    int defCount = grid.ColumnDefinitions.Count;
    //    //    int childCount = grid.Children.Count;

    //    //    // just sanity. should always be true.
    //    //    if (defCount > childCount)
    //    //    {
    //    //        grid.ColumnDefinitions.RemoveRange(childCount - 1, defCount - childCount);
    //    //    }

    //    //    // Align attached column property for all children.
    //    //    for (int col = 0; col < grid.Children.Count; col++)
    //    //    {
    //    //        if (grid.Children[col] is UIElement element)
    //    //        {
    //    //            SetColumn(element, col);
    //    //        }
    //    //    }

    //    //    // First insert index is one more than DivisionCount to allow 
    //    //    // for the first column that holds the name of the pattern.
    //    //    int insertIdx = DivisionCount + 1;

    //    //    while (insertIdx < grid.Children.Count - 1)
    //    //    {
    //    //        grid.ColumnDefinitions.Insert(insertIdx, new ColumnDefinition()
    //    //        {
    //    //            Width = new GridLength(1, GridUnitType.Auto),
    //    //        });

    //    //        Border sep = GetPanelDivider(topMargin);

    //    //        SetColumn(sep, insertIdx);

    //    //        grid.Children.Add(sep);

    //    //        // push all SongPointSelectors up a column (if they're at insertIdx or greater)
    //    //        // to allow for the added divider.
    //    //        foreach (var child in grid.Children.OfType<PointSelector>())
    //    //        {
    //    //            int col = GetColumn(child);
    //    //            if (col >= insertIdx)
    //    //            {
    //    //                SetColumn(child, col + 1);
    //    //            }
    //    //        }
    //    //        insertIdx += DivisionCount + 1;
    //    //    }
    //    //}
    //    #endregion

    //    /************************************************************************/

    //    #region Private methods
    //    // Debugging aid
    //    //private void ShowGrid(string step, Grid grid)
    //    //{
    //    //    Debug.WriteLine($"Debug {step}");
    //    //    Debug.WriteLine("==================");
    //    //    Debug.WriteLine($"RowDefinitions: {grid.RowDefinitions.Count}");
    //    //    Debug.WriteLine($"ColDefinitions: {grid.ColumnDefinitions.Count}");
    //    //    Debug.WriteLine($"Children: {grid.Children.Count}");
    //    //    foreach (var child in grid.Children.OfType<UIElement>())
    //    //    {
    //    //        int row = GetRow(child);
    //    //        int col = GetColumn(child);
    //    //        Debug.WriteLine($"Row: {row} Col:{col} {child}");
    //    //    }
    //    //}
    //    #endregion
    //}
}
