using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Extends <see cref="ControlObjectVisualGrid"/> to provide a visual display panel for <see cref="DrumPattern"/>
    /// </summary>
    public class DrumPatternPresenter : ControlObjectVisualGrid
    {
        #region Private
        private int quarterNoteCount;
        private int totalTicks;
        private double scale;
        #endregion

        /************************************************************************/

        #region Constructors
        internal DrumPatternPresenter(DrumPattern owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            // Setting private fields instead of properties to avoid triggering the setters
            quarterNoteCount = Constants.DrumPattern.QuarterNote.Default;
            totalTicks = Constants.DrumPattern.TotalTick.Default;
            scale = Constants.DrumPattern.Scale.Default;
        }

        static DrumPatternPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrumPatternPresenter), new FrameworkPropertyMetadata(typeof(DrumPatternPresenter)));
        }
        #endregion

        /************************************************************************/

        #region Internal properties
        /// <summary>
        /// Gets the <see cref="DrumPattern"/> that owns this instance.
        /// </summary>
        internal DrumPattern Owner
        {
            get;
        }

        /// <summary>
        /// From this assembly, gets or sets the quarter note count.
        /// </summary>
        internal int QuarterNoteCount
        {
            get => quarterNoteCount;
            set
            {
                quarterNoteCount = value;
                OnQuarterNoteChanged();
            }
        }

        /// <summary>
        /// From this assembly, gets or sets the total number of ticks.
        /// </summary>
        internal int TotalTicks
        {
            get => totalTicks;
            set
            {
                totalTicks = value;
                OnTotalTicksChanged();
            }
        }

        /// <summary>
        /// From this assembly, gets or sets the scale
        /// </summary>
        internal double Scale
        {
            get => scale;
            set
            {
                scale = value;
                OnScaleChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region DrumKit
        /// <summary>
        /// Gets the drum kit. Shorthand property to <see cref="ProjectContainer.DrumKit"/>.
        /// </summary>
        private DrumKit DrumKit
        {
            get => Owner.Owner.DrumKit;
        }
        #endregion

        /************************************************************************/
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CreateHeader();
            CreateBody();
        }

        public override string ToString()
        {
            return $"{nameof(DrumPatternPresenter)} Q:{QuarterNoteCount} T:{TotalTicks} Scale:{Scale}";
        }

        #region IXElement
        /// <summary>
        /// Gets the XElement for this object.
        /// This method satisfies the interface requirement but is not used.
        /// It always throws an InvalidOperationException.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public override XElement GetXElement()
        {
            var element = new XElement(nameof(DrumPatternPresenter));


            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement.
        /// This method satisfies the interface requirement but is not used.
        /// It always throws an InvalidOperationException.
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            throw new InvalidOperationException();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        /// <summary>
        /// Creates the header and ticks. This is a one time operation called when the template is applied.
        /// </summary>
        private void CreateHeader()
        {
            Grid.ColumnDefinitions.Clear();
            Grid.Children.Clear();
            AddColumnDefinition(Constants.DrumPattern.FirstColumnWidth);
            int headerRow = AddRowDefinition();
            for (int q = 1; q <= quarterNoteCount; q++)
            {
                int colIdx = AddColumnDefinition(scale);
                AddElement(new VisualQuarterNote(q)
                {
                    TotalTicks = totalTicks
                }, headerRow, colIdx);
            }
        }

        private void OnQuarterNoteChanged()
        {

        }

        private void OnTotalTicksChanged()
        {
            foreach (var child in Grid.Children.OfType<VisualQuarterNote>())
            {
                child.TotalTicks = totalTicks;
            }
        }

        private void OnScaleChanged()
        {
            for (int col = 1; col < Grid.ColumnDefinitions.Count; col++)
            {
                Grid.ColumnDefinitions[col].Width = new GridLength(scale);
            }
        }


        private void CreateBody()
        {
            foreach (Instrument ins in DrumKit.Instruments)
            {
                int rowIdx = AddRowDefinition();

                var controller = new InstrumentController()
                {
                    DisplayName = ins.DisplayName,
                    Margin = new Thickness(0,2,2,0)
                };

                AddElement(controller, rowIdx, 0);

                //    for (int col = 1; col < Grid.ColumnDefinitions.Count; col++)
                //    {

                //        var ps = new PointSelector(PointSelectorType.SongRow);
                //        AddElement(ps, rowIdx, col);
                //    }

                //    //rowIdx = AddRowDefinition();
                //    //Separator s = new Separator();
                //    //AddElement(s, rowIdx, 0);

            }
        }
        #endregion

    }
}
