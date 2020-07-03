using AnimationTransitionExample.Animations;
using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using System;
using System.Drawing;
using System.Linq;

namespace AnimationTransitionExample
{
    public class Enemy : LivingEntity
    {
        private Bitmap bmp;
        private Graphics gfx;

        public Enemy(int x, int y) : base(Sprite.Sprites["enemy2"], x, y, 16, 16)
        {
            combo = new AttackCombo(3, 30);
            walkCycle = 2;
        }

        public static Entity Create(Enemy enemy)
        {
            Entity entity = new Entity(enemy);
            entity.TickAction += enemy.Tick;
            enemy.DrawAction += enemy.Draw;
            return entity;
        }

        public new void Tick(Location location, Entity entity)
        {
            if (base.Tick(location, entity))
            {
                return;
            }

            if (target != null)
            {
                animations.Push(new AnimationChain(
                    AnimationManager.Instance[$"-bite{combo.Attack + 1}"].MakeInterruptable().MakePausing(),
                    AnimationManager.Instance[$"bite{combo.Attack + 1}"].MakeInterruptable().MakePausing(),
                    AnimationManager.Instance["move"].MakeInterruptable().MakePausing()
                        .Trigger(pd => this.Distance(this.target) < 20 && !this.IsBeingKnockedBack())));
            }
        }

        public static void Bite(IDescription d)
        {
            Enemy enemy = d as Enemy;
            if (enemy == null)
            {
                return;
            }

            AnimationDistance(enemy, 0, 0.8, (t, s) => -(t * 2 * Math.PI) * Math.Sin(t * 2 * Math.PI) * s, Math.Max(0, enemy.target.Distance(enemy) - 8) / 5);
        }

        public static void FastBite(IDescription d)
        {
            Enemy enemy = d as Enemy;
            if (enemy == null)
            {
                return;
            }

            AnimationDistance(enemy, 0.5, 0.8, (t, s) => -(t * 2 * Math.PI) * Math.Sin(t * 2 * Math.PI) * s, Math.Max(0, enemy.target.Distance(enemy) - 8) / 5);
        }

        public static void BiteRecovery(IDescription d)
        {
            Enemy enemy = d as Enemy;
            if (enemy == null)
            {
                return;
            }

            AnimationDistance(enemy, 0.8, 1.05, (t, s) => -(t * 2 * Math.PI) * Math.Sin(t * 2 * Math.PI) * s, Math.Max(0, enemy.target.Distance(enemy) - 8) / 5);
        }

        public Bitmap Draw()
        {
            if (bmp == null)
            {
                bmp = BitmapExtensions.CreateBitmap(this.Width, this.Height);
                gfx = Graphics.FromImage(bmp);
            }

            gfx.Clear(Color.Transparent);
            gfx.DrawImage(this.Sprite.GetImage(this.ImageIndex), 0, 0);
            Color color = Color.Black;

            if (animations.Any() && animations.Peek().Peek() is AttackAnimation)
            {
                if (combo.Attack == 0)
                {
                    color = Color.Aquamarine;
                }
                if (combo.Attack == 1)
                {
                    color = Color.Chartreuse;
                }
                if (combo.Attack == 2)
                {
                    color = Color.Teal;
                }
            }

            if (animations.Any() && animations.Peek().Peek().Name == "slideback")
            {
                color = Color.DarkOrange;
            }

            if (animations.Any() && animations.Peek().Peek().Name == "knockback")
            {
                color = Color.SaddleBrown;
            }

            if (stun > 0 || animations.Any() && animations.Peek().Peek().Name == "blocked")
            {
                color = Color.LightYellow;
            }

            if (IsDead())
            {
                color = Color.DarkViolet;
            }

            if (color != Color.Black)
            {
                color = Color.FromArgb(255 / 2, color);
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color c = bmp.GetPixel(i, j);
                        Color n = Color.FromArgb(c.A, (c.R + color.R) / 2, (c.G + color.G) / 2, (c.B + color.B) / 2);
                        bmp.SetPixel(i, j, n);
                    }
                }
            }

            return bmp;
        }
    }
}
