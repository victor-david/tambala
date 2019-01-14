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
        private Stopwatch songLoopTimer;
        private int songTotalSteps;
        private int songSleepTime;
        private int songOperationSet;

        private bool isPatternStarted;
        private AutoResetEvent patternPlaySignaler;
        private AutoResetEvent patternEndPlaySignaler;
        private Thread patternPlayThread;
        private Stopwatch patternLoopTimer;
        private int patternTotalSteps;
        private int patternSleepTime;
        private int patternOperationSet;
        private bool isControlClosing;
        #endregion

        private void InitializeThreads()
        {
            songLoopTimer = new Stopwatch();
            patternLoopTimer = new Stopwatch();

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
            Debug.WriteLine("Start SongPlayThread");
            while (!isControlClosing)
            {
                songPlaySignaler.WaitOne();
                int pass = 1;

                if (!isControlClosing)
                {
                    while (isSongStarted)
                    {
                        songLoopTimer.Restart();
                        int step = 0;
                        while (isPatternStarted && step < songTotalSteps)
                        {
                            PlayOneSongStep(pass, step++);
                            Thread.Sleep(songSleepTime);
                        }

                        ClearAllSteps();

                        songLoopTimer.Stop();
                        Debug.WriteLine($"SongPlay {pass}. {songLoopTimer.ElapsedMilliseconds}");

                        pass++;
                    }
                }
            }
            Debug.WriteLine("End SongPlayThread");
            songEndPlaySignaler.Set();
        }


        private void PatternPlayThreadHandler()
        {
            Debug.WriteLine("Start PatternPlayThread");
            while (!isControlClosing)
            {
                patternPlaySignaler.WaitOne();
                int pass = 1;

                if (!isControlClosing)
                {
                    while (isPatternStarted)
                    {
                        patternLoopTimer.Restart();
                        int step = 0;


                        for (int quarterNote = 1; quarterNote <= 4; quarterNote++)
                        {
                            PlayOneQuarterNote(quarterNote);
                        }






                        while (isPatternStarted && step < patternTotalSteps)
                        {
                            PlayOnePatternStep(pass, step++);
                            Thread.Sleep(patternSleepTime);
                        }

                        ClearAllSteps();

                        patternLoopTimer.Stop();
                        Debug.WriteLine($"PatternPlay {pass}. {patternLoopTimer.ElapsedMilliseconds}");

                        //if (isRendering && pass == maxRenderPass)
                        //{
                        //    isStarted = false;
                        //    Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback
                        //        ((args) =>
                        //        {
                        //            IsStarted = false;
                        //            return null;
                        //        }), null);

                        //    AudioHost.Instance.AudioCapture.FadeAndStopCapture();
                        //    isRendering = false;
                        //    RenderCompleted?.Invoke(this, new AudioRenderEventArgs(AudioHost.Instance.AudioCapture.RenderParms));
                        //}

                        pass++;
                    }
                }
            }
            Debug.WriteLine("End PatternPlayThread");
            patternEndPlaySignaler.Set();
        }

        private void PlayOneSongStep(int pass, int step)
        {
            songOperationSet++;
        }





        private void PlayOneQuarterNote(int quarterNote)
        {
            for (int position = 0; position < Constants.DrumPattern.LowestCommon; position++)
            {
                if (isPatternStarted)
                {
                    patternOperationSet++;
                    Owner.ThreadSafeActiveDrumPattern.ThreadSafePresenter.Play(quarterNote, position, patternOperationSet);
                    Thread.Sleep(patternSleepTime);
                    AudioHost.Instance.AudioDevice.CommitChanges(patternOperationSet);
                }
            }
        }

        private void PlayOnePatternStep(int pass, int step)
        {
            //Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback
            //    ((args) =>
            //    {
            //        HeaderBoxes.Boxes[step].PlayFrequency = StepPlayFrequency.EveryPass;
            //        int subDiv = (step % StepsPerBeat) + 1;
            //        int beat = (step / StepsPerBeat) + 1;
            //        PassText = $"00{pass}";
            //        CounterText = $"0{beat}:0{subDiv}";
            //        return null;
            //    }), null);

            if (!IsUserMuted && !IsAutoMuted)
            {
                patternOperationSet++;
                //metronome.Play(step, operationSet);

                //Owner.DrumKit.Instruments.DoForAll((instrument) =>
                //{

                //});

                Owner.ThreadSafeActiveDrumPattern.ThreadSafePresenter.Controllers.DoForAll((c) =>
                {
                    //c.Play(pass, step, patternOperationSet);
                });

                //Tracks.DoForAll((track) =>
                //{
                //    track.ThreadSafeController.Play(pass, step, operationSet);
                //});

                AudioHost.Instance.AudioDevice.CommitChanges(patternOperationSet);
            }
        }

        private void ClearAllSteps()
        {
            //Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback
            //    ((args) =>
            //    {
            //        HeaderBoxes.DeselectAllBoxes();
            //        PassText = DefaultPassText;
            //        CounterText = DefaultCounterText;
            //        return null;
            //    }), null);
        }

        private void OnPlayModeChanged()
        {
            // TODO
        }

        private void OnIsStartedChanged()
        {
            // isSongStarted = IsStarted;
            isPatternStarted = IsStarted;
            // TODO -deal with mode
            if (isPatternStarted)
            {
                //songPlaySignaler.Set();
                patternPlaySignaler.Set();
            }
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
            patternSleepTime = MilliSecondsPerMinute / (int)tempo / 24;
        }
        #endregion
    }
}
