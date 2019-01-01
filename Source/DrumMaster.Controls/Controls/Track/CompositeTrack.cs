using Restless.App.DrumMaster.Controls.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a composite track, a <see cref="TrackController"/> and its corresponding <see cref="TrackBoxContainer"/>
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
        /// Gets the track controller
        /// </summary>
        internal TrackBoxContainer BoxContainer
        {
            get => (TrackBoxContainer)GetValue(BoxContainerProperty);
            private set => SetValue(BoxContainerPropertyKey, value);
        }

        private static readonly DependencyPropertyKey BoxContainerPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(BoxContainer), typeof(TrackBoxContainer), typeof(CompositeTrack), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="BoxContainer"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty BoxContainerProperty = BoxContainerPropertyKey.DependencyProperty;

        #endregion

        #region Internal properties
        internal TrackController InternalController
        {
            get;
        }

        internal TrackBoxContainer InternalBoxContainer
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

            Controller = InternalController = new TrackController(this)
            {
                Piece = piece,
                Margin = new Thickness(4),
                Padding = new Thickness(4),
                //BorderThickness = new Thickness(1, 1, 0, 1),
                //BorderBrush = Brushes.LightSlateGray,
            };

            BoxContainer = InternalBoxContainer = new TrackBoxContainer(this)
            {
                Margin = new Thickness(4),
                Padding = new Thickness(4),
                //BorderThickness = new Thickness(0,1,1,1),
                //BorderBrush = Brushes.LightSlateGray,
                BoxType = TrackBoxType.TrackStep,
                TotalSteps = owner.TotalSteps,
                BoxSize = owner.BoxSize,
                SelectedBackgroundBrush = new SolidColorBrush(Colors.LightBlue)
            };

            //Controller.SetBoxContainer(BoxContainer);
            //BoxContainer.SetController(Controller);
        }

        static CompositeTrack()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CompositeTrack), new FrameworkPropertyMetadata(typeof(CompositeTrack)));
        }
        #endregion
    }
}
