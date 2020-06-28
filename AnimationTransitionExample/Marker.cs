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
                bmp = new Bitmap(this.Width, this.Height);
                gfx = Graphics.FromImage(bmp);
            }

            gfx.DrawLine(Pens.Black, 1, 1, bmp.Width, bmp.Height);
            gfx.DrawLine(Pens.Black, bmp.Width, 0, 0, bmp.Height);

            return bmp;
        }
    }
}
