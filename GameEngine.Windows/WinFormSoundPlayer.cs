using System.IO;
using System.Media;

namespace GameEngine.UI.WinForms
{
    public class WinFormSoundPlayer : ISoundPlayer
    {
        private SoundPlayer player;

        public WinFormSoundPlayer()
        {
            player = new SoundPlayer();
        }

        public void Play(Stream stream)
        {
            player.Stream = stream;
            player.Load();
            player.Play();
        }

        public void PlaySound(ISound sound)
        {
            throw new System.NotImplementedException();
        }

        public void PlayStream(Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public void PlayTrack(ITrack track)
        {
            throw new System.NotImplementedException();
        }
    }
}
