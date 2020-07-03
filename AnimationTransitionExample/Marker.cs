using GameEngine;
using GameEngine._2D;
using System.Drawing;

namespace AnimationTransitionExample
{
    public class Marker : Description2D
    {
        private Bitmap bmp;
        private Graphics gfx;

        public Marker(int x, int y) : base(Sprite.Sprites["marker"], x, y, 8, 8)
        {

        }

        public static Entity Create(Marker marker)
        {
            Entity entity = new Entity(marker);
            marker.DrawAction += marker.Draw;
            return entity;
        }

        public Bitmap Draw()
        {
            if (bmp == null)
            {
                bmp = BitmapExtensions.CreateBitmap(this.Width, this.Height);
                gfx = Graphics.FromImage(bmp);
            }

            gfx.DrawLine(Pens.Black, 1, 1, bmp.Width - 2, bmp.Height - 2);
            gfx.DrawLine(Pens.Black, bmp.Width - 2, 1, 1, bmp.Height - 2);

            return bmp;
        }
    }
}
