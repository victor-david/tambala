/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Represents a text block that has a command
    /// </summary>
    public class LinkedTextBlock : TextBlock
    {
        #region Private
        private Brush originalForeground;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Defines a dependency property for the command.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register
            (
                nameof(Command), typeof(ICommand), typeof(LinkedTextBlock), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the command parameter
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Defines a dependency property for the command parameter.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register
            (
                nameof(CommandParameter), typeof(object), typeof(LinkedTextBlock), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the brush used for rollover.
        /// </summary>
        public Brush RolloverBrush
        {
            get => (Brush)GetValue(RolloverBrushProperty);
            set => SetValue(RolloverBrushProperty, value);
        }

        /// <summary>
        /// Defines a dependency property for the rollover brush
        /// </summary>
        public static readonly DependencyProperty RolloverBrushProperty = DependencyProperty.Register
            (
                nameof(RolloverBrush), typeof(Brush), typeof(LinkedTextBlock), new PropertyMetadata(null)
            );
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedTextBlock"/> class.
        /// </summary>
        public LinkedTextBlock()
        {
            RolloverBrush = new SolidColorBrush(Colors.DarkRed);
            Cursor = Cursors.Hand;
            TextDecorations = System.Windows.TextDecorations.Underline;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Occurs when the mouse enters the control.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            originalForeground = Foreground;
            Foreground = RolloverBrush;
        }

        /// <summary>
        /// Occurs when the mouse leaves the control.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Foreground = originalForeground;
        }

        /// <summary>
        /// Occurs when a mouse button is released.
        /// </summary>
        /// <param name="e">The event args</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.ChangedButton == MouseButton.Left)
            {
                if (Command != null && Command.CanExecute(CommandParameter))
                {
                    Command.Execute(CommandParameter);
                }
            }
        }
        #endregion
    }


}