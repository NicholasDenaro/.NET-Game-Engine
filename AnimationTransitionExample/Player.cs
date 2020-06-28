using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using GameEngine.Windows;
using System;
using System.Drawing;
using System.Linq;

namespace AnimationTransitionExample
{
    public class Player : Description2D
    {
        private bool finishedMoving = false;
        private int keyController;
        private int mouseController;

        public Player(int x, int y, int keyController, int mouseController) : base(Sprite.Sprites["player"], x, y, 16, 16)
        {
            this.keyController = keyController;
            this.mouseController = mouseController;
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
            if (Program.Engine.Controllers[keyController][(int)Actions.UP].IsDown())
            {
                this.ChangeCoordsDelta(0, -1);
            }

            Marker markerD = Program.Engine.Location.GetEntities<Marker>().First();

            if (Program.Engine.Controllers[mouseController][(int)Actions.MOVE].IsDown())
            {
                MouseControllerInfo mci = Program.Engine.Controllers[mouseController][(int)Actions.MOVE].Info as MouseControllerInfo;
                Point p = new Point(mci.X, mci.Y);

                if(Program.Engine.Controllers[keyController][(int)Actions.ALT].IsDown())
                {
                    Enemy enemy = Program.Engine.Location.GetEntities<Enemy>().First();
                    p.X = (int)enemy.X;
                    p.Y = (int)enemy.Y;
                }
                markerD.SetCoords(p.X, p.Y);
                finishedMoving = false;
            }

            if (Math.Abs(this.X - markerD.X) < 1 && Math.Abs(this.Y - markerD.Y) < 1)
            {
                finishedMoving = true;
            }

            if (!finishedMoving)
            {
                double dir = Math.Atan2(markerD.Y - this.Y, markerD.X - this.X);
                this.ChangeCoordsDelta(Math.Cos(dir), Math.Sin(dir));
            }
        }

        public Bitmap Draw()
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.FillEllipse(Brushes.Black, 0, 0, bmp.Width, bmp.Height);
            }

            return bmp;
        }
    }
}
