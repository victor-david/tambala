/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Tambala.
 * Tambala is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Tambala is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using NAudio.CoreAudioApi;
using Restless.Tambala.Controls.Audio;
using Restless.Tambala.Controls.Core;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Threading;

namespace Restless.Tambala.Controls
{
    /// <summary>
    /// Partial of master play to hold the thread stuff.
    /// </summary>
    internal sealed partial class MasterPlay
    {
        #region Private
        private const int MilliSecondsPerMinute = 60000;
        private const string DefaultCounterText = "00:00:00";

        private bool isSongStarted;
        private AutoResetEvent songPlaySignaler;
        private Thread songPlayThread;

        private bool isPatternStarted;
        private AutoResetEvent patternPlaySignaler;
        private Thread patternPlayThread;

        private AutoResetEvent audioMonitorSignaler;
        private Thread audioMonitorThread;

        private int patternSleepTime;
        private int patternOperationSet;
        private bool isControlClosing;
        private PlayMode playMode;
        private Metronome metronome;
        #endregion
            
        /************************************************************************/

        #region Private methods (InitializeThreads)
        private void InitializeThreads()
        {
            CounterText = DefaultCounterText;
            playMode = PlayMode.Pattern;
            patternSleepTime = 100;

            metronome = new Metronome
            {
                Instrument = Owner.DrumKits[DrumKitCollection.DrumKitCubanId].Instruments[6]
            };

            songPlaySignaler = new AutoResetEvent(false);
            songPlayThread = new Thread(SongPlayThreadHandler)
            {
                Name = "SongPlay"
            };
            songPlayThread.Start();

            patternPlaySignaler = new AutoResetEvent(false);
            patternPlayThread = new Thread(PatternPlayThreadHandler)
            {
                Name = "PatternPlay"
            };
            patternPlayThread.Start();

            audioMonitorSignaler = new AutoResetEvent(false);
            audioMonitorThread = new Thread(AudioMonitorThreadHandler)
            {
                Name = "AudioMonitor"
            };
            audioMonitorThread.Start();
        }
        #endregion

        /************************************************************************/

        #region Private methods (Thread handlers)
        private void AudioMonitorThreadHandler()
        {
            MMDevice audioEndpoint = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            while (!isControlClosing)
            {

                audioMonitorSignaler.WaitOne();
                if (!isControlClosing)
                {
                    while (isSongStarted || isPatternStarted)
                    {
                        Owner.ThreadSafeMasterOutput.SetVolumePeak(audioEndpoint.AudioMeterInformation.MasterPeakValue);
                        Thread.Sleep(25);
                    }
                    Owner.ThreadSafeMasterOutput.SetVolumePeak(0.0f);
                }
            }
        }

        private void SongPlayThreadHandler()
        {
            while (!isControlClosing)
            {
                songPlaySignaler.WaitOne();
                SongPresenter song = Owner.SongContainer.Presenter;

                if (!isControlClosing)
                {
                    isPatternStarted = true;

                    while (isSongStarted)
                    {
                        int pos = 1;
                        int maxPos = song.SongSelectors.GetMaxSelectedPosition();
                        while (pos <= maxPos && isSongStarted)
                        {
                            song.InvokeHighlightSongHeader(pos, true);

                            int[] selected = song.SongSelectors.GetRowsAtPosition(pos);
                            var patterns = Owner.DrumPatterns.CreateFromIndices(selected);

                            PlaySongPatterns(PointSelectorSongUnit.None, pos, patterns);

                            song.InvokeHighlightSongHeader(pos, false);
                            pos++;
                            maxPos = song.SongSelectors.GetMaxSelectedPosition();
                        }
                    }

                    isPatternStarted = false;
                }
                song = null;
            }
        }

