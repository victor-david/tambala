using Restless.App.DrumMaster.Controls.Core;
using SharpDX.XAudio2;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Restless.App.DrumMaster.Controls.Audio
{
    /// <summary>
    /// Provides a metronome
    /// </summary>
    internal sealed class Metronome : IDisposable
    {
        #region Private
        private readonly ProjectContainer owner;
        private Instrument piece;
        private bool isAudioEnabled;
        private SubmixVoice submixVoice;
        private VoicePool voicePool;
        private int beats;
        private int stepsPerBeat;
        // TODO Make volume adjustable.
        private readonly float volume;
        private readonly float pitchNormal;
        private readonly float pitchAccent;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Metronome"/> class.
        /// </summary>
        /// <param name="owner">The owner of this instance.</param>
        internal Metronome(ProjectContainer owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            submixVoice = new SubmixVoice(AudioHost.Instance.AudioDevice);

            //submixVoice.SetOutputVoices(new VoiceSendDescriptor(this.owner.SubmixVoice));
            volume = XAudio2.DecibelsToAmplitudeRatio(-11.0f);
            pitchNormal = XAudio2.SemitonesToFrequencyRatio(Constants.Pitch.Default);
            pitchAccent = XAudio2.SemitonesToFrequencyRatio(1.5f);

        }
        #endregion

        /************************************************************************/

        #region Properties
        internal Instrument Piece
        {
            get => piece;
            set
            {
                piece = value;
                OnPieceChanged();
            }
        }

        internal bool IsActive
        {
            get;
            set;
        }
        #endregion

        /************************************************************************/

        #region IDisposable
        /// <summary>
        /// Disposes resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        /// <param name="disposing">true if disposing</param>
        [SuppressMessage("Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification="Disposal happens via SharpDx.Utilities")]
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (submixVoice != null)
                {
                    SharpDX.Utilities.Dispose(ref submixVoice);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        internal void UpdateBeatValues(int beats, int stepsPerBeat)
        {
            this.beats = beats;
            this.stepsPerBeat = stepsPerBeat;
        }

        internal void Play(int step, int operationSet)
        {
            if (IsActive & isAudioEnabled)
            {
                float pitch = (step % stepsPerBeat == 0) ? pitchAccent : pitchNormal;
                voicePool.Play(volume, pitch, operationSet);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void OnPieceChanged()
        {
            isAudioEnabled = (Piece != null && Piece.IsAudioInitialized);
            if (isAudioEnabled)
            {
                AudioHost.Instance.DestroyVoicePool(voicePool);
                voicePool = AudioHost.Instance.CreateVoicePool("Metronome", Piece.Audio, submixVoice, Constants.InitialVoicePool.Normal);
            }
        }
        #endregion
    }
}
