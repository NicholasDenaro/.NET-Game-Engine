using GameEngine;
using GameEngine._2D;
using GameEngine.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
                bmp = BitmapExtensions.CreateBitmap(this.Width, this.Height);
                gfx = Graphics.FromImage(bmp);
            }

            gfx.Clear(Color.Transparent);
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            gfx.ScaleTransform(4, 4);

            Font f = new Font(Program.FontCollection.Families[0], 18);
            Player player = Program.Engine.Location.GetEntities<Player>().FirstOrDefault();

            DrawEntityHealth(gfx);

            DrawSkills(gfx, Program.Engine.Location.GetEntities<LivingEntity>());

            DrawPlayerSkill(gfx, player);

            DrawTargetLine(gfx, player);

            DrawHotbar(gfx, player, f);
            DrawPlayerHud(gfx, player, f);

            gfx.ScaleTransform((float)0.25, (float)0.25);

            DrawDevInfo(gfx, f);
            string targetKey = Program.keyMap.First(kvp => kvp.Value == Actions.TARGET).Key.ToString();
            string moveKey = Program.mouseMap.First(kvp => kvp.Value == Actions.MOVE).Key.ToString();

            MouseControllerInfo movepmci = (Program.Engine.Controllers[mouseController][(int)Actions.MOVE].Info as MouseControllerInfo);
            MouseControllerInfo mouseInfopmci = (Program.Engine.Controllers[mouseController][(int)Actions.MOUSEINFO].Info as MouseControllerInfo);

            gfx.FillRectangle(Brushes.Black, 0, 0, Width, 52);
            gfx.DrawString($"{Program.tickTime}\t{movepmci?.X ?? 0:000},{movepmci?.Y ?? 0:000}\t{mouseInfopmci?.X ?? 0:000},{mouseInfopmci?.Y ?? 0:000}", f, Brushes.White, new Point(0, 0));
            gfx.DrawString($"{Program.drawTime}", f, Brushes.White, new Point(0, 16));
            gfx.DrawString($"{Program.tps} | {(Program.tickTime + Program.drawTime) * 100.0 / (TimeSpan.FromSeconds(1).Ticks/Program.TPS):##}%", f, Brushes.White, new Point(0, 32));

            gfx.FillRectangle(Brushes.Black, 0, Height - 52, Width, 52);
            gfx.DrawString($"{moveKey} click to move", f, Brushes.White, new Point(0, Height - 52));
            gfx.DrawString($"Hold {targetKey} to target", f, Brushes.White, new Point(0, Height - 36));
            gfx.DrawString($"{targetKey} + {moveKey} click to attack", f, Brushes.White, new Point(0, Height - 20));

            return bmp;
        }

        private void DrawDevInfo(Graphics gfx, Font f)
        {
            string targetKey = Program.keyMap.First(kvp => kvp.Value == Actions.TARGET).Key.ToString();
            string moveKey = Program.mouseMap.First(kvp => kvp.Value == Actions.MOVE).Key.ToString();

            MouseControllerInfo movepmci = (Program.Engine.Controllers[mouseController][(int)Actions.MOVE].Info as MouseControllerInfo);
            MouseControllerInfo mouseInfopmci = (Program.Engine.Controllers[mouseController][(int)Actions.MOUSEINFO].Info as MouseControllerInfo);

            gfx.FillRectangle(Brushes.Black, 0, 0, Width, 52);
            gfx.DrawString($"{Program.tickTime}\t{movepmci?.X ?? 0:000},{movepmci?.Y ?? 0:000}\t{mouseInfopmci?.X ?? 0:000},{mouseInfopmci?.Y ?? 0:000}", f, Brushes.White, new Point(0, 0));
            gfx.DrawString($"{Program.drawTime}", f, Brushes.White, new Point(0, 16));
            gfx.DrawString($"{Program.tps} | {(Program.tickTime + Program.drawTime) * 100.0 / (TimeSpan.FromSeconds(1).Ticks / Program.TPS):##}%", f, Brushes.White, new Point(0, 32));

            gfx.FillRectangle(Brushes.Black, 0, Height - 52, Width, 52);
            gfx.DrawString($"{moveKey} click to move", f, Brushes.White, new Point(0, Height - 52));
            gfx.DrawString($"Hold {targetKey} to target", f, Brushes.White, new Point(0, Height - 36));
            gfx.DrawString($"{targetKey} + {moveKey} click to attack", f, Brushes.White, new Point(0, Height - 20));
        }

        private void DrawTargetLine(Graphics gfx, Player player)
        {
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

            if (player.Target != null)
            {
                le = player.Target;
                gfx.DrawArc(Pens.Red, (float)le.X - 4, (float)le.Y - 3, 8, 4, 180, -180);
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

                    gfx.DrawImage(skill.Icon.Image(), (int)entity.X - 2, (int)entity.Y - 24, 8 * scale, 8 * scale);
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
                    Pen outline = new Pen(Brushes.Yellow);
                    outline.Width = 0.25f;
                    gfx.DrawRectangle(outline, (int)player.X - 2.25f, (int)player.Y - 24.25f, 8.25f, 8.25f);
                }
                gfx.DrawImage(skill.Icon.Image(), (int)player.X - 2, (int)player.Y - 24, 8 * scale, 8 * scale);
            }
        }

        private void DrawEntityHealth(Graphics gfx)
        {
            foreach (LivingEntity e in Program.Engine.Location.GetEntities<LivingEntity>())
            {
                gfx.FillRectangle(Brushes.White, new Rectangle(e.Position.X - 10, e.Position.Y + 2, 20, 4));
                gfx.FillRectangle(Brushes.MediumPurple, new Rectangle(e.Position.X - 9, e.Position.Y + 3, 18 * e.balance / 100, 2));
                gfx.FillRectangle(Brushes.White, new Rectangle(e.Position.X - 10, e.Position.Y + 5, 20, 4));
                gfx.FillRectangle(Brushes.IndianRed, new Rectangle(e.Position.X - 9, e.Position.Y + 6, 18 * e.health / 100, 2));
            }
        }

        private void DrawHotbar(Graphics gfx, Player player, Font f)
        {
            gfx.DrawImage(player.Hotbar.Image(), 0, bmp.Height / 4 - 16 * 2);

            gfx.ScaleTransform(0.25f, 0.25f);

            for (int i = (int)Actions.HOTBAR1; i <= (int)Actions.HOTBAR4; i++)
            {
                string key = Program.keyMap.First(kvp => (int)kvp.Value == i).Key.ToString();
                gfx.FillRectangle(Brushes.Black, 16 * 4 * (i - (int)Actions.HOTBAR1), bmp.Height - 16 * 4 * 2, f.Size, f.Size);
                gfx.DrawString(key, f, Brushes.White, 16 * 4 * (i - (int)Actions.HOTBAR1), bmp.Height - 16 * 4 * 2);
            }

            gfx.ScaleTransform(4, 4);
        }

        private void DrawPlayerHud(Graphics gfx, Player player, Font f)
        {
            gfx.FillRectangle(Brushes.DarkSlateGray, 0, bmp.Height / 4 - 16, bmp.Width / 4, f.Size);
            gfx.ScaleTransform(0.25f, 0.25f);
            // HP
            gfx.DrawString("HP", f, Brushes.White, 8, bmp.Height - 16 * 4 - 4);
            gfx.ScaleTransform(4, 4);

            DrawBar(gfx, player.health / 100f, 8, bmp.Height / 4 - 16 - 2, 5, 0);

            gfx.ScaleTransform(0.25f, 0.25f);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            gfx.DrawString($"{player.health:0}/{100}", f, Brushes.White, 8 + 8 * 6 * 4 / 2, bmp.Height - 16 * 4 - 4, format);

            // Stamina
            gfx.DrawString("SP", f, Brushes.White, 8 + 8 * 6 * 4, bmp.Height - 16 * 4 - 4);
            gfx.ScaleTransform(4, 4);

            DrawBar(gfx, player.stamina / 30, 8 + 8 * 6, bmp.Height / 4 - 16 - 2, 5, 1);

            gfx.ScaleTransform(0.25f, 0.25f);
            gfx.DrawString($"{player.stamina:0}/{30}", f, Brushes.Black, 8 + 8 * 6 * 4 + 8 * 6 * 4 / 2, bmp.Height - 16 * 4 - 4, format);
            gfx.ScaleTransform(4, 4);
        }

        private void DrawBar(Graphics gfx, float percent, float x, float y, int segments, int imageIndex)
        {
            // TODO: Fix this for bars of size <= 2
            Sprite s = Sprite.Sprites["bars"];
            float i = 0;
            float max = 7.5f * segments;
            float incr = 7.5f;
            float over;

            gfx.DrawImage(s.GetImage(0 + imageIndex * 6), x + i, y);
            if (percent > i / max)
            {
                over = Math.Min(percent - i / max, (1.0f / segments)) / (1.0f / segments) * 8;
                gfx.DrawImage(s.GetImage(3 + imageIndex * 6), new RectangleF(x + i, y, over, 8), new RectangleF(0, 0, over, 8), GraphicsUnit.Pixel);
            }

            for (i = incr; i < max - incr - 1; i += incr)
            {
                gfx.DrawImage(s.GetImage(1 + imageIndex * 6), x + i, y);
                if (percent > i / max)
                {
                    over = Math.Min(percent - i / max, (1.0f / segments)) / (1.0f / segments) * 8;
                    gfx.DrawImage(s.GetImage(4 + imageIndex * 6), new RectangleF(x + i, y, over, 8), new RectangleF(0, 0, over, 8), GraphicsUnit.Pixel);
                }
            }

            gfx.DrawImage(s.GetImage(2 + imageIndex * 6), x + i, y);
            if (percent >= i / max)
            {
                over = Math.Min(percent - i / max, (1.0f / segments)) / (1.0f / segments) * 8;
                gfx.DrawImage(s.GetImage(5 + imageIndex * 6), new RectangleF(x + i, y, over, 8), new RectangleF(0, 0, over, 8), GraphicsUnit.Pixel);
            }
        }
    }
}
