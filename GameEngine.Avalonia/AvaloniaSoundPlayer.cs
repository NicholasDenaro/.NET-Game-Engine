using GameEngine.UI.AvaloniaUI.LinuxAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GameEngine.UI.AvaloniaUI
{
    public class AvaloniaSoundPlayer : ISoundPlayer
    {
        private IWavePlayer player;
        private MixingSampleProvider provider;
        private bool initialized = false;

        public AvaloniaSoundPlayer()
        {
            Console.WriteLine($"RuntimeIdentifier: {RuntimeInformation.RuntimeIdentifier}\nOSDescription: {RuntimeInformation.OSDescription}");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                player = new WaveOutEvent();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                player = new LinuxWaveOutEvent();
            }
            else
            {
                return;
            }

            WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
            provider = new MixingSampleProvider(format);
            provider.ReadFully = true;
            player.Init(provider);
            initialized = true;
        }

        public void PlayStream(Stream stream)
        {
            if (!initialized)
            {
                return;
            }

            WaveFileReader reader = new WaveFileReader(stream);
            provider.AddMixerInput((ISampleProvider)new AudioConverter(reader));
            //player.Volume = 0.8f; // Don't do this, it changes the entire system volume
            player.Play();
        }

        public void PlaySound(ISound s)
        {
            if (!initialized)
            {
                return;
            }

            AvaloniaSound sound = s as AvaloniaSound;
            provider.AddMixerInput((ISampleProvider)sound.GetOutput());
            //player.Volume = 0.8f; // Don't do this, it changes the entire system volume
            player.Play();
        }

        public void PlayTrack(ITrack t)
        {
            if (!initialized)
            {
                return;
            }

            foreach (ISound sound in t.Channels())
            {
                PlaySound(sound);
            }
        }
    }
}
