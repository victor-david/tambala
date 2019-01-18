using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
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
        /// Gets or sets a value that indicates if
        /// </summary>
        protected bool IsCreated
        {
            get;
            private set;
        }
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

        ///// <summary>
        ///// Gets or sets the element to defer. See remarks.
        ///// </summary>
        ///// <remarks>
        ///// This property is used by some derived classes to defer
        ///// processing of the XElement until the control has been loaded.
        ///// If your class cannot restore state at the time of the call
        ///// to <see cref="RestoreFromXElement(XElement)"/>
        ///// (because it must wait until the control has finished loading), save
        ///// its passed element to this property and override <see cref="OnLoaded"/> to
        ///// perform the operation then.
        ///// </remarks>
        //protected XElement DeferredElement
        //{
        //    get;
        //    set;
        //}

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
        #endregion
    }
}
