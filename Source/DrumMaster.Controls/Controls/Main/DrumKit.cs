using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Reflection;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a drum kit, a collection of playable instuments
    /// </summary>
    public class DrumKit : IXElement
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumKit"/> class.
        /// </summary>
        public DrumKit()
        {
            Name = "Standard";
            ResourcePath = "Resources.DrumKit.Default";
            Instruments = new InstrumentCollection();
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets the name of this drum kit.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or (from this assembly) sets the id of this drum kit
        /// </summary>
        public string ResourcePath
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the collection of instruments for this drum kit.
        /// </summary>
        public InstrumentCollection Instruments
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
        public XElement GetXElement()
        {
            var element = new XElement(nameof(DrumKit));
            element.Add(new XElement(nameof(Name), Name));
            element.Add(new XElement(nameof(ResourcePath), ResourcePath));
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public void RestoreFromXElement(XElement element)
        {
        }
        #endregion

        /************************************************************************/

        #region Methods
        /// <summary>
        /// Loads instruments from the specified directory.
        /// </summary>
        /// <param name="directory">The directory</param>
        public void LoadExternalInstuments(string directory)
        {
            if (string.IsNullOrEmpty(directory)) throw new ArgumentNullException(nameof(directory));
            Instruments.LoadFromDirectory(directory);
        }

        /// <summary>
        /// Loads built in instuments according to <see cref="ResourcePath"/> of the drum kit
        /// </summary>
        internal void LoadBuiltInInstruments()
        {
            if (string.IsNullOrEmpty(ResourcePath)) throw new InvalidOperationException($"{nameof(ResourcePath)} must be set before calling this method");
            Assembly assembly = Assembly.GetAssembly(typeof(Data.Core.AudioResourceMetadata));
            Instruments.LoadFromAssembly(assembly, ResourcePath);
        }
        #endregion
    }
}
