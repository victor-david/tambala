using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.Tambala.Controls.Core
{
    /// <summary>
    /// Defines properties for controls that implement volume bias
    /// </summary>
    internal interface IVolume
    {
        /// <summary>
        /// Gets or sets the volume. This value is expressed in decibels.
        /// </summary>
        float Volume { get; set; }
    }
}
