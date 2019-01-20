using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Controls.Core
{
    /// <summary>
    /// Represents a collection of <see cref="DrumKit"/> objects.
    /// </summary>
    public class DrumKitCollection : GenericList<DrumKit>
    {
        #region Public fields
        /// <summary>
        /// Gets the id for the standard (default) drum kit.
        /// </summary>
        public const string DrumKitStandardId = "1B5E7DEF-A74B-4669-98A3-F42B6702DC05";
        /// <summary>
        /// Gets the id for the cuban drum kit.
        /// </summary>
        public const string DrumKitCubanId = "873DC37B-6708-4181-8AFB-FFBC2B5A352F";
        /// <summary>
        /// Gets the id for the Tr-808 drum kit.
        /// </summary>
        public const string DrumKitTr808Id = "A1841D58-4172-40CE-AE56-0E136DCCD09D";
        #endregion
        
        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DrumKitCollection"/> class.
        /// </summary>
        public DrumKitCollection()
        {
            Initialize();
        }
        #endregion

        /************************************************************************/
        #region Public methods
        /// <summary>
        /// Gets a boolean value that indicates if the specified drum kit exists in the collection.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainsDrumKit(string id)
        {
            foreach (DrumKit kit in this)
            {
                if (kit.Id == id) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the specified drum kit. Throws if it doesn't exist.
        /// </summary>
        /// <param name="id">The id of the drum kit to get.</param>
        /// <returns>The drum kit.</returns>
        public DrumKit GetDrumKit(string id)
        {
            foreach (DrumKit kit in this)
            {
                if (kit.Id == id) return kit;
            }

            throw new ArgumentOutOfRangeException(nameof(id), "Drum pattern with the specified id does not exist");
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void Initialize()
        {
            Add(new DrumKit()
            {
                Name = "Standard",
                ResourcePath = "Resources.DrumKit.Default",
                Id = DrumKitStandardId
            });
            Add(new DrumKit()
            {
                Name = "Cuban",
                ResourcePath = "Resources.DrumKit.Cuban",
                Id = DrumKitCubanId

            });
            Add(new DrumKit()
            {
                Name = "TR-808",
                ResourcePath = "Resources.DrumKit.TR808",
                Id = DrumKitTr808Id
            });

            DoForAll((kit) =>
            {
                kit.LoadBuiltInInstruments();
            });
        }
        #endregion
    }
}