        private void PatternPlayThreadHandler()
        {
            while (!isControlClosing)
            {
                patternPlaySignaler.WaitOne();
                int pass = 0;
                if (!isControlClosing)
                {
                    while (isPatternStarted)
                    {
                        pass++;
                        PlayPattern(PointSelectorSongUnit.None, pass, Owner.ThreadSafeActiveDrumPattern);
                    }
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (Play audio)
        private void PlaySongPatterns(PointSelectorSongUnit songUnit, int pass, DrumPatternCollection patterns)
        {
            int quarterNoteCount = patterns.GetMaxQuarterNoteCount();
            for (int quarterNote = 1; quarterNote <= quarterNoteCount; quarterNote++)
            {
                if (isSongStarted)
                {
                    PlayOneQuarterNote(songUnit, pass, quarterNote, patterns.ToArray());
                }
            }
        }

        private void PlayPattern(PointSelectorSongUnit songUnit, int pass, DrumPattern pattern)
        {
            int quarterNoteCount = pattern.ThreadSafeController.ThreadSafeQuarterNoteCount;

            for (int quarterNote = 1; quarterNote <= quarterNoteCount; quarterNote++)
            {
                if (isPatternStarted)
                {
                    pattern.ThreadSafePresenter.InvokeAddTickHighlight(quarterNote);
                    PlayOneQuarterNote(songUnit, pass, quarterNote, pattern);
                    pattern.ThreadSafePresenter.InvokeRemoveTickHighlight(quarterNote);
                }
            }
        }

        /// <summary>
        /// Plays a single quarter note
        /// </summary>
        /// <param name="songUnit">The song unit (currently not used, reserved for future feature)</param>
        /// <param name="pass">The pass. Used for counter display.</param>
        /// <param name="quarterNote">The quarter note: 1,2,3,4,etc.</param>
        /// <param name="patterns">The patterns</param>
        /// <remarks>
        /// This method invokes other methods that submit their voices (if selected)
        /// to the voice pool and commits the operation set to the audio device.
        /// 
        /// Pattern Play
        /// ------------
        /// When in pattern play, <paramref name="patterns"/> has a single element.
        /// 
        /// Song Play
        /// ---------
        /// When in song play, <paramref name="patterns"/> may have zero, one, or more than one elements.
        /// Each element (DrumPattern) may have a different number of quarter notes. 
        /// PlaySongPatterns() above cycles through the quarter notes from one to the highest number of
        /// quarter notes among the selected patterns. Therefore, for some patterns in the set here,
        /// a quarter note value may not be appropiate. For example, with two patterns on the same song step,
        /// one could have 4 quarter notes and the other 3 quarter notes. We'll cycle down to this method
        /// for all four quarter notes, but not play anything beyond the 3rd quarter for the pattern
        /// that only has 3.
        /// 
        /// When changing the number of quarter notes for a drum pattern, previously selected notes on the timeline
        /// are not altered. A pattern may have notes selected within the fourth quarter note, and then have its
        /// quarter note count reduced to three. The notes in the fourth quarter remain, but are not played.
        /// </remarks>
        private void PlayOneQuarterNote(PointSelectorSongUnit songUnit, int pass, int quarterNote, params DrumPattern[] patterns)
        {
            for (int position = 0; position < Ticks.LowestCommon; position++)
            {
                if (Ticks.Playable.Contains(position))
                {
                    if (isPatternStarted)
                    {
                        InvokeSetCounterText(pass, quarterNote, position);
                        patternOperationSet++;
                        metronome.Play(position, patternOperationSet);
                        foreach (DrumPattern pattern in patterns)
                        {
                            if (pattern.ThreadSafeController.ThreadSafeQuarterNoteCount >= quarterNote)
                            {
                                pattern.ThreadSafePresenter.Play(songUnit, quarterNote, position, patternOperationSet);
                            }
                        }

                        AudioHost.Instance.AudioDevice.CommitChanges(patternOperationSet);
                    }
                }

                Thread.Sleep(patternSleepTime);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (Start / stop)
        private void OnPlayModeChanged()
        {
            IsStarted = false;
            playMode = PlayMode;
        }

        private void OnIsStartedChanged()
        {
            isPatternStarted = isSongStarted = false;
            SetWhoIsStarted(PlayMode.Pattern, ref isPatternStarted);
            SetWhoIsStarted(PlayMode.Song, ref isSongStarted);

            InvokeSetCounterText(0, 0, 0);

            if (isSongStarted)
            {
                songPlaySignaler.Set();
                audioMonitorSignaler.Set();
            }
            if (isPatternStarted)
            {
                patternPlaySignaler.Set();
                audioMonitorSignaler.Set();
            }
        }

        private void SetWhoIsStarted(PlayMode mode, ref bool isStarted)
        {
            if (PlayMode == mode) isStarted = IsStarted;
        }

        private void InvokeSetCounterText(int pass, int quarter, int position)
        {
            // 00:00:00
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(()=> 
            {
                CounterText = $"{pass:D2}:{quarter:D2}:{position:D2}";
            }));
        }
        #endregion

        /************************************************************************/

        #region IDisposable
        /// <summary>
        /// Disposes resources.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification="Disposal happens via Shutdown() method")]
        public void Dispose()
        {
            Shutdown();
            GC.SuppressFinalize(this);
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Adjusts the thread sleep time according to the specified tempo.
        /// </summary>
        /// <param name="tempo">The tempo.</param>
        internal void SetTempo(double tempo)
        {
            int tempoInt = (int)tempo;
            patternSleepTime = MilliSecondsPerMinute / tempoInt / Ticks.LowestCommon;
        }
        #endregion

        /************************************************************************/

        #region Private method
        private void Shutdown()
        {
            IsStarted = false;
            isControlClosing = true;

            if (!songPlaySignaler.SafeWaitHandle.IsClosed)
            {
                songPlaySignaler.Set();
                songPlaySignaler.Dispose();
            }

            if (!patternPlaySignaler.SafeWaitHandle.IsClosed)
            {
                patternPlaySignaler.Set();
                patternPlaySignaler.Dispose();
            }

            if (!audioMonitorSignaler.SafeWaitHandle.IsClosed)
            {
                audioMonitorSignaler.Set();
                audioMonitorSignaler.Dispose();
            }

            metronome.Dispose();
        }
        #endregion

    }
}