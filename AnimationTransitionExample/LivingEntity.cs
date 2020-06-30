using AnimationTransitionExample.Animations;
using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace AnimationTransitionExample
{
    public class LivingEntity : Description2D
    {
        protected Stack<AnimationChain> animations;
        public int health;
        public int balance;

        protected int stun;
        private Point knockbackFrom;
        protected AttackCombo combo;
        protected Point attackPosition;

        protected LivingEntity target;
        public LivingEntity Target => target;

        public LivingEntity(Sprite sprite, int x, int y, int w, int h) : base(sprite, x, y, w, h)
        {
            health = 100;
            balance = 100;
            knockbackFrom = Point.Empty;
            animations = new Stack<AnimationChain>();
        }

        public bool Tick(Location location, Entity entity)
        {
            if (health <= 0)
            {
                location.RemoveEntity(entity.Id);
                return true;
            }

            if (balance < 100 && (!animations.Any() || !animations.Peek().Peek().Name.Contains("back")))
            {
                balance++;
            }

            if (combo.IsStarted() && (animations.Any() && !(animations.Peek().Peek() is AttackAnimation) || !animations.Any()) && combo.Tick())
            {
                combo.Reset();
            }

            if (stun > 0)
            {
                stun--;
            }

            if (animations.Any())
            {
                if (stun > 0 && animations.Peek().Peek().IsInterruptable())
                {
                    return true;
                }

                if (animations.Peek().Tick(this))
                {
                    animations.Peek().Pop();
                    if (!animations.Peek().Any())
                    {
                        animations.Pop();
                    }
                }

                if (animations.Any() && animations.Peek().Peek().IsPausing())
                {
                    return true;
                }
            }

            return false;
        }

        public void Hit(Description2D hitter, bool finisher, int balanceDiff, int damage)
        {
            if (this is Enemy)
            {
                this.target = hitter as LivingEntity;
            }

            stun = 15;
            if (animations.Any() && animations.Peek().Peek().IsInterruptable())
            {
                animations.Pop();
            }

            health -= damage;
            balance -= balanceDiff;
            if (balance <= 0)
            {
                if (animations.Any())
                {
                    animations.Pop();
                }
                animations.Push(new AnimationChain(AnimationManager.Instance["getup"].MakeInterruptable().MakePausing()));
                animations.Push(new AnimationChain(AnimationManager.Instance[finisher ? "knockback" : "slideback"].MakeInterruptable().MakePausing()));
                knockbackFrom = hitter.Position;
                stun = 0;
            }
        }

        public static void Move(IDescription d)
        {
            LivingEntity le = d as LivingEntity;
            if (le == null)
            {
                return;
            }

            double dir = Math.Atan2(le.Target.Y - le.Y, le.Target.X - le.X);
            le.ChangeCoordsDelta(Math.Cos(dir), Math.Sin(dir));
        }

        public static void StartAttack(IDescription d)
        {
            LivingEntity le = d as LivingEntity;
            if (le == null)
            {
                return;
            }

            le.attackPosition = le.Position;
        }

        public static void Combo(IDescription d)
        {
            LivingEntity le = d as LivingEntity;
            if (le == null)
            {
                return;
            }

            if (le.combo.CanChain())
            {
                le.combo.Advance();
            }
        }

        public static void Strike(IDescription d, bool finisher, int balance, int damage)
        {
            LivingEntity le = d as LivingEntity;
            if (le != null)
            {
                LivingEntity t = le.Target;
                t.Hit(le, finisher, balance, damage);
            }
        }

        public bool IsBeingKnockedBack()
        {
            return animations.Any() ? animations.Peek().Peek().Name == "knockback" || animations.Peek().Peek().Name == "slideback" : false;
        }

        public static void KnockBack(IDescription description)
        {
            LivingEntity le = description as LivingEntity;
            if (le == null)
            {
                return;
            }

            double direction = Math.Atan2(le.Y - le.knockbackFrom.Y, le.X - le.knockbackFrom.X);
            le.ChangeCoordsDelta(Math.Cos(direction) * 4, Math.Sin(direction) * 4);
        }

        public static void SlideBack(IDescription description)
        {
            LivingEntity le = description as LivingEntity;
            if (le == null)
            {
                return;
            }

            double direction = Math.Atan2(le.Y - le.knockbackFrom.Y, le.X - le.knockbackFrom.X);
            le.ChangeCoordsDelta(Math.Cos(direction) * 4, Math.Sin(direction) * 4);
        }

        public static void GetUp(IDescription description)
        {
            LivingEntity le = description as LivingEntity;
            if (le == null)
            {
                return;
            }

            le.balance = 100;
        }

        public static double AnimationFrame(LivingEntity le, double start, double cutoff)
        {
            double ticksLeft = le.animations.Peek().Peek().TicksLeft();
            double duration = le.animations.Peek().Peek().Duration;

            return start + (duration - ticksLeft) * 1.0 / duration * (cutoff - start);
        }
    }
}
