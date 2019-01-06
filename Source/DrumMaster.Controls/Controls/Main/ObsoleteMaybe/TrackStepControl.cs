using System;
using System.Windows;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a track control that handles beats, steps per beat, total steps, and the corresponding sizing
    /// of the visual elements. This class must be inherited.
    /// </summary>
    public abstract class TrackStepControl : ControlBase
    {
        #region Beats
        /// <summary>
        /// Gets or sets the number of beats
        /// </summary>
        public int Beats
        {
            get => (int)GetValue(BeatsProperty);
            set => SetValue(BeatsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Beats"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BeatsProperty = DependencyProperty.Register
            (
                nameof(Beats), typeof(int), typeof(TrackStepControl), new PropertyMetadata(TrackVals.Beats.Default, OnTotalStepsChanged, OnBeatsCoerce)
            );

        private static object OnBeatsCoerce(DependencyObject d, object baseValue)
        {
            int proposed = (int)baseValue;
            return Math.Min(TrackVals.Beats.Max, Math.Max(TrackVals.Beats.Min, proposed));
        }

        /// <summary>
        /// Gets or sets the beats text descriptor
        /// </summary>
        public string BeatsText
        {
            get => (string)GetValue(BeatsTextProperty);
            set => SetValue(BeatsTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BeatsText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BeatsTextProperty = DependencyProperty.Register
            (
                nameof(BeatsText), typeof(string), typeof(TrackStepControl), new PropertyMetadata(TrackVals.Beats.DefaultText)
            );

        /// <summary>
        /// Gets the minimum beats allowed. Used for binding in the control template.
        /// </summary>
        public int MinBeats
        {
            get => TrackVals.Beats.Min;
        }

        /// <summary>
        /// Gets the maximum beats allowed. Used for binding in the control template.
        /// </summary>
        public int MaxBeats
        {
            get => TrackVals.Beats.Max;
        }
        #endregion

        /************************************************************************/

        #region StepsPerBeat
        /// <summary>
        /// Gets or sets the number of steps per beat
        /// </summary>
        public int StepsPerBeat
        {
            get => (int)GetValue(StepsPerBeatProperty);
            set => SetValue(StepsPerBeatProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StepsPerBeat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StepsPerBeatProperty = DependencyProperty.Register
            (
                nameof(StepsPerBeat), typeof(int), typeof(TrackStepControl), new PropertyMetadata(TrackVals.StepsPerBeat.Default, OnTotalStepsChanged, OnStepsPerBeatCoerce)
            );

        private static object OnStepsPerBeatCoerce(DependencyObject d, object baseValue)
        {
            int proposed = (int)baseValue;
            return Math.Min(TrackVals.StepsPerBeat.Max, Math.Max(TrackVals.StepsPerBeat.Min, proposed));
        }

        /// <summary>
        /// Gets or sets the steps per beat text descriptor
        /// </summary>
        public string StepsPerBeatText
        {
            get => (string)GetValue(StepsPerBeatTextProperty);
            set => SetValue(StepsPerBeatTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StepsPerBeatText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StepsPerBeatTextProperty = DependencyProperty.Register
            (
                nameof(StepsPerBeatText), typeof(string), typeof(TrackStepControl), new PropertyMetadata(TrackVals.StepsPerBeat.DefaultText)
            );

        /// <summary>
        /// Gets the minimum steps per beats allowed. Used for binding in the control template.
        /// </summary>
        public int MinStepsPerBeat
        {
            get => TrackVals.StepsPerBeat.Min;
        }

        /// <summary>
        /// Gets the maximum steps per beat allowed. Used for binding in the control template.
        /// </summary>
        public int MaxStepsPerBeat
        {
            get => TrackVals.StepsPerBeat.Max;
        }
        #endregion

        /************************************************************************/

        #region TotalSteps
        /// <summary>
        /// Gets the total number of steps.
        /// </summary>
        public int TotalSteps
        {
            get => (int)GetValue(TotalStepsProperty);
            private set => SetValue(TotalStepsPropertyKey, value);
        }

        private static readonly DependencyPropertyKey TotalStepsPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(TotalSteps), typeof(int), typeof(TrackStepControl), new FrameworkPropertyMetadata(TrackVals.TotalSteps.Default)
            );

        /// <summary>
        /// Identifies the <see cref="TotalSteps"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TotalStepsProperty = TotalStepsPropertyKey.DependencyProperty;

        private static void OnTotalStepsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackStepControl c)
            {
                c.TotalSteps = c.Beats * c.StepsPerBeat;
                c.OnTotalStepsChanged();
                c.SetIsChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region BoxSize
        /// <summary>
        /// Gets or sets the size of the boxes.
        /// </summary>
        public double BoxSize
        {
            get => (double)GetValue(BoxSizeProperty);
            set => SetValue(BoxSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BoxSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BoxSizeProperty = DependencyProperty.Register
            (
                nameof(BoxSize), typeof(double), typeof(TrackStepControl), new PropertyMetadata(TrackVals.BoxSize.Default, OnBoxSizeChanged, OnBoxSizeCoerce)
            );

        private static void OnBoxSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackStepControl c)
            {
                c.OnBoxSizeChanged();
                c.SetIsChanged();
            }
        }

        private static object OnBoxSizeCoerce(DependencyObject d, object baseValue)
        {
            double proposed = (double)baseValue;
            return Math.Min(TrackVals.BoxSize.Max, Math.Max(TrackVals.BoxSize.Min, proposed));
        }

        /// <summary>
        /// Gets or sets the text used to display the box size
        /// </summary>
        public string BoxSizeText
        {
            get => (string)GetValue(BoxSizeTextProperty);
            set => SetValue(BoxSizeTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BoxSizeText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BoxSizeTextProperty = DependencyProperty.Register
            (
                nameof(BoxSizeText), typeof(string), typeof(TrackStepControl), new PropertyMetadata(TrackVals.BoxSize.DefaultText)
            );

        /// <summary>
        /// Gets the minimum box size allowed. Used for binding in the control template.
        /// </summary>
        public double MinBoxSize
        {
            get => TrackVals.BoxSize.Min;
        }

        /// <summary>
        /// Gets the maximum box size allowed. Used for binding in the control template.
        /// </summary>
        public double MaxBoxSize
        {
            get => TrackVals.BoxSize.Max;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Sets the <see cref="Beats"/> and <see cref="StepsPerBeat"/>
        /// </summary>
        /// <param name="beats">The number of beats.</param>
        /// <param name="stepsPerBeat">The number of steps per beat.</param>
        public void SetBeats(int beats, int stepsPerBeat)
        {
            Beats = beats;
            StepsPerBeat = stepsPerBeat;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the <see cref="TotalSteps"/> property has changed.
        /// A derived class may override this method to perform other updates.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void OnTotalStepsChanged()
        {
        }
        /// <summary>
        /// Called when the <see cref="BoxSize"/> property changes.
        /// A derived class may override this method to perform other updates.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void OnBoxSizeChanged()
        {
        }
        #endregion

        /************************************************************************/

        #region Private methods (instance)
        #endregion

    }
}
