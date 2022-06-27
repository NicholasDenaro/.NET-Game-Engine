using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.IO;

namespace GameEngine.UI.AvaloniaUI
{
    public class AvaloniaSoundPlayer : ISoundPlayer
    {
        private IWavePlayer player;
        private MixingSampleProvider provider;
        private bool initialized = false;

        public AvaloniaSoundPlayer()
        {
            if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                return;
            }

            player = new WaveOutEvent();
            WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
            provider = new MixingSampleProvider(format);
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
            provider.AddMixerInput(reader);
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
