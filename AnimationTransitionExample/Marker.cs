using GameEngine;
using GameEngine._2D;
using System.Drawing;

namespace AnimationTransitionExample
{
    public class Marker : Description2D
    {
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
            Bitmap bmp = new Bitmap(this.Width, this.Height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.DrawLine(Pens.Black, 0, 0, bmp.Width, bmp.Height);
                gfx.DrawLine(Pens.Black, bmp.Width, 0, 0, bmp.Height);
            }

            return bmp;
        }
    }
}
