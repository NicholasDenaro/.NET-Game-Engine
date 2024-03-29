﻿using AnimationTransitionExample.Animations;
using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using System;
using System.Linq;

namespace AnimationTransitionExample
{
    public class LivingEntity : Description2D
    {
        protected AnimationStack animations;
        public int health;
        public float stamina;
        public int balance;

        protected int stun;
        private Point knockbackFrom;
        protected AttackCombo combo;

        protected Point MoveTarget;
        public LivingEntity Target { get; protected set; }

        protected SkillBook SkillBook { get; set; }

        private Animation skillActivation;
        public Skill PreppedSkill { get; protected set; }
        public Skill ActiveSkill { get; protected set; }

        public int SkillActivationTime => skillActivation.TicksLeft();

        protected int direction;
        private double walkIndex = 0;
        protected int walkCycle;

        public LivingEntity(Sprite sprite, int x, int y, int w, int h) : base(sprite, x, y, w, h)
        {
            health = 100;
            stamina = 30;
            balance = 100;
            knockbackFrom = new Point();
            animations = new AnimationStack();
            this.onMove += LivingEntity.WalkIndexing;
            skillActivation = AnimationManager.Instance["activateskill"].CreateNew();
        }

        public bool Tick(Location location, Entity entity)
        {
            ImageIndex = direction + ((int)walkIndex % walkCycle);

            if (!IsDead() && balance < 100 && (!animations.Any() || !animations.Peek().Peek().Name.Contains("back")))
            {
                balance++;
            }

            if (stamina < 30)
            {
                stamina += 1.0f / Program.TPS / 2.5f;
            }

            SkillBook.Tick();

            if (combo.IsStarted() && ((animations.Any() && !(animations.Peek().Peek() is AttackAnimation)) || !animations.Any()) && combo.Tick())
            {
                combo.Reset();
            }

            if (stun > 0)
            {
                stun--;
            }

            if (PreppedSkill != null)
            {
                skillActivation.Tick(this);
            }

            if (animations.Any())
            {
                if (stun > 0 && animations.Peek().Peek().IsInterruptable())
                {
                    return true;
                }

                AnimationChain chain = animations.Peek();
                if (animations.Peek().Tick(this))
                {
                    if (chain == animations.Peek())
                    {
                        animations.Peek().Pop();
                    }

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

            if (health <= 0)
            {
                return true;
            }

            return false;
        }

        public void SetPreppedSkill(Skill skill)
        {
            this.skillActivation = AnimationManager.Instance["activateskill"].CreateNew();
            this.PreppedSkill = skill;
        }

        public void CancelSkill()
        {
            this.skillActivation = null;
            this.PreppedSkill = null;
            this.ActiveSkill = null;
        }

        public static void ActivateSkill(IDescription d)
        {
            LivingEntity le = d as LivingEntity;
            if (le == null)
            {
                return;
            }

            le.ActiveSkill = le.PreppedSkill;
            le.PreppedSkill = null;
        }

        public static void WalkIndexing(Description2D d2d)
        {
            LivingEntity le = d2d as LivingEntity;
            if(le == null)
            {
                return;
            }

            le.walkIndex += 0.25;
        }

        public static void EndMove(IDescription d)
        {
            LivingEntity le = d as LivingEntity;
            if (le == null)
            {
                return;
            }

            le.walkIndex = 0;
        }

        protected void Attack(LivingEntity entity, Location location, string attackName)
        {
            Target = entity;
            if (ActiveSkill == null)
            {
                int index = attackName == "punch" ? 1 : this.combo.Attack + 1;
                animations.Push(new AnimationChain(
                    AnimationManager.Instance[$"-{attackName}{index}"].MakeInterruptable().MakePausing(),
                    AnimationManager.Instance[$"{attackName}{index}"].MakeInterruptable().MakePausing(),
                    AnimationManager.Instance["attackmove"].MakeInterruptable().Trigger(le => ((LivingEntity)le).Distance(Target) < 20 && !Target.IsBeingKnockedBack())));
            }
            else
            {
                ActiveSkill.Cooldown();
                ActiveSkill.SAction(location, this);
            }
        }

        public void Hit(Description2D hitter, bool finisher, int balanceDiff, int damage)
        {
            if (this is Enemy)
            {
                this.Target = hitter as LivingEntity;
            }

            LivingEntity leHitter = hitter as LivingEntity;
            if (leHitter != null && ActiveSkill != null)
            {
                if (ActiveSkill.Name == "block")
                {
                    //Program.Frame.PlayResource("Sounds.GAME_MENU_SCORE_SFX001755.wav");
                    ActiveSkill.Cooldown();
                    if (balanceDiff != 100)
                    {
                        ActiveSkill = null;
                        health -= damage;
                        leHitter.animations.Queue(new AnimationChain(AnimationManager.Instance["blocked"].MakeInterruptable().MakePausing()));

                        return;
                    }
                }
                else if (ActiveSkill.Name == "counter")
                {
                    ActiveSkill.Cooldown();
                    leHitter.Hit(this, true, 100, damage);
                    ActiveSkill = null;
                    return;
                }
            }

            //Program.Frame.PlayResource("Sounds.GAME_MENU_SCORE_SFX001771-16-1.wav");
            //Program.Frame.PlayResource("Sounds.GAME_MENU_SCORE_SFX001771-24-2.wav");
            Program.Frame.PlayResource("Sounds.sfxR07.wav");
            //Program.Frame.PlayResource("Sounds.sfxR07-left-only.wav");
            //Program.Frame.PlayTrack(new GameEngine.UI.NAudio.NAudioMMLTrack(GameEngine.UI.NAudio.SinWaveSound.Waves.ZINGY, new GameEngine.UI.Audio.MML(new string[] { "l4cdefg" })));
            this.PreppedSkill = null;
            this.skillActivation?.Reset();
            stun = 15;
            DrawOffsetX = 0;
            DrawOffsetY = 0;
            if (animations.Any() && animations.Peek().Peek().IsInterruptable())
            {
                animations.Pop();
                combo.Reset();
            }

            ActiveSkill = null;
            health -= damage;
            balance -= balanceDiff;
            if (balance <= 0 || IsDead())
            {
                if (animations.Any())
                {
                    animations.Pop();
                }

                if (!IsDead())
                {
                    animations.Push(new AnimationChain(AnimationManager.Instance["getup"].MakeInterruptable().MakePausing()));
                }

                animations.Push(new AnimationChain(AnimationManager.Instance[finisher || IsDead() ? "knockback" : "slideback"].MakeInterruptable().MakePausing()));
                knockbackFrom = hitter.Position;
                stun = 0;
            }
        }

        public static void AttackMove(IDescription d)
        {
            LivingEntity le = d as LivingEntity;
            if (le == null)
            {
                return;
            }

            double dir = le.Direction(le.Target);
            WalkDirection(le, dir);
            double scale = 1;
            Skill skill = le.PreppedSkill ?? le.ActiveSkill;

            if (skill?.Name == "block")
            {
                scale = 0.5;
            }
            else if (skill?.Name == "ranged")
            {
                scale = 0.5;
            }
            else if (skill?.Name == "counter")
            {
                scale = 0;
            }

            le.ChangeCoordsDelta(Math.Cos(dir) * scale, Math.Sin(dir) * scale);
        }

        public static void Move(IDescription d)
        {
            LivingEntity le = d as LivingEntity;
            if (le == null)
            {
                return;
            }

            double dir = le.Direction(le.MoveTarget);
            WalkDirection(le, dir);

            double scale = 1;

            Skill skill = le.PreppedSkill ?? le.ActiveSkill;
            if (skill?.Name == "block")
            {
                scale = 0.5;
            }
            else if (skill?.Name == "counter")
            {
                scale = 0;
            }

            le.ChangeCoordsDelta(Math.Cos(dir) * scale, Math.Sin(dir) * scale);
        }

        public static void WalkDirection(LivingEntity le, double direction)
        {
            double imgDir = -direction;
            if (imgDir < 0)
            {
                imgDir += Math.PI * 2;
            }

            le.direction = (int)((imgDir + Math.PI / 4) / (Math.PI / 2) + 3) * 4;
        }

        public static void StartAttack(IDescription d)
        {
            LivingEntity le = d as LivingEntity;
            if (le == null)
            {
                return;
            }
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
            else
            {
                le.combo.Reset();
            }
        }

        public static void ResetToAttackPosition(IDescription d)
        {
            LivingEntity le = d as LivingEntity;
            if (le == null)
            {
                return;
            }

            le.DrawOffsetX = 0;
            le.DrawOffsetY = 0;
        }

        public static void CombatSkillEnd(IDescription d)
        {
            LivingEntity le = d as LivingEntity;
            if (le == null)
            {
                return;
            }

            le.ActiveSkill = null;
        }

        public static void Strike(IDescription d, bool finisher, int stamina, int balance, int damage)
        {
            LivingEntity le = d as LivingEntity;
            if (le != null)
            {
                LivingEntity t = le.Target;
                le.stamina -= stamina;
                t.Hit(le, finisher, balance, damage);
            }
        }

        public bool IsBeingKnockedBack()
        {
            return animations.Any() ? animations.Peek().Peek().Name == "knockback" || animations.Peek().Peek().Name == "slideback" : false;
        }

        public bool IsBlocked()
        {
            return animations.Any(astack => astack.Any(a => a.Name == "blocked"));
        }

        public bool IsDead()
        {
            return health <= 0;
        }

        public bool IsAttacking()
        {
            return animations.Peek()?.Any(a => a is AttackAnimation) ?? false;
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
            le.DrawOffsetX = 0;
            le.DrawOffsetY = 0;
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
            le.DrawOffsetX = 0;
            le.DrawOffsetY = 0;
        }

        public static void GetUp(IDescription description)
        {
            LivingEntity le = description as LivingEntity;
            if (le == null)
            {
                return;
            }

            le.balance = 100;
            le.DrawOffsetX = 0;
            le.DrawOffsetY = 0;
        }

        public static void DoSkill(LivingEntity livingEntity, Skill skill)
        {
            livingEntity.ActiveSkill = skill;
        }

        public static bool CheckSkill(LivingEntity target, string name)
        {
            return name != null && (target?.PreppedSkill?.Name == name || target?.ActiveSkill?.Name == name)
                || name == null && (target?.PreppedSkill?.Name == null && target?.ActiveSkill?.Name == null);
        }

        public static bool HeavyAttack(Location location, IDescription description)
        {
            LivingEntity le = description as LivingEntity;
            if (le == null)
            {
                return false;
            }

            le.animations.Push(new AnimationChain(
                AnimationManager.Instance[$"-heavy"].MakeInterruptable().MakePausing(),
                AnimationManager.Instance[$"heavy"].MakeInterruptable().MakePausing(),
                AnimationManager.Instance["attackmove"].MakeInterruptable().Trigger(pd => ((LivingEntity)le).Distance(le.Target) < 20 && !le.Target.IsBeingKnockedBack())));

            return true;
        }

        public static void HeavyAnimation(IDescription d)
        {
            LivingEntity le = d as LivingEntity;
            if (le == null)
            {
                return;
            }

            AnimationDistance(le, 0.5, 0.8, (t, s) => Math.Min(-(t * 2 * Math.PI) * Math.Sin(t * 2 * Math.PI) * s, 3), Math.Max(0, le.Target.Distance(le) - 8) / 5);
        }

        public static void HeavyBackAnimation(IDescription d)
        {
            LivingEntity le = d as LivingEntity;
            if (le == null)
            {
                return;
            }

            AnimationDistance(le, 0.8, 1.05, (t, s) => -(t * 2 * Math.PI) * Math.Sin(t * 2 * Math.PI) * s, Math.Max(0, le.Target.Distance(le) - 8) / 5);
        }

        public static double AnimationFrame(LivingEntity le, double start, double cutoff)
        {
            double ticksLeft = le.animations.Peek().Peek().TicksLeft();
            double duration = le.animations.Peek().Peek().Duration;

            return start + (duration - ticksLeft) * 1.0 / duration * (cutoff - start);
        }

        public static void AnimationDistance(LivingEntity le, double start, double end, Func<double, double, double> distFunc, double scale)
        {
            double t = AnimationFrame(le, start, end);
            double dist = distFunc(t, scale);
            double angle = Math.Atan2(le.Target.Y - le.Y, le.Target.X - le.X);
            le.DrawOffsetX = Math.Cos(angle) * dist;
            le.DrawOffsetY = Math.Sin(angle) * dist;
        }
    }
}
