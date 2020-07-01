using AnimationTransitionExample.Animations;
using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using GameEngine.Windows;
using System;
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

        public LivingEntity LockTarget { get; private set; }

        public Player(int x, int y, int keyController, int mouseController) : base(Sprite.Sprites["player2"], x, y, 16, 16)
        {
            this.keyController = keyController;
            this.mouseController = mouseController;
            combo = new AttackCombo(3, 15);
            walkCycle = 4;
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

            MouseControllerInfo mci = Program.Engine.Controllers[mouseController][(int)Actions.MOUSEINFO].Info as MouseControllerInfo;
            if (Program.Engine.Controllers[keyController][(int)Actions.TARGET].State == HoldState.PRESS)
            {
                Enemy nearest = null;
                LockTarget = Program.Engine.Location.GetEntities<Enemy>().Where(
                    e =>
                    {
                        if (e.IsDead())
                        {
                            return false;
                        }

                        if (nearest == null)
                        {
                            nearest = e;
                            return true;
                        }

                        if (e.Distance(mci.X, mci.Y) < nearest.Distance(mci.X, mci.Y))
                        {
                            nearest = e;
                            return true;
                        }

                        return false;
                    }).Last();
            }
            else if (Program.Engine.Controllers[keyController][(int)Actions.TARGET].State == HoldState.RELEASE)
            {
                LockTarget = null;
            }

            if (Program.Engine.Controllers[mouseController][(int)Actions.MOVE].IsDown())
            {
                mci = Program.Engine.Controllers[mouseController][(int)Actions.MOVE].Info as MouseControllerInfo;
                Point p = new Point(mci.X, mci.Y);

                if (Program.Engine.Controllers[keyController][(int)Actions.TARGET].IsDown())
                {
                    if (LockTarget != null)
                    {
                        p.X = (int)LockTarget.X;
                        p.Y = (int)LockTarget.Y;
                        markerD.SetCoords(p.X, p.Y);

                        if (animations.Any() && animations.Peek().Peek().Name.Contains("move"))
                        {
                            animations.Pop();
                        }

                        target = LockTarget;
                        animations.Push(new AnimationChain(
                            AnimationManager.Instance[$"-sword{combo.Attack + 1}"].MakeInterruptable().MakePausing(),
                            AnimationManager.Instance[$"sword{combo.Attack + 1}"].MakeInterruptable().MakePausing(),
                            AnimationManager.Instance["move"].MakeInterruptable().Trigger(pd => ((Player)pd).Distance(target) < 20 && !target.IsBeingKnockedBack())));
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

            double dir = player.Direction(markerD);
            WalkDirection(player, dir);
            
            player.ChangeCoordsDelta(Math.Cos(dir), Math.Sin(dir));
        }

        public static void Swing(IDescription d)
        {
            Player player = d as Player;
            if (player == null)
            {
                return;
            }

            double scale = player.target.Distance(player) / 5;
            double t = AnimationFrame(player, 0, 0.8);
            double x = t * 2 * Math.PI;
            double dist = -x * Math.Sin(x) * scale;
            double angle = Math.Atan2(player.target.Y - player.Y, player.target.X - player.X);
            player.DrawOffsetX = Math.Cos(angle) * dist;
            player.DrawOffsetY = Math.Sin(angle) * dist;
        }

        public static void BackSwing(IDescription d)
        {
            Player player = d as Player;
            if (player == null)
            {
                return;
            }

            double scale = player.target.Distance(player) / 5;
            double t = AnimationFrame(player, 0.8, 1.05);
            double x = t * 2 * Math.PI;
            double dist = -x * Math.Sin(x) * scale;
            double angle = Math.Atan2(player.target.Y - player.Y, player.target.X - player.X);
            player.DrawOffsetX = Math.Cos(angle) * dist;
            player.DrawOffsetY = Math.Sin(angle) * dist;
        }

        public Bitmap Draw()
        {
            if (bmp == null)
            {
                bmp = new Bitmap(this.Width, this.Height);
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

            if (stun > 0)
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
                //Brush br = new SolidBrush(color);
                //gfx.FillRectangle(br, 0, 0, bmp.Width, bmp.Height);
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color c = bmp.GetPixel(i, j);
                        Color n = Color.FromArgb(c.A, (c.R + color.R) / 2, (c.G + color.G) / 2, (c.B + color.B) / 2);
                        bmp.SetPixel(i, j, n);
                    }
                }
                //gfx.DrawString("P", new Font("courier new", 10), Brushes.White, 2, 2);
            }

            return bmp;
        }
    }
}
