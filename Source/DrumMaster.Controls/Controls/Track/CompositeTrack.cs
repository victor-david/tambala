using Restless.App.DrumMaster.Controls.Audio;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a composite track, a <see cref="TrackController"/> and its corresponding <see cref="TrackBoxContainerStep"/>
    /// </summary>
    public class CompositeTrack : ContentControl
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public Properties
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

        #endregion

        #region Internal properties
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
        internal CompositeTrack(TrackContainer owner, AudioPiece piece)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));

            Controller = ThreadSafeController = new TrackController(this)
            {
                Piece = piece,
                Margin = new Thickness(4),
                Padding = new Thickness(4),
            };

            BoxContainer = ThreadSafeBoxContainer = new TrackBoxContainerStep(this)
            {
                Margin = new Thickness(4),
                Padding = new Thickness(4),
                Beats = owner.Beats,
                StepsPerBeat = owner.StepsPerBeat,
                BoxSize = owner.BoxSize,
                SelectedBackgroundBrush = new SolidColorBrush(Colors.LightBlue)
            };
        }

        static CompositeTrack()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CompositeTrack), new FrameworkPropertyMetadata(typeof(CompositeTrack)));
        }
        #endregion
    }
}
