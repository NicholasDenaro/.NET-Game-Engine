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

        public AvaloniaSoundPlayer()
        {
            player = new WasapiOut(AudioClientShareMode.Shared, 0);
            WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
            provider = new MixingSampleProvider(format);
            player.Init(provider);
        }

        public void PlayStream(Stream stream)
        {
            WaveFileReader reader = new WaveFileReader(stream);
            provider.AddMixerInput(reader);
            //player.Volume = 0.8f; // Don't do this, it changes the entire system volume
            player.Play();
        }

        public void PlaySound(ISound s)
        {
            AvaloniaSound sound = s as AvaloniaSound;
            provider.AddMixerInput((ISampleProvider)sound.GetOutput());
            //player.Volume = 0.8f; // Don't do this, it changes the entire system volume
            player.Play();
        }

        public void PlayTrack(ITrack t)
        {
            foreach(ISound sound in t.Channels())
            {
                PlaySound(sound);
            }
        }
    }
}
