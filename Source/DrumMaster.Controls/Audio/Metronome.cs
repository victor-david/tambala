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
        private TrackContainer owner;
        private AudioPiece piece;
        private bool isAudioEnabled;
        private SubmixVoice submixVoice;
        private VoicePool voicePool;
        private int beats;
        private int stepsPerBeat;
        private float volume;
        private float pitchNormal;
        private float pitchAccent;

        internal AudioPiece Piece
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
            volume = XAudio2.DecibelsToAmplitudeRatio(-6.0f);
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
                voicePool = AudioHost.Instance.CreateVoicePool(Piece.Audio, submixVoice);
            }
        }
    }
}
