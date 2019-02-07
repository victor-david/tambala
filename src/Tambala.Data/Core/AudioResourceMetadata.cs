/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.IO;
using System.Reflection;
using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;


namespace Restless.App.Tambala.Data.Core
{
    /// <summary>
    /// Represents the meta data taken from an application audio resource.
    /// </summary>
    public class AudioResourceMetadata
    {
        #region Public properties
        /// <summary>
        /// Gets the name of the audio resource.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the album name of the audio resource.
        /// </summary>
        public string Album
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the comment of the audio resource.
        /// </summary>
        public string Comment
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the track number of the audio resource.
        /// </summary>
        public string TrackNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the exception that occured while extracting the meta data of the audio resource, or null if none.
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }
        #endregion
        
        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioResourceMetadata"/> class.
        /// </summary>
        /// <param name="assembly">The assembly that contains the audio resource.</param>
        /// <param name="resourceName">The name of the audio resource</param>
        public AudioResourceMetadata(Assembly assembly, string resourceName)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException(nameof(resourceName));
            GetMetadata(assembly, resourceName);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void GetMetadata(Assembly assembly, string resourceName)
        {
            string ext = Path.GetExtension(resourceName);
            string tempName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()) + ext;

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, (int)stream.Length);
                    File.WriteAllBytes(tempName, buffer);
                }
                
                using (ShellObject shellObj = ShellObject.FromParsingName(tempName))
                {
                    Name = GetValue(shellObj, SysProps.System.Title); 
                    Album = GetValue(shellObj, SysProps.System.Music.AlbumTitle);
                    Comment = GetValue(shellObj, SysProps.System.Comment);
                    TrackNumber = GetValue(shellObj, SysProps.System.Music.TrackNumber);
                }

                //var fileAbstraction = new AudioResourceAbstraction(assembly, resourceName);
                //using (var file = TagLib.File.Create(fileAbstraction))
                //{
                //    Name = file.Tag.Title;
                //    Album = file.Tag.Album;
                //    Comment = file.Tag.Comment;
                //}

                //var testFile = TagLib.File.Create(@"F:\temp\01.hihat.wav");
                //if (String.IsNullOrEmpty(testFile.Tag.Album))
                //{
                //    Debug.WriteLine("No album tag");
                //    testFile.Tag.Album = "MY NEW ALBUM";
                //    testFile.Save();
                //}
                //else
                //{
                //    Debug.WriteLine($"Album is: {testFile.Tag.Album}");
                //}

            }
            catch (Exception ex)
            {
                Exception = ex;
            }
            finally
            {
                File.Delete(tempName);
            }

        }

        private string GetValue(ShellObject shellObj, PropertyKey key)
        {
            if (shellObj != null)
            {
                var prop = shellObj.Properties.GetProperty(key);
                if (prop != null && prop.ValueAsObject != null)
                {
                    return prop.ValueAsObject.ToString();
                }
            }
            return string.Empty;
        }
        #endregion
    }
}