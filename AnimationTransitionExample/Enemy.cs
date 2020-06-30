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

        public Enemy(int x, int y) : base(Sprite.Sprites["enemy"], x, y, 16, 16)
        {
            combo = new AttackCombo(3, 5);
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
                    AnimationManager.Instance["move"].MakeInterruptable()
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

            double t = AnimationFrame(enemy, 0, 0.8);
            double x = t * 2 * Math.PI;
            double dist = -x * Math.Sin(x);
            double angle = Math.Atan2(enemy.target.Y - enemy.Y, enemy.target.X - enemy.X);
            enemy.SetCoords(enemy.attackPosition.X + Math.Cos(angle) * dist, enemy.attackPosition.Y + Math.Sin(angle) * dist);
        }

        public static void FastBite(IDescription d)
        {
            Enemy enemy = d as Enemy;
            if (enemy == null)
            {
                return;
            }

            double t = AnimationFrame(enemy, 0.5, 0.8);
            double x = t * 2 * Math.PI;
            double dist = -x * Math.Sin(x);
            double angle = Math.Atan2(enemy.target.Y - enemy.Y, enemy.target.X - enemy.X);
            enemy.SetCoords(enemy.attackPosition.X + Math.Cos(angle) * dist, enemy.attackPosition.Y + Math.Sin(angle) * dist);
        }

        public static void BiteRecovery(IDescription d)
        {
            Enemy enemy = d as Enemy;
            if (enemy == null)
            {
                return;
            }

            double t = AnimationFrame(enemy, 0.8, 1.05);
            double x = t * 2 * Math.PI;
            double dist = -x * Math.Sin(x);
            double angle = Math.Atan2(enemy.target.Y - enemy.Y, enemy.target.X - enemy.X);
            enemy.SetCoords(enemy.attackPosition.X + Math.Cos(angle) * dist, enemy.attackPosition.Y + Math.Sin(angle) * dist);
        }

        public Bitmap Draw()
        {
            if (bmp == null)
            {
                bmp = new Bitmap(this.Width, this.Height);
                gfx = Graphics.FromImage(bmp);
            }

            Brush brush = Brushes.Yellow;

            if (animations.Any() && animations.Peek().Peek() is AttackAnimation)
            {
                if (combo.Attack == 0)
                {
                    brush = Brushes.Aquamarine;
                }
                if (combo.Attack == 1)
                {
                    brush = Brushes.Chartreuse;
                }
                if (combo.Attack == 2)
                {
                    brush = Brushes.Teal;
                }
            }
            else if (animations.Any())
            {
                brush = Brushes.Orange;
            }

            if (animations.Any() && animations.Peek().Peek().Name == "slideback")
            {
                brush = Brushes.DarkOrange;
            }

            if (animations.Any() && animations.Peek().Peek().Name == "knockback")
            {
                brush = Brushes.SaddleBrown;
            }

            if (stun > 0)
            {
                brush = Brushes.LightYellow;
            }

            gfx.FillEllipse(brush, 0, 0, bmp.Width, bmp.Height);
            gfx.DrawString("E", new Font("courier new", 10), Brushes.Navy, 2, 2);

            return bmp;
        }
    }
}
