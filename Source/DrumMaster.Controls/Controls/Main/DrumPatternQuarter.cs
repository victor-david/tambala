using Restless.App.DrumMaster.Controls.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a single quarter note and its associated ticks for a drum pattern.
    /// </summary>
    public class DrumPatternQuarter : ControlElement
    {
        #region Private
        private static readonly Dictionary<int, List<int>> FullTickMap = new Dictionary<int, List<int>>
        {
            { 2, new List<int>() { 12 } }, // 8th
            { 4, new List<int>() { 6, 12, 18 } }, // 16th
            { 8, new List<int>() { 3, 6, 9, 12, 15, 18, 21 } }, // 32nd 
            { 3, new List<int>() { 8, 16 } }, // 8th triplet
        };

        private static readonly Dictionary<PointSelectorUnit, List<int>> TickColumns = new Dictionary<PointSelectorUnit, List<int>>
        {
            { PointSelectorUnit.EighthNote, new List<int>() { 12 } }, // 8th
            { PointSelectorUnit.SixteenthNote, new List<int>() { 6, 18 } }, // 16th
            { PointSelectorUnit.ThirtySecondNote, new List<int>() { 3, 9, 15, 21 } }, // 32nd 
            { PointSelectorUnit.EighthNoteTriplet, new List<int>() { 8, 16 } }, // 8th triplet

             // 3, 6, 8, 9, 12, 15, 16, 18, 21,
        };

        private readonly Dictionary<int, PointSelector> pointSelectors;

        private VisualTick quarterNoteTick;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumPatternQuarter"/> class.
        /// </summary>
        internal DrumPatternQuarter()
        {
            Visual = new Grid();
            pointSelectors = new Dictionary<int, PointSelector>();
        }

        static DrumPatternQuarter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrumPatternQuarter), new FrameworkPropertyMetadata(typeof(DrumPatternQuarter)));
        }
        #endregion

        /************************************************************************/

        #region QuarterType
        /// <summary>
        /// Gets or sets the type for this control.
        /// </summary>
        public DrumPatternQuarterType QuarterType
        {
            get => (DrumPatternQuarterType)GetValue(QuarterTypeProperty);
            set => SetValue(QuarterTypeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="QuarterType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty QuarterTypeProperty = DependencyProperty.Register
            (
                nameof(QuarterType), typeof(DrumPatternQuarterType), typeof(DrumPatternQuarter), new PropertyMetadata(DrumPatternQuarterType.None)
            );
        #endregion

        /************************************************************************/

        #region QuarterNote
        /// <summary>
        /// Gets the quarter note value, 1,2,3,4,5, etc.
        /// </summary>
        public int QuarterNote
        {
            get => (int)GetValue(QuarterNoteProperty);
            set => SetValue(QuarterNoteProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="QuarterNote"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty QuarterNoteProperty = DependencyProperty.Register
            (
                nameof(QuarterNote), typeof(int), typeof(DrumPatternQuarter), new PropertyMetadata(0)
            );
        #endregion

        /************************************************************************/

        #region TotalTicks
        /// <summary>
        /// Gets or sets the total number of ticks for this quarter note, including the quarter note itself.
        /// </summary>
        public int TotalTicks
        {
            get => (int)GetValue(TotalTicksProperty);
            set => SetValue(TotalTicksProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TotalTicks"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TotalTicksProperty = DependencyProperty.Register
            (
                nameof(TotalTicks), typeof(int), typeof(DrumPatternQuarter), new PropertyMetadata(Constants.DrumPattern.TicksPerQuarterNote.Default, OnTotalTicksChanged)
            );

        private static void OnTotalTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrumPatternQuarter c)
            {
                c.ApplyTotalTicks();
            }
        }

        #endregion

        /************************************************************************/

        #region Visual
        /// <summary>
        /// Gets the visual grid.
        /// </summary>
        public Grid Visual
        {
            get => (Grid)GetValue(VisualProperty);
            private set => SetValue(VisualPropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey VisualPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(Visual), typeof(Grid), typeof(DrumPatternQuarter), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="Visual"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisualProperty = VisualPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region IXElement 
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public override XElement GetXElement()
        {
            var element = new XElement(nameof(DrumPatternQuarter));
            element.Add(new XElement(nameof(QuarterNote), QuarterNote));
            element.Add(new XElement(nameof(QuarterType), QuarterType));
            foreach(var item in pointSelectors.Where((s)=> s.Value.IsSelected))
            {
                element.Add(item.Value.GetXElement());
            }
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            foreach (XElement e in ChildElementList(element))
            {
                if (e.Name == nameof(PointSelector))
                {
                    int key = 0;
                    XAttribute pos = e.Attribute(ControlObjectSelector.PositionProperty.Name);
                    if (pos != null && int.TryParse(pos.Value, out int result))
                    {
                        key = result;
                    }
                    if (pointSelectors.ContainsKey(key))
                    {
                        pointSelectors[key].RestoreFromXElement(e);
                    }
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Get the count of selecteors that are selected.
        /// </summary>
        /// <returns>The number of selectors in this quarter that are currently selected.</returns>
        internal int GetSelectedCount()
        {
            int count = 0;
            foreach (var item in pointSelectors)
            {
                if (item.Value.IsSelected) count++;
            }
            return count;
        }
        /// <summary>
        /// Gets a boolean value that indicates if the selector
        /// at the specified position is selected.
        /// </summary>
        /// <param name="songUnit">The song unit.</param>
        /// <param name="position">The position.</param>
        /// <returns>true if selected; otherwise, false.</returns>
        internal bool IsSelected(PointSelectorSongUnit songUnit, int position)
        {
            // TODO - songUnit
            if (pointSelectors.ContainsKey(position))
            {
                return pointSelectors[position].ThreadSafeIsSelected;
            }
            return false;
        }

        /// <summary>
        /// Adds the highlight on the quarter note tick using Dispatcher.BeginInvoke.
        /// </summary>
        internal void InvokeAddQuarterNoteTickHighlight()
        {
            // sanity only.
            if (quarterNoteTick != null)
            {
                quarterNoteTick.InvokeAddTickHighlight();
            }
        }

        /// <summary>
        /// Removes the highlight from the quarter note tick using Dispatcher.BeginInvoke.
        /// </summary>
        internal void InvokeRemoveQuarterNoteTickHighlight()
        {
            // quarterNoteTick can be null because the template hsan't yet been applied.
            // With the default of 4 quarter notes per pattern, quarterNoteTick is null
            // for quarters 5 and 6. When the user bumps up the count to 6, the template
            // is then applied (even though the objects themselves were created ahead of time)
            // and quarterNoteTick is then set to its normal non-null value.
            if (quarterNoteTick != null)
            {
                quarterNoteTick.InvokeRemoveTickHighlight();
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods

        protected override void OnElementCreate()
        {
            pointSelectors.Clear();

            for (int k = 1; k <= Constants.DrumPattern.LowestCommon; k++)
            {
                Visual.ColumnDefinitions.Add(new ColumnDefinition());
            }

            switch (QuarterType)
            {
                case DrumPatternQuarterType.Header:
                    CreateHeaderType();
                    break;
                case DrumPatternQuarterType.Selector:
                    CreateSelectorType();
                    break;
            }

            ApplyTotalTicks();
        }

        private void CreateHeaderType()
        {
            Visual.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
            Visual.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
            TextBlock text = new TextBlock() { Text = QuarterNote.ToString(), Foreground = Brushes.DarkBlue };
            text.HorizontalAlignment = HorizontalAlignment.Center;
            AddElement(text, 0, 0);

            quarterNoteTick = new VisualTick(0, PointSelectorUnit.QuarterNote);
            AddElement(quarterNoteTick, 1, 0);

            foreach(var map in TickColumns)
            {
                foreach (int col in map.Value)
                {
                    VisualTick tick = new VisualTick(col, map.Key);
                    AddElement(tick, 1, col);
                }
            }
        }

        private void CreateSelectorType()
        {
            Visual.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            var qs = new PointSelector()
            {
                SelectorType = PointSelectorType.PatternRow,
                SelectorUnit = PointSelectorUnit.QuarterNote,
                Position = 0,
            };

            pointSelectors.Add(0,qs);
            AddElement(qs, 0, 0);

            foreach (var map in TickColumns)
            {
                foreach (int col in map.Value)
                {
                    var s = new PointSelector()
                    {
                        SelectorType = PointSelectorType.PatternRow,
                        SelectorUnit = map.Key,
                        Position = col,
                        IsEnabled = false,
                        Visibility = Visibility.Collapsed,
                    };

                    pointSelectors.Add(col,s);
                    AddElement(s, 0, col);
                }
            }
        }

        private void AddElement(UIElement element, int row, int column)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
            Visual.Children.Add(element);
        }

        private void ApplyTotalTicks()
        {
            if (FullTickMap.ContainsKey(TotalTicks))
            {
                foreach (var child in Visual.Children.OfType<VisualTick>().Where((t)=>t.SelectorUnit != PointSelectorUnit.QuarterNote))
                {
                    if (FullTickMap[TotalTicks].Contains(child.Position))
                    {
                        child.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        child.Visibility = Visibility.Collapsed;
                    }
                }

                if (TotalTicks == 3)
                    ApplyTotalTripletTicks();
                else
                    ApplyTotalEvenTicks();
            }
        }
        private void ApplyTotalEvenTicks()
        {
            foreach (var child in Visual.Children.OfType<PointSelector>().Where((t) => t.SelectorUnit != PointSelectorUnit.QuarterNote))
            {
                if (child.SelectorUnit != PointSelectorUnit.EighthNoteTriplet)
                {
                    child.Visibility = Visibility.Visible;
                    switch (TotalTicks)
                    {
                        case Constants.DrumPattern.TicksPerQuarterNote.Eighth:
                            child.IsEnabled = child.SelectorUnit == PointSelectorUnit.EighthNote;
                            break;
                        case Constants.DrumPattern.TicksPerQuarterNote.Sixteenth:
                            child.IsEnabled = child.SelectorUnit == PointSelectorUnit.SixteenthNote || child.SelectorUnit == PointSelectorUnit.EighthNote;
                            break;
                        default:
                            child.IsEnabled = true;
                            break;
                    }
                }
                else
                    child.Visibility = Visibility.Collapsed;

                //child.Visibility = child.SelectorUnit != PointSelectorUnit.EighthNoteTriplet ? Visibility.Visible : Visibility.Collapsed;
                //child.IsEnabled = child.SelectorUnit != PointSelectorUnit.EighthNoteTriplet ? true : false;
            }
        }

        private void ApplyTotalTripletTicks()
        {
            foreach (var child in Visual.Children.OfType<PointSelector>().Where((t) => t.SelectorUnit != PointSelectorUnit.QuarterNote))
            {
                child.Visibility = child.SelectorUnit == PointSelectorUnit.EighthNoteTriplet ? Visibility.Visible : Visibility.Collapsed;
                child.IsEnabled = child.SelectorUnit == PointSelectorUnit.EighthNoteTriplet ? true : false;
            }

        }
        #endregion

    }
}
