using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents the base class for controls. This class must be inherited.
    /// </summary>
    /// <remarks>
    /// This class provides the base methods and properties for commands, <see cref="DisplayName"/>,
    /// <see cref="IsChanged"/> notification, <see cref="IsExpanded"/> functionality, and
    /// <see cref="IsSelected"/> functionality. Not all derived classes use all properties.
    /// </remarks>
    public abstract class DependencyControlObject : ContentControl, IXElement
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of <see cref="DependencyControlObject"/>.
        /// </summary>
        protected DependencyControlObject()
        {
            Commands = new Dictionary<string, ICommand>();
            MinimizeImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Minimize.64.png", UriKind.Relative));
            MaximizeImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Maximize.64.png", UriKind.Relative));
            ActiveExpandedStateImageSource = MinimizeImageSource;
            Commands.Add("ToggleExpanded", new RelayCommand(RunToggleExpandedCommand));
        }
        #endregion

        /************************************************************************/

        #region DisplayName
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName
        {
            get => (string)GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DisplayName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register
            (
                nameof(DisplayName), typeof(string), typeof(DependencyControlObject), new PropertyMetadata(null, OnDisplayNameChanged)
            );

        private static void OnDisplayNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DependencyControlObject c)
            {
                c.SetIsChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region Commands
        /// <summary>
        /// Gets a dictionary of commands. Used internally by the control template
        /// </summary>
        public Dictionary<string, ICommand> Commands
        {
            get => (Dictionary<string, ICommand>)GetValue(CommandsProperty);
            private set => SetValue(CommandsPropertyKey, value);
        }

        private static readonly DependencyPropertyKey CommandsPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(Commands), typeof(Dictionary<string, ICommand>), typeof(AudioControlBase), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="Commands"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandsProperty = CommandsPropertyKey.DependencyProperty;
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
                nameof(IsChanged), typeof(bool), typeof(TrackContainer), new FrameworkPropertyMetadata(false)
            );

        /// <summary>
        /// Identifies the <see cref="IsChanged"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsChangedProperty = IsChangedPropertyKey.DependencyProperty;

        #endregion

        /************************************************************************/

        #region IsExpanded
        /// <summary>
        /// Gets or sets a boolean value that indicates if the control is expanded (true) or collapsed (false).
        /// </summary>
        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsExpanded"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register
            (
                nameof(IsExpanded), typeof(bool), typeof(DependencyControlObject), new FrameworkPropertyMetadata(true, OnIsExpandedChanged)
            );

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DependencyControlObject c)
            {
                c.OnExpandedImageSourceChanged();
                c.OnIsExpandedChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region IsSelected
        /// <summary>
        /// Gets or sets a boolean value that indicates if the control is selected.
        /// A derived class decides how or if to use this property.
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSelected"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register
            (
                nameof(IsSelected), typeof(bool), typeof(DependencyControlObject), new FrameworkPropertyMetadata(false, OnIsSelectedChanged)
            );

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DependencyControlObject c)
            {
                c.OnIsSelectedChanged();
                c.OnIsSelectedBrushChanged();
            }
        }

        /// <summary>
        /// Gets or sets a brush to use when <see cref="IsSelected"/> is true.
        /// </summary>
        public Brush IsSelectedBrush
        {
            get => (Brush)GetValue(IsSelectedBrushProperty);
            set => SetValue(IsSelectedBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsSelectedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedBrushProperty = DependencyProperty.Register
            (
                nameof(IsSelectedBrush), typeof(Brush), typeof(DependencyControlObject), new PropertyMetadata(null, OnIsSelectedBrushChanged)
            );

        /// <summary>
        /// Gets or sets a brush to use when <see cref="IsSelected"/> is false.
        /// </summary>
        public Brush IsDeselectedBrush
        {
            get => (Brush)GetValue(IsDeselectedBrushProperty);
            set => SetValue(IsDeselectedBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsDeselectedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDeselectedBrushProperty = DependencyProperty.Register
            (
                nameof(IsDeselectedBrush), typeof(Brush), typeof(DependencyControlObject), new PropertyMetadata(null, OnIsSelectedBrushChanged)
            );

        private static void OnIsSelectedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DependencyControlObject c)
            {
                c.OnIsSelectedBrushChanged();
            }
        }

        /// <summary>
        /// Gets the active brush according to the value of <see cref="IsSelected"/>.
        /// A derived class decides how or if to use this property.
        /// </summary>
        public Brush ActiveIsSelectedBrush
        {
            get => (Brush)GetValue(ActiveIsSelectedBrushProperty);
            private set => SetValue(ActiveIsSelectedBrushPropertyKey, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsDeselectedBrush"/> dependency property.
        /// </summary>
        private static readonly DependencyPropertyKey ActiveIsSelectedBrushPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ActiveIsSelectedBrush), typeof(Brush), typeof(DependencyControlObject), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ActiveIsSelectedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveIsSelectedBrushProperty = ActiveIsSelectedBrushPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Images (Minimize / maximize [has state change]
        /// <summary>
        /// Gets or sets the image source to use for the minimize button
        /// </summary>
        public ImageSource MinimizeImageSource
        {
            get => (ImageSource)GetValue(MinimizeImageSourceProperty);
            set => SetValue(MinimizeImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MinimizeImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimizeImageSourceProperty = DependencyProperty.Register
            (
                nameof(MinimizeImageSource), typeof(ImageSource), typeof(DependencyControlObject), new PropertyMetadata(null, OnIsExpandedImageSourceChanged)
            );

        /// <summary>
        /// Gets or sets the image source to use for the maximize button
        /// </summary>
        public ImageSource MaximizeImageSource
        {
            get => (ImageSource)GetValue(MaximizeImageSourceProperty);
            set => SetValue(MaximizeImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MaximizeImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximizeImageSourceProperty = DependencyProperty.Register
            (
                nameof(MaximizeImageSource), typeof(ImageSource), typeof(DependencyControlObject), new PropertyMetadata(null, OnIsExpandedImageSourceChanged)
            );

        private static void OnIsExpandedImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DependencyControlObject c)
            {
                c.OnExpandedImageSourceChanged();
            }
        }

        /// <summary>
        /// Gets the image source that is currently for the mimimized / maximized state
        /// </summary>
        public ImageSource ActiveExpandedStateImageSource
        {
            get => (ImageSource)GetValue(ActiveExpandedStateImageSourceProperty);
            private set => SetValue(ActiveExpandedStateImageSourcePropertyKey, value);
        }

        private static readonly DependencyPropertyKey ActiveExpandedStateImageSourcePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ActiveExpandedStateImageSource), typeof(ImageSource), typeof(DependencyControlObject), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ActiveExpandedStateImageSource"/> dependency property,
        /// </summary>
        public static readonly DependencyProperty ActiveExpandedStateImageSourceProperty = ActiveExpandedStateImageSourcePropertyKey.DependencyProperty;
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

        #region Routed Events
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
                nameof(IsChangedSet), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AudioControlBase)
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
                nameof(IsChangedReset), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AudioControlBase)
            );
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Called when the template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            IsTemplateApplied = true;
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
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Sets the <see cref="IsChanged"/> property to true and raises the <see cref="IsChangedSetEvent"/>.
        /// </summary>
        protected void SetIsChanged()
        {
            IsChanged = true;
            RoutedEventArgs args = new RoutedEventArgs(IsChangedSetEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Sets the <see cref="IsChanged"/> property to false and raises the <see cref="IsChangedResetEvent"/>.
        /// </summary>
        protected void ResetIsChanged()
        {
            IsChanged = false;
            RoutedEventArgs args = new RoutedEventArgs(IsChangedResetEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Called when <see cref="IsExpanded"/> is changed. A derived class can override this method to perform updates as needed.
        /// The base implementaion does nothing.
        /// </summary>
        protected virtual void OnIsExpandedChanged()
        {
        }

        /// <summary>
        /// Called when <see cref="IsSelected"/> is changed. A derived class can override this method to perform updates as needed.
        /// The base implementaion does nothing.
        /// </summary>
        protected virtual void OnIsSelectedChanged()
        {
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

        /************************************************************************/

        #region Private methods
        private void OnExpandedImageSourceChanged()
        {
            ActiveExpandedStateImageSource = IsExpanded ? MinimizeImageSource : MaximizeImageSource;
        }

        private void OnIsSelectedBrushChanged()
        {
            ActiveIsSelectedBrush = IsSelected ? IsSelectedBrush : IsDeselectedBrush;
        }

        private void RunToggleExpandedCommand(object parm)
        {
            IsExpanded = !IsExpanded;
        }
        #endregion
    }
}
