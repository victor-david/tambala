﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Represents the top most container for a song and its associated series of patterns.
    /// </summary>
    public class SongContainer : DependencyControlObject
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SongContainer"/> class.
        /// </summary>
        public SongContainer()
        {
            MasterPlay = new MasterPlay(this);
            MasterOutput = new MasterOutput(this);
            SongPatterns = new SongPatternsContainer(this);
            AddHandler(IsChangedSetEvent, new RoutedEventHandler(IsChangedSetEventHandler));
            AddHandler(IsChangedResetEvent, new RoutedEventHandler(IsChangedResetEventHandler));
        }

        static SongContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SongContainer), new FrameworkPropertyMetadata(typeof(SongContainer)));
        }
        #endregion

        /************************************************************************/

        #region MasterOutput
        /// <summary>
        /// Gets the master output control for the container.
        /// </summary>
        public MasterOutput MasterOutput
        {
            get => (MasterOutput)GetValue(MasterOutputProperty);
            private set => SetValue(MasterOutputPropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey MasterOutputPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(MasterOutput), typeof(MasterOutput), typeof(SongContainer), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="MasterOutput"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MasterOutputProperty = MasterOutputPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region MasterPlay
        /// <summary>
        /// Gets the master play control for the container.
        /// </summary>
        public MasterPlay MasterPlay
        {
            get => (MasterPlay)GetValue(MasterPlayProperty);
            private set => SetValue(MasterPlayPropertyKey, value);
        }

        private static readonly DependencyPropertyKey MasterPlayPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(MasterPlay), typeof(MasterPlay), typeof(SongContainer), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="MasterPlay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MasterPlayProperty = MasterPlayPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region SongPatterns
        /// <summary>
        /// Gets the pattern collection. Used to create the song.
        /// </summary>
        public SongPatternsContainer SongPatterns
        {
            get => (SongPatternsContainer)GetValue(SongPatternsProperty);
            set => SetValue(SongPatternsPropertyKey, value);
        }
        
        private static readonly DependencyPropertyKey SongPatternsPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(SongPatterns), typeof(SongPatternsContainer), typeof(SongContainer), new PropertyMetadata(null)
            );

        /// <summary>
        /// Identifies the <see cref="SongPatterns"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SongPatternsProperty = SongPatternsPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region FileName
        /// <summary>
        /// Gets the current file name for the container, or null if none.
        /// This is the .xml file
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region IXElement
        /// <summary>
        /// Gets the XElement for this object.
        /// </summary>
        /// <returns>The XElement that describes the state of this object.</returns>
        public override XElement GetXElement()
        {
            var element = new XElement(nameof(SongContainer));
            element.Add(new XElement(nameof(DisplayName), DisplayName));
            //element.Add(new XElement(nameof(Volume), Volume));
            //element.Add(new XElement(nameof(Tempo), Tempo));
            //element.Add(new XElement(nameof(Beats), Beats));
            //element.Add(new XElement(nameof(StepsPerBeat), StepsPerBeat));
            //element.Add(new XElement(nameof(BoxSize), BoxSize));
            //element.Add(RenderParms.GetXElement());

            //Tracks.DoForAll((track) =>
            //{
            //    element.Add(track.GetXElement());
            //});
            return element;
        }

        /// <summary>
        /// Restores the object from the specified XElement
        /// </summary>
        /// <param name="element">The element</param>
        public override void RestoreFromXElement(XElement element)
        {
            IEnumerable<XElement> childList = from el in element.Elements() select el;

            foreach (XElement e in childList)
            {
                if (e.Name == nameof(DisplayName)) SetDependencyProperty(DisplayNameProperty, e.Value);
                //if (e.Name == nameof(Volume)) SetDependencyProperty(VolumeProperty, e.Value);
                //if (e.Name == nameof(Tempo)) SetDependencyProperty(TempoProperty, e.Value);
                //if (e.Name == nameof(Beats)) SetDependencyProperty(BeatsProperty, e.Value);
                //if (e.Name == nameof(StepsPerBeat)) SetDependencyProperty(StepsPerBeatProperty, e.Value);
                //if (e.Name == nameof(BoxSize)) SetDependencyProperty(BoxSizeProperty, e.Value);
                //if (e.Name == nameof(AudioRenderParameters))
                //{
                //    RenderParms.RestoreFromXElement(e);
                //    AudioHost.Instance.AudioCapture.RenderParms = RenderParms;
                //}

                // For backward compatibility
                //if (e.Name == nameof(TrackController))
                //{
                //    AddTrack(null);
                //    OnBoxSizeChanged();
                //    OnTotalStepsChanged();
                //    Tracks[Tracks.Count - 1].Controller.RestoreFromXElement(e);
                //    IEnumerable<XElement> subChildList = from subE in e.Elements() select subE;
                //    foreach (XElement ee in subChildList)
                //    {
                //        if (ee.Name == "TrackBoxContainer")
                //        {
                //            Tracks[Tracks.Count - 1].BoxContainer.RestoreFromXElement(ee);
                //        }
                //    }
                //}

                // For new format
                //if (e.Name == nameof(CompositeTrack))
                //{
                //    AddTrack(null);
                //    Tracks[Tracks.Count - 1].RestoreFromXElement(e);
                //}
            }
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Creates an XDocument representation of the container and saves it to the specified file.
        /// </summary>
        /// <returns>The XDocument object</returns>
        public void Save(string filename)
        {
            try
            {
                DisplayName = FileName = filename;
                var xe = GetXElement();
                System.IO.File.WriteAllText(filename, xe.ToString());
                ResetIsChanged();
            }
            catch (Exception ex)
            {
                throw new System.IO.IOException($"Unable to save {filename}", ex);
            }
        }

        /// <summary>
        /// Opens the specified file and sets all tracks and values according to the contents.
        /// </summary>
        /// <param name="filename">The file name</param>
        public void Open(string filename)
        {
            try
            {
                //Tracks.Clear();
                XDocument doc = XDocument.Load(filename);
                RestoreFromXElement(doc.Root);
                ResetIsChanged();
                DisplayName = FileName = filename;
            }
            catch (Exception ex)
            {
                throw new System.IO.IOException($"Unable to load {filename}", ex);
            }
        }

        /// <summary>
        /// Shuts down the song container.
        /// </summary>
        public void Shutdown()
        {
            // StopPlayThread();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void IsChangedSetEventHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource != this)
            {
                SetIsChanged();
            }
        }

        private void IsChangedResetEventHandler(object sender, RoutedEventArgs e)
        {
            if (e.Source != this)
            {
                ResetIsChanged();
            }
        }
        #endregion


    }
}
