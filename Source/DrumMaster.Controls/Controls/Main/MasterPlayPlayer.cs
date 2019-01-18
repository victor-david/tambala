using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Restless.App.DrumMaster.Controls
{
    /// <summary>
    /// Partial of master play to hold the thread stuff.
    /// </summary>
    public sealed partial class MasterPlay
    {
        #region Private
        private const int MilliSecondsPerMinute = 60000;

        private bool isSongStarted;
        private AutoResetEvent songPlaySignaler;
        private AutoResetEvent songEndPlaySignaler;
        private Thread songPlayThread;
        private int songTotalSteps;
        private int songSleepTime;
        private int songOperationSet;

        private bool isPatternStarted;
        private AutoResetEvent patternPlaySignaler;
        private AutoResetEvent patternEndPlaySignaler;
        private Thread patternPlayThread;
        private int patternTotalSteps;
        private int patternSleepTime;
        private int patternOperationSet;
        private bool isControlClosing;
        private PlayMode playMode;
        #endregion

        private void InitializeThreads()
        {
            playMode = PlayMode.Pattern;

            // TODO
            songTotalSteps = 5;
            songSleepTime = 125;
            // TODO
            patternTotalSteps = 4;
            patternSleepTime = 100;

            songPlaySignaler = new AutoResetEvent(false);
            songEndPlaySignaler = new AutoResetEvent(false);
            songPlayThread = new Thread(SongPlayThreadHandler)
            {
                Name = "SongPlay"
            };
            songPlayThread.Start();

            patternPlaySignaler = new AutoResetEvent(false);
            patternEndPlaySignaler = new AutoResetEvent(false);
            patternPlayThread = new Thread(PatternPlayThreadHandler)
            {
                Name = "PatternPlay"
            };
            patternPlayThread.Start();
        }

        private void SongPlayThreadHandler()
        {
            int patternIdx = 0;
            while (!isControlClosing)
            {
                songPlaySignaler.WaitOne();

                if (!isControlClosing)
                {
                    isPatternStarted = true;

                    while (isSongStarted)
                    {
                        // TODO
                        // Owner.ActivateThreadSafeDrumPattern(patternIdx);
                        PlayPattern(Owner.DrumPatterns[patternIdx]);
                        PlayPattern(Owner.DrumPatterns[patternIdx]);

                        // TODO
                        patternIdx++;
                        if (patternIdx == 2) patternIdx++;
                        if (patternIdx == 4) patternIdx = 0;
                    }

                    isPatternStarted = false;
                }
            }
            songEndPlaySignaler.Set();
        }


        private void PatternPlayThreadHandler()
        {
            //Debug.WriteLine("Start PatternPlayThread");

            while (!isControlClosing)
            {
                patternPlaySignaler.WaitOne();

                if (!isControlClosing)
                {
                    while (isPatternStarted)
                    {
                        PlayPattern(Owner.ThreadSafeActiveDrumPattern);
                    }
                }
            }
            patternEndPlaySignaler.Set();
        }

        private void PlayPattern(DrumPattern pattern)
        {
            int quarterNoteCount = pattern.ThreadSafeController.ThreadSafeQuarterNoteCount;

            for (int quarterNote = 1; quarterNote <= quarterNoteCount; quarterNote++)
            {
                pattern.ThreadSafePresenter.InvokeAddTickHighlight(quarterNote);
                PlayOneQuarterNote(pattern, quarterNote);
                pattern.ThreadSafePresenter.InvokeRemoveTickHighlight(quarterNote);
            }
        }


        private void PlayOneQuarterNote(DrumPattern pattern, int quarterNote)
        {
            for (int position = 0; position < Constants.DrumPattern.LowestCommon; position++)
            {
                if (isPatternStarted)
                {
                    patternOperationSet++;
                    pattern.ThreadSafePresenter.Play(quarterNote, position, patternOperationSet);
                    AudioHost.Instance.AudioDevice.CommitChanges(patternOperationSet);
                    Thread.Sleep(patternSleepTime);
                }
            }
        }

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

            if (isSongStarted)
            {
                songPlaySignaler.Set();
            }
            if (isPatternStarted)
            {
                patternPlaySignaler.Set();
            }

        }

        private void SetWhoIsStarted(PlayMode mode, ref bool isStarted)
        {
            if (PlayMode == mode) isStarted = IsStarted;
        }

        #region Internal methods
        internal void Shutdown()
        {
            IsStarted = false;
            isControlClosing = true;

            if (!songPlaySignaler.SafeWaitHandle.IsClosed)
            {
                songPlaySignaler.Set();
                songPlaySignaler.Dispose();
            }
            if (songEndPlaySignaler != null)
            {
                songEndPlaySignaler.WaitOne();
                songEndPlaySignaler.Dispose();
                songEndPlaySignaler = null;
            }

            if (!patternPlaySignaler.SafeWaitHandle.IsClosed)
            {
                patternPlaySignaler.Set();
                patternPlaySignaler.Dispose();
            }
            if (patternEndPlaySignaler != null)
            {
                patternEndPlaySignaler.WaitOne();
                patternEndPlaySignaler.Dispose();
                patternEndPlaySignaler = null;
            }
        }

        internal void SetTempo(double tempo)
        {
            int tempoInt = (int)tempo;
            patternSleepTime = MilliSecondsPerMinute / tempoInt / Constants.DrumPattern.LowestCommon;
        }
        #endregion
    }
}
