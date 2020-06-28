using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using System.Drawing;

namespace AnimationTransitionExample
{
    public class Enemy : Description2D
    {
        public Enemy(int x, int y) : base(Sprite.Sprites["enemy"], x, y, 16, 16)
        {
        }

        public static Entity Create(Enemy enemy)
        {
            Entity entity = new Entity(enemy);
            entity.TickAction += enemy.Tick;
            enemy.DrawAction += enemy.Draw;
            return entity;
        }

        public void Tick(Location location, IDescription description)
        {
        }

        public Bitmap Draw()
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.FillEllipse(Brushes.Yellow, 0, 0, bmp.Width, bmp.Height);
            }

            return bmp;
        }
    }
}
