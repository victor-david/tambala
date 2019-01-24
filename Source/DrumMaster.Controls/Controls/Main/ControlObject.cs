using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Extends <see cref="ControlElement"/> to provide base functionality for controls. This class must be inherited.
    /// </summary>
    /// <remarks>
    /// This class provides the base methods and properties for commands, <see cref="DisplayName"/>,
    /// <see cref="IsChanged"/> notification, <see cref="IsExpanded"/> functionality.
    /// </remarks>
    public abstract class ControlObject : ControlElement
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of <see cref="ControlObject"/>.
        /// </summary>
        protected ControlObject()
        {
            Commands = new Dictionary<string, ICommand>();
            ExpandedImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Caret.Up.White.32.png", UriKind.Relative));
            CollapsedImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Caret.Down.White.32.png", UriKind.Relative));
            StretchedImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Caret.Up.Down.White.32.png", UriKind.Relative));
            ActiveExpandedStateImageSource = ExpandedImageSource;
            ToggleExpandedCommand = new RelayCommand(RunToggleExpandedCommand);
            ToggleStretchedCommand = new RelayCommand(RunToggleStretchedCommand);
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
                nameof(DisplayName), typeof(string), typeof(ControlObject), new PropertyMetadata(null, OnDisplayNameChanged)
            );

        private static void OnDisplayNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ControlObject c)
            {
                c.SetIsChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region ToggleExpandedCommand
        /// <summary>
        /// Gets the command to toggle <see cref="IsExpanded"/>.
        /// </summary>
        public ICommand ToggleExpandedCommand
        {
            get => (ICommand)GetValue(ToggleExpandedCommandProperty);
            private set => SetValue(ToggleExpandedCommandPropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey ToggleExpandedCommandPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ToggleExpandedCommand), typeof(ICommand), typeof(ControlObject), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ToggleExpandedCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToggleExpandedCommandProperty = ToggleExpandedCommandPropertyKey.DependencyProperty;
        #endregion


        /************************************************************************/

        #region ToggleStretchedCommand
        /// <summary>
        /// Gets the command to toggle <see cref="IsStretched"/>.
        /// </summary>
        public ICommand ToggleStretchedCommand
        {
            get => (ICommand)GetValue(ToggleStretchedCommandProperty);
            private set => SetValue(ToggleStretchedCommandPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ToggleStretchedCommandPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ToggleStretchedCommand), typeof(ICommand), typeof(ControlObject), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ToggleStretchedCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToggleStretchedCommandProperty = ToggleStretchedCommandPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Commands (Generic dictionary)
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
                nameof(IsChanged), typeof(bool), typeof(ControlObject), new FrameworkPropertyMetadata(false)
            );

        /// <summary>
        /// Identifies the <see cref="IsChanged"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsChangedProperty = IsChangedPropertyKey.DependencyProperty;

        #endregion

        /************************************************************************/

        #region IsExpanded
        /// <summary>
        /// Gets or sets a boolean value that indicates if the control is expanded.
        /// </summary>
        /// <remarks>
        /// It is up to derived classes to determine how to use this property.
        /// By itself, it does nothing.
        /// </remarks>
        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsExpanded"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register
            (
                nameof(IsExpanded), typeof(bool), typeof(ControlObject), new FrameworkPropertyMetadata(true, OnIsExpandedChanged)
            );

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ControlObject c)
            {
                c.OnExpandedImageSourceChanged();
                c.OnIsExpandedChanged();
            }
        }

        /// <summary>
        /// Gets or sets the image source to use when <see cref="IsExpanded"/> is true.
        /// </summary>
        public ImageSource ExpandedImageSource
        {
            get => (ImageSource)GetValue(ExpandedImageSourceProperty);
            set => SetValue(ExpandedImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ExpandedImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandedImageSourceProperty = DependencyProperty.Register
            (
                nameof(ExpandedImageSource), typeof(ImageSource), typeof(ControlObject), new PropertyMetadata(null, OnIsExpandedImageSourceChanged)
            );

        /// <summary>
        /// Gets or sets the image source to use when <see cref="IsExpanded"/> is false.
        /// </summary>
        public ImageSource CollapsedImageSource
        {
            get => (ImageSource)GetValue(CollapsedImageSourceProperty);
            set => SetValue(CollapsedImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CollapsedImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CollapsedImageSourceProperty = DependencyProperty.Register
            (
                nameof(CollapsedImageSource), typeof(ImageSource), typeof(ControlObject), new PropertyMetadata(null, OnIsExpandedImageSourceChanged)
            );

        private static void OnIsExpandedImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ControlObject c)
            {
                c.OnExpandedImageSourceChanged();
            }
        }

        /// <summary>
        /// Gets the image source that is currently for the expanded / collapsed state
        /// </summary>
        public ImageSource ActiveExpandedStateImageSource
        {
            get => (ImageSource)GetValue(ActiveExpandedStateImageSourceProperty);
            private set => SetValue(ActiveExpandedStateImageSourcePropertyKey, value);
        }

        private static readonly DependencyPropertyKey ActiveExpandedStateImageSourcePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ActiveExpandedStateImageSource), typeof(ImageSource), typeof(ControlObject), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="ActiveExpandedStateImageSource"/> dependency property,
        /// </summary>
        public static readonly DependencyProperty ActiveExpandedStateImageSourceProperty = ActiveExpandedStateImageSourcePropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region IsStretched
        /// <summary>
        /// Gets or sets a boolean value that indicates if the control is stretched.
        /// </summary>
        /// <remarks>
        /// It is up to derived classes to determine how to use this property.
        /// By itself, it does nothing.
        /// </remarks>
        public bool IsStretched
        {
            get => (bool)GetValue(IsStretchedProperty);
            set => SetValue(IsStretchedProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsStretched"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsStretchedProperty = DependencyProperty.Register
            (
                nameof(IsStretched), typeof(bool), typeof(ControlObject), new FrameworkPropertyMetadata(true, OnIsStretchedChanged)
            );

        private static void OnIsStretchedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ControlObject c)
            {
                c.OnIsStretchedChanged();
            }
        }

        /// <summary>
        /// Gets or sets the image source to use in connection with the <see cref="IsStretched"/> property.
        /// This image does not toggle.
        /// </summary>
        public ImageSource StretchedImageSource
        {
            get => (ImageSource)GetValue(StretchedImageSourceProperty);
            set => SetValue(StretchedImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StretchedImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StretchedImageSourceProperty = DependencyProperty.Register
            (
                nameof(StretchedImageSource), typeof(ImageSource), typeof(ControlObject), new PropertyMetadata(null)
            );
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
                nameof(IsChangedSet), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ControlObject)
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
                nameof(IsChangedReset), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ControlObject)
            );
        #endregion

        /************************************************************************/

        #region Protected methods
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
        /// Called when <see cref="IsExpanded"/> is changed. A derived class can override this method to perform updates as needed.
        /// The base implementaion does nothing.
        /// </summary>
        protected virtual void OnIsExpandedChanged()
        {
        }

        /// <summary>
        /// Called when <see cref="IsStretched"/> is changed. A derived class can override this method to perform updates as needed.
        /// The base implementaion does nothing.
        /// </summary>
        protected virtual void OnIsStretchedChanged()
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
            ActiveExpandedStateImageSource = IsExpanded ? ExpandedImageSource : CollapsedImageSource;
        }

        private void RunToggleExpandedCommand(object parm)
        {
            IsExpanded = !IsExpanded;
        }

        private void RunToggleStretchedCommand(object parm)
        {
            IsStretched = !IsStretched;
        }
        #endregion
    }
}
