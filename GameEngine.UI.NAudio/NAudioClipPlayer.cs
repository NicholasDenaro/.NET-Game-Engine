using GameEngine.UI.Audio;
using GameEngine.UI.NAudio.LinuxAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static GameEngine.UI.Audio.ISoundPlayer;

namespace GameEngine.UI.NAudio
{
    public class NAudioClipPlayer : ISoundPlayer
    {
        private bool initialized = false;

        private WaveEvent EventEmitter;

        public Stream MemoryStream => throw new NotImplementedException();

        public NAudioClipPlayer()
        {
            Console.WriteLine($"RuntimeIdentifier: {RuntimeInformation.RuntimeIdentifier}\nOSDescription: {RuntimeInformation.OSDescription}");
            initialized = true;
        }

        public void Hook(WaveEvent evt)
        {
            EventEmitter += evt;
        }

        public void PlayStream(Stream stream)
        {
            if (!initialized)
            {
                return;
            }

            WaveFileReader reader = new WaveFileReader(stream);
            PlayImpl(new AudioConverter(reader), 0);
        }

        public void PlaySound(ISound s)
        {
            if (!initialized)
            {
                return;
            }

            NAudioSound sound = s as NAudioSound;
            PlayImpl(sound.Name);
        }

        public void PlayTrack(ITrack t)
        {
            if (!initialized)
            {
                Console.WriteLine("not initialized");
                return;
            }

            foreach (ISound sound in t.Channels())
            {
                PlaySound(sound);
            }
        }

        private void PlayImpl(ISampleProvider provider, int samples)
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<float> output = new List<float>(samples);

            float[] buf = new float[samples];

            int read;
            read = provider.Read(buf, 0, buf.Length);
            sw.Stop();
        }

        private void PlayImpl(Stream stream)
        {
            EventEmitter(null, stream);
        }

        private void PlayImpl(string name)
        {
            EventEmitter(name, null);
        }
    }
}
