using Restless.App.DrumMaster.Controls.Audio;
using Restless.App.DrumMaster.Controls.Core;
using System.Threading;

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

        private bool isPatternStarted;
        private AutoResetEvent patternPlaySignaler;
        private AutoResetEvent patternEndPlaySignaler;
        private Thread patternPlayThread;
        private int patternSleepTime;
        private int patternOperationSet;
        private bool isControlClosing;
        private PlayMode playMode;
        #endregion

        private void InitializeThreads()
        {
            playMode = PlayMode.Pattern;
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

                            PlaySongPatterns(PointSelectorSongUnit.None, patterns);

                            song.InvokeHighlightSongHeader(pos, false);
                            pos++;
                            maxPos = song.SongSelectors.GetMaxSelectedPosition();
                        }
                    }

                    isPatternStarted = false;
                }
                song = null;
            }
            songEndPlaySignaler.Set();
        }


        private void PatternPlayThreadHandler()
        {
            while (!isControlClosing)
            {
                patternPlaySignaler.WaitOne();

                if (!isControlClosing)
                {
                    while (isPatternStarted)
                    {
                        PlayPattern(PointSelectorSongUnit.None, Owner.ThreadSafeActiveDrumPattern);
                    }
                }
            }
            patternEndPlaySignaler.Set();
        }

        private void PlaySongPatterns(PointSelectorSongUnit songUnit, DrumPatternCollection patterns)
        {
            int quarterNoteCount = patterns.GetMaxQuarterNoteCount();
            for (int quarterNote = 1; quarterNote <= quarterNoteCount; quarterNote++)
            {
                PlayOneQuarterNote(songUnit, quarterNote, patterns.ToArray());
            }
        }

        private void PlayPattern(PointSelectorSongUnit songUnit, DrumPattern pattern)
        {
            int quarterNoteCount = pattern.ThreadSafeController.ThreadSafeQuarterNoteCount;

            for (int quarterNote = 1; quarterNote <= quarterNoteCount; quarterNote++)
            {
                pattern.ThreadSafePresenter.InvokeAddTickHighlight(quarterNote);
                PlayOneQuarterNote(songUnit, quarterNote, pattern);
                pattern.ThreadSafePresenter.InvokeRemoveTickHighlight(quarterNote);
            }
        }

        /// <summary>
        /// Plays a single quarter note
        /// </summary>
        /// <param name="songUnit">The song unit (currently not used)</param>
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
        private void PlayOneQuarterNote(PointSelectorSongUnit songUnit, int quarterNote, params DrumPattern[] patterns)
        {
            for (int position = 0; position < Constants.DrumPattern.LowestCommon; position++)
            {
                if (isPatternStarted)
                {
                    patternOperationSet++;
                    foreach(DrumPattern pattern in patterns)
                    {
                        if (pattern.ThreadSafeController.ThreadSafeQuarterNoteCount >= quarterNote)
                        {
                            pattern.ThreadSafePresenter.Play(songUnit, quarterNote, position, patternOperationSet);
                        }
                    }

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
