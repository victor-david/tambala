using System;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    public class MasterControl : ControlBase
    {
        #region Private
        private SongContainer owner;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterControl"/> class.
        /// </summary>
        internal MasterControl(SongContainer owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        static MasterControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterControl), new FrameworkPropertyMetadata(typeof(MasterControl)));
        }
        #endregion

        /************************************************************************/

        #region IXElement
        public override XElement GetXElement()
        {
            throw new NotImplementedException();
        }

        public override void RestoreFromXElement(XElement element)
        {
        }
        #endregion
    }
}
