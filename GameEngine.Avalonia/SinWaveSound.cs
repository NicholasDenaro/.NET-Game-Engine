using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine.UI.AvaloniaUI
{
    // https://markheath.net/post/playback-of-sine-wave-in-naudio
    public class SinWaveSound : WaveProvider32
    {
        int sample;

        public SinWaveSound(int time, params float[] freqs)
        {
            Frequencies = new float[freqs.Length * 2];
            for (int i = 0; i < freqs.Length; i++)
            {
                Frequencies[i * 2] = freqs[i];
                Frequencies[i * 2 + 1] = time;
            }

            Amplitude = 0.04f; // let's not hurt our ears
            loop = true;
        }
        public SinWaveSound(bool loop, params float[] freqs) : this(freqs)
        {
            this.loop = loop;
        }
        public SinWaveSound(params float[] freqs)
        {
            Frequencies = freqs;
            Amplitude = 0.04f; // let's not hurt our ears
        }

        // array is like freq,time,freq,time,...
        public float[] Frequencies { get; set; }
        public float Amplitude { get; set; }

        private int index;

        private int freq;

        private bool loop;

        public bool IsFinished => freq >= Frequencies.Length;

        public bool Silent { get; set; } = false;
        public bool Quiet { get; set; } = false;

        public bool Attenuate { get; set; } = false;

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = WaveFormat.SampleRate;
            for (int n = 0; n < sampleCount; n++)
            {
                index++;
                if (freq < Frequencies.Length && index > Frequencies[freq + 1])
                {
                    freq += 2;
                    index = 0;
                }

                if (freq >= Frequencies.Length)
                {
                    if (loop)
                    {
                        freq = 0;
                    }
                    else
                    {
                        Amplitude = 0;
                    }
                }

                float f = freq < Frequencies.Length ? Frequencies[freq] : 0;

                float amp = Silent ? 0 : Amplitude;

                amp = Quiet ? amp / 2 : amp;

                if (Attenuate && amp > 0)
                {
                    amp = amp * (Frequencies[freq + 1] - index) / Frequencies[freq + 1] * 0.9f;
                }

                buffer[n + offset] = (float)(amp * Math.Clamp(Math.Sin((2 * Math.PI * sample * f) / sampleRate), -0.5, 0.5) * 2);
                sample++;
                if (sample >= sampleRate) sample = 0;
            }
            return sampleCount;
        }
    }
}
