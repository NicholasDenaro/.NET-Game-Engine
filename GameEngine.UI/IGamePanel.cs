using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine.UI
{
    public interface IGamePanel
    {
        double ScaleX { get; }
        double ScaleY { get; }
        void DrawHandle(object sender, View view);
    }
}
