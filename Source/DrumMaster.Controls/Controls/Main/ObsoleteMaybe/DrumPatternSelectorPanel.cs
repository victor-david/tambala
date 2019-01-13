using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    ///// <summary>
    ///// Represents a control that presents and manages a series of patterns to incorporate into a song.
    ///// </summary>
    //public class DrumPatternSelectorPanel : SizeableSelectorPanel
    //{
    //    #region Private
    //    #endregion
        
    //    /************************************************************************/

    //    #region Constructors
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="DrumPatternSelectorPanel"/> class.
    //    /// </summary>
    //    /// <param name="owner">The owner.</param>
    //    internal DrumPatternSelectorPanel(SongPresenter owner)
    //    {
    //        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
    //        DrumPattern = new DrumPattern(this);
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region CLR Properties
    //    /// <summary>
    //    /// Gets the <see cref="SongPresenter"/> that owns this instance.
    //    /// </summary>
    //    internal SongPresenter Owner
    //    {
    //        get;
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region DrumPattern
    //    /// <summary>
    //    /// Gets the drum pattern associated with this selector
    //    /// </summary>
    //    public DrumPattern DrumPattern
    //    {
    //        get => (DrumPattern)GetValue(DrumPatternProperty);
    //        private set => SetValue(DrumPatternPropertyKey, value);
    //    }
        
    //    private static readonly DependencyPropertyKey DrumPatternPropertyKey = DependencyProperty.RegisterReadOnly
    //        (
    //            nameof(DrumPattern), typeof(DrumPattern), typeof(DrumPatternSelectorPanel), new PropertyMetadata(null)
    //        );

    //    /// <summary>
    //    /// Identifies the <see cref="DrumPattern"/> dependency property.
    //    /// </summary>
    //    public static readonly DependencyProperty DrumPatternProperty = DrumPatternPropertyKey.DependencyProperty;
    //    #endregion

    //    /************************************************************************/

    //    #region Public methods
    //    ///// <summary>
    //    ///// Occurs when the template is applied
    //    ///// </summary>
    //    //public override void OnApplyTemplate()
    //    //{
    //    //    base.OnApplyTemplate();
    //    //    hostGrid = GetTemplateChild(PartHostGrid) as Grid;
    //    //    CreateHostGrid();
    //    //}
    //    #endregion

    //    /************************************************************************/

    //    #region IXElement
    //    /// <summary>
    //    /// Gets the XElement for this object.
    //    /// </summary>
    //    /// <returns>The XElement that describes the state of this object.</returns>
    //    public override XElement GetXElement()
    //    {
    //        var element = new XElement(nameof(DrumPatternSelectorPanel));
    //        element.Add(DrumPattern.GetXElement());
    //        foreach (var child in Children.OfType<PointSelector>())
    //        {
    //            element.Add(child.GetXElement());
    //        }
    //        return element;
    //    }

    //    /// <summary>
    //    /// Restores the object from the specified XElement
    //    /// </summary>
    //    /// <param name="element">The element</param>
    //    public override void RestoreFromXElement(XElement element)
    //    {
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Protected methods
    //    ///// <summary>
    //    ///// Creates the content. Called after the template has been applied.
    //    ///// </summary>
    //    //protected override void CreateContent()
    //    //{
    //    //    int rowIdx = AddRowDefinition();

    //    //    var link = new LinkedTextBlock()
    //    //    {
    //    //        VerticalAlignment = VerticalAlignment.Center,
    //    //        Command = new RelayCommand(RunSelectPatternCommand)
    //    //    };

    //    //    Binding binding = new Binding(nameof(DrumPattern.DisplayName))
    //    //    {
    //    //        Source = DrumPattern
    //    //    };
    //    //    link.SetBinding(TextBlock.TextProperty, binding);

    //    //    var border = new Border()
    //    //    {
    //    //        Padding = new Thickness(3,0,3,0),
    //    //        Margin = new Thickness(2),
    //    //        CornerRadius = new CornerRadius(1),
    //    //        Child = link
    //    //    };

    //    //    int colIdx = AddColumnDefinition(Constants.SelectorPanel.FirstColumnWidth);
    //    //    AddElement(border, rowIdx, colIdx);

    //    //    for (int k = 1; k <= Constants.SelectorPanel.SongCount.Default; k++)
    //    //    {
    //    //        colIdx = AddColumnDefinition();
    //    //        var sps = new PointSelector(PointSelectorType.SongRow)
    //    //        {
    //    //            Margin = new Thickness(1),
    //    //            Position = k,
    //    //        };
    //    //        AddElement(sps, rowIdx, colIdx);

    //    //        if (k % DivisionCount == 0 && k != Constants.SelectorPanel.SongCount.Default)
    //    //        {
    //    //            // add divider
    //    //            Border div = GetPanelDivider(0);
    //    //            colIdx = AddColumnDefinition();
    //    //            AddElement(div, rowIdx, colIdx);
    //    //        }
    //    //    }
    //    //}

    //    /// <summary>
    //    /// Called when <see cref="ControlObjectSelector.Position"/> changes.
    //    /// </summary>
    //    protected override void OnPositionChanged()
    //    {
    //        DisplayName = $"PatternSelectorPanel {Position}";
    //        DrumPattern.DisplayName = $"Pattern {Position}";
    //    }

    //    /// <summary>
    //    /// Called when <see cref="ControlObjectSelector.SelectorSize"/> changes.
    //    /// </summary>
    //    protected override void OnSelectorSizeChanged()
    //    {
    //        foreach (var point in Children.OfType<PointSelector>())
    //        {
    //            point.SelectorSize = SelectorSize;
    //        }
    //    }

    //    ///// <summary>
    //    ///// Called when <see cref="ControlObjectSelector.DivisionCount"/> changes.
    //    ///// </summary>
    //    //protected override void OnDivisionCountChanged()
    //    //{
    //    //    ChangeDivisions(this);
    //    //}
    //    #endregion

    //    /************************************************************************/

    //    #region Public methods
    //    /// <summary>
    //    /// Returns a string representation of this object.
    //    /// </summary>
    //    /// <returns>A string that describes this object</returns>
    //    public override string ToString()
    //    {
    //        return $"{nameof(DrumPatternSelectorPanel)} Position: {Position} Name: {DisplayName}";
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Internal methods
    //    internal void SetVisualSelected(bool isSelected)
    //    {
    //        if (Children[0] is Border border)
    //        {
    //            if (isSelected)
    //                border.SetResourceReference(Border.BackgroundProperty, "SongPointHeaderDeselected");
    //            else
    //                border.Background = null;
    //        }
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Private methods

    //    private void RunSelectPatternCommand(object parm)
    //    {
    //        Owner.HighlightSelectedPattern(this);
    //        Owner.Owner.Owner.DrumPatternContainer.ActivateDrumPattern(DrumPattern); 
    //    }
    //    #endregion
    //}
}
