using Restless.App.Tambala.Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Restless.App.Tambala.Controls
{
    /// <summary>
    /// Represents the base control for objects in the library. This class provides
    /// the declaration of <see cref="IXElement"/> and related methods / properties.
    /// This class must be inherited.
    /// </summary>
    public abstract class ControlElement : ContentControl, IXElement
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlElement"/> class.
        /// </summary>
        protected ControlElement()
        {
            AddHandler(LoadedEvent, new RoutedEventHandler((s, e) =>
            {
                OnLoaded();
                e.Handled = true;
            }));
        }
        #endregion

        /************************************************************************/

        #region IsTemplateApplied
        /// <summary>
        /// Gets a boolean value that indicates if <see cref="OnApplyTemplate"/> has been called.
        /// </summary>
        protected bool IsTemplateApplied
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region IsCreated
        /// <summary>
        /// Gets a value that indicates if the <see cref="Create"/> method has been called.
        /// </summary>
        protected bool IsCreated
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region IsChanged
        /// <summary>
        /// Gets a boolean value that indicates if changes have occured since this object was established.
        /// </summary>
        public bool IsChanged
        {
            get => (bool)GetValue(IsChangedProperty);
            private set => SetValue(IsChangedPropertyKey, value);
        }

        private static readonly DependencyPropertyKey IsChangedPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(IsChanged), typeof(bool), typeof(ControlElement), new FrameworkPropertyMetadata(false)
            );

        /// <summary>
        /// Identifies the <see cref="IsChanged"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsChangedProperty = IsChangedPropertyKey.DependencyProperty;


        /// <summary>
        /// Provides notification when the <see cref="IsChanged"/> property is set to true.
        /// </summary>
        public event RoutedEventHandler IsChangedSet
        {
            add => AddHandler(IsChangedSetEvent, value);
            remove => RemoveHandler(IsChangedSetEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsChangedSet"/> routed event.
        /// </summary>
        public static readonly RoutedEvent IsChangedSetEvent = EventManager.RegisterRoutedEvent
            (
                nameof(IsChangedSet), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ControlElement)
            );

        /// <summary>
        /// Provides notification when the <see cref="IsChanged"/> property is set to false.
        /// </summary>
        public event RoutedEventHandler IsChangedReset
        {
            add => AddHandler(IsChangedResetEvent, value);
            remove => RemoveHandler(IsChangedResetEvent, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsChangedReset"/> routed event.
        /// </summary>
        public static readonly RoutedEvent IsChangedResetEvent = EventManager.RegisterRoutedEvent
            (
                nameof(IsChangedReset), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ControlElement)
            );
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Called when the template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            IsTemplateApplied = true;
        }

        /// <summary>
        /// If <see cref="IsCreated"/> is false, calls the <see cref="OnElementCreate"/>
        /// method  
        /// </summary>
        public void Create()
        {
            if (!IsCreated)
            {
                OnElementCreate();
                IsCreated = true;
            }
        }
        #endregion

        /************************************************************************/

        #region IXElement 
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public abstract XElement GetXElement();
        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public abstract void RestoreFromXElement(XElement element);

        /// <summary>
        /// Gets an IEnumerable fot the child elements of <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element</param>
        /// <returns>An IEnumerable.</returns>
        protected IEnumerable<XElement> ChildElementList(XElement element)
        {
            return from el in element.Elements() select el;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the loaded event fires. Override this method in a derived class
        /// to perform processing after the control is loaded. The base implementation does nothing.
        /// </summary>
        protected virtual void OnLoaded()
        {
        }

        /// <summary>
        /// Called when The <see cref="Create"/> method is called.
        /// When this method finishes, <see cref="IsCreated"/> is set to true.
        /// Override in a derived class to perform create operations.
        /// </summary>
        protected virtual void OnElementCreate()
        {
        }

        /// <summary>
        /// Sets the <see cref="IsChanged"/> property to true and raises the <see cref="IsChangedSetEvent"/>.
        /// </summary>
        protected void SetIsChanged()
        {
            IsChanged = true;
            RaiseEvent(new RoutedEventArgs(IsChangedSetEvent));
        }

        /// <summary>
        /// Sets the <see cref="IsChanged"/> property to false and raises the <see cref="IsChangedResetEvent"/>.
        /// </summary>
        protected void ResetIsChanged()
        {
            IsChanged = false;
            RaiseEvent(new RoutedEventArgs(IsChangedResetEvent));
        }

        /// <summary>
        /// Sets the specified dependency property to the specified string.
        /// </summary>
        /// <param name="prop">The dependency property.</param>
        /// <param name="val">The value</param>
        protected void SetDependencyProperty(DependencyProperty prop, string val)
        {
            if (prop == null) throw new ArgumentNullException(nameof(prop));

            if (prop.PropertyType == typeof(string))
            {
                SetValue(prop, val);
            }

            if (prop.PropertyType == typeof(int))
            {
                if (int.TryParse(val, out int result))
                {
                    SetValue(prop, result);
                }
            }

            if (prop.PropertyType == typeof(double))
            {
                if (double.TryParse(val, out double result))
                {
                    SetValue(prop, result);
                }
            }

            if (prop.PropertyType == typeof(float))
            {
                if (float.TryParse(val, out float result))
                {
                    SetValue(prop, result);
                }
            }

            if (prop.PropertyType == typeof(bool))
            {
                if (bool.TryParse(val, out bool result))
                {
                    SetValue(prop, result);
                }
            }
        }
        #endregion
    }
}
