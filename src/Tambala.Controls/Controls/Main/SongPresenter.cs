/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Provides the visual for <see cref="SongContainer"/>
    /// </summary>
    public class SongPresenter : ControlObjectVisualGrid
    {
        #region Private
        private readonly Dictionary<int, PointSelector> headerSelectors;
        private const string SplitterName = "Splitter";
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SongPresenter"/> class.
        /// </summary>
        /// <param name="owner">The song container that owns this instance.</param>
        internal SongPresenter(SongContainer owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            headerSelectors = new Dictionary<int, PointSelector>();
            SongSelectors = new SongSelectorCollection();
        }

        static SongPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SongPresenter), new FrameworkPropertyMetadata(typeof(SongPresenter)));
        }
        #endregion

        /************************************************************************/

        #region Owner (CLR)
        /// <summary>
        /// Gets the <see cref="SongContainer"/> that owns this instance.
        /// </summary>
        internal SongContainer Owner
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region SongSelectors
        /// <summary>
        /// Gets the body selectors
        /// </summary>
        internal SongSelectorCollection SongSelectors
        {
            get;
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
            var element = new XElement(nameof(SongPresenter));
            // sanity check.
            if (Grid.ColumnDefinitions.Count > 0)
            {
                element.Add(new XElement(SplitterName, Grid.ColumnDefinitions[0].Width));
            }


            foreach (var child in Grid.Children.OfType<PointSelector>().Where((ps) => ps.SelectorType == PointSelectorType.SongRow && ps.IsSelected))
            {
                element.Add(child.GetXElement());
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
                if (e.Name == SplitterName && Grid.ColumnDefinitions.Count > 0)
                {
                    if (int.TryParse(e.Value, out int result))
                    {
                        Grid.ColumnDefinitions[0].Width = new GridLength(result);
                    }
                }
                if (e.Name == nameof(PointSelector))
                {
                    XAttribute rowa = e.Attribute(nameof(PointSelector.Row));
                    XAttribute posa = e.Attribute(nameof(Position));
                    
                    if (rowa != null && posa != null)
                    {
                        if (int.TryParse(rowa.Value, out int row) && int.TryParse(posa.Value, out int pos))
                        {
                            PointSelector ps = GetPointSelectorAt(row, pos);
                            if (ps != null)
                            {
                                ps.RestoreFromXElement(e);
                            }
                        }
                    }

                }
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Creates the content. Called after the template has been applied.
        /// </summary>
        protected override void OnElementCreate()
        {
            headerSelectors.Clear();
            SongSelectors.Clear();
            int rowIdx = AddRowDefinition();

            AddColumnDefinition(Constants.SongSelector.MinFirstColumnWidth);
            Grid.ColumnDefinitions[0].MinWidth = Constants.SongSelector.MinFirstColumnWidth;
            Grid.ColumnDefinitions[0].MaxWidth = Constants.SongSelector.MaxFirstColumnWidth;

            int splitterColIdx = AddColumnDefinition(6.0);
            GridSplitter splitter = new GridSplitter();
            splitter.SetResourceReference(StyleProperty, "VerticalGridSplitter");
            Grid.SetRowSpan(splitter, Constants.DrumPattern.MaxCount);
            AddElement(splitter, rowIdx+1, splitterColIdx);

            for (int k = 1; k <= Constants.SongSelector.Count.Default; k++)
            {
                int colIdx = AddColumnDefinition();

                var sps = new PointSelector()
                {
                    SelectorType = PointSelectorType.SongHeader,
                    Margin = new Thickness(1),
                    Position = k,
                };

                headerSelectors.Add(k, sps);
                AddElement(sps, rowIdx, colIdx);

                if (k < Constants.SongSelector.Count.Default)
                {
                    VisualDivider div = new VisualDivider(k)
                    {
                        Visibility = k % DivisionCount == 0 ? Visibility.Visible : Visibility.Collapsed
                    };
                    colIdx = AddColumnDefinition();
                    AddElement(div, rowIdx, colIdx);
                    Grid.SetRowSpan(div, Constants.DrumPattern.MaxCount + 1);
                }
            }

            for (int idx = 0; idx < Constants.DrumPattern.MaxCount; idx++)
            {
                CreatePatternRow(idx);
            }
        }

        /// <summary>
        /// Called when <see cref="ControlObjectSelector.SelectorSize"/> changes
        /// to update children of this instance.
        /// </summary>
        protected override void OnSelectorSizeChanged()
        {
            foreach (var point in Grid.Children.OfType<PointSelector>())
            {
                point.SelectorSize = SelectorSize;
            }
        }

        /// <summary>
        /// Called when <see cref="ControlObjectSelector.DivisionCount"/> changes.
        /// to update children of this instance.
        /// </summary>
        protected override void OnDivisionCountChanged()
        {
            foreach (var divider in Grid.Children.OfType<VisualDivider>())
            {
                divider.Visibility = divider.Position % DivisionCount == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Highlights the specified pattern and removes highlight from all others.
        /// </summary>
        /// <param name="patternIdx">The selected pattern index</param>
        internal void HighlightSelectedPattern(int patternIdx)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                foreach (var child in Grid.Children.OfType<DrumPatternSelector>())
                {
                    child.IsSelected = child.Position == patternIdx;
                }
            }));
        }

        /// <summary>
        /// Highlights the song header at the specified position using the Dispatcher.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="select">true to highlight; false to turn highlight off.</param>
        internal void InvokeHighlightSongHeader(int position, bool select)
        {
            if (headerSelectors.ContainsKey(position))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                {
                    headerSelectors[position].IsSelected = select;
                }));
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private void CreatePatternRow(int patternIdx)
        {
            int rowIdx = AddRowDefinition();
            int colIdx = 0;

            DrumPatternSelector button = new DrumPatternSelector(patternIdx)
            {
                Command = new RelayCommand(RunSelectPatternCommand),
                CommandParameter = patternIdx,
            };

            /* binding keeps us in sync with user-editable track name  */
            Binding binding = new Binding(nameof(DisplayName))
            {
                Source = Owner.Owner.DrumPatterns[patternIdx]
            };
            button.SetBinding(Button.ContentProperty, binding);

            AddElement(button, rowIdx, colIdx);
            // skip over the splitter column.
            colIdx++;

            for (int k = 1; k <= Constants.SongSelector.Count.Default; k++)
            {
                colIdx++;
                var sps = new PointSelector()
                {
                    SelectorType = PointSelectorType.SongRow,
                    Margin = new Thickness(1),
                    Position = k,
                    Row = rowIdx,
                };
                SongSelectors.Add(rowIdx-1, k, sps);
                AddElement(sps, rowIdx, colIdx);
                colIdx++;
            }
        }

        private void RunSelectPatternCommand(object parm)
        {
            if (parm is int patternIdx)
            {
                Owner.Owner.ActivateDrumPattern(patternIdx);
            }
        }

        private PointSelector GetPointSelectorAt(int row, int position)
        {
            foreach (var child in Grid.Children.OfType<PointSelector>().Where((ps) => ps.SelectorType == PointSelectorType.SongRow))
            {
                if (child.Row == row && child.Position == position)
                {
                    return child;
                }
            }
            return null;
        }
        #endregion
    }
}