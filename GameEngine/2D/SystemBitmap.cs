using GameEngine._2D.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameEngine._2D
{
    public class SystemBitmap : IGameBitmap
    {
        private Bitmap bmp;
        private Graphics gfx;

        public SystemBitmap(Bitmap bmp)
        {
            this.bmp = bmp;
            this.gfx = Graphics.FromImage(bmp);
        }

        public int Width => bmp.Width;

        public int Height => bmp.Height;

        public Bitmap Image => this.bmp;

        public Graphics Context => this.gfx;
    }
}
