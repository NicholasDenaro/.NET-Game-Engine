using Blazor.Extensions.Canvas.Canvas2D;
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

        public void SetLayout(MainLayout layout)
        {
            this.layout = layout;
        }

        private SortedDictionary<int, List<Action<Canvas2DContext>>> drawings;
        private SortedDictionary<int, List<Action<Canvas2DContext>>> overlays;

        public void DrawHandle(object sender, View view)
        {
            Drawer2DBlazor d = view.Drawer as Drawer2DBlazor;
            Task.Run(async () => await layout?.Draw(d));
        }

        public void Resize(int width, int height)
        {
            //throw new NotImplementedException();
        }
    }
}
