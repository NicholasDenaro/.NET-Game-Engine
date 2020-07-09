using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameEngine.UI
{
    public interface IGameFrame
    {
        void Start();

        void PlaySound(string resource);

        IGameWindow Window { get; }

        Rectangle Bounds { get; }

        double ScaleX { get; }

        double ScaleY { get; }
    }
}
