using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Provides the visual for <see cref="SongContainer"/>
    /// </summary>
    public class SongPresenter : ControlObjectVisualGrid
    {
        #region Private
        private Brush isSelectedBrush;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SongPresenter"/> class.
        /// </summary>
        internal SongPresenter(SongContainer owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            isSelectedBrush = new SolidColorBrush(Colors.LightGray);
        }

        static SongPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SongPresenter), new FrameworkPropertyMetadata(typeof(SongPresenter)));
        }
        #endregion

        /************************************************************************/

        #region CLR Properties
        /// <summary>
        /// Gets the <see cref="SongContainer"/> that owns this instance.
        /// </summary>
        internal SongContainer Owner
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
            element.Add(new XComment($"{nameof(SongPresenter)} receives {nameof(SelectorSize)} and {nameof(DivisionCount)} from its owner {nameof(SongContainer)}"));
            //element.Add(new XComment($"and updates its {nameof(DrumPatternSelectorPanel)} children"));
            //foreach (var child in Grid.Children.OfType<DrumPatternSelectorPanel>())
            //{
            //    element.Add(child.GetXElement());
            //}
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Creates the content. Called after the template has been applied.
        /// </summary>
        protected override void CreateVisualElement()
        {
            int rowIdx = AddRowDefinition();
            AddColumnDefinition(Constants.Selector.FirstColumnWidth);
            AddElement(new TextBlock(), 0, 0);

            for (int k = 1; k <= Constants.Selector.Count.Default; k++)
            {
                int colIdx = AddColumnDefinition();

                var sps = new PointSelector(PointSelectorType.SongHeader)
                {
                    Margin = new Thickness(1),
                    Position = k,
                };

                AddElement(sps, rowIdx, colIdx);

                if (k < Constants.Selector.Count.Default)
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
        /// Called when <see cref="ControlObjectVisual.SelectorSize"/> changes
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
        /// Called when <see cref="ControlObjectVisual.DivisionCount"/> changes.
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
            foreach (var child in Grid.Children.OfType<VisualSelector>())
            {
                child.IsSelected = child.Position == patternIdx;
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private void CreatePatternRow(int patternIdx)
        {
            int rowIdx = AddRowDefinition();
            int colIdx = 0;

            var link = new LinkedTextBlock()
            {
                VerticalAlignment = VerticalAlignment.Center,
                Command = new RelayCommand(RunSelectPatternCommand),
                CommandParameter = patternIdx,
            };

            Binding binding = new Binding(nameof(DrumPattern.DisplayName))
            {
                Source = Owner.Owner.DrumPatterns[patternIdx]
            };
            link.SetBinding(TextBlock.TextProperty, binding);

            var border = new VisualSelector(patternIdx)
            {
                IsSelectedBrush = isSelectedBrush,
                Child = link
            };

            AddElement(border, rowIdx, colIdx);

            for (int k = 1; k <= Constants.Selector.Count.Default; k++)
            {
                colIdx++;
                var sps = new PointSelector(PointSelectorType.SongRow)
                {
                    Margin = new Thickness(1),
                    Position = k,
                };
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



























        #endregion
    }
}
