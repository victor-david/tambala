using Restless.App.DrumMaster.Core;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Drum
{
    /// <summary>
    /// Represents a drum pattern.
    /// </summary>
    public class DrumPattern : ObjectBase
    {
        #region Private
        private const int MilliSecondsPerMinute = 60000;
        private XAudio2 device;
        private int beats;
        private int stepsPerBeat;
        private int tempo;
        private int sleepTime;
        //private AutoResetEvent submitSignaler;
        private AutoResetEvent playSignaler;

        //private Thread submitThread;
        private Thread playThread;
        private bool stopRequested;
        private bool shutdownRequested;
        //private List<int> operationSets;
        #endregion

        /************************************************************************/

        #region Public Fields
        public const int MinBeats = 4;
        public const int MaxBeats = 8;
        public const int DefaultBeats = 4;

        public const int MinStepsPerBeat = 1;
        public const int MaxStepsPerBeat = 6;
        public const int DefaultStepsPerBeat = 4;

        public const int MinTotalSteps = MinBeats * MinStepsPerBeat;
        public const int MaxTotalSteps = MaxBeats * MaxStepsPerBeat;

        public const int MinTempo = 65;
        public const int MaxTempo = 220;
        public const int DefaultTempo = 120;

        public const int MinTrack = 1;
        public const int MaxTrack = 12;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the number of main beats for this pattern.
        /// </summary>
        public int Beats
        {
            get => beats;
            set
            {
                beats = Math.Max(MinBeats, Math.Min(MaxBeats, value));
            }
        }

        /// <summary>
        /// Gets or sets the number of steps per beat (or sub-divisions) for this pattern.
        /// </summary>
        public int StepsPerBeat
        {
            get => stepsPerBeat;
            set
            {
                stepsPerBeat = Math.Max(MinStepsPerBeat, Math.Min(MaxStepsPerBeat, value));
                sleepTime = (MilliSecondsPerMinute / tempo) / stepsPerBeat;
            }
        }

        /// <summary>
        /// Gets or sets the tempo
        /// </summary>
        public int Tempo
        {
            get => tempo;
            set
            {
                tempo = Math.Max(MinTempo, Math.Min(MaxTempo, value));
                sleepTime = (MilliSecondsPerMinute / tempo) / stepsPerBeat;
            }
        }

        public bool IsPlaying
        {
            get;
            private set;
        }

        public DrumTrackCollection Tracks
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor

        public DrumPattern(XAudio2 device)
        {
            ValidateNull(device, nameof(device));
            this.device = device;
            Tracks = new DrumTrackCollection();
            beats = DefaultBeats;
            stepsPerBeat = DefaultStepsPerBeat;
            Tempo = DefaultTempo;

            //operationSets = new List<int>();

            //submitSignaler = new AutoResetEvent(false);
            playSignaler = new AutoResetEvent(false);


            //submitThread = new Thread(SubmitThreadHandler);
            //submitThread.Start();

            playThread = new Thread(PlayThreadHandler);
            playThread.Start();





        }
        #endregion

        /************************************************************************/

        #region Public methods


        public void Start()
        {
            if (!IsPlaying && Tracks.Count > 0)
            {
                stopRequested = false;
                IsPlaying = true;
                playSignaler.Set();
                // submitSignaler.Set();
            }
        }

        public void Stop()
        {
            stopRequested = true;
            IsPlaying = false;
        }

        public void Shutdown()
        {
            shutdownRequested = true;
            stopRequested = true;
            playSignaler.Set();
        }
        #endregion


        //private void SubmitThreadHandler()
        //{
        //    int operationSet = 1;
        //    while (!shutdownRequested)
        //    {
        //        submitSignaler.WaitOne();
        //        Debug.WriteLine("Submit");
        //        if (!shutdownRequested)
        //        {
        //            int totalSteps = Beats * StepsPerBeat;
        //            operationSets.Clear();
        //            for (int step = 0; step < totalSteps; step++)
        //            {
        //                foreach (DrumTrack track in Tracks)
        //                {
        //                    track.Play(step, operationSet);
        //                }

        //                operationSets.Add(operationSet);
        //                //device.CommitChanges(operationSet);
        //                //Thread.Sleep(sleepTime);
        //                operationSet++;
        //            }
        //            playSignaler.Set();
        //        }
        //    }
        //}

        private void PlayThreadHandler()
        {
            int operationSet = 1;
            // int loopCount = 1;
            //List<int> operationSets = new List<int>();

            while (!shutdownRequested)
            {
                playSignaler.WaitOne();
                if (!shutdownRequested)
                {
                    while (!stopRequested)
                    {
                        int totalSteps = Beats * StepsPerBeat;

                        for (int step = 0; step < totalSteps; step++)
                        {
                            foreach (DrumTrack track in Tracks)
                            {
                                track.Play(step, operationSet);
                            }
                            //Thread.Sleep(sleepTime);
                            // operationSets.Add(operationSet);
                            device.CommitChanges(operationSet);


                            operationSet++;
                            if (stopRequested) break;
                            Thread.Sleep(sleepTime);
                        }
                    }
                }
            }
        }
    }
}
