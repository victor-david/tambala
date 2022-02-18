/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Reflection;

namespace Restless.Tambala.Core
{
    /// <summary>
    /// A singleton class that provides information about the application.
    /// </summary>
    public sealed class ApplicationInfo
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the assembly object.
        /// </summary>
        public Assembly Assembly
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the title of the assembly.
        /// </summary>
        public string Title
        {
            get => GetAssemblyAttribute<AssemblyTitleAttribute>()?.Title;
        }

        /// <summary>
        /// Gets the raw version property for the assmebly.
        /// </summary>
        public Version VersionRaw
        {
            get => Assembly.GetName().Version;
        }

        /// <summary>
        /// Gets the full version of the assembly as a string.
        /// </summary>
        public string Version
        {
            get => VersionRaw.ToString();
        }

        /// <summary>
        /// Gets the major version of the assembly.
        /// </summary>
        public string VersionMajor
        {
            get
            {
                var version = Assembly.GetName().Version;
                return $"{version.Major}.{version.Minor}";
            }
        }

        /// <summary>
        /// Gets the framework version of the assembly.
        /// </summary>
        public string FrameworkVersion
        {
            get => GetAssemblyAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>()?.FrameworkName;
        }

        /// <summary>
        /// Gets the description of the assembly.
        /// </summary>
        public string Description
        {
            get => GetAssemblyAttribute<AssemblyDescriptionAttribute>()?.Description;
        }

        /// <summary>
        /// Gets the product name of the assembly.
        /// </summary>
        public string Product
        {
            get => GetAssemblyAttribute<AssemblyProductAttribute>()?.Product;
        }

        /// <summary>
        /// Gets the copyright of the assembly.
        /// </summary>
        public string Copyright
        {
            get => GetAssemblyAttribute<AssemblyCopyrightAttribute>()?.Copyright;
        }

        /// <summary>
        /// Gets the company of the assembly.
        /// </summary>
        public string Company
        {
            get => GetAssemblyAttribute<AssemblyCompanyAttribute>()?.Company;
        }

        /// <summary>
        /// Gets the repository url.
        /// </summary>
        public string RepositoryUrl
        {
            get
            {
                var attrib = GetAssemblyAttribute<AssemblyMetadataAttribute>();
                if (attrib != null && attrib.Key == nameof(RepositoryUrl))
                {
                    return attrib.Value;
                }
                return null;
            }
        }
        #endregion

        /************************************************************************/

        #region Singleton access and constructors
        /// <summary>
        /// Gets the singleton instance of this class.
        /// </summary>
        public static ApplicationInfo Instance { get; } = new ApplicationInfo();

        private ApplicationInfo()
        {
            Assembly = Assembly.GetEntryAssembly();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private T GetAssemblyAttribute<T>() where T: Attribute
        {
            object[] attributes = Assembly.GetCustomAttributes(typeof(T), false);
            if (attributes.Length > 0)
            {
                return attributes[0] as T;
            }
            return null;
        }
        #endregion
    }
}