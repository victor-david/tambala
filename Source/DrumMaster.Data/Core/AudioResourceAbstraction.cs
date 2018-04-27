using System;
using System.IO;
using System.Reflection;

namespace Restless.App.DrumMaster.Data.Core
{
    /// <summary>
    /// Represents a read only tag lib file abstraction for an embedded resource.
    /// </summary>
    //internal class AudioResourceAbstraction : TagLib.File.IFileAbstraction
    //{
    //    #region Private
    //    private Stream resourceStream;
    //    #endregion

    //    /************************************************************************/

    //    #region Public properties
    //    /// <summary>
    //    /// Gets the resource name
    //    /// </summary>
    //    public string Name
    //    {
    //        get;
    //    }

    //    /// <summary>
    //    /// Gets the read stream
    //    /// </summary>
    //    public Stream ReadStream
    //    {
    //        get => resourceStream;
    //    }

    //    /// <summary>
    //    /// This abstraction does not support writing. 
    //    /// Always throws a NotSupportedException.
    //    /// </summary>
    //    public Stream WriteStream
    //    {
    //        get => throw new NotSupportedException();
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Constructor
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="AudioResourceAbstraction"/> class.
    //    /// </summary>
    //    /// <param name="assembly">The assembly</param>
    //    /// <param name="resourceName">The resource name</param>
    //    public AudioResourceAbstraction(Assembly assembly, string resourceName)
    //    {
    //        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
    //        if (String.IsNullOrEmpty(resourceName)) throw new ArgumentNullException(nameof(resourceName));
    //        Name = resourceName;
    //        resourceStream = assembly.GetManifestResourceStream(resourceName);
    //    }
    //    #endregion


    //    /************************************************************************/
    //    #region Public methods
    //    /// <summary>
    //    /// Closes the stream
    //    /// </summary>
    //    /// <param name="stream">The stream</param>
    //    public void CloseStream(Stream stream)
    //    {
    //        if (stream != null)
    //        {
    //            stream.Close();
    //        }
    //    }
    //    #endregion
    //}
}
