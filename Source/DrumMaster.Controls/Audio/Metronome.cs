using Restless.App.DrumMaster.Controls.Core;
using Restless.App.DrumMaster.Controls.Obsolete;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.DrumMaster.Controls.Audio
{
    internal class Metronome
    {
        private readonly TrackContainer owner;
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


        internal Metronome(TrackContainer owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            submixVoice = new SubmixVoice(AudioHost.Instance.AudioDevice);
            submixVoice.SetOutputVoices(new VoiceSendDescriptor(this.owner.SubmixVoice));
            volume = XAudio2.DecibelsToAmplitudeRatio(-11.0f);
            pitchNormal = XAudio2.SemitonesToFrequencyRatio(TrackVals.Pitch.Default);
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
                voicePool = AudioHost.Instance.CreateVoicePool("Metronome", Piece.Audio, submixVoice, TrackVals.InitialVoicePool.Normal);
            }
        }
    }
}
