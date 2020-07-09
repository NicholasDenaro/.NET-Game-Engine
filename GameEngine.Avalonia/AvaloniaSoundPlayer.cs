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
            WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat(48000, 2);
            provider = new MixingSampleProvider(format);
            player.Init(provider);
        }

        public void Play(Stream stream)
        {
            WaveFileReader reader = new WaveFileReader(stream);
            provider.AddMixerInput(reader);
            player.Volume = 0.8f;
            player.Play();
        }
    }
}
