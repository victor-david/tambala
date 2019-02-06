using System.Xml.Linq;

namespace Restless.App.Tambala.Controls.Core
{
    /// <summary>
    /// Defines methods for controls that can save and restore their state using XElements.
    /// </summary>
    interface IXElement
    {
        /// <summary>
        /// Gets the XElement for an object object.
        /// </summary>
        /// <returns>The XElement that describes the state of the object.</returns>
        XElement GetXElement();

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        void RestoreFromXElement(XElement element);
    }
}
