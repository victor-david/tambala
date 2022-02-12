/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tambala.Controls.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace Restless.Tambala.Controls.Audio
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
        private int fadeTime;
        private int fadeSamples;
        private int passCount;
        private string fileName;
        private bool parmsInFileName;
        #endregion

        /************************************************************************/

        #region Values
        public static class Values
        {
            public static class SampleRate
            {
                public const int Rate44100 = 44100;
                public const int Rate48000 = 48000;
                public const int Default = Rate44100;
            }

            public static class BitDepth
            {
                public const int Depth16 = 16;
                public const int Depth24 = 24;
                public const int Depth32 = 32;
                public const int Default = Depth16;
            }

            public static class Channel
            {
                public const int Channel1 = 1;
                public const int Channel2 = 2;
                public const int Default = Channel2;
            }

            public static class FadeTime
            {
                public const double Minimum = 0;
                public const double Maximum = 2000;
                public const int Default = 0;
            }

            public static class PassCount
            {
                public const double Minimum = 1;
                public const double Maximum = 4;
                public const int Default = 1;
            }

            public static class ParmsInFileName
            {
                public const bool Default = false;
            }

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
        /// Gets or sets the number milliseconds to fade. Clamped between 0 and 2000
        /// </summary>
        public int FadeTime
        {
            get => fadeTime;
            set => SetProperty(ref fadeTime, Math.Min(Math.Max(value, (int)Values.FadeTime.Minimum), (int)Values.FadeTime.Maximum));
        }

        /// <summary>
        /// Gets the number of fade samples.
        /// </summary>
        public int FadeSamples
        {
            get => fadeSamples;
            private set
            {
                SetProperty(ref fadeSamples, value);
                OnPropertyChanged(nameof(FadeText));
            }
        }

        public string FadeText
        {
            get => $"{FadeTime} / {FadeSamples}";
        }

        /// <summary>
        /// Gets or sets the number of passes for rendering.
        /// </summary>
        public int PassCount
        {
            get => passCount;
            set => SetProperty(ref passCount, Math.Min(Math.Max(value, (int)Values.PassCount.Minimum), (int)Values.PassCount.Maximum));
        }

        /// <summary>
        /// Gets the number of frames to capture
        /// </summary>
        public int FramesToCapture
        {
            get;
            private set;
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
            private set;
        }
        #endregion

        /************************************************************************/

        #region Changed event (internal)
        /// <summary>
        /// Occurs when any parameter is changed.
        /// </summary>
        internal event EventHandler<bool> Changed;
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
            if (propertyName == nameof(FadeTime))
            {
                SetFadeSamples();
            }
            IsChanged = true;
            Changed?.Invoke(this, IsChanged);
            return true;
        }

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
            SupportedSampleRate = new List<int>()
            {
                Values.SampleRate.Rate44100,
                Values.SampleRate.Rate48000
            };

            SupportedBitDepth = new List<int>()
            {
                Values.BitDepth.Depth16,
                Values.BitDepth.Depth24,
                Values.BitDepth.Depth32,
            };

            SupportedChannels = new List<int>()
            {
                Values.Channel.Channel1,
                Values.Channel.Channel2
            };

            SampleRate = Values.SampleRate.Default;
            BitDepth = Values.BitDepth.Default;
            Channels = Values.Channel.Default;
            FadeTime = Values.FadeTime.Default;
            PassCount = Values.PassCount.Default;
            ParmsInFileName = Values.ParmsInFileName.Default;
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
            return $"Rate: {SampleRate} Bits: {BitDepth} Channels: {Channels} Fade: {FadeTime} File: {FileName} Render: {RenderFileName}";
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
                p.FadeTime == FadeTime &&
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
                hash = hash * 23 + SampleRate.GetHashCode();
                hash = hash * 23 + BitDepth.GetHashCode();
                hash = hash * 23 + Channels.GetHashCode();
                hash = hash * 23 + FadeTime.GetHashCode();
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
            element.Add(new XElement(nameof(FadeTime), FadeTime));
            element.Add(new XElement(nameof(PassCount), PassCount));
            element.Add(new XElement(nameof(FileName), FileName));
            element.Add(new XElement(nameof(ParmsInFileName), ParmsInFileName));
            return element;
        }

        /// <summary>
        /// Restores the state of this object from the specified element
        /// </summary>
        /// <param name="element">The element</param>
        public void RestoreFromXElement(XElement element)
        {
            IEnumerable<XElement> childList = from el in element.Elements() select el;

            foreach (XElement e in childList)
            {
                if (e.Name == nameof(SampleRate)) SampleRate = GetIntValue(e.Value, Values.SampleRate.Default);
                if (e.Name == nameof(BitDepth)) BitDepth = GetIntValue(e.Value, Values.BitDepth.Default);
                if (e.Name == nameof(Channels)) Channels = GetIntValue(e.Value, Values.Channel.Default);
                if (e.Name == nameof(FadeTime)) FadeTime = GetIntValue(e.Value, Values.FadeTime.Default);
                if (e.Name == nameof(PassCount)) PassCount = GetIntValue(e.Value, Values.PassCount.Default);
                if (e.Name == nameof(FileName)) FileName = e.Value;
                if (e.Name == nameof(ParmsInFileName)) ParmsInFileName = GetBoolValue(e.Value, Values.ParmsInFileName.Default);
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

        internal void CalculateFramesToCapture(double tempo, int quarterNoteCount)
        {
            int delay = Ticks.GetTickDelayFromTempo(tempo);
            int msTotal = delay * Ticks.LowestCommon * quarterNoteCount;
            FramesToCapture = (int)(SampleRate * (msTotal / 1000d) * PassCount);
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

        private void SetFadeSamples()
        {
            // Fade samples must always be an even number.
            double fadeSamples = SampleRate * (FadeTime / 1000.0) * Channels;
            int fadeInt = (int)fadeSamples;
            if (fadeInt % 2 != 0) fadeInt++;
            FadeSamples = fadeInt;
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