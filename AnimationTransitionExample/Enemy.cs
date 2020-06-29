using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AnimationTransitionExample
{
    public class Enemy : LivingEntity
    {
        private Bitmap bmp;
        private Graphics gfx;

        public Enemy(int x, int y) : base(Sprite.Sprites["enemy"], x, y, 16, 16)
        {
            animations = new Stack<AnimationChain>();
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
            if (animations.Any())
            {
                if (animations.Peek().Tick(this))
                {
                    animations.Peek().Pop();
                    if (!animations.Peek().Any())
                    {
                        animations.Pop();
                    }
                }

                if (animations.Any() && !animations.Peek().Peek().IsInterruptable())
                {
                    return;
                }
            }

            if (balance < 100)
            {
                balance++;
            }
        }

        public Bitmap Draw()
        {
            if (bmp == null)
            {
                bmp = new Bitmap(this.Width, this.Height);
                gfx = Graphics.FromImage(bmp);
            }

            Brush brush = Brushes.Yellow;

            if (flashFrames-- > 0)
            {
                brush = Brushes.LightYellow;
            }

            if (animations.Any())
            {
                brush = Brushes.Orange;
            }

            gfx.FillEllipse(brush, 0, 0, bmp.Width, bmp.Height);

            return bmp;
        }
    }
}
