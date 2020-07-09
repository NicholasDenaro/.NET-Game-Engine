using System.IO;

namespace GameEngine.UI.WinForms
{
    public class WinFormSoundPlayer : ISoundPlayer
    {
        public void Play(Stream stream)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(stream);
            player.Load();
            player.Play();
        }
    }
}
