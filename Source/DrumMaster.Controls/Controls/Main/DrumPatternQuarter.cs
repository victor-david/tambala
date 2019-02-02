using Restless.App.DrumMaster.Controls.Core;
using SharpDX.XAudio2;
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
    /// Represents a single quarter note and its associated ticks for a drum pattern.
    /// </summary>
    public class DrumPatternQuarter : ControlElement
    {
        #region Private
        private VisualTick quarterNoteTick;
        private readonly Dictionary<int, PointSelector> pointSelectors;
        private readonly Dictionary<int, VelocitySlider> velocitySliders;
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
            velocitySliders = new Dictionary<int, VelocitySlider>();
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

        #region Public methods
        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string that describes the object.</returns>
        public override string ToString()
        {
            switch (QuarterType)
            {
                case DrumPatternQuarterType.VelocitySelector:
                    return $"{nameof(DrumPatternQuarter)} Type:{QuarterType} Note:{QuarterNote} Sliders:{velocitySliders.Count}";
                default:
                    return $"{nameof(DrumPatternQuarter)} Type:{QuarterType} Note:{QuarterNote} Selectors:{pointSelectors.Count}";
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
        /// Gets the volume for the selector at the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>A volume value expressed as an XAudio2 amplitude ratio</returns>
        internal float GetSelectorVolume(int position)
        {
            if (pointSelectors.ContainsKey(position))
            {
                return pointSelectors[position].ThreadSafeVolume;
            }

            return XAudio2.DecibelsToAmplitudeRatio(Constants.Volume.Default);
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

        /// <summary>
        /// Sets this object's visibility, either visible or collpased.
        /// </summary>
        /// <param name="maxQuarter">
        /// The max quarter. If <see cref="QuarterNote"/> is less than or equal
        /// to <paramref name="maxQuarter"/> (and <paramref name="otherCondition"/> is true),
        /// the control is visible; otherwise, collapsed.
        /// </param>
        /// <param name="otherCondition">Another condition to evaulate.</param>
        internal void SetVisibility(int maxQuarter, bool otherCondition = true)
        {
            Visibility = QuarterNote <= maxQuarter && otherCondition ? Visibility.Visible : Visibility.Collapsed;
        }


        internal void SyncToVelocity(DrumPatternQuarter velocityQuarter)
        {
            if (velocityQuarter == null || velocityQuarter.QuarterType != DrumPatternQuarterType.VelocitySelector)
            {
                throw new ArgumentException($"{nameof(velocityQuarter)} must be {DrumPatternQuarterType.VelocitySelector}");
            }

            if (pointSelectors.Count != velocityQuarter.velocitySliders.Count)
            {
                throw new InvalidOperationException("Point selectors / Velocity selectors count mismatch");
            }

            foreach (var item in velocityQuarter.velocitySliders)
            {
                item.Value.Selector = pointSelectors[item.Key];
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the <see cref="ControlElement.Create"/> method is invoked
        /// by a client. This method creates the drum pattern quarter.
        /// </summary>
        protected override void OnElementCreate()
        {
            pointSelectors.Clear();

            for (int k = 1; k <= Ticks.LowestCommon; k++)
            {
                Visual.ColumnDefinitions.Add(new ColumnDefinition());
            }

            switch (QuarterType)
            {
                case DrumPatternQuarterType.Header:
                    CreateHeaderType();
                    break;
                case DrumPatternQuarterType.PatternSelector:
                    CreatePatternSelectorType();
                    break;
                case DrumPatternQuarterType.VelocitySelector:
                    CreateVelocitySelectorType();
                    break;
            }

            ApplyTotalTicks();
        }
        #endregion

        /************************************************************************/

        #region Private methods (Create)
        private void CreateHeaderType()
        {
            Visual.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
            Visual.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
            TextBlock text = new TextBlock() { Text = QuarterNote.ToString(), Foreground = Brushes.DarkBlue };
            text.HorizontalAlignment = HorizontalAlignment.Center;
            AddElement(text, 0, 0);

            quarterNoteTick = new VisualTick(0, PointSelectorUnit.QuarterNote);
            AddElement(quarterNoteTick, 1, 0);

            foreach(var map in Ticks.UniqueTickPositionMap)
            {
                foreach (int col in map.Value)
                {
                    VisualTick tick = new VisualTick(col, map.Key);
                    AddElement(tick, 1, col);
                }
            }
        }

        private void CreatePatternSelectorType()
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

            foreach (var map in Ticks.UniqueTickPositionMap)
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

        private void CreateVelocitySelectorType()
        {
            Visual.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            var qs = MakeVelocitySlider();
            qs.SelectorUnit = PointSelectorUnit.QuarterNote;
            velocitySliders.Add(0, qs);
            AddElement(qs, 0, 0);

            foreach (var map in Ticks.UniqueTickPositionMap)
            {
                foreach (int col in map.Value)
                {
                    var s = MakeVelocitySlider();
                    s.SelectorUnit = map.Key;
                    velocitySliders.Add(col, s);
                    AddElement(s, 0, col);
                }
            }
        }

        private VelocitySlider MakeVelocitySlider()
        {
            VelocitySlider s = new VelocitySlider()
            {
                Height = 112.0,
                Orientation = Orientation.Vertical,
                Minimum = Constants.Volume.Selector.Min,
                Maximum = Constants.Volume.Selector.Max,
                Value = Constants.Volume.Default,
            };

            return s;
        }
        #endregion

        /************************************************************************/

        #region Private methods (other)
        private void AddElement(UIElement element, int row, int column)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
            Visual.Children.Add(element);
        }

        private void ApplyTotalTicks()
        {
            if (Ticks.FullTickPositionMap.ContainsKey(TotalTicks))
            {
                foreach (var child in Visual.Children.OfType<VisualTick>().Where((t)=>t.SelectorUnit != PointSelectorUnit.QuarterNote))
                {
                    if (Ticks.FullTickPositionMap[TotalTicks].Contains(child.Position))
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
            foreach (var ch in Visual.Children.OfType<ISelectorUnit>().Where((t) => t.SelectorUnit != PointSelectorUnit.QuarterNote))
            {
                UIElement child = (UIElement)ch;

                if (ch.SelectorUnit != PointSelectorUnit.EighthNoteTriplet)
                {
                    child.Visibility = Visibility.Visible;
                    switch (TotalTicks)
                    {
                        case Constants.DrumPattern.TicksPerQuarterNote.Eighth:
                            child.IsEnabled = ch.SelectorUnit == PointSelectorUnit.EighthNote;
                            break;
                        case Constants.DrumPattern.TicksPerQuarterNote.Sixteenth:
                            child.IsEnabled = ch.SelectorUnit == PointSelectorUnit.SixteenthNote || ch.SelectorUnit == PointSelectorUnit.EighthNote;
                            break;
                        default:
                            child.IsEnabled = true;
                            break;
                    }
                }
                else
                    child.Visibility = Visibility.Collapsed;
            }
        }

        private void ApplyTotalTripletTicks()
        {
            foreach (var ch in Visual.Children.OfType<ISelectorUnit>().Where((t) => t.SelectorUnit != PointSelectorUnit.QuarterNote))
            {
                UIElement child = (UIElement)ch;
                child.Visibility = ch.SelectorUnit == PointSelectorUnit.EighthNoteTriplet ? Visibility.Visible : Visibility.Collapsed;
                child.IsEnabled = ch.SelectorUnit == PointSelectorUnit.EighthNoteTriplet ? true : false;
            }
        }
        #endregion

    }
}
