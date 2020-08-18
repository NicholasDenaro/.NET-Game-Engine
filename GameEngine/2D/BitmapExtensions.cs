using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace GameEngine._2D
{
    public static class BitmapExtensions
    {
        public static Bitmap CreateBitmap(int width, int height)
        {
            return new Bitmap(width, height).SetRes();
        }

        public static Bitmap CreateBitmap(Stream stream)
        {
            return new Bitmap(stream).SetRes();
        }

        private static Bitmap SetRes(this Bitmap bmp)
        {
            //bmp.SetResolution(Sprite.dpiX, Sprite.dpiY); // intentional
            return bmp;
        }
    }
}
