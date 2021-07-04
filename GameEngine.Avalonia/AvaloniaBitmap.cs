using Avalonia.Media;
using Avalonia.Media.Imaging;
using GameEngine._2D.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine.UI.AvaloniaUI
{
    public class AvaloniaBitmap : IGameBitmap
    {
        private RenderTargetBitmap bmp;
        private DrawingContext gfx;

        public AvaloniaBitmap(RenderTargetBitmap bmp)
        {
            this.bmp = bmp;
            gfx = new DrawingContext(bmp.CreateDrawingContext(null));
        }

        public int Width => (int)bmp.Size.Width;

        public int Height => (int)bmp.Size.Height;

        public Bitmap Image => bmp;

        public DrawingContext Context => gfx;
    }
}
