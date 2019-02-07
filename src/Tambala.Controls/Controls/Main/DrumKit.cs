/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Tambala.Controls.Audio;
using Restless.App.Tambala.Controls.Core;
using System;
using System.Reflection;
using System.Xml.Linq;

namespace Restless.App.Tambala.Controls
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
        /// <param name="resourcePath">The resource path to the drum kit.</param>
        public DrumKit(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath)) throw new ArgumentNullException(nameof(resourcePath));
            ResourcePath = resourcePath;
            DrumKitType = DrumKitType.UserDefined;
            Instruments = new InstrumentCollection();
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrumKit"/> class.
        /// </summary>
        internal DrumKit()
        {
            DrumKitType = DrumKitType.BuiltIn;
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
        /// Get the type of drum kit.
        /// </summary>
        public DrumKitType DrumKitType
        {
            get;
        }

        /// <summary>
        /// Gets the unique id for this drum kit.
        /// </summary>
        public string Id
        {
            get;
            internal set;
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
            element.Add(new XElement(nameof(Id), Id));
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

        /// <summary>
        /// Gets a string display of this object.
        /// </summary>
        /// <returns>A string with <see cref="Name"/> and <see cref="DrumKitType"/></returns>
        public override string ToString()
        {
            return $"{Name} ({DrumKitType})";
        }
        #endregion
    }
}