using GameEngine._2D;
using System;
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

        public HotbarAction this[int i]
        {
            get
            {
                return actions[i];
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
                bmp = Bitmap.Create(actions.Length * 16, 16);
                gfx = bmp.GetGraphics();
            }

            Sprite window = Sprite.Sprites["window"];
            Color transparentBlack = new Color(0, 0, 0, 255 / 2);
            //using (gfx.ScaleTransform(1, 1))
            {
                gfx.DrawImage(window.GetImage(0 + 3 * window.HImages), 0, 0);
                if (actions.First() != null)
                {
                    gfx.DrawImage(actions.First().Image(), 0, 0);
                    Skill skill = actions.First() as Skill;
                    if (skill != null)
                    {
                        float percentCooldown = skill.CooldownTime * 1.0f / skill.CooldownDuration;
                        gfx.FillRectangle(transparentBlack, 0, (int)((1 - percentCooldown) * 16), 16, (int)(percentCooldown * 16));
                    }
                }

                for (int i = 1; i < actions.Length - 1; i++)
                {
                    gfx.DrawImage(window.GetImage(1 + 3 * window.HImages), i * 16, 0);
                    if (actions[i] != null)
                    {
                        gfx.DrawImage(actions[i].Image(), i * 16, 0);
                        Skill skill = actions[i] as Skill;
                        if (skill != null)
                        {
                            float percentCooldown = skill.CooldownTime * 1.0f / skill.CooldownDuration;
                            gfx.FillRectangle(transparentBlack, i * 16, (int)((1 - percentCooldown) * 16), 16, (int)(percentCooldown * 16));
                        }
                    }
                }

                gfx.DrawImage(window.GetImage(2 + 3 * window.HImages), bmp.Width - 16, 0);
                if (actions.Last() != null)
                {
                    gfx.DrawImage(actions.Last().Image(), bmp.Width - 16, 0);
                    Skill skill = actions.Last() as Skill;
                    if (skill != null)
                    {
                        float percentCooldown = skill.CooldownTime * 1.0f / skill.CooldownDuration;
                        gfx.FillRectangle(transparentBlack, bmp.Width - 16, (int)((1 - percentCooldown) * 16), 16, (int)(percentCooldown * 16));
                    }
                }
            }

            return bmp;
        }
    }
}
