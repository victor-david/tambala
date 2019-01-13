using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a controll for a single instrument of a drum pattern.
    /// </summary>
    public class InstrumentController : AudioControlBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentController"/> class.
        /// </summary>
        internal InstrumentController()
        {
            ExpandedImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Caret.Up.Blue.32.png", UriKind.Relative));
            CollapsedImageSource = new BitmapImage(new Uri("/DrumMaster.Controls;component/Resources/Images/Image.Caret.Down.Blue.32.png", UriKind.Relative));
        }

        static InstrumentController()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InstrumentController), new FrameworkPropertyMetadata(typeof(InstrumentController)));
        }
        #endregion

        /************************************************************************/

        #region IXElement
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public override XElement GetXElement()
        {
            var element = new XElement(nameof(InstrumentController));
            element.Add(new XElement(nameof(Volume), Volume));
            element.Add(new XElement(nameof(Panning), Panning));
            element.Add(new XElement(nameof(Pitch), Pitch));
            element.Add(new XElement(nameof(IsMuted), IsMuted));
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
        }
        #endregion
    }
}
