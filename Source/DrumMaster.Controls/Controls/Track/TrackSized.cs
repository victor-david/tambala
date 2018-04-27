using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a track control that handles sizing. This class must be inherited.
    /// </summary>
    public abstract class TrackSized : TrackControlBase
    {
        #region Public properties (Type / Steps / Sizes)
        /// <summary>
        /// Gets or sets the size of the boxes.
        /// </summary>
        public double BoxSize
        {
            get => (double)GetValue(BoxSizeProperty);
            set => SetValue(BoxSizeProperty, value);
        }

        public static readonly DependencyProperty BoxSizeProperty = DependencyProperty.Register
            (
                nameof(BoxSize), typeof(double), typeof(TrackSized), new PropertyMetadata(TrackVals.BoxSize.Default, OnBoxSizeChanged, OnBoxSizeCoerce)
            );

        /// <summary>
        /// Gets or sets the text used to display the box size
        /// </summary>
        public string BoxSizeText
        {
            get => (string)GetValue(BoxSizeTextProperty);
            set => SetValue(BoxSizeTextProperty, value);
        }

        public static readonly DependencyProperty BoxSizeTextProperty = DependencyProperty.Register
            (
                nameof(BoxSizeText), typeof(string), typeof(TrackSized), new PropertyMetadata(TrackVals.BoxSize.DefaultText)
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

        #region Protected methods
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

        /************************************************************************/

        #region Private methods (static)

        private static void OnBoxSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackSized c)
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
        #endregion
    }
}
