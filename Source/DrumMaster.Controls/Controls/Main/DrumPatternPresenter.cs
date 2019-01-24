using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
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
    /// Extends <see cref="ControlObjectVisualGrid"/> to provide a visual display panel for <see cref="DrumPattern"/>
    /// </summary>
    internal class DrumPatternPresenter : ControlObjectVisualGrid
    {
        #region Private
        private int quarterNoteCount;
        private int ticksPerQuarterNote;
        private double scale;
        private readonly Dictionary<int, DrumPatternQuarter> headQuarters;
        private readonly Dictionary<int, DrumPatternQuarter> velocityQuarters;
        private InstrumentController selectedController;
        private int velocityRowIdx;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumPatternPresenter"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="DrumPattern"/> that owns this presenter.</param>
        internal DrumPatternPresenter(DrumPattern owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Controllers = new GenericList<InstrumentController>();
            quarterNoteCount = Constants.DrumPattern.QuarterNoteCount.Default;
            ticksPerQuarterNote = Constants.DrumPattern.TicksPerQuarterNote.Default;
            scale = Constants.DrumPattern.Scale.Default;

            headQuarters = new Dictionary<int, DrumPatternQuarter>();
            velocityQuarters = new Dictionary<int, DrumPatternQuarter>();

            Owner.AddHandler(DrumPatternController.QuarterNoteCountChangedEvent, new RoutedEventHandler(ControllerQuarterNoteCountChanged));
            Owner.AddHandler(DrumPatternController.TicksPerQuarterNoteChangedEvent, new RoutedEventHandler(ControllerTicksPerQuarterNoteChanged));
            Owner.AddHandler(DrumPatternController.ScaleChangedEvent, new RoutedEventHandler(ControllerScaleChanged));
            Owner.AddHandler(DrumPattern.DrumKitChangedEvent, new RoutedEventHandler(DrumPatternDrumKitChanged));
            Owner.AddHandler(InstrumentController.IsSelectedChangedEvent, new RoutedEventHandler(ControllerIsSelectedChanged));
        }

        static DrumPatternPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrumPatternPresenter), new FrameworkPropertyMetadata(typeof(DrumPatternPresenter)));
        }
        #endregion

        /************************************************************************/

        #region Internal / private properties
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

        /// <summary>
        /// Gets or sets the selected controller
        /// </summary>
        private InstrumentController SelectedController
        {
            get => selectedController;
            set
            {
                selectedController = value;
                OnSelectedControllerChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region DrumKit
        /// <summary>
        /// Gets the drum kit. Shorthand property to <see cref="DrumPattern.DrumKit"/>.
        /// </summary>
        private DrumKit DrumKit
        {
            get => Owner.DrumKit;
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
            velocityQuarters.Clear();
            CreateHeader();
            CreateInstrumentRows();
            CreateVelocityControls();
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
        /// Get the count of selectors that are selected.
        /// </summary>
        /// <returns>The number of selectors in this drum pattern presenter that are currently selected.</returns>
        internal int GetSelectedCount()
        {
            int count = 0;
            foreach (InstrumentController controller in Controllers)
            {
                foreach (var item in controller.PatternQuarters)
                {
                    count += item.Value.GetSelectedCount();
                }
            }
            return count;
        }

        /// <summary>
        /// Instructs each instrument controller to submit to the voice pool if appropiate
        /// according to the parameters.
        /// </summary>
        /// <param name="songUnit">The song unit.</param>
        /// <param name="quarterNote">The quarter note.</param>
        /// <param name="position">The position within the quarter note.</param>
        /// <param name="operationSet">The operation set. Used only when submitting the voice to the pool.</param>
        internal void Play(PointSelectorSongUnit songUnit, int quarterNote, int position, int operationSet)
        {
            foreach (InstrumentController controller in Controllers)
            {
                controller.Play(songUnit, quarterNote, position, operationSet);
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

        /// <summary>
        /// Sets the visibility of controllers
        /// </summary>
        /// <param name="allVisible">When true, all controllers visible; otherwise, only the selected one.</param>
        internal void SetControllerVisibility(bool allVisible)
        {
            if (selectedController == null && Controllers.Count > 0)
            {
                Controllers[0].IsSelected = true;
            }

            Controllers.DoForAll((con) =>
            {
                con.SetIsVisible(allVisible || con == selectedController, quarterNoteCount);
            });
        }
        #endregion

        /************************************************************************/

        #region Private methods (Creation)
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

        private void CreateInstrumentRows()
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
                        QuarterType = DrumPatternQuarterType.PatternSelector,
                        TotalTicks = ticksPerQuarterNote,
                        Width = scale,
                        Visibility = q <= quarterNoteCount ? Visibility.Visible : Visibility.Collapsed,
                    };
                    quarter.Create();
                    controller.PatternQuarters.Add(q, quarter);
                    AddElement(quarter, rowIdx, q);
                }
                // Add an extra row after each controller row. Height is Auto with nothing in it.
                // Later, when selecting the controller, we place the velocity selectors in
                // this row, i.e selectedController.Row + 1.
                AddRowDefinition();
            }
        }

        private void CreateVelocityControls()
        {
            // Velocity controls are held in a hidden row, Height = 0.0
            // Later, when the user selects a controller, we move them into
            // selectedController.Row + 1. If selected controller is set to null,
            // the velocity controls go back into the hidden row.
            velocityRowIdx = AddRowDefinition(0.0);

            for (int q = 1; q <= Constants.DrumPattern.QuarterNoteCount.Max; q++)
            {
                var quarter = new DrumPatternQuarter()
                {
                    QuarterNote = q,
                    QuarterType = DrumPatternQuarterType.VelocitySelector,
                    TotalTicks = ticksPerQuarterNote,
                    Width = scale,
                    Visibility = q <= quarterNoteCount ? Visibility.Visible : Visibility.Collapsed,
                };
                quarter.Create();
                velocityQuarters.Add(q, quarter);
                AddElement(quarter, velocityRowIdx, q);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (Other)

        private void ControllerQuarterNoteCountChanged(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is DrumPatternController controller)
            {
                quarterNoteCount = controller.QuarterNoteCount;
                ChangeQuarterNoteCount();
                e.Handled = true;
            }
        }

        private void ChangeQuarterNoteCount()
        {
            foreach (var item in headQuarters)
            {
                item.Value.SetVisibility(quarterNoteCount);
            }
            Controllers.DoForAll((con) =>
            {
                foreach (var item in con.PatternQuarters)
                {
                    item.Value.SetVisibility(quarterNoteCount, con.IsEnabledForPlay && con.IsVisible);
                }
            });
            foreach (var item in velocityQuarters)
            {
                item.Value.SetVisibility(quarterNoteCount);
            }
        }

        private void ControllerTicksPerQuarterNoteChanged(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is DrumPatternController controller)
            {
                ticksPerQuarterNote = controller.TicksPerQuarterNote;
                foreach (var child in Grid.Children.OfType<DrumPatternQuarter>())
                {
                    child.TotalTicks = ticksPerQuarterNote;
                }
                e.Handled = true;
            }
        }

        private void ControllerScaleChanged(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is DrumPatternController controller)
            {
                scale = controller.Scale;
                foreach (var child in Grid.Children.OfType<DrumPatternQuarter>())
                {
                    child.Width = scale;
                }
                e.Handled = true;
            }
        }

        private void UpdateControllerInstruments()
        {
            int controllerCount = Controllers.Count;
            int drumKitCount = DrumKit.Instruments.Count;
            int idx = 0;
            while (idx < controllerCount && idx < drumKitCount)
            {
                Controllers[idx].Instrument = DrumKit.Instruments[idx];
                Controllers[idx].DisplayName = DrumKit.Instruments[idx].DisplayName;
                Controllers[idx].SetIsEnabledForPlay(true, quarterNoteCount);
                idx++;
            }

            while (idx < controllerCount)
            {
                Controllers[idx].SetIsEnabledForPlay(false, quarterNoteCount);
                idx++;
            }

            if (SelectedController != null && !SelectedController.IsEnabledForPlay)
            {
                SelectedController.IsSelected = false;
            }
        }

        private void DrumPatternDrumKitChanged(object sender, RoutedEventArgs e)
        {
            UpdateControllerInstruments();
            e.Handled = true;
        }

        private void OnSelectedControllerChanged()
        {
            foreach (var item in velocityQuarters)
            {
                if (selectedController != null)
                {
                    selectedController.PatternQuarters[item.Key].SyncToVelocity(item.Value);
                    Grid.SetRow(item.Value, Grid.GetRow(selectedController) + 1);
                }
                else
                {
                    Grid.SetRow(item.Value, velocityRowIdx);
                }
            }
        }

        private void ControllerIsSelectedChanged(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is InstrumentController controller)
            {
                // This method will deselect all controllers except
                // the one that is the original source of this event
                // which will raise the event again. Need to check
                // if the controller is selected so we don't get re-entrancy
                // and a stack overflow.
                if (controller.IsSelected)
                {
                    Controllers.DoForAll((con) =>
                    {
                        con.IsSelected = con == controller;
                    });

                    SelectedController = controller;
                }
                else
                {
                    SelectedController = null;
                }
                
                e.Handled = true;
            }
        }
        #endregion
    }
}