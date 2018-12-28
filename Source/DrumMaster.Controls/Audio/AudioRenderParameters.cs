using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls.Audio
{
    /// <summary>
    /// Provides parms for rendering audio.
    /// </summary>
    public class AudioRenderParameters : IXElement, INotifyPropertyChanged
    {
        #region Private
        private int sampleRate;
        private int bitDepth;
        private int channels;
        private string fileName;
        private bool parmsInFileName;
        #endregion

        /************************************************************************/

        #region Defaults
        /// <summary>
        /// Provides static default values
        /// </summary>
        public static class Default
        {
            /// <summary>
            /// The default value for sample rate
            /// </summary>
            public const int SampleRate = 44100;
            /// <summary>
            /// The default value for bit depth.
            /// </summary>
            public const int BitDepth = 16;
            /// <summary>
            /// The default value for number of channels
            /// </summary>
            public const int Channels = 2;
            /// <summary>
            /// The default value for parms in file name.
            /// </summary>
            public const bool ParmsInFileName = false;
        }
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the list of supported sample rates.
        /// </summary>
        public List<int> SupportedSampleRate
        {
            get;
        }

        /// <summary>
        /// Gets the list of supported bit depths.
        /// </summary>
        public List<int> SupportedBitDepth
        {
            get;
        }

        /// <summary>
        /// Gets the list of supported channels.
        /// </summary>
        public List<int> SupportedChannels
        {
            get;
        }

        /// <summary>
        /// Gets or sets the sample rate
        /// </summary>
        public int SampleRate
        {
            get => sampleRate;
            set => SetProperty(ref sampleRate, value);
        }

        /// <summary>
        /// Gets or sets the bit depth.
        /// </summary>
        public int BitDepth
        {
            get => bitDepth;
            set => SetProperty(ref bitDepth, value);
        }

        /// <summary>
        /// Gets or sets the number of channels.
        /// </summary>
        public int Channels
        {
            get => channels;
            set => SetProperty(ref channels, value);
        }

        /// <summary>
        /// Gets or sets the file name for the rendered output.
        /// If you set this property to null or an empty string,
        /// it will assign itself a random file name in the temp directory.
        /// </summary>
        public string FileName
        {
            get => fileName;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    string file = $"{Guid.NewGuid()}.wav";
                    value = Path.Combine(Path.GetTempPath(), file);
                }
                SetProperty(ref fileName, value);
            }
        }

        /// <summary>
        /// Gets the name of the rendered file. If <see cref="ParmsInFileName"/> is true,
        /// this name contains the parms, ex: D:\Sounds\Rendered_44100_16_2.wav. Otherwise,
        /// this is the same as <see cref="FileName"/>.
        /// </summary>
        public string RenderFileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value that determines if the parms will be made part of the file name.
        /// Example: D:\Sounds\Rendered_44100_16_2.wav
        /// </summary>
        public bool ParmsInFileName
        {
            get => parmsInFileName;
            set => SetProperty(ref parmsInFileName, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if this instance has changed any of its values since it was created
        /// </summary>
        public bool IsChanged
        {
            get;
            internal set;
        }
        #endregion

        /************************************************************************/

        #region INotifyPropertyChanged
        /// <summary>
        /// Raised when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            SetRenderFileName();
            OnPropertyChanged(nameof(RenderFileName));
            IsChanged = true;
            return true;
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRenderParameters"/> class
        /// </summary>
        public AudioRenderParameters()
        {
            SupportedSampleRate = new List<int>();
            SupportedBitDepth = new List<int>();
            SupportedChannels = new List<int>();
            SupportedSampleRate.Add(44100);
            SupportedSampleRate.Add(48000);
            SupportedBitDepth.Add(16);
            SupportedBitDepth.Add(24);
            SupportedBitDepth.Add(32);
            SupportedChannels.Add(1);
            SupportedChannels.Add(2);
            SampleRate = Default.SampleRate;
            BitDepth = Default.BitDepth;
            Channels = Default.Channels;
            IsChanged = false;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Creates and returns an <see cref="AudioRenderParameters"/> object with default values.
        /// </summary>
        /// <returns>An <see cref="AudioRenderParameters"/> object with default values.</returns>
        public static AudioRenderParameters CreateDefault()
        {
            return new AudioRenderParameters()
            {
                // When setting FileName to null, it assigns itself a random file name in the temp directory
                FileName = null,
                IsChanged = false,
            };
        }

        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>A string that shows the properties.</returns>
        public override string ToString()
        {
            return $"Rate: {SampleRate} Bits: {BitDepth} Channels: {Channels} File: {FileName} Render: {RenderFileName}";
        }

        /// <summary>
        /// Gets a boolean value that indicates if <paramref name="obj"/> equals this instance.
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>true if equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }

            AudioRenderParameters p = (AudioRenderParameters)obj;

            return
                p.SampleRate == SampleRate &&
                p.BitDepth == BitDepth &&
                p.Channels == Channels &&
                p.FileName == FileName &&
                p.ParmsInFileName == ParmsInFileName;
        }

        /// <summary>
        /// Gets the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + SampleRate.GetHashCode();
                hash = hash * 23 + BitDepth.GetHashCode();
                hash = hash * 23 + Channels.GetHashCode();
                hash = hash * 23 + ParmsInFileName.GetHashCode();
                if (FileName != null)
                {
                    hash = hash * 23 + FileName.GetHashCode();
                }
                return hash;
            }
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
            var element = new XElement(nameof(AudioRenderParameters));
            element.Add(new XElement(nameof(SampleRate), SampleRate));
            element.Add(new XElement(nameof(BitDepth), BitDepth));
            element.Add(new XElement(nameof(Channels), Channels));
            element.Add(new XElement(nameof(FileName), FileName));
            element.Add(new XElement(nameof(ParmsInFileName), ParmsInFileName));
            return element;
        }

        public void RestoreFromXElement(XElement element)
        {
            IEnumerable<XElement> childList = from el in element.Elements() select el;

            foreach (XElement e in childList)
            {
                if (e.Name == nameof(SampleRate)) SampleRate = GetIntValue(e.Value, Default.SampleRate);
                if (e.Name == nameof(BitDepth)) BitDepth = GetIntValue(e.Value, Default.BitDepth);
                if (e.Name == nameof(Channels)) Channels = GetIntValue(e.Value, Default.Channels);
                if (e.Name == nameof(FileName)) FileName = e.Value;
                if (e.Name == nameof(ParmsInFileName)) ParmsInFileName = GetBoolValue(e.Value, Default.ParmsInFileName);
            }
            IsChanged = false;
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Validates all parms. Throws if any parameter is unsupported.
        /// </summary>
        internal void Validate()
        {
            if (!SupportedSampleRate.Contains(SampleRate))
            {
                ThrowInvalidParm("Sample rate", SupportedSampleRate);
            }

            if (!SupportedBitDepth.Contains(BitDepth))
            {
                ThrowInvalidParm("Bit depth", SupportedBitDepth);
            }

            if (!SupportedChannels.Contains(Channels))
            {
                ThrowInvalidParm("Channels", SupportedChannels);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void ThrowInvalidParm(string name, List<int> supportedValues)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Unsupported value for {name}. Valid values are:");
            foreach (int value in supportedValues)
            {
                sb.Append($" {value},");
            }
            sb.Remove(sb.Length - 1, 1);
            throw new ArgumentException(sb.ToString());
        }

        private void SetRenderFileName()
        {
            RenderFileName = FileName;
            if (!string.IsNullOrEmpty(FileName) && ParmsInFileName)
            {
                string dir = Path.GetDirectoryName(FileName);
                string file = Path.GetFileNameWithoutExtension(FileName);
                string ext = Path.GetExtension(FileName);
                file = $"{file}_{SampleRate}_{BitDepth}_{Channels}{ext}";
                RenderFileName = Path.Combine(dir, file);
            }
        }

        private int GetIntValue(string str, int defaultValue)
        {
            if (int.TryParse(str, out int result))
            {
                return result;
            }
            return defaultValue;
        }

        private bool GetBoolValue(string str, bool defaultValue)
        {
            if (bool.TryParse(str, out bool result))
            {
                return result;
            }
            return defaultValue;
        }
        #endregion
    }
}
