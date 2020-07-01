using GameEngine;
using GameEngine._2D;
using GameEngine.Windows;
using System;
using System.Drawing;
using System.Linq;

namespace AnimationTransitionExample
{
    public class Hud : Description2D
    {
        int keyController;
        int mouseController;
        private Bitmap bmp;
        private Graphics gfx;

        public Hud (int keyControllerIndex, int mouseControllerIndex, int width, int height) : base(Sprite.Sprites["hud"], 0, 0, width, height)
        {
            keyController = keyControllerIndex;
            mouseController = mouseControllerIndex;
            DrawInOverlay = true;
        }

        public static Entity Create(Hud hud)
        {
            Entity entity = new Entity(hud);
            hud.DrawAction += hud.Draw;
            return entity;
        }

        public Bitmap Draw()
        {
            if (bmp == null)
            {
                bmp = new Bitmap(this.Width, this.Height);
                gfx = Graphics.FromImage(bmp);
            }
            gfx.Clear(Color.Transparent);

            gfx.ScaleTransform(4, 4);
            foreach (LivingEntity e in Program.Engine.Location.GetEntities<LivingEntity>())
            {
                gfx.FillRectangle(Brushes.White, new Rectangle(e.Position.X - 10, e.Position.Y + 2, 20, 4));
                gfx.FillRectangle(Brushes.MediumPurple, new Rectangle(e.Position.X - 9, e.Position.Y + 3, 18 * e.balance / 100, 2));
                gfx.FillRectangle(Brushes.White, new Rectangle(e.Position.X - 10, e.Position.Y + 5, 20, 4));
                gfx.FillRectangle(Brushes.IndianRed, new Rectangle(e.Position.X - 9, e.Position.Y + 6, 18 * e.health / 100, 2));
            }

            Player player = Program.Engine.Location.GetEntities<Player>().FirstOrDefault();
            LivingEntity le = player.LockTarget;

            if (le != null && Program.Engine.Controllers[keyController][(int)Actions.TARGET].IsDown())
            {
                gfx.DrawEllipse(Pens.Cyan, (float)le.X - le.Sprite.X - 2, (float)le.Y - le.Sprite.Y - 2, le.Width + 4, le.Height + 4);
                MouseControllerInfo mci;
                if (Program.Engine.Controllers[mouseController][(int)Actions.MOVE].IsDown())
                {
                    mci = Program.Engine.Controllers[mouseController][(int)Actions.MOVE].Info as MouseControllerInfo;
                }
                else
                {
                    mci = Program.Engine.Controllers[mouseController][(int)Actions.MOUSEINFO].Info as MouseControllerInfo;
                }

                Point center = new Point((int)(le.X - le.Sprite.X - 2 + (le.Width + 4) / 2), (int)(le.Y - le.Sprite.Y - 2 + (le.Height + 4) / 2));
                gfx.DrawLine(Pens.Cyan, mci.X, mci.Y, center.X, center.Y);
            }

            string targetKey = Program.keyMap.Where(kvp => kvp.Value == Actions.TARGET).First().Key.ToString();
            string moveKey = Program.mouseMap.Where(kvp => kvp.Value == Actions.MOVE).First().Key.ToString();

            gfx.ScaleTransform((float)0.25, (float)0.25);

            Font f = new Font(Program.FontCollection.Families[0], 18);

            gfx.FillRectangle(Brushes.Black, 0, 0, Width, 52);
            gfx.DrawString($"{Program.tickTime}", f, Brushes.White, new Point(0, 0));
            gfx.DrawString($"{Program.drawTime}", f, Brushes.White, new Point(0, 16));
            gfx.DrawString($"{Program.tps} | {(Program.tickTime + Program.drawTime) * 100.0 / (TimeSpan.FromSeconds(1).Ticks/Program.TPS):##}%", f, Brushes.White, new Point(0, 32));

            gfx.FillRectangle(Brushes.Black, 0, Height - 52, Width, 52);
            gfx.DrawString($"{moveKey} click to move", f, Brushes.White, new Point(0, Height - 52));
            gfx.DrawString($"Hold {targetKey} to target", f, Brushes.White, new Point(0, Height - 36));
            gfx.DrawString($"{targetKey} + {moveKey} click to attack", f, Brushes.White, new Point(0, Height - 20));

            return bmp;
        }
    }
}
