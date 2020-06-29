using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AnimationTransitionExample
{
    public class LivingEntity : Description2D
    {
        protected Stack<AnimationChain> animations;
        private int health;
        public int balance;

        private Point knockbackFrom;
        protected int flashFrames;

        protected LivingEntity target;
        public LivingEntity Target => target;

        public LivingEntity(Sprite sprite, int x, int y, int w, int h) : base(sprite, x, y, w, h)
        {
            health = 100;
            balance = 100;
            knockbackFrom = Point.Empty;
        }

        public void Hit(Description2D hitter, bool finisher, int balanceDiff)
        {
            if (this is Enemy)
            {
                this.target = hitter as LivingEntity;
                animations.Push(new AnimationChain(
                    AnimationManager.Instance["move"]
                    .MakeInterruptable()
                    .Trigger(pd => this.Distance(this.target) < 20 && !this.IsBeingKnockedBack())));
            }

            balance -= balanceDiff;
            if (balance <= 0)
            {
                animations.Push(new AnimationChain(AnimationManager.Instance["getup"]));
                animations.Push(new AnimationChain(AnimationManager.Instance[finisher ? "knockback" : "slideback"]));
                knockbackFrom = hitter.Position;
            }
            flashFrames = 5;
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
