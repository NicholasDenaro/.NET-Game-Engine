﻿using GameEngine.UI.Audio;
using GameEngine.UI.NAudio.LinuxAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace GameEngine.UI.NAudio
{
    public class NAudioSoundPlayer : ISoundPlayer
    {
        private IWavePlayer player;
        private MixingSampleProvider provider;
        private bool initialized = false;

        public NAudioSoundPlayer()
        {
            Console.WriteLine($"RuntimeIdentifier: {RuntimeInformation.RuntimeIdentifier}\nOSDescription: {RuntimeInformation.OSDescription}");

            Init();
        }

        private void Init(int deviceIndex = -1)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (deviceIndex > -1)
                {
                    player = new WaveOutEvent()
                    {
                        DeviceNumber = deviceIndex
                    };
                }
                else
                {
                    player = new WaveOutEvent();
                }
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

        public NAudioSoundPlayer(int deviceIndex)
        {
            Console.WriteLine($"RuntimeIdentifier: {RuntimeInformation.RuntimeIdentifier}\nOSDescription: {RuntimeInformation.OSDescription}");

            Console.WriteLine($"{string.Join("\n", DirectSoundOut.Devices.Select((dev, i) => (i, dev.Description)))}");

            Init(deviceIndex);
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

            NAudioSound sound = s as NAudioSound;
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
