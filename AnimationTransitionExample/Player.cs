using AnimationTransitionExample.Animations;
using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using GameEngine.UI;
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

        public Hotbar Hotbar { get; private set; }

        public Player(int x, int y, int keyController, int mouseController) : base(Sprite.Sprites["player2"], x, y, 16, 16)
        {
            this.keyController = keyController;
            this.mouseController = mouseController;
            combo = new AttackCombo(3, 15);
            ////Hotbar = new Hotbar(
            ////    le => ((Player)le).ActiveSkill = (CombatSkill)SkillManager.Instance["heavy"].CreateNew(),
            ////    le => ((Player)le).ActiveSkill = (CombatSkill)SkillManager.Instance["block"].CreateNew(),
            ////    le => ((Player)le).ActiveSkill = (CombatSkill)SkillManager.Instance["counter"].CreateNew(),
            ////    le => ((Player)le).ActiveSkill = (CombatSkill)SkillManager.Instance["ranged"].CreateNew());
            Hotbar = new Hotbar(
                SkillManager.Instance["heavy"],
                SkillManager.Instance["block"],
                SkillManager.Instance["counter"],
                SkillManager.Instance["ranged"]
                );
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

            for (int i = (int)Actions.HOTBAR1; i <= (int)Actions.HOTBAR4; i++)
            {
                if (Program.Engine.Controllers[keyController][i].State == HoldState.PRESS)
                {
                    this.PreppedSkill = null;
                    this.ActiveSkill = null;
                    Hotbar.Execute(i - (int)Actions.HOTBAR1, this);
                }
            }

            ////if (Program.Engine.Controllers[keyController][(int)Actions.HOTBAR1].State == HoldState.PRESS)
            ////{
            ////    Hotbar.Execute(0, this);
            ////}
            ////else if (Program.Engine.Controllers[keyController][(int)Actions.HOTBAR2].State == HoldState.PRESS)
            ////{
            ////    Hotbar.Execute(1, this);
            ////}
            ////else if (Program.Engine.Controllers[keyController][(int)Actions.HOTBAR3].State == HoldState.PRESS)
            ////{
            ////    Hotbar.Execute(2, this);
            ////}
            ////else if (Program.Engine.Controllers[keyController][(int)Actions.HOTBAR4].State == HoldState.PRESS)
            ////{
            ////    Hotbar.Execute(3, this);
            ////}

            if (Program.Engine.Controllers[mouseController][(int)Actions.CANCEL].IsDown())
            {
                base.ActiveSkill = null;
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

                        if (ActiveSkill == null)
                        {
                            animations.Push(new AnimationChain(
                                AnimationManager.Instance[$"-sword{combo.Attack + 1}"].MakeInterruptable().MakePausing(),
                                AnimationManager.Instance[$"sword{combo.Attack + 1}"].MakeInterruptable().MakePausing(),
                                AnimationManager.Instance["move"].MakeInterruptable().Trigger(pd => ((Player)pd).Distance(target) < 20 && !target.IsBeingKnockedBack())));
                        }
                        else
                        {
                            ActiveSkill.SAction(location, this);
                        }
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

            double scale = 1;

            Skill skill = player.PreppedSkill ?? player.ActiveSkill;
            if (skill?.Name == "block")
            {
                scale = 0.5;
            }
            else if (skill?.Name == "counter")
            {
                scale = 0;
            }

            player.ChangeCoordsDelta(Math.Cos(dir) * scale, Math.Sin(dir) * scale);
        }

        public static void Swing(IDescription d)
        {
            Player player = d as Player;
            if (player == null)
            {
                return;
            }

            AnimationDistance(player, 0, 0.8, (t, s) => -(t * 2 * Math.PI) * Math.Sin(t * 2 * Math.PI) * s, Math.Max(0, player.target.Distance(player) - 8) / 5);
        }

        public static void BackSwing(IDescription d)
        {
            Player player = d as Player;
            if (player == null)
            {
                return;
            }

            AnimationDistance(player, 0.8, 1.05, (t, s) => -(t * 2 * Math.PI) * Math.Sin(t * 2 * Math.PI) * s, Math.Max(0, player.target.Distance(player) - 8) / 5);
        }

        public Bitmap Draw()
        {
            if (bmp == null)
            {
                bmp = BitmapExtensions.CreateBitmap(this.Width, this.Height);
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
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color c = bmp.GetPixel(i, j);
                        Color n = Color.FromArgb(c.A, (c.R + color.R) / 2, (c.G + color.G) / 2, (c.B + color.B) / 2);
                        bmp.SetPixel(i, j, n);
                    }
                }
            }

            return bmp;
        }
    }
}
