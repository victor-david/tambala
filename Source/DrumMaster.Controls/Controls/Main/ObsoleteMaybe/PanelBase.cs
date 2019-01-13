using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    ///// <summary>
    ///// Represents the base class for a visual display panel. This class must be inherited.
    ///// </summary>
    //public abstract class PanelBase : Grid
    //{
    //    #region Private
    //    #endregion
        
    //    /************************************************************************/

    //    #region Constructors
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="PanelBase"/> class.
    //    /// </summary>
    //    internal PanelBase()
    //    {
    //        Loaded += PanelBaseLoaded;
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region DisplayName
    //    /// <summary>
    //    /// Gets or sets the display name.
    //    /// </summary>
    //    public string DisplayName
    //    {
    //        get => (string)GetValue(DisplayNameProperty);
    //        set => SetValue(DisplayNameProperty, value);
    //    }

    //    /// <summary>
    //    /// Identifies the <see cref="DisplayName"/> dependency property.
    //    /// </summary>
    //    public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register
    //        (
    //            nameof(DisplayName), typeof(string), typeof(PanelBase), new PropertyMetadata(null, OnDisplayNameChanged)
    //        );

    //    private static void OnDisplayNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is PanelBase c)
    //        {
    //            // c.SetIsChanged();
    //        }
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Position
    //    /// <summary>
    //    /// Gets or sets the position, i.e. the sequence value
    //    /// </summary>
    //    public int Position
    //    {
    //        get => (int)GetValue(PositionProperty);
    //        set => SetValue(PositionProperty, value);
    //    }

    //    /// <summary>
    //    /// Identifies the <see cref="Position"/> dependency property
    //    /// </summary>
    //    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register
    //        (
    //            nameof(Position), typeof(int), typeof(PanelBase), new PropertyMetadata(0, OnPositionChanged)
    //        );

    //    private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is PanelBase c)
    //        {
    //            c.OnPositionChanged();
    //        }
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region CLR properties
    //    #endregion

    //    /************************************************************************/

    //    #region Public methods
    //    #endregion

    //    /************************************************************************/

    //    #region Internal methods
    //    /// <summary>
    //    /// Adds a row definition and returns its index.
    //    /// </summary>
    //    /// <param name="height">The height. Pass 0 (the default) for Auto</param>
    //    /// <returns>The index of the newly added row definition</returns>
    //    internal int AddRowDefinition(double height = 0.0)
    //    {
    //        GridLength glen = (height > 0) ? new GridLength(height) : new GridLength(1, GridUnitType.Auto);
    //        RowDefinitions.Add(new RowDefinition() { Height = glen });
    //        return RowDefinitions.Count - 1;
    //    }

    //    /// <summary>
    //    /// Adds a column definition and returns its index.
    //    /// </summary>
    //    /// <param name="width">The height. Pass 0 (the default) for Auto</param>
    //    /// <returns>The index of the newly added column definition</returns>
    //    internal int AddColumnDefinition(double width = 0.0)
    //    {
    //        GridLength glen = (width > 0) ? new GridLength(width) : new GridLength(1, GridUnitType.Auto);
    //        ColumnDefinitions.Add(new ColumnDefinition() { Width = glen });
    //        return ColumnDefinitions.Count - 1;
    //    }

    //    /// <summary>
    //    /// Adds the specifed UIElement
    //    /// </summary>
    //    /// <param name="element">The element to add</param>
    //    /// <param name="row">The row</param>
    //    /// <param name="column">The column</param>
    //    internal void AddElement(UIElement element, int row, int column)
    //    {
    //        row = Math.Max(row, 0);
    //        column = Math.Max(column, 0);
    //        if (element == null) throw new ArgumentNullException(nameof(element));
    //        SetRow(element, row);
    //        SetColumn(element, column);
    //        Children.Add(element);
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Protected methods
    //    /// <summary>
    //    /// Override in a derived class to create content.
    //    /// This method is called after the control is loaded.
    //    /// The base implementation does nothing.
    //    /// </summary>
    //    protected virtual void CreateContent()
    //    {
    //    }

    //    /// <summary>
    //    /// Called when <see cref="Position"/> changes. A derived class can override
    //    /// this method to perform updates. The base implementation does nothing.
    //    /// </summary>
    //    protected virtual void OnPositionChanged()
    //    {
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Private methods
    //    private void PanelBaseLoaded(object sender, RoutedEventArgs e)
    //    {
    //        CreateContent();
    //        e.Handled = true;
    //    }
    //    #endregion
    //}
}
