using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    ///// <summary>
    ///// Represents a control that presents and manages the instruments (snare, cymbal, etc.)
    ///// that comprise a drum kit and provides the ability to select which instruments play along
    ///// a timeline.
    ///// </summary>
    //public class DrumPatternContainer : ControlObject
    //{
    //    #region Private
    //    #endregion

    //    /************************************************************************/

    //    #region Constructors
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="DrumPatternContainer"/> class.
    //    /// </summary>
    //    internal DrumPatternContainer(ProjectContainer owner)
    //    {
    //        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
    //    }

    //    static DrumPatternContainer()
    //    {
    //        DefaultStyleKeyProperty.OverrideMetadata(typeof(DrumPatternContainer), new FrameworkPropertyMetadata(typeof(DrumPatternContainer)));
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Properties
    //    /// <summary>
    //    /// Gets the <see cref="ProjectContainer"/> that owns this instance.
    //    /// </summary>
    //    internal ProjectContainer Owner
    //    {
    //        get;
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region ActivePattern
    //    /// <summary>
    //    /// Gets the active <see cref="DrumPattern"/> object.
    //    /// </summary>
    //    public DrumPattern ActivePattern
    //    {
    //        get => (DrumPattern)GetValue(ActivePatternProperty);
    //        private set => SetValue(ActivePatternPropertyKey, value);
    //    }

    //    private static readonly DependencyPropertyKey ActivePatternPropertyKey = DependencyProperty.RegisterReadOnly
    //        (
    //            nameof(ActivePattern), typeof(DrumPattern), typeof(DrumPatternContainer), new PropertyMetadata(null)
    //        );

    //    /// <summary>
    //    /// Identifies the <see cref="ActivePattern"/> dependency property.
    //    /// </summary>
    //    public static readonly DependencyProperty ActivePatternProperty = ActivePatternPropertyKey.DependencyProperty;
    //    #endregion

    //    /************************************************************************/

    //    #region IXElement
    //    /// <summary>
    //    /// Gets the XElement for this object.
    //    /// </summary>
    //    /// <returns>The XElement that describes the state of this object.</returns>
    //    public override XElement GetXElement()
    //    {
    //        var element = new XElement(nameof(DrumPatternContainer));
    //        return element;
    //    }

    //    /// <summary>
    //    /// Restores the object from the specified XElement
    //    /// </summary>
    //    /// <param name="element">The element</param>
    //    public override void RestoreFromXElement(XElement element)
    //    {
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Internal methods
    //    /// <summary>
    //    /// Activate the specified drum pattern
    //    /// </summary>
    //    internal void ActivateDrumPattern(DrumPattern pattern)
    //    {
    //        ActivePattern = pattern;
    //    }
    //    #endregion
    //}
}
