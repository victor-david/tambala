using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Collections.Generic;
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
    internal class DrumPatternPresenter : ControlObjectVisualGrid
    {
        #region Private
        private int quarterNoteCount;
        private int ticksPerQuarterNote;
        private double scale;
        private readonly Dictionary<int, DrumPatternQuarter> headQuarters;
        #endregion

        /************************************************************************/

        #region Constructors
        internal DrumPatternPresenter(DrumPattern owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Controllers = new GenericList<InstrumentController>();
            quarterNoteCount = Constants.DrumPattern.QuarterNoteCount.Default;
            ticksPerQuarterNote = Constants.DrumPattern.TicksPerQuarterNote.Default;
            scale = Constants.DrumPattern.Scale.Default;

            headQuarters = new Dictionary<int, DrumPatternQuarter>();

            Owner.AddHandler(DrumPatternController.QuarterNoteCountChangedEvent, new RoutedEventHandler(ControllerQuarterNoteCountChanged));
            Owner.AddHandler(DrumPatternController.TicksPerQuarterNoteChangedEvent, new RoutedEventHandler(ControllerTicksPerQuarterNoteChanged));
            Owner.AddHandler(DrumPatternController.ScaleChangedEvent, new RoutedEventHandler(ControllerScaleChanged));
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
        private GenericList<InstrumentController> Controllers
        {
            get;
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
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{nameof(DrumPatternPresenter)}";
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
            Controllers.DoForAll((controller) =>
            {
                element.Add(controller.GetXElement());
            });
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement.
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            int idx = 0;
            foreach (XElement e in ChildElementList(element))
            {
                if (e.Name == nameof(InstrumentController))
                {
                    if (idx < Controllers.Count)
                    {
                        Controllers[idx].Create();
                        Controllers[idx].RestoreFromXElement(e);
                    }
                    idx++;
                }
            }
            ResetIsChanged();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called in response to the <see cref="ControlElement.Create"/> method.
        /// </summary>
        protected override void OnElementCreate()
        {
            headQuarters.Clear();
            CreateHeader();
            CreateBody();
        }

        /// <summary>
        /// Called when the control has been loaded.
        /// </summary>
        protected override void OnLoaded()
        {
            // This class listens to the following events (assigned in the constructor). When restoring,
            // the controller raises the events in response to changes made by its RestoreFromXElement()
            // method. However, the events don't arrive here because (at the time), this control isn't yet loaded
            // and attached to the element tree. Once loaded, we raise the events to modify the quarter note
            // count, ticks per quarter note and scale.
            Owner.Controller.RaiseEvent(new RoutedEventArgs(DrumPatternController.QuarterNoteCountChangedEvent));
            Owner.Controller.RaiseEvent(new RoutedEventArgs(DrumPatternController.TicksPerQuarterNoteChangedEvent));
            Owner.Controller.RaiseEvent(new RoutedEventArgs(DrumPatternController.ScaleChangedEvent));
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Get the count of selecteors that are selected.
        /// </summary>
        /// <returns>The number of selectors in this drum pattern presenter that are currently selected.</returns>
        internal int GetSelectedCount()
        {
            int count = 0;
            foreach (InstrumentController controller in Controllers)
            {
                foreach (var item in controller.Quarters)
                {
                    count += item.Value.GetSelectedCount();
                }
            }
            return count;
        }

        internal void Play(int quarterNote, int position, int operationSet)
        {
            // 64 bodyQuarters. 16 instruments. 4-6 quarters each
            foreach (InstrumentController controller in Controllers)
            {
                controller.Play(quarterNote, position, operationSet);
            }
        }

        /// <summary>
        /// Adds the highlight on the specified quarter note tick using Dispatcher.BeginInvoke.
        /// </summary>
        /// <param name="quarterNote">The quarter note to highlight.</param>
        internal void InvokeAddTickHighlight(int quarterNote)
        {
            if (headQuarters.ContainsKey(quarterNote))
            {
                headQuarters[quarterNote].InvokeAddQuarterNoteTickHighlight();
            }
        }

        /// <summary>
        /// Removes the highlight from the specified quarter note tick using Dispatcher.BeginInvoke.
        /// </summary>
        /// <param name="quarterNote">The quarter note to remove the highlight from.</param>
        internal void InvokeRemoveTickHighlight(int quarterNote)
        {
            if (headQuarters.ContainsKey(quarterNote))
            {
                headQuarters[quarterNote].InvokeRemoveQuarterNoteTickHighlight();
            }
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
            AddElement(new Border()
            {
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(0, 0, 0, 2)
            }, 0, 0);

            int headerRow = AddRowDefinition();
            for (int q = 1; q <= Constants.DrumPattern.QuarterNoteCount.Max; q++)
            {
                int colIdx = AddColumnDefinition(); 

                var quarter = new DrumPatternQuarter()
                {
                    QuarterNote = q,
                    QuarterType = DrumPatternQuarterType.Header,
                    TotalTicks = ticksPerQuarterNote,
                    Width = scale,
                    Visibility = q <= quarterNoteCount ? Visibility.Visible : Visibility.Collapsed
                };

                quarter.Create();
                headQuarters.Add(q, quarter);
                AddElement(quarter, headerRow, colIdx);
            }
        }

        private void ChangeQuarterNoteCount()
        {
            foreach (var child in Grid.Children.OfType<DrumPatternQuarter>())
            {
                child.Visibility = child.QuarterNote <= quarterNoteCount ? Visibility.Visible : Visibility.Collapsed;
            }

        }

        private void ChangeTicksPerQuarterNote()
        {
            foreach (var child in Grid.Children.OfType<DrumPatternQuarter>())
            {
                child.TotalTicks = ticksPerQuarterNote;
            }
        }

        private void ChangeScale()
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
                };

                Controllers.Add(controller);
                AddElement(controller, rowIdx, 0);

                for (int q = 1; q <= Constants.DrumPattern.QuarterNoteCount.Max; q++)
                {
                    var quarter = new DrumPatternQuarter()
                    {
                        QuarterNote = q,
                        QuarterType = DrumPatternQuarterType.Selector,
                        TotalTicks = ticksPerQuarterNote,
                        Width = scale,
                        Visibility = q <= quarterNoteCount ? Visibility.Visible : Visibility.Collapsed
                    };
                    quarter.Create();
                    controller.Quarters.Add(q, quarter);
                    AddElement(quarter, rowIdx, q);
                }
            }
        }

        private void ControllerQuarterNoteCountChanged(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is DrumPatternController controller)
            {
                quarterNoteCount = controller.QuarterNoteCount;
                ChangeQuarterNoteCount();
                e.Handled = true;
            }
        }

        private void ControllerTicksPerQuarterNoteChanged(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is DrumPatternController controller)
            {
                ticksPerQuarterNote = controller.TicksPerQuarterNote;
                ChangeTicksPerQuarterNote();
                e.Handled = true;
            }
        }

        private void ControllerScaleChanged(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is DrumPatternController controller)
            {
                scale = controller.Scale;
                ChangeScale();
                e.Handled = true;
            }
        }
        #endregion

    }
}
