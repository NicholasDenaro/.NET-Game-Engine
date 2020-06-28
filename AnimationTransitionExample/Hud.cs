using GameEngine;
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

            Enemy enemy = Program.Engine.Location.GetEntities<Enemy>().First();

            if (Program.Engine.Controllers[keyController][(int)Actions.ALT].IsDown())
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

            return bmp;
        }
    }
}
