using System.Windows;
using System.Windows.Controls;

namespace Restless.App.DrumMaster.Controls
{
    public class VolumeMeter : ProgressBar
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeMeter"/> class.
        /// </summary>
        public VolumeMeter()
        {
        }

        static VolumeMeter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VolumeMeter), new FrameworkPropertyMetadata(typeof(VolumeMeter)));
        }
        #endregion
    }
}
