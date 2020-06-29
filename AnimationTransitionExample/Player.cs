using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using GameEngine.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AnimationTransitionExample
{
    public class Player : LivingEntity
    {
        private int keyController;
        private int mouseController;

        private int swingAni;
        public const int swingChainTime = 15;
        public const int swingChainLength = 3;
        private int swingChainCooldown = swingChainTime;

        private Bitmap bmp;
        private Graphics gfx;

        private Point attackPosition;

        public Player(int x, int y, int keyController, int mouseController) : base(Sprite.Sprites["player"], x, y, 16, 16)
        {
            this.keyController = keyController;
            this.mouseController = mouseController;
            animations = new Stack<AnimationChain>();
            swingAni = 0;
        }

        public static Entity Create(Player player)
        {
            Entity entity = new Entity(player);
            entity.TickAction += player.Tick;
            player.DrawAction += player.Draw;
            return entity;
        }

        public void Tick(Location location, IDescription description)
        {
            if (animations.Any())
            {
                if (animations.Peek().Tick(this))
                {
                    if (animations.Peek().Pop().Name.StartsWith("sword"))
                    {
                        swingAni++;
                        swingAni = swingAni % swingChainLength;
                        swingChainCooldown = swingChainTime;
                    }

                    if (!animations.Peek().Any())
                    {
                        animations.Pop();
                    }
                    else if (animations.Peek().Peek().Name.StartsWith("sword"))
                    {
                        this.attackPosition = this.Position;
                    }
                }

                if (animations.Any() && !animations.Peek().Peek().IsInterruptable())
                {
                    return;
                }
            }
            else
            {
                if (swingChainCooldown > 0)
                {
                    swingChainCooldown--;
                }
                else
                {
                    swingAni = 0;
                }
            }

            Marker markerD = Program.Engine.Location.GetEntities<Marker>().First();

            if (Program.Engine.Controllers[mouseController][(int)Actions.MOVE].IsDown())
            {
                MouseControllerInfo mci = Program.Engine.Controllers[mouseController][(int)Actions.MOVE].Info as MouseControllerInfo;
                Point p = new Point(mci.X, mci.Y);

                if (Program.Engine.Controllers[keyController][(int)Actions.ALT].IsDown())
                {
                    Enemy enemy = Program.Engine.Location.GetEntities<Enemy>().First();
                    p.X = (int)enemy.X;
                    p.Y = (int)enemy.Y;
                    markerD.SetCoords(p.X, p.Y);

                    if (animations.Any() && animations.Peek().Peek().Name.Contains("move"))
                    {
                        animations.Pop();
                    }

                    target = enemy;
                    animations.Push(new AnimationChain(
                        AnimationManager.Instance[$"-sword{swingAni + 1}"],
                        AnimationManager.Instance[$"sword{swingAni + 1}"],
                        AnimationManager.Instance["move"].MakeInterruptable().Trigger(pd => ((Player)pd).Distance(enemy) < 20 && !enemy.IsBeingKnockedBack())));
                }
                else
                {
                    if (animations.Any() && animations.Peek().Peek().Name.Contains("move"))
                    {
                        animations.Pop();
                    }
                    markerD.SetCoords(p.X, p.Y);
                    animations.Push(new AnimationChain(
                        AnimationManager.Instance["playermove"].MakeInterruptable().Trigger(pd => ((Player)pd).Distance(markerD) < 1)));
                    swingAni = 0;
                }
            }
        }

        public static void PlayerMove(IDescription d)
        {
            Player player = d as Player;
            if (player == null)
            {
                return;
            }

            Marker markerD = Program.Engine.Location.GetEntities<Marker>().First();

            double dir = Math.Atan2(markerD.Y - player.Y, markerD.X - player.X);
            player.ChangeCoordsDelta(Math.Cos(dir), Math.Sin(dir));
        }

        public static void Swing(IDescription d)
        {
            Player player = d as Player;
            if (player == null)
            {
                return;
            }

            double t = AnimationFrame(player, 0, 0.8);
            double x = t * 2 * Math.PI;
            double dist = -x * Math.Sin(x);
            double angle = Math.Atan2(player.target.Y - player.Y, player.target.X - player.X);
            player.SetCoords(player.attackPosition.X + Math.Cos(angle) * dist, player.attackPosition.Y + Math.Sin(angle) * dist);
        }

        public static void BackSwing(IDescription d)
        {
            Player player = d as Player;
            if (player == null)
            {
                return;
            }

            double t = AnimationFrame(player, 0.8, 1.05);
            double x = t * 2 * Math.PI;
            double dist = -x * Math.Sin(x);
            double angle = Math.Atan2(player.target.Y - player.Y, player.target.X - player.X);
            player.SetCoords(player.attackPosition.X + Math.Cos(angle) * dist, player.attackPosition.Y + Math.Sin(angle) * dist);
        }

        public static void Strike(IDescription d, bool finisher, int balance)
        {
            Player p = d as Player;
            if (p != null)
            {
                LivingEntity le = p.Target;
                le.Hit(p, finisher, balance);
            }
        }

        public Bitmap Draw()
        {
            if (bmp == null)
            {
                bmp = new Bitmap(this.Width, this.Height);
                gfx = Graphics.FromImage(bmp);
            }

            Brush brush = Brushes.Black;
            if (swingChainCooldown > 0)
            {
                brush = Brushes.Aqua;
            }
            if (animations.Any() && animations.Peek().Peek().Name == "sword1")
            {
                brush = Brushes.Aquamarine;
            }
            if (animations.Any() && animations.Peek().Peek().Name == "sword2")
            {
                brush = Brushes.Chartreuse;
            }
            if (animations.Any() && animations.Peek().Peek().Name == "sword3")
            {
                brush = Brushes.Teal;
            }
            if (animations.Any() && animations.Peek().Peek().Name == "move")
            {
                brush = Brushes.Bisque;
            }

            gfx.FillEllipse(brush, 0, 0, bmp.Width, bmp.Height);

            return bmp;
        }
    }
}
