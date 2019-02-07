/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Tambala.Controls.Core;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Restless.App.Tambala.Controls
{
    /// <summary>
    /// Extends Border to provide a tick mark for a <see cref="DrumPatternQuarter"/>
    /// </summary>
    internal class VisualTick : VisualBorder, ISelectorUnit
    {
        #region Private
        private readonly Brush originalBrush;
        private double originalWidth;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualTick"/> class.
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="selectorUnit">The selector unit.</param>
        internal VisualTick(int position, PointSelectorUnit selectorUnit) : base(position)
        {
            SelectorUnit = selectorUnit;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Bottom;
            Visibility = Visibility.Collapsed;

            switch (SelectorUnit)
            {
                case PointSelectorUnit.QuarterNote:
                    Visibility = Visibility.Visible;
                    Background = originalBrush = Brushes.DarkBlue;
                    Height = 14.0;
                    Width = originalWidth = 2.0;
                    break;
                case PointSelectorUnit.EighthNote:
                    Background = originalBrush = Brushes.Black;
                    Height = 12.0;
                    Width = originalWidth = 1.0;
                    break;
                default:
                    Background = originalBrush = Brushes.DarkGray;
                    Height = 10.0;
                    Width = originalWidth = 1.0;
                    break;
            }
        }
        #endregion

        /************************************************************************/

        #region ISelectorUnit
        /// <summary>
        /// Gets or sets the selector unit.
        /// </summary>
        public PointSelectorUnit SelectorUnit
        {
            get;
            set;
        }
        #endregion


        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Calls Dispatcher.BeginInvoke to add highlight to the tick.
        /// </summary>
        public void InvokeAddTickHighlight()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                {
                    Background = Brushes.Red;
                    Width = Width * 3.0;
                }));
        }

        /// <summary>
        /// Calls Dispatcher.BeginInvoke to remove highlight from the tick.
        /// </summary>
        public void InvokeRemoveTickHighlight()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(()=>
                {
                    Background = originalBrush;
                    Width = originalWidth;
                }));
        }
        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>A string that describes this object.</returns>
        public override string ToString()
        {
            return $"{nameof(VisualTick)} Unit:{SelectorUnit} Position:{Position}";
        }
        #endregion
    }
}