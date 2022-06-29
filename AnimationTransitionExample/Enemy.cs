using AnimationTransitionExample.Animations;
using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using System;
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
            SkillBook = new SkillBook(
                SkillManager.Instance["heavy"].CreateNew(),
                SkillManager.Instance["block"].CreateNew());
        }

        public static Entity Create(Enemy enemy)
        {
            Entity entity = new Entity(enemy);
            entity.TickAction += enemy.Tick;
            enemy.DrawAction += enemy.Draw;
            return entity;
        }

        public void Tick(GameState state, Entity entity)
        {
            Location location = state.Location;
            if (base.Tick(location, entity))
            {
                return;
            }

            if (Target != null)
            {
                if (Target.IsBlocked() || this.combo.IsStarted() || this.ActiveSkill?.Name == "heavy")
                {
                    if (animations.Any() && animations.Peek().Peek().Name.Contains("move"))
                    {
                        animations.Pop();
                    }

                    Attack(Target, location, "bite");
                }
                else if (CheckSkill(this, null))
                {
                    if (!animations.Any())
                    {
                        if (CheckSkill(Target, "block") && SkillBook["heavy"].IsReady())
                        {
                            SkillBook["heavy"].Action(this);
                            animations.Push(new AnimationChain(
                                AnimationManager.Instance["attackmove"].MakeInterruptable().MakePausing()
                                    .Trigger(pd => this.Distance(this.Target) < 20 && !this.IsBeingKnockedBack())));
                        }
                        else if (CheckSkill(Target, null) && Target.IsAttacking() && SkillBook["block"].IsReady())
                        {
                            SkillBook["block"].Action(this);
                            animations.Push(new AnimationChain(
                                AnimationManager.Instance["attackmove"].MakeInterruptable().MakePausing()
                                    .Trigger(pd => this.Distance(this.Target) < 20 && !this.IsBeingKnockedBack())));
                        }
                        else
                        {
                            Attack(Target, location, "bite");
                        }
                    }
                }
            }
        }

        public static void Bite(IDescription d)
        {
            Enemy enemy = d as Enemy;
            if (enemy == null)
            {
                return;
            }

            AnimationDistance(enemy, 0, 0.8, (t, s) => -(t * 2 * Math.PI) * Math.Sin(t * 2 * Math.PI) * s, Math.Max(0, enemy.Target.Distance(enemy) - 8) / 5);
        }

        public static void FastBite(IDescription d)
        {
            Enemy enemy = d as Enemy;
            if (enemy == null)
            {
                return;
            }

            AnimationDistance(enemy, 0.5, 0.8, (t, s) => -(t * 2 * Math.PI) * Math.Sin(t * 2 * Math.PI) * s, Math.Max(0, enemy.Target.Distance(enemy) - 8) / 5);
        }

        public static void BiteRecovery(IDescription d)
        {
            Enemy enemy = d as Enemy;
            if (enemy == null)
            {
                return;
            }

            AnimationDistance(enemy, 0.8, 1.05, (t, s) => -(t * 2 * Math.PI) * Math.Sin(t * 2 * Math.PI) * s, Math.Max(0, enemy.Target.Distance(enemy) - 8) / 5);
        }

        public Bitmap Draw()
        {
            if (bmp == null)
            {
                bmp = Bitmap.Create(this.Width, this.Height);
                gfx = bmp.GetGraphics();
            }

            gfx.Clear(Color.Transparent);
            gfx.DrawImage(this.Sprite.GetImage(this.ImageIndex), new Rectangle(0, 0, this.Sprite.Width, this.Sprite.Height));
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
                        Color n = new Color((byte)((c.R + color.R) / 2), (byte)((c.G + color.G) / 2), (byte)((c.B + color.B) / 2), c.A);
                        bmp.SetPixel(i, j, n);
                    }
                }
            }

            return bmp;
        }
    }
}
