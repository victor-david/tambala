using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a control that presents and manages a series of patterns to incorporate into a song.
    /// </summary>
    [TemplatePart(Name = PartHostGrid, Type = typeof(Grid))]
    public class SongPatternSelector : PatternSelector
    {
        #region Private
        private const string PartHostGrid = "PART_HostGrid";
        private Grid hostGrid;
        private readonly SongPatternsContainer owner;
        #endregion
        
        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SongPatternSelector"/> class.
        /// </summary>
        internal SongPatternSelector(SongPatternsContainer owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Commands.Add("SelectPattern", new RelayCommand(RunSelectPatternCommand));
        }

        static SongPatternSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SongPatternSelector), new FrameworkPropertyMetadata(typeof(SongPatternSelector)));
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
                if (hostGrid.ColumnDefinitions.Count > 1)
                {
                    int removeCount = hostGrid.ColumnDefinitions.Count - 1;
                    hostGrid.ColumnDefinitions.RemoveRange(1, removeCount);
                }

                for (int k = 1; k <= 48; k++)
                {
                    hostGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(24, GridUnitType.Pixel) });

                    SongPointSelector point = new SongPointSelector(this)
                    {
                        Margin = new Thickness(2),
                        Position = k,
                        SelectorType = SelectorType,
                        DisplayName = SelectorType == SongPatternSelectorType.Header ? $"{k}" : null,
                    };
                    Grid.SetColumn(point, k);
                    hostGrid.Children.Add(point);
                }
            }
        }

        private void RunSelectPatternCommand(object parm)
        {
            Debug.WriteLine($"{DisplayName} selected");
        }
        #endregion

    }
}
