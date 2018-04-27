using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Restless.App.DrumMaster.Controls
{
    ///// <summary>
    ///// Represents a step box 
    ///// </summary>
    //[TemplatePart(Name = PartButtonToggle, Type = typeof(ButtonBase))]
    //public class TrackStepBoxControl : TrackBox
    //{
    //    #region Private
    //    private const string PartButtonToggle = "PART_BTN_TOGGLE";
    //    private ButtonBase buttonToggle;
    //    #endregion

    //    /************************************************************************/

    //    #region Public properties
    //    #endregion

    //    /************************************************************************/

    //    #region Protected properties
    //    #endregion

    //    /************************************************************************/

    //    #region Constructors 
    //    /// <summary>
    //    /// Creates a new instance of <see cref="TrackStepBoxControl"/>
    //    /// </summary>
    //    /// <param name="owner">The owner of this box.</param>
    //    internal TrackStepBoxControl(TrackControlBase owner) : base (owner)
    //    {
    //    }

    //    static TrackStepBoxControl()
    //    {
    //        DefaultStyleKeyProperty.OverrideMetadata(typeof(TrackStepBoxControl), new FrameworkPropertyMetadata(typeof(TrackStepBoxControl)));
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Public methods
    //    /// <summary>
    //    /// Called when the template is applied.
    //    /// </summary>
    //    public override void OnApplyTemplate()
    //    {
    //        base.OnApplyTemplate();
    //        if (buttonToggle != null) buttonToggle.Click -= ButtonToggleClick;
    //        buttonToggle = GetTemplateChild(PartButtonToggle) as ButtonBase;
    //        if (buttonToggle != null) buttonToggle.Click += ButtonToggleClick;
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Private methods (Instance)
    //    private void ButtonToggleClick(object sender, RoutedEventArgs e)
    //    {
    //        IsSelected = !IsSelected;
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Private methods (Static)
    //    #endregion
    //}
}
