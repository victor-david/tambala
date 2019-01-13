using Restless.App.DrumMaster.Controls.Audio;
using System;
using System.Reflection;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents a drum kit, a collection of playable instuments
    /// </summary>
    public class DrumKit
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
