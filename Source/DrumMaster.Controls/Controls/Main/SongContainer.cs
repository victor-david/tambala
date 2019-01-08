using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a control that presents and manages a series of patterns to incorporate into a song.
    /// </summary>
    /// <remarks>
    /// This control provides the ability to select and manage which individual drum kit patterns
    /// comprise a song and the timeline for selecting which drum kit patterns play and for how
    /// many times.
    /// </remarks>
    [TemplatePart(Name = PartHostGrid, Type = typeof(Grid))]
    public class SongContainer : SizeablePatternSelector
    {
        #region Private
        private const string PartHostGrid = "PART_HostGrid";
        private Grid hostGrid;
        #endregion
        
        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SongContainer"/> class.
        /// </summary>
        internal SongContainer(ProjectContainer owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        static SongContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SongContainer), new FrameworkPropertyMetadata(typeof(SongContainer)));
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the <see cref="ProjectContainer"/> that owns this instance.
        /// </summary>
        internal ProjectContainer Owner
        {
            get;
        }
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
        /// Called when <see cref="SizeablePatternSelector.SelectorSize"/> changes.
        /// </summary>
        protected override void OnSelectorSizeChanged()
        {
            foreach(var selector in hostGrid.Children.OfType<DrumPatternSelector>())
            {
                selector.SelectorSize = SelectorSize;
            }
            SetIsChanged();
        }

        /// <summary>
        /// Called when <see cref="SizeablePatternSelector.DivisionCount"/> changes.
        /// </summary>
        protected override void OnDivisionCountChanged()
        {
            foreach (var selector in hostGrid.Children.OfType<DrumPatternSelector>())
            {
                selector.DivisionCount = DivisionCount;
            }
            SetIsChanged();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void CreateHostGrid()
        {
            if (hostGrid != null)
            {
                for (int k=0; k < 9; k++)
                {
                    hostGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    SongDrumPatternSelectorType type = k == 0 ? SongDrumPatternSelectorType.Header : SongDrumPatternSelectorType.Standard;
                    DrumPatternSelector selector = new DrumPatternSelector(this, type)
                    {
                        DisplayName = "zaz",
                        Position = k,
                        
                    };
                    Grid.SetRow(selector, k);
                    hostGrid.Children.Add(selector);
                }
            }
        }
        #endregion

    }
}
