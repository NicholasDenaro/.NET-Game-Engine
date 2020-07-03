using GameEngine._2D;
using System;
using System.Drawing;
using System.Linq;

namespace AnimationTransitionExample
{
    public class Hotbar
    {
        private HotbarAction[] actions;

        private Bitmap bmp;
        private Graphics gfx;

        public Hotbar(params HotbarAction[] acts)
        {
            actions = new HotbarAction[Math.Max(acts.Length, 10)];
            for (int i = 0; i < acts.Length; i++)
            {
                actions[i] = acts[i];
            }
        }

        public void Execute(int action, LivingEntity entity)
        {
            actions[action]?.Action(entity);
        }

        public Bitmap Image()
        {
            if (bmp == null)
            {
                bmp = BitmapExtensions.CreateBitmap(actions.Length * 16, 16);
                gfx = Graphics.FromImage(bmp);
            }

            Sprite window = Sprite.Sprites["window"];

            gfx.DrawImage(window.GetImage(0 + 3 * window.HImages), 0, 0);
            if (actions.First() != null)
            {
                gfx.DrawImage(actions.First().Image(), 0, 0);
            }

            for (int i = 1; i < actions.Length - 1; i++)
            {
                gfx.DrawImage(window.GetImage(1 + 3 * window.HImages), i * 16, 0);
                if (actions[i] != null)
                {
                    gfx.DrawImage(actions[i].Image(), i * 16, 0);
                }
            }

            gfx.DrawImage(window.GetImage(2 + 3 * window.HImages), bmp.Width - 16, 0);
            if (actions.Last() != null)
            {
                gfx.DrawImage(actions.Last().Image(), bmp.Width - 16, 0);
            }

            return bmp;
        }
    }
}
