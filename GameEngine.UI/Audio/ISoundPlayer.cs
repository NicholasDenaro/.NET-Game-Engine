using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.UI.Audio
{
    public interface ISoundPlayer
    {

        //public delegate Task WaveEvent(float[] stream, int channels);
        public delegate Task WaveEvent(string name, Stream stream);

        void Hook(WaveEvent evt);

        Stream MemoryStream { get; }

        void PlayStream(Stream stream);

        void PlaySound(ISound sound);

        void PlayTrack(ITrack track);
    }
}
