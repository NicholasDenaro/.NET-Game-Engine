using GameEngine;
using GameEngine._2D;
using GameEngine.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AnimationTransitionExample
{
    public class Hud : Description2D
    {
        int keyController;
        int mouseController;
        private Bitmap bmp;
        private Graphics gfx;

        private int windowScale;

        public Hud (int keyControllerIndex, int mouseControllerIndex, int width, int height, int scale) : base(Sprite.Sprites["hud"], 0, 0, width, height)
        {
            keyController = keyControllerIndex;
            mouseController = mouseControllerIndex;
            DrawInOverlay = true;
            this.windowScale = scale;
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
                bmp = Bitmap.Create(this.Width, this.Height, true);
                gfx = bmp.GetGraphics();
            }

            int size = 18;
            gfx.Clear(Color.Transparent);
            using (gfx.ScaleTransform(windowScale, windowScale))
            {
                Player player = Program.Engine.Location(0).GetEntities<Player>().FirstOrDefault();

                DrawEntityHealth(gfx);

                DrawSkills(gfx, Program.Engine.Location(0).GetEntities<LivingEntity>());

                DrawPlayerSkill(gfx, player);

                DrawTargetLine(gfx, player);

                DrawHotbar(gfx, player, size);
                DrawPlayerHud(gfx, player, size);
            }

            DrawDevInfo(gfx, size);
            return bmp;
        }

        private void DrawDevInfo(Graphics gfx, int size)
        {
            string targetKey = Program.keyMap.First(kvp => kvp.Value == Actions.TARGET).Key.ToString();
            string moveKey = Program.mouseMap.First(kvp => kvp.Value == Actions.MOVE).Key.ToString();

            MouseControllerInfo movepmci = (Program.Engine.Controllers(0)[mouseController][Actions.MOVE].Info as MouseControllerInfo);
            MouseControllerInfo mouseInfopmci = (Program.Engine.Controllers(0)[mouseController][Actions.MOUSEINFO].Info as MouseControllerInfo);

            gfx.FillRectangle(Color.Black, 0, 0, Width, 52);
            gfx.DrawText($"{Program.tickTime:0000000000}\t{movepmci?.X ?? 0:000},{movepmci?.Y ?? 0:000}\t{mouseInfopmci?.X ?? 0:000},{mouseInfopmci?.Y ?? 0:000}", new Point(0, 0), Color.White, size);
            gfx.DrawText($"{Program.drawTime:0000000000}", new Point(0, 16), Color.White, size);
            gfx.DrawText($"{Program.frameTime:0000000000} | {Program.tps} | {(Program.tickTime + Program.drawTime) * 100.0 / (Stopwatch.Frequency / Program.TPS):000}%", new Point(0, 32), Color.White, size);

            gfx.FillRectangle(Color.Black, 0, Height - 52, Width, 52);
            gfx.DrawText($"{moveKey} click to move", new Point(0, Height - 52), Color.White, size);
            gfx.DrawText($"Hold {targetKey} to target", new Point(0, Height - 36), Color.White, size);
            gfx.DrawText($"{targetKey} + {moveKey} click to attack", new Point(0, Height - 20), Color.White, size);
        }

        private void DrawTargetLine(Graphics gfx, Player player)
        {
            LivingEntity le = player.LockTarget;

            if (le != null && Program.Engine.Controllers(0)[keyController][Actions.TARGET].IsDown())
            {
                gfx.DrawEllipse(Color.Cyan, (int)le.X - le.Sprite.X - 2, (int)le.Y - le.Sprite.Y - 2, le.Width + 4, le.Height + 4);
                MouseControllerInfo mci;
                if (Program.Engine.Controllers(0)[mouseController][Actions.MOVE].IsDown())
                {
                    mci = Program.Engine.Controllers(0)[mouseController][Actions.MOVE].Info as MouseControllerInfo;
                }
                else
                {
                    mci = Program.Engine.Controllers(0)[mouseController][Actions.MOUSEINFO].Info as MouseControllerInfo;
                }

                Point center = new Point((int)(le.X - le.Sprite.X - 2 + (le.Width + 4) / 2), (int)(le.Y - le.Sprite.Y - 2 + (le.Height + 4) / 2));
                gfx.DrawLine(Color.Cyan, mci.X, mci.Y, center.X, center.Y);
            }

            if (player.Target != null)
            {
                le = player.Target;
                gfx.DrawArc(Color.Red, (float)le.X - 4, (float)le.Y - 3, 8, 4, 180, -180);
            }
        }

        private void DrawSkills(Graphics gfx, IEnumerable<LivingEntity> entities)
        {
            foreach (LivingEntity entity in entities)
            {
                Skill skill = entity.PreppedSkill ?? entity.ActiveSkill;
                if (skill != null)
                {
                    float scale = 1;
                    if (skill == entity.PreppedSkill)
                    {
                        scale = (AnimationManager.Instance["activateskill"].Duration - entity.SkillActivationTime) * 1.0f / AnimationManager.Instance["activateskill"].Duration / 2 + 0.6f;                    
                    }

                    gfx.DrawImage(skill.Icon.Image(), new Rectangle((int)entity.X - 2, (int)entity.Y - 24, (int)(8 * scale), (int)(8 * scale)));
                }
            }
        }

        private void DrawPlayerSkill(Graphics gfx, Player player)
        {
            Skill skill = player.PreppedSkill ?? player.ActiveSkill;
            if (skill != null)
            {
                float scale = 1;
                if (skill == player.PreppedSkill)
                {
                    scale = (AnimationManager.Instance["activateskill"].Duration - player.SkillActivationTime) * 1.0f / AnimationManager.Instance["activateskill"].Duration / 2 + 0.6f;
                }
                if (skill == player.ActiveSkill)
                {
                    Color outline = Color.Yellow;
                    gfx.DrawRectangle(outline, (int)(player.X - 2.25f), (int)(player.Y - 24.25f), (int)8.25f, (int)8.25f);
                }
                gfx.DrawImage(skill.Icon.Image(), new Rectangle((int)player.X - 2, (int)player.Y - 24, (int)(8 * scale), (int)(8 * scale)));
            }
        }

        private void DrawEntityHealth(Graphics gfx)
        {
            foreach (LivingEntity e in Program.Engine.Location(0).GetEntities<LivingEntity>())
            {
                gfx.FillRectangle(Color.White, new Rectangle(e.Position.X - 10, e.Position.Y + 2, 20, 4));
                gfx.FillRectangle(Color.MediumPurple, new Rectangle(e.Position.X - 9, e.Position.Y + 3, 18 * e.balance / 100, 2));
                gfx.FillRectangle(Color.White, new Rectangle(e.Position.X - 10, e.Position.Y + 5, 20, 4));
                gfx.FillRectangle(Color.IndianRed, new Rectangle(e.Position.X - 9, e.Position.Y + 6, 18 * e.health / 100, 2));
            }
        }

        private void DrawHotbar(Graphics gfx, Player player, int size)
        {
            gfx.DrawImage(player.Hotbar.Image(), 0, bmp.Height / 4 - 16 * 2);

            using (gfx.ScaleTransform(1.0f / windowScale, 1.0f / windowScale))
            {
                for (int i = (int)Actions.HOTBAR1; i <= (int)Actions.HOTBAR4; i++)
                {
                    string key = Program.keyMap.First(kvp => (int)kvp.Value == i).Key.ToString();
                    gfx.FillRectangle(Color.Black, 16 * 4 * (i - (int)Actions.HOTBAR1), bmp.Height - 16 * 4 * 2, size, size);
                    gfx.DrawText(key, 16 * 4 * (i - (int)Actions.HOTBAR1), bmp.Height - 16 * 4 * 2, Color.White, size);
                }
            }
        }

        private void DrawPlayerHud(Graphics gfx, Player player, int size)
        {
            gfx.FillRectangle(Color.DarkSlateGray, 0, bmp.Height / 4 - 16, bmp.Width / 4, size);
            using (gfx.ScaleTransform(1.0f / windowScale, 1.0f / windowScale))
            {
                // HP
                gfx.DrawText("HP", 8, bmp.Height - 16 * 4 - 4, Color.White, size);
            }

            DrawBar(gfx, player.health / 100f, 8, bmp.Height / 4 - 16 - 2, 5, 0);

            using (gfx.ScaleTransform(1.0f / windowScale, 1.0f / windowScale))
            {
                //StringFormat format = new StringFormat();
                //format.Alignment = StringAlignment.Center;
                gfx.DrawText($"{player.health:0}/{100}", 8 + 8 * 6 * 4 / 2, bmp.Height - 16 * 4 - 4, Color.White, size);

                // Stamina
                gfx.DrawText("SP", 8 + 8 * 6 * 4, bmp.Height - 16 * 4 - 4, Color.White, size);
            }

            DrawBar(gfx, player.stamina / 30, 8 + 8 * 6, bmp.Height / 4 - 16 - 2, 5, 1);

            using (gfx.ScaleTransform(1.0f / windowScale, 1.0f / windowScale))
            {
                gfx.DrawText($"{player.stamina:0}/{30}", 8 + 8 * 6 * 4 + 8 * 6 * 4 / 2, bmp.Height - 16 * 4 - 4, Color.Black, size);
            }
        }

        private void DrawBar(Graphics gfx, float percent, float x, float y, int segments, int imageIndex)
        {
            // TODO: Fix this for bars of size <= 2
            Sprite s = Sprite.Sprites["bars"];
            float i = 0;
            float max = 7.5f * segments;
            float incr = 7.5f;
            float over;

            gfx.DrawImage(s.GetImage(0 + imageIndex * 6), (int)(x + i), (int)y);
            if (percent > i / max)
            {
                over = Math.Min(percent - i / max, (1.0f / segments)) / (1.0f / segments) * 8;
                gfx.DrawImage(s.GetImage(3 + imageIndex * 6), new RectangleF(x + i, y, over, 8));
            }

            for (i = incr; i < max - incr - 1; i += incr)
            {
                gfx.DrawImage(s.GetImage(1 + imageIndex * 6), (int)(x + i), (int)y);
                if (percent > i / max)
                {
                    over = Math.Min(percent - i / max, (1.0f / segments)) / (1.0f / segments) * 8;
                    gfx.DrawImage(s.GetImage(4 + imageIndex * 6), new RectangleF(x + i, y, over, 8));
                }
            }

            gfx.DrawImage(s.GetImage(2 + imageIndex * 6), (int)(x + i), (int)y);
            if (percent >= i / max)
            {
                over = Math.Min(percent - i / max, (1.0f / segments)) / (1.0f / segments) * 8;
                gfx.DrawImage(s.GetImage(5 + imageIndex * 6), new RectangleF(x + i, y, over, 8));
            }
        }
    }
}
