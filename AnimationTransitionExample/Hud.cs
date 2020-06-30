﻿using GameEngine;
using GameEngine._2D;
using GameEngine.Windows;
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

        public Hud(int keyControllerIndex, int mouseControllerIndex, int width, int height) : base(Sprite.Sprites["hud"], 0, 0, width, height)
        {
            keyController = keyControllerIndex;
            mouseController = mouseControllerIndex;
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

            foreach(LivingEntity le in Program.Engine.Location.GetEntities<LivingEntity>())
            {
                gfx.FillRectangle(Brushes.White, new Rectangle(le.Position.X - 10, le.Position.Y + 16, 20, 4));
                gfx.FillRectangle(Brushes.MediumPurple, new Rectangle(le.Position.X - 9, le.Position.Y + 17, 18 * le.balance / 100, 2));
                gfx.FillRectangle(Brushes.White, new Rectangle(le.Position.X - 10, le.Position.Y + 21, 20, 4));
                gfx.FillRectangle(Brushes.IndianRed, new Rectangle(le.Position.X - 9, le.Position.Y + 22, 18 * le.health / 100, 2));
            }

            Enemy enemy = Program.Engine.Location.GetEntities<Enemy>().FirstOrDefault();

            if (enemy != null && Program.Engine.Controllers[keyController][(int)Actions.TARGET].IsDown())
            {
                gfx.DrawEllipse(Pens.Cyan, (float)enemy.X - enemy.Sprite.X - 2, (float)enemy.Y - enemy.Sprite.Y - 2, enemy.Width + 4, enemy.Height + 4);
                MouseControllerInfo mci;
                if (Program.Engine.Controllers[mouseController][(int)Actions.MOVE].IsDown())
                {
                    mci = Program.Engine.Controllers[mouseController][(int)Actions.MOVE].Info as MouseControllerInfo;
                }
                else
                {
                    mci = Program.Engine.Controllers[mouseController][(int)Actions.MOUSEINFO].Info as MouseControllerInfo;
                }

                gfx.DrawLine(Pens.Cyan, mci.X, mci.Y, (float)enemy.X, (float)enemy.Y);
            }

            string targetKey = Program.keyMap.Where(kvp => kvp.Value == Actions.TARGET).First().Key.ToString();
            string moveKey = Program.mouseMap.Where(kvp => kvp.Value == Actions.MOVE).First().Key.ToString();

            gfx.DrawString($"{moveKey} click to move", new Font("courier new", 6), Brushes.White, new Point(0, Height - 24));
            gfx.DrawString($"Hold {targetKey} to target", new Font("courier new", 6), Brushes.White, new Point(0, Height - 16));
            gfx.DrawString($"{targetKey} + {moveKey} click to attack", new Font("courier new", 6), Brushes.White, new Point(0, Height - 8));

            return bmp;
        }
    }
}