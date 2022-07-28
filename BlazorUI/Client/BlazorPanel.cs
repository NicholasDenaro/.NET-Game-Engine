using Blazor.Extensions.Canvas.Canvas2D;
using BlazorUI.Shared;
using GameEngine;
using GameEngine._2D;
using GameEngine.UI;

namespace BlazorUI.Client
{
    public class BlazorPanel : IGamePanel
    {
        private MainLayout layout;

        public double DesktopScaling { get; set; } = 1;
        public BlazorPanel(int width, int height, double xScale, double yScale)
        {
            Width = width;
            Height = height;
            ScaleX = xScale;
            ScaleY = yScale;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public double ScaleX { get; private set; }

        public double ScaleY { get; private set; }

        public void SetLayout(MainLayout layout)
        {
            this.layout = layout;
            layout.SetBounds(Width, Height, ScaleX, ScaleY);
            layout.SetDesktopScaling(this);
        }

        private SortedDictionary<int, List<Action<Canvas2DContext>>> drawings;
        private SortedDictionary<int, List<Action<Canvas2DContext>>> overlays;

        public void DrawHandle(object sender, View view)
        {
            Drawer2DBlazor d = view.Drawer as Drawer2DBlazor;
            d.View = view as GameView2D;
            Task.Run(async () => await layout?.Draw(this, d));
        }

        public void Resize(int width, int height)
        {
            //throw new NotImplementedException();
        }
    }
}
