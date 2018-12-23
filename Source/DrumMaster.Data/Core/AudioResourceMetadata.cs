using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;


namespace Restless.App.DrumMaster.Data.Core
{
    /// <summary>
    /// Represents the meta data taken from an application audio resource.
    /// </summary>
    public class AudioResourceMetadata
    {
        public string Name
        {
            get;
            private set;
        }

        public string Album
        {
            get;
            private set;
        }

        public string Comment
        {
            get;
            private set;
        }

        public string TrackNumber
        {
            get;
            private set;
        }

        public Exception Exception
        {
            get;
            private set;
        }

        public AudioResourceMetadata(Assembly assembly, string resourceName)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException(nameof(resourceName));
            GetMetadata(assembly, resourceName);
        }

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
    }
}
