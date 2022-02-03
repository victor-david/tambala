/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.Tambala.Controls.Core
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
        /// <summary>
        /// Gets the id for the Tribal drum kit.
        /// </summary>
        public const string DrumKitTribalId = "F8402489-D0B5-4448-A515-B63212B50CC2";
        /// <summary>
        /// Gets the id for the Piano drum kit.
        /// </summary>
        public const string DrumKitPianoId = "5CF87D0E-FFFB-4AFE-B7CA-D938FC82E092";
        /// <summary>
        /// Gets the id for the default drum kit (DrumKitStandardId)
        /// </summary>
        public const string DrumKitDefaultId = DrumKitStandardId;
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

        #region Public properties
        /// <summary>
        /// Gets the drum kit indexed by its id.
        /// </summary>
        /// <param name="id">The drum kit id.</param>
        /// <returns>The drum kit. Throws if no kit with the specified id.</returns>
        public DrumKit this[string id]
        {
            get
            {
                DrumKit kit = GetDrumKit(id);
                return kit ?? throw new ArgumentOutOfRangeException(nameof(id), "Drum kit with the specified id does not exist");
            }
        }

        /// <summary>
        /// Gets the maximum number of instruments allowed in a single drum kit.
        /// </summary>
        public int MaxInstrumentPerKit
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets a boolean value that indicates if the specified drum kit exists in the collection.
        /// </summary>
        /// <param name="id">The id of the drum kit to check.</param>
        /// <returns>true if the drum kit with the specified id exists; otherwise, false.</returns>
        public bool ContainsDrumKit(string id)
        {
            foreach (DrumKit kit in this)
            {
                if (kit.Id == id) return true;
            }
            return false;
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
            Add(new DrumKit()
            {
                Name = "Tribal",
                ResourcePath = "Resources.DrumKit.Tribal",
                Id = DrumKitTribalId
            });
            Add(new DrumKit()
            {
                Name = "Piano",
                ResourcePath = "Resources.DrumKit.Piano",
                Id = DrumKitPianoId
            });

            DoForAll((kit) =>
            {
                kit.LoadBuiltInInstruments();
            });

            MaxInstrumentPerKit = this[DrumKitDefaultId].Instruments.Count;
        }

        /// <summary>
        /// Gets the specified drum kit.
        /// </summary>
        /// <param name="id">The id of the drum kit to get.</param>
        /// <returns>The drum kit, or null if it doesn't exist.</returns>
        private DrumKit GetDrumKit(string id)
        {
            foreach (DrumKit kit in this)
            {
                if (kit.Id == id) return kit;
            }
            return null;
        }
        #endregion
    }
}