using BlazorUI.Shared;
using GameEngine;
using GameEngine.UI;

namespace BlazorUI.Client
{
    public class BlazorPanel : IGamePanel
    {
        private MainLayout layout;

        public double ScaleX => throw new NotImplementedException();

        public double ScaleY => throw new NotImplementedException();

        public void DrawHandle(object sender, View view)
        {
            //layout.Draw();
        }

        public void Resize(int width, int height)
        {
            //throw new NotImplementedException();
        }
    }
}
