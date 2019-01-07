using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a control that presents and manages a series of patterns to incorporate into a song.
    /// </summary>
    [TemplatePart(Name = PartHostGrid, Type = typeof(Grid))]
    public class SongPatternsContainer : DependencyControlObject
    {
        #region Private
        private const string PartHostGrid = "PART_HostGrid";
        private Grid hostGrid;
        private readonly SongContainer owner;
        #endregion
        
        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SongPatternsContainer"/> class.
        /// </summary>
        internal SongPatternsContainer(SongContainer owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        static SongPatternsContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SongPatternsContainer), new FrameworkPropertyMetadata(typeof(SongPatternsContainer)));
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

        #region Private methods
        private void CreateHostGrid()
        {
            if (hostGrid != null)
            {
                for (int k=0; k < 16; k++)
                {
                    hostGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    SongPatternSelector selector = new SongPatternSelector(this)
                    {
                        DisplayName = $"Pattern {k}",
                        SelectorType = k == 0 ? SongPatternSelectorType.Header : SongPatternSelectorType.Standard
                    };

                    Grid.SetRow(selector, k);
                    hostGrid.Children.Add(selector);
                }
            }
        }
        #endregion

    }
}
