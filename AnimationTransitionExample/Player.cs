using AnimationTransitionExample.Animations;
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

        private Bitmap bmp;
        private Graphics gfx;

        public Player(int x, int y, int keyController, int mouseController) : base(Sprite.Sprites["player"], x, y, 16, 16)
        {
            this.keyController = keyController;
            this.mouseController = mouseController;
            combo = new AttackCombo(3, 15);
        }

        public static Entity Create(Player player)
        {
            Entity entity = new Entity(player);
            entity.TickAction += player.Tick;
            player.DrawAction += player.Draw;
            return entity;
        }

        public new void Tick(Location location, Entity entity)
        {
            if (base.Tick(location, entity))
            {
                return;
            }

            Marker markerD = Program.Engine.Location.GetEntities<Marker>().First();

            if (Program.Engine.Controllers[mouseController][(int)Actions.MOVE].IsDown())
            {
                MouseControllerInfo mci = Program.Engine.Controllers[mouseController][(int)Actions.MOVE].Info as MouseControllerInfo;
                Point p = new Point(mci.X, mci.Y);

                if (Program.Engine.Controllers[keyController][(int)Actions.TARGET].IsDown())
                {
                    Enemy enemy = Program.Engine.Location.GetEntities<Enemy>().FirstOrDefault();
                    if (enemy != null)
                    {
                        p.X = (int)enemy.X;
                        p.Y = (int)enemy.Y;
                        markerD.SetCoords(p.X, p.Y);

                        if (animations.Any() && animations.Peek().Peek().Name.Contains("move"))
                        {
                            animations.Pop();
                        }

                        target = enemy;
                        animations.Push(new AnimationChain(
                            AnimationManager.Instance[$"-sword{combo.Attack + 1}"].MakeInterruptable().MakePausing(),
                            AnimationManager.Instance[$"sword{combo.Attack + 1}"].MakeInterruptable().MakePausing(),
                            AnimationManager.Instance["move"].MakeInterruptable().Trigger(pd => ((Player)pd).Distance(enemy) < 20 && !enemy.IsBeingKnockedBack())));
                    }
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
                    combo.Reset();
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

        public Bitmap Draw()
        {
            if (bmp == null)
            {
                bmp = new Bitmap(this.Width, this.Height);
                gfx = Graphics.FromImage(bmp);
            }

            Brush brush = Brushes.Black;
            if (combo.CanChain())
            {
                brush = Brushes.Aqua;
            }
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
            if (animations.Any() && animations.Peek().Peek().Name == "move")
            {
                brush = Brushes.Bisque;
            }
            if (stun > 0)
            {
                brush = Brushes.LightYellow;
            }

            gfx.FillEllipse(brush, 0, 0, bmp.Width, bmp.Height);
            gfx.DrawString("P", new Font("courier new", 10), Brushes.White, 2, 2);

            return bmp;
        }
    }
}
