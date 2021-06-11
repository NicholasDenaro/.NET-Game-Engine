using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace GameEngine.UI
{
    public interface IGameFrame
    {
        void Start();

        void PlayResource(string resource);
        void PlayStream(Stream stream);
        void PlaySound(ISound sound);
        void PlayTrack(ITrack track);

        IGameWindow Window { get; }

        Rectangle Bounds { get; }

        double ScaleX { get; }

        double ScaleY { get; }
    }
}
