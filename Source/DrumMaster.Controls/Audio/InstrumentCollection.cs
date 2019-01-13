using System;
using System.Collections.Generic;
using System.Reflection;

namespace Restless.App.DrumMaster.Controls.Audio
{
    /// <summary>
    /// Represents a collection of <see cref="Instrument"/> objects.
    /// </summary>
    public class InstrumentCollection : List<Instrument>
    {
        #region Internal methods

        internal void LoadFromDirectory(string directory)
        {
            // TODO
            throw new NotImplementedException();
        }

        internal void LoadFromAssembly(Assembly assembly, string resourcePath)
        {
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.Contains(resourcePath))
                {
                    Add(new Instrument(resourceName, assembly));
                }
            }
        }
        #endregion
    }
}
