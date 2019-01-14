using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        private readonly GenericList<DrumPatternQuarter> headQuarters;
        private readonly GenericList<DrumPatternQuarter> bodyQuarters;
        #endregion

        /************************************************************************/

        #region Constructors
        internal DrumPatternPresenter(DrumPattern owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Controllers = new GenericList<InstrumentController>();
            headQuarters = new GenericList<DrumPatternQuarter>();
            bodyQuarters = new GenericList<DrumPatternQuarter>();
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
        /// Gets the list of instrument controllers.
        /// </summary>
        internal GenericList<InstrumentController> Controllers
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

        #region Public methods
        /// <summary>
        /// Called when the template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            headQuarters.Clear();
            bodyQuarters.Clear();
            CreateHeader();
            CreateBody();
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{nameof(DrumPatternPresenter)} Q:{QuarterNoteCount} T:{TotalTicks} Scale:{Scale}";
        }
        #endregion
        
        /************************************************************************/

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

        internal void Play(int quarterNote, int position, int operationSet)
        {
            // 64 bodyQuarters. 16 instruments. 4 quarters each
            //Debug.Assert(quarterNote >= 0 && quarterNote < bodyQuarters.Count);
            int cidx = 0;
            foreach (InstrumentController controller in Controllers)
            {
                int idx = cidx * (quarterNoteCount-1) + quarterNote - 1;
                Debug.WriteLine($"Idx: {idx}");
                DrumPatternQuarter quarter = bodyQuarters[idx];
                if (quarter.IsSelected(position))
                {
                    controller.Play(operationSet);
                }
                cidx++;

            }

            //DrumPatternQuarter quarter =  bodyQuarters[quarterNote - 1];

            //if (quarter.IsSelected(position))
            //{
            //    Debug.WriteLine($"{position} SELECTED");
            //}
        }

        #region Private methods
        /// <summary>
        /// Creates the header and ticks. This is a one time operation called when the template is applied.
        /// </summary>
        private void CreateHeader()
        {
            Grid.ColumnDefinitions.Clear();
            Grid.Children.Clear();
            AddColumnDefinition(Constants.DrumPattern.FirstColumnWidth);
            AddElement(new Border()
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(0, 0, 0, 2)
            }, 0, 0);

            int headerRow = AddRowDefinition();
            for (int q = 1; q <= quarterNoteCount; q++)
            {
                int colIdx = AddColumnDefinition(); // scale);

                var quarter = new DrumPatternQuarter()
                {
                    QuarterNote = q,
                    QuarterType = DrumPatternQuarterType.Header,
                    TotalTicks = totalTicks,
                    Width = scale,
                };

                headQuarters.Add(quarter);
                AddElement(quarter, headerRow, colIdx);
            }
        }

        private void OnQuarterNoteChanged()
        {

        }

        private void OnTotalTicksChanged()
        {
            foreach (var child in Grid.Children.OfType<DrumPatternQuarter>())
            {
                child.TotalTicks = totalTicks;
            }
        }

        private void OnScaleChanged()
        {
            foreach (var child in Grid.Children.OfType<DrumPatternQuarter>())
            {
                child.Width = scale;
            }
        }


        private void CreateBody()
        {
            Controllers.Clear();

            foreach (Instrument ins in DrumKit.Instruments)
            {
                int rowIdx = AddRowDefinition();

                var controller = new InstrumentController(this)
                {
                    DisplayName = ins.DisplayName,
                    Margin = new Thickness(0,2,0,0),
                    Padding = new Thickness(0,2,12,0),
                    Instrument = ins,
                    IsMuted = true,
                };

                Controllers.Add(controller);
                AddElement(controller, rowIdx, 0);

                for (int q = 1; q <= quarterNoteCount; q++)
                {
                    var quarter = new DrumPatternQuarter()
                    {
                        QuarterNote = q,
                        QuarterType = DrumPatternQuarterType.Selector,
                        TotalTicks = totalTicks,
                        Width = scale,
                    };
                    bodyQuarters.Add(quarter);
                    AddElement(quarter, rowIdx, q);
                }
            }
        }
        #endregion

    }
}
