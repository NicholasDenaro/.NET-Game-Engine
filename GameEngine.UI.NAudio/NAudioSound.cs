using GameEngine.UI.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using static GameEngine.UI.NAudio.SinWaveSound;

namespace GameEngine.UI.NAudio
{
    public class NAudioSound : ISound
    {
        private SinWaveSound wav;

        private int totalSamples;
        public int TotalSamples => totalSamples;
        public static Dictionary<string, NAudioSound> Sounds { get; private set; } = new Dictionary<string, NAudioSound>();

        private MemoryStream stream;
        public string Name { get; private set; }

        public NAudioSound(string name, Waves wave, MMLNote[] notes)
        {
            this.Name = name;
            float[] input = new float[notes.Length * 2];
            int i = 0;
            foreach (MMLNote note in notes)
            {
                input[i++] = note.GetTone();
                input[i++] = 44100.0f * note.GetDuration();
                totalSamples += (int)(44100.0f * note.GetDuration());
            }
            wav = new SinWaveSound(wave, input);
            wav.Attenuate = true;
            wav.SetWaveFormat(44100, 2);
            Sounds.Add(name, this);
        }

        public Stream GetStream()
        {
            if (stream == null)
            {
                Stopwatch sw = Stopwatch.StartNew();
                stream = new MemoryStream(totalSamples);
                int read = 0;
                byte[] buf = new byte[1024];

                int count = 0;
                while ((read = wav.Read(buf, 0, buf.Length)) > 0)
                {
                    stream.Write(buf, 0, read);
                    count += read;
                }

                sw.Stop();
            }

            stream.Position = 0;

            return stream;
        }

        public SinWaveSound GetOutput()
        {
            return wav;
        }
    }
}
