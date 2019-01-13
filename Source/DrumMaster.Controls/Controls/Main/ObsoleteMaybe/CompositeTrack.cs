using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls.Obsolete
{
    /// <summary>
    /// Represents a composite track, a <see cref="TrackController"/> and its corresponding <see cref="TrackBoxContainerStep"/>
    /// </summary>
    public class CompositeTrack : ContentControl, IXElement
    {
        #region Private
        private Window dragCursor;
        private Point startPoint;
        private bool dragging;
        #endregion

        /************************************************************************/

        #region DragHighlightBrush
        /// <summary>
        /// Gets or sets the drag highlight brush.
        /// Used on the background of a track when another track is dragged over it.
        /// </summary>
        public Brush DragHighlightBrush
        {
            get => (Brush)GetValue(DragHighlightBrushProperty);
            set => SetValue(DragHighlightBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DragHighlightBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DragHighlightBrushProperty = DependencyProperty.Register
            (
                nameof(DragHighlightBrush), typeof(Brush), typeof(CompositeTrack), new PropertyMetadata(new SolidColorBrush(Colors.LightYellow))
            );
        #endregion

        /************************************************************************/

        #region Internal dependency properties
        /// <summary>
        /// Gets the track container that owns this composite track
        /// </summary>
        internal TrackContainer Owner
        {
            get;
        }

        /// <summary>
        /// Gets the track controller
        /// </summary>
        internal TrackController Controller
        {
            get => (TrackController)GetValue(ControllerProperty);
            private set => SetValue(ControllerPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ControllerPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(Controller), typeof(TrackController), typeof(CompositeTrack), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="Controller"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty ControllerProperty = ControllerPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the box container for the track steps.
        /// </summary>
        internal TrackBoxContainerStep BoxContainer
        {
            get => (TrackBoxContainerStep)GetValue(BoxContainerProperty);
            private set => SetValue(BoxContainerPropertyKey, value);
        }

        private static readonly DependencyPropertyKey BoxContainerPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(BoxContainer), typeof(TrackBoxContainerStep), typeof(CompositeTrack), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="BoxContainer"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty BoxContainerProperty = BoxContainerPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the active drag background brush. Used when one track is being dragged over another
        /// </summary>
        public Brush ActiveDragHighlightBrush
        {
            get => (Brush)GetValue(ActiveDragHighlightBrushProperty);
            private set => SetValue(ActiveDragHighlightBrushPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ActiveDragHighlightBrushPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ActiveDragHighlightBrush), typeof(Brush), typeof(CompositeTrack), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ActiveDragHighlightBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveDragHighlightBrushProperty = ActiveDragHighlightBrushPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Internal CLR properties
        /// <summary>
        /// Gets the thread safe reference to <see cref="Controller"/>
        /// </summary>
        internal TrackController ThreadSafeController
        {
            get;
        }

        /// <summary>
        /// Gets the thread safe reference to <see cref="BoxContainer"/>.
        /// </summary>
        internal TrackBoxContainerStep ThreadSafeBoxContainer
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructors (Internal / Static)
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeTrack"/> class.
        /// </summary>
        internal CompositeTrack(TrackContainer owner, Instrument piece)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));

            Controller = ThreadSafeController = new TrackController(this)
            {
                Piece = piece,
                Padding = new Thickness(4),
                MutedImageSource = owner.MutedImageSource,
                VoicedImageSource = owner.VoicedImageSource,
            };

            BoxContainer = ThreadSafeBoxContainer = new TrackBoxContainerStep(this)
            {
                Padding = new Thickness(4),
                Beats = owner.Beats,
                StepsPerBeat = owner.StepsPerBeat,
                BoxSize = owner.BoxSize,
                SelectedBackgroundBrush = new SolidColorBrush(Colors.LightBlue)
            };

            AllowDrop = true;
        }

        static CompositeTrack()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CompositeTrack), new FrameworkPropertyMetadata(typeof(CompositeTrack)));
        }
        #endregion

        /************************************************************************/

        #region IXElement 
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public XElement GetXElement()
        {
            var element = new XElement(nameof(CompositeTrack));
            element.Add(Controller.GetXElement());
            element.Add(BoxContainer.GetXElement());
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public void RestoreFromXElement(XElement element)
        {
            IEnumerable<XElement> childList = from el in element.Elements() select el;

            foreach (XElement e in childList)
            {
                if (e.Name == nameof(TrackController)) Controller.RestoreFromXElement(e);
                if (e.Name == nameof(TrackBoxContainerStep)) BoxContainer.RestoreFromXElement(e);
            }

        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the PreviewMouseLeftButtonDown event is raised.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (GetRoutedEventTrack(e) is CompositeTrack track)
            {
                startPoint = e.GetPosition(null);
            }
        }

        /// <summary>
        /// Called when the PreviewMouseMove event is raised.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            if (!dragging && e.LeftButton == MouseButtonState.Pressed && GetRoutedEventTrack(e) is CompositeTrack track)
            {
                Point pos = e.GetPosition(null);

                if (Math.Abs(pos.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(pos.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    AllowDrop = false;
                    CreateDragCursor(track);
                    dragCursor.Show();
                    dragging = true;
                    DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
                    AllowDrop = true;
                    dragging = false;
                    dragCursor.Close();
                    dragCursor = null;
                }
            }
        }

        /// <summary>
        /// Called when the DragEnter event is raised.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            if (GetRoutedEventTrack(e) is CompositeTrack track)
            {
                SetDragEffect();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called when the DragOver event is rasied.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            if (GetRoutedEventTrack(e) == null)
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Called when the GiveFeedback event is raised.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
            e.UseDefaultCursors = false;
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            dragCursor.Left = w32Mouse.X + 8;
            dragCursor.Top = w32Mouse.Y - (dragCursor.ActualHeight / 2);
            dragCursor.Opacity = (e.Effects == DragDropEffects.Move) ? 1.0 : 0.35;
            e.Handled = true;
        }

        /// <summary>
        /// Called when the DragLeave event is raised.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
            if (GetRoutedEventTrack(e) is CompositeTrack track)
            {
                RemoveDragEffect();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called when the Drop event is raised.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            if (GetRoutedEventTrack(e) is CompositeTrack track)
            {
                if (e.Data.GetData(typeof(CompositeTrack)) is CompositeTrack data)
                {
                    // dropped data on track
                    Owner.MoveTracks(data, track);
                    e.Handled = true;
                }
                RemoveDragEffect();
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private CompositeTrack GetRoutedEventTrack(RoutedEventArgs e)
        {
            if (e.OriginalSource is Grid grid)
            {
                return CoreHelper.GetVisualParent<CompositeTrack>(grid);
            }
            return null;
        }

        private void SetDragEffect()
        {
            ActiveDragHighlightBrush = DragHighlightBrush;
        }

        private void RemoveDragEffect()
        {
            ActiveDragHighlightBrush = null;
        }

        private void CreateDragCursor(CompositeTrack track)
        {
            dragCursor = new Window()
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Topmost = true,
                ShowInTaskbar = false,
                Width = 180.0,
                Height = 30.0,
                Content = new Border()
                {
                    Background = new LinearGradientBrush(Colors.DodgerBlue, Colors.FloralWhite, 30.0),
                    BorderBrush = new SolidColorBrush(Colors.DarkBlue),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(2.5),
                    Child = new TextBlock()
                    {
                        Text = track.Controller.Piece.DisplayName,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = new SolidColorBrush(Colors.DarkBlue),
                    },
                    
                },
            };
        }
        #endregion

        /************************************************************************/

        #region Win32
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public int X;
            public int Y;
        };
        #endregion
    }
}
