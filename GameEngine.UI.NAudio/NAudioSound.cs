using GameEngine.UI.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static GameEngine.UI.NAudio.SinWaveSound;

namespace GameEngine.UI.NAudio
{
    public class NAudioSound : ISound
    {
        private SinWaveSound wav;

        public NAudioSound(Waves wave, MMLNote[] notes)
        {
            float[] input = new float[notes.Length * 2];
            int i = 0;
            foreach (MMLNote note in notes)
            {
                input[i++] = note.GetTone();
                input[i++] = 44100.0f * note.GetDuration();
            }
            wav = new SinWaveSound(wave, input);
            wav.Attenuate = true;
            wav.SetWaveFormat(44100, 2);
        }

        public Stream GetStream()
        {
            throw new NotImplementedException();
        }

        public SinWaveSound GetOutput()
        {
            return wav;
        }
    }
}
