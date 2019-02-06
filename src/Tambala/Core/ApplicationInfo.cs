/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the MIT License
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.IO;
using System.Reflection;

namespace Restless.App.Tambala.Core
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
            get
            {
                object[] attributes = Assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.CodeBase);
            }
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
                return string.Format("{0}.{1}", version.Major, version.Minor);
            }
        }

        /// <summary>
        /// Gets the framework version of the assembly.
        /// </summary>
        public string FrameworkVersion
        {
            get => Assembly.ImageRuntimeVersion;
        }

        /// <summary>
        /// Gets the description of the assembly.
        /// </summary>
        public string Description
        {
            get
            {
                object[] attributes = Assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        /// <summary>
        /// Gets the product name of the assembly.
        /// </summary>
        public string Product
        {
            get
            {
                object[] attributes = Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        /// <summary>
        /// Gets the copyright of the assembly.
        /// </summary>
        public string Copyright
        {
            get
            {
                object[] attributes = Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        /// <summary>
        /// Gets the company of the assembly.
        /// </summary>
        public string Company
        {
            get
            {
                object[] attributes = Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        /// <summary>
        /// Gets the build date for the assembly.
        /// </summary>
        public DateTime BuildDate
        {
            get
            {
                var version = VersionRaw;
                DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
                return buildDate;
            }
        }

        /// <summary>
        /// Gets the root folder for the application.
        /// </summary>
        public string RootFolder
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the reference help file.
        /// </summary>
        public string ReferenceFileName
        {
            get => Path.Combine(RootFolder, "DrumMaster.Reference.chm");
        }

        /// <summary>
        /// Gets a boolean value that indicates if the current process is a 64 bit process.
        /// </summary>
        public bool Is64Bit
        {
            get => Environment.Is64BitProcess;
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
            RootFolder = Path.GetDirectoryName(Assembly.Location);
        }

        static ApplicationInfo()
        {
        }
        #endregion
    }
}