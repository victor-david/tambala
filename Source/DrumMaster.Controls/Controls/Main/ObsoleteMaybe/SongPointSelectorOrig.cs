using System;
using System.Diagnostics;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls.Obsolete
{
    ///// <summary>
    ///// Represents a single song box selector
    ///// </summary>
    //public class SongPointSelectorOrig : PatternSelectorOrig
    //{
    //    #region Private
    //    #endregion

    //    /************************************************************************/

    //    #region Constructors
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="SongPointSelectorOrig"/> class.
    //    /// </summary>
    //    internal SongPointSelectorOrig(DrumPatternSelector owner, SongDrumPatternSelectorType type) : base(type)
    //    {
    //        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
    //        //if (SelectorType == SongDrumPatternSelectorType.Standard)
    //        //{
    //        //    Commands.Add("ToggleSelect", new RelayCommand((p) => IsSelected = !IsSelected));
    //        //}
    //    }

    //    static SongPointSelectorOrig()
    //    {
    //        DefaultStyleKeyProperty.OverrideMetadata(typeof(SongPointSelectorOrig), new FrameworkPropertyMetadata(typeof(SongPointSelectorOrig)));
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Properties
    //    /// <summary>
    //    /// Gets the <see cref="DrumPatternSelector"/> that owns this instance.
    //    /// </summary>
    //    private DrumPatternSelector Owner
    //    {
    //        get;
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region IXElement
    //    /// <summary>
    //    /// Gets the XElement for this object.
    //    /// </summary>
    //    /// <returns>The XElement that describes the state of this object.</returns>
    //    public override XElement GetXElement()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    /// <summary>
    //    /// Restores the object from the specified XElement
    //    /// </summary>
    //    /// <param name="element">The element</param>
    //    public override void RestoreFromXElement(XElement element)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Protected methods
    //    /// <summary>
    //    /// Called when <see cref="PatternSelectorOrig.Position"/> changes.
    //    /// </summary>
    //    protected override void OnPositionChanged()
    //    {
    //        if (SelectorType == SongDrumPatternSelectorType.Header)
    //        {
    //            DisplayName = $"{Position}";
    //        }
    //    }

    //    ///// <summary>
    //    ///// Called when <see cref="ControlObject.IsSelected"/> changes.
    //    ///// </summary>
    //    //protected override void OnIsSelectedChanged()
    //    //{
    //    //    SetIsChanged();
    //    //    Debug.WriteLine($"Pattern {Owner.DisplayName} Position {Position}");
    //    //}
    //    #endregion
    //}
}
