using GameEngine;
using GameEngine._2D;

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
                bmp = Bitmap.Create(this.Width, this.Height);
                gfx = bmp.GetGraphics();
            }

            gfx.DrawLine(Color.Black, 1, 1, bmp.Width - 2, bmp.Height - 2);
            gfx.DrawLine(Color.Black, bmp.Width - 2, 1, 1, bmp.Height - 2);

            return bmp;
        }
    }
}
