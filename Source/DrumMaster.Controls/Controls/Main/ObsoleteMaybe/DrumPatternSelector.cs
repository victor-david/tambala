using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls.Obsolete
{
    /// <summary>
    /// Represents a control that presents and manages a series of patterns to incorporate into a song.
    /// </summary>
    [TemplatePart(Name = PartHostGrid, Type = typeof(Grid))]
    public class DrumPatternSelector : ControlObjectSelector
    {
        #region Private
        private const string PartHostGrid = "PART_HostGrid";
        private Grid hostGrid;
        #endregion
        
        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumPatternSelector"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="type">The selector type</param>
        internal DrumPatternSelector(SongContainer owner, PointSelectorType type)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            //DrumPattern = new DrumPattern(this);
            Commands.Add("SelectPattern", new RelayCommand(RunSelectPatternCommand));
        }

        static DrumPatternSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrumPatternSelector), new FrameworkPropertyMetadata(typeof(DrumPatternSelector)));
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the <see cref="SongContainer"/> that owns this instance.
        /// </summary>
        internal SongContainer Owner
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region DrumPattern
        /// <summary>
        /// Gets the drum pattern associated with this selector
        /// </summary>
        public DrumPattern DrumPattern
        {
            get => (DrumPattern)GetValue(DrumPatternProperty);
            private set => SetValue(DrumPatternPropertyKey, value);
        }

        
        private static readonly DependencyPropertyKey DrumPatternPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(DrumPattern), typeof(DrumPattern), typeof(DrumPatternSelector), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="DrumPattern"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrumPatternProperty = DrumPatternPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Occurs when the template is applied
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            hostGrid = GetTemplateChild(PartHostGrid) as Grid;
            CreateHostGrid();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            throw new NotImplementedException();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when <see cref="PatternSelectorOrig.Position"/> changes.
        /// </summary>
        protected override void OnPositionChanged()
        {
            // Debug.WriteLine(ToString());
            DrumPattern.DisplayName = $"Pattern {Position}";
        
        }

        ///// <summary>
        ///// Called when <see cref="SizeablePatternSelector.SelectorSize"/> changes.
        ///// </summary>
        //protected override void OnSelectorSizeChanged()
        //{
        //    foreach (var point in hostGrid.Children.OfType<SongPointSelectorOrig>())
        //    {
        //        point.SelectorSize = SelectorSize;
        //    }
        //}

        ///// <summary>
        ///// Called when <see cref="SizeablePatternSelector.DivisionCount"/> changes.
        ///// </summary>
        //protected override void OnDivisionCountChanged()
        //{
        //    ChangeDivisionSeparators();
        //}
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string that describes this object</returns>
        public override string ToString()
        {
            return $"{nameof(DrumPatternSelector)} Position: {Position} Name: {DisplayName}";
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void CreateHostGrid()
        {
        //    //if (hostGrid != null)
        //    //{
        //    //    if (hostGrid.ColumnDefinitions.Count > 1)
        //    //    {
        //    //        int removeCount = hostGrid.ColumnDefinitions.Count - 1;
        //    //        hostGrid.ColumnDefinitions.RemoveRange(1, removeCount);
        //    //    }

        //    //    int col = 1;
        //    //    for (int k = 1; k <= Constants.SongSelector.Count.Default; k++)
        //    //    {
        //    //        hostGrid.ColumnDefinitions.Add(new ColumnDefinition()
        //    //        {
        //    //            Width = new GridLength(1, GridUnitType.Auto)
        //    //        });

        //    //        SongPointSelectorOrig point = new SongPointSelectorOrig(this, SelectorType)
        //    //        {
        //    //            Margin = new Thickness(1),
        //    //            Position = k,
        //    //        };
        //    //        Grid.SetColumn(point, col++);
        //    //        hostGrid.Children.Add(point);

        //    //        if (k % DivisionCount == 0 && k != Constants.SongSelector.Count.Default)
        //    //        {
        //    //            int top = SelectorType == SongDrumPatternSelectorType.Header ? 1 : 0;
        //    //            hostGrid.ColumnDefinitions.Add(new ColumnDefinition()
        //    //            {
        //    //                Width = new GridLength(1, GridUnitType.Auto),
        //    //            });
        //    //            Border sep = GetDivisionBorder();
        //    //            Grid.SetColumn(sep, col++);
        //    //            hostGrid.Children.Add(sep);
        //    //        }
        //    //    }
        //    //}
        }

        //private void ChangeDivisionSeparators()
        //{
        //    foreach (var currentSep in hostGrid.Children.OfType<Border>().ToList())
        //    {
        //        // Column zero holds a border around the linked text box
        //        // that changes background when selected. Don't want to remove it.
        //        // All other Border objects are dividers.
        //        int col = Grid.GetColumn(currentSep);
        //        if (col > 0)
        //        {
        //            hostGrid.Children.Remove(currentSep);
        //        }
        //    }

        //    // All columns defs (except the first one) are Width=Auto
        //    // so we can just trim off excess defs from the end
        //    int defCount = hostGrid.ColumnDefinitions.Count;
        //    int childCount = hostGrid.Children.Count;

        //    // just sanity. should always be true.
        //    if (defCount > childCount)
        //    {
        //        hostGrid.ColumnDefinitions.RemoveRange(childCount - 1, defCount - childCount);
        //    }

        //    // Align attached column property for all children.
        //    for (int col=0; col < hostGrid.Children.Count; col++)
        //    {
        //        if (hostGrid.Children[col] is UIElement element)
        //        {
        //            Grid.SetColumn(element, col);
        //        }
        //    }

        //    // First insert index is one more than DivisionCount to allow 
        //    // for the first column that holds the name of the pattern.
        //    int insertIdx = DivisionCount + 1;

        //    while (insertIdx < hostGrid.Children.Count - 1)
        //    {

        //        hostGrid.ColumnDefinitions.Insert(insertIdx, new ColumnDefinition()
        //        {
        //            Width = new GridLength(1, GridUnitType.Auto),
        //        });

        //        Border sep = GetDivisionBorder();

        //        Grid.SetColumn(sep, insertIdx);
        //        hostGrid.Children.Add(sep);

        //        // push all SongPointSelectors up a column (if they're at insertIdx or greater)
        //        // to allow for the added divider.
        //        foreach (var child in hostGrid.Children.OfType<SongPointSelectorOrig>())
        //        {
        //            int col = Grid.GetColumn(child);
        //            if (col >= insertIdx)
        //            {
        //                Grid.SetColumn(child, col + 1);
        //            }
        //        }
        //        insertIdx += DivisionCount + 1;
        //    }
        //}

        //private Border GetDivisionBorder()
        //{
        //    int topMargin = SelectorType == SongDrumPatternSelectorType.Header ? 1 : 0;
        //    return new Border()
        //    {
        //        Margin = new Thickness(1, topMargin, 1, 0),
        //        Width = 3.0,
        //        Background = new SolidColorBrush(Colors.DarkGray),
        //        VerticalAlignment = VerticalAlignment.Stretch
        //    };
        //}

        private void RunSelectPatternCommand(object parm)
        {
            //Owner.HighlightSelectedPattern(this);
            //Owner.Owner.DrumPatternContainer.ActivateDrumPattern(DrumPattern); 
        }
        #endregion
    }
}
