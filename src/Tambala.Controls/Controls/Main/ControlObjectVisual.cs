/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the MIT License
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Windows;

namespace Restless.App.Tambala.Controls
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

        #region Public methods
        /// <summary>
        /// Called when the template is applied. Calls the base ApplyTemplate
        /// method and then calls <see cref="ControlElement.Create"/>, which
        /// in turn (if <see cref="ControlElement.IsCreated"/> is false) 
        /// calls <see cref="ControlElement.OnElementCreate"/>
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Create();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion
    }
}