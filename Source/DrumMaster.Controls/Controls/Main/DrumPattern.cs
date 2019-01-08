using System;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    public class DrumPattern : AudioControlBase
    {
        #region Private
        //private const string PartHostGrid = "PART_HostGrid";
        //private Grid hostGrid;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumPattern"/> class.
        /// </summary>
        internal DrumPattern(DrumPatternContainer owner)
        {
            //Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        static DrumPattern()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrumPattern), new FrameworkPropertyMetadata(typeof(DrumPattern)));
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the <see cref="DrumPatternContainer"/> that owns this instance.
        /// </summary>
        private DrumPatternContainer Owner
        {
            get;
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            throw new NotImplementedException();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string that describes this object</returns>
        public override string ToString()
        {
            return $"{nameof(DrumPattern)} {DisplayName}";
        }
        #endregion
    }
}
