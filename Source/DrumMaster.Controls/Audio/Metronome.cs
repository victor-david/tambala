using Restless.App.DrumMaster.Controls.Core;
using SharpDX.XAudio2;
using System;

namespace Restless.App.DrumMaster.Controls.Audio
{
    internal class Metronome
    {
        private readonly ProjectContainer owner;
        private Instrument piece;
        private bool isAudioEnabled;
        private readonly SubmixVoice submixVoice;
        private VoicePool voicePool;
        private int beats;
        private int stepsPerBeat;
        // TODO Make volume adjustable.
        private readonly float volume;
        private readonly float pitchNormal;
        private readonly float pitchAccent;

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


        internal Metronome(ProjectContainer owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            submixVoice = new SubmixVoice(AudioHost.Instance.AudioDevice);

            //submixVoice.SetOutputVoices(new VoiceSendDescriptor(this.owner.SubmixVoice));
            volume = XAudio2.DecibelsToAmplitudeRatio(-11.0f);
            pitchNormal = XAudio2.SemitonesToFrequencyRatio(Constants.Pitch.Default);
            pitchAccent = XAudio2.SemitonesToFrequencyRatio(1.5f);

        }

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

        private void OnPieceChanged()
        {
            isAudioEnabled = (Piece != null && Piece.IsAudioInitialized);
            if (isAudioEnabled)
            {
                AudioHost.Instance.DestroyVoicePool(voicePool);
                voicePool = AudioHost.Instance.CreateVoicePool("Metronome", Piece.Audio, submixVoice, Constants.InitialVoicePool.Normal);
            }
        }
    }
}
