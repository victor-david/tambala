using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Windows;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Extends <see cref="ControlObjectSelector"/> for classes that require a visual
    /// component that is defined as a property of the class. This class must be inherited.
    /// </summary>
    public abstract class ControlObjectVisual : ControlObjectSelector
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of <see cref="ControlObjectVisual"/>.
        /// </summary>
        internal ControlObjectVisual()
        {
        }
        #endregion

        /************************************************************************/

        #region Visual
        /// <summary>
        /// Gets or (from a derived class) sets the visual element.
        /// </summary>
        public UIElement VisualElement
        {
            get => (UIElement)GetValue(VisualElementProperty);
            protected set => SetValue(VisualElementPropertyKey, value);
        }

        private static readonly DependencyPropertyKey VisualElementPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(VisualElement), typeof(UIElement), typeof(ControlObjectVisual), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="VisualElement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisualElementProperty = VisualElementPropertyKey.DependencyProperty;

        #endregion

        /************************************************************************/

        #region ISelector
        ///// <summary>
        ///// Gets or sets the selector size
        ///// </summary>
        //public double SelectorSize
        //{
        //    get => selectorSize;
        //    set
        //    {
        //        selectorSize = Math.Min(Constants.Selector.Size.Max, Math.Max(Constants.Selector.Size.Min, value));
        //        OnSelectorSizeChanged();
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the division count
        ///// </summary>
        //public int DivisionCount
        //{
        //    get => divisionCount;
        //    set
        //    {
        //        divisionCount = Math.Min(Constants.Selector.Division.Max, Math.Max(Constants.Selector.Division.Min, value));
        //        OnDivisionCountChanged();
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the position
        ///// </summary>
        //public int Position
        //{
        //    get => position;
        //    set
        //    {
        //        position = value;
        //        OnPositionChanged();
        //    }
        //}
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Called when the template is applied
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CreateVisualElement();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        ///// <summary>
        ///// Called when the <see cref="SelectorSize"/> property changes.
        ///// A derived class can override this method to provide further processing.
        ///// The base implementation does nothing.
        ///// </summary>
        //protected virtual void OnSelectorSizeChanged()
        //{
        //}

        ///// <summary>
        ///// Called when the <see cref="DivisionCount"/> property changes.
        ///// A derived class can override this method to provide further processing.
        ///// The base implementation does nothing.
        ///// </summary>
        //protected virtual void OnDivisionCountChanged()
        //{
        //}

        ///// <summary>
        ///// Called when <see cref="Position"/> changes. A derived class can override
        ///// this method to perform updates. The base implementation does nothing.
        ///// </summary>
        //protected virtual void OnPositionChanged()
        //{
        //}

        /// <summary>
        /// Override in a derived class to create <see cref="VisualElement"/>
        /// This method is called after the template is applied.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void CreateVisualElement()
        {
        }
        #endregion
    }
}
