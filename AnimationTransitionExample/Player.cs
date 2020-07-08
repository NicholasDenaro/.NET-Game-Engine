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
            SkillBook = new SkillBook(
                SkillManager.Instance["heavy"].CreateNew(),
                SkillManager.Instance["block"].CreateNew(),
                SkillManager.Instance["counter"].CreateNew(),
                SkillManager.Instance["ranged"].CreateNew());
            Hotbar = new Hotbar(
                SkillBook["heavy"],
                SkillBook["block"],
                SkillBook["counter"],
                SkillBook["ranged"]
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

            if (Program.Engine.Controllers[keyController][(int)Actions.TARGET].State == HoldState.PRESS)
            {
                LockTarget = GetLivingEntityNearestMouse(location);
            }
            else if (Program.Engine.Controllers[keyController][(int)Actions.TARGET].State == HoldState.RELEASE)
            {
                LockTarget = null;
            }

            for (int i = (int)Actions.HOTBAR1; i <= (int)Actions.HOTBAR4; i++)
            {
                if (Program.Engine.Controllers[keyController][i].State == HoldState.PRESS)
                {
                    Skill skill = Hotbar[i - (int)Actions.HOTBAR1] as Skill;
                    if (skill != null && skill.CooldownTime == 0)
                    {
                        stamina -= skill.Stamina;
                        this.PreppedSkill = null;
                        this.ActiveSkill = null;
                        Hotbar.Execute(i - (int)Actions.HOTBAR1, this);
                    }
                }
            }

            MouseControllerInfo mci;
            MouseControllerInfo info = Program.Engine.Controllers[mouseController][(int)Actions.CANCEL].Info as MouseControllerInfo;
            

            if (Program.Engine.Controllers[mouseController][(int)Actions.CANCEL].IsDown())
            {
                if (info.X > X - 4 && info.X < X + 4 && info.Y > Y - 24 && info.Y < Y - 16)
                {
                    if (Program.Engine.Controllers[mouseController][(int)Actions.CANCEL].IsPress())
                    {
                        base.CancelSkill();
                    }
                }
                else
                {
                    if (Program.Engine.Controllers[keyController][(int)Actions.TARGET].IsDown())
                    {
                        if (LockTarget != null)
                        {
                            markerD.SetCoords(LockTarget.X, LockTarget.Y);

                            if (animations.Any() && animations.Peek().Peek().Name.Contains("move"))
                            {
                                animations.Pop();
                            }

                            Attack(LockTarget, location, "sword");
                        }
                    }
                    else
                    {
                        LivingEntity selected = GetLivingEntityAtMouse(location);
                        if (selected != null)
                        {
                            if (animations.Any() && animations.Peek().Peek().Name.Contains("move"))
                            {
                                animations.Pop();
                            }

                            Attack(selected, location, "sword");
                        }
                    }
                }
            }

            if (Program.Engine.Controllers[mouseController][(int)Actions.MOVE].IsDown())
            {
                mci = Program.Engine.Controllers[mouseController][(int)Actions.MOVE].Info as MouseControllerInfo;
                Point p = new Point(mci.X, mci.Y);

                if (p.Y > Program.SCREENHEIGHT - 16 * 2 && p.Y < Program.SCREENHEIGHT - 16)
                {
                    int i = p.X / 16;
                    if (Program.Engine.Controllers[mouseController][(int)Actions.MOVE].IsPress())
                    {
                        Skill skill = Hotbar[i] as Skill;
                        if (skill != null && skill.CooldownTime == 0 && stamina >= skill.Stamina)
                        {
                            stamina -= skill.Stamina;
                            this.PreppedSkill = null;
                            this.ActiveSkill = null;
                            Hotbar.Execute(i, this);
                        }
                    }
                }
                else
                {
                    while (animations.Any() && animations.Peek().Peek().Name.Contains("move"))
                    {
                        animations.Pop();
                    }

                    markerD.SetCoords(p.X, p.Y);
                    this.MoveTarget = p;
                    animations.Push(new AnimationChain(
                        AnimationManager.Instance["move"].MakeInterruptable().Trigger(pd => ((Player)pd).Distance(((Player)pd).MoveTarget) < 1)));
                    combo.Reset();
                }
            }
        }

        private LivingEntity GetLivingEntityAtMouse(Location location)
        {
            MouseControllerInfo info = Program.Engine.Controllers[mouseController][(int)Actions.CANCEL].Info as MouseControllerInfo;
            double dist = double.MaxValue;
            return location.GetEntities<Enemy>().Where(e =>
            {
                double d = e.Distance(info.X, info.Y + 8);

                if (d < dist)
                {
                    dist = d;
                }

                return d < 8;
            }).LastOrDefault();
        }

        private LivingEntity GetLivingEntityNearestMouse(Location location)
        {
            MouseControllerInfo mci = Program.Engine.Controllers[mouseController][(int)Actions.MOUSEINFO].Info as MouseControllerInfo;
            Enemy nearest = null;
            return location.GetEntities<Enemy>().Where(
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

        public static void Swing(IDescription d)
        {
            Player player = d as Player;
            if (player == null)
            {
                return;
            }

            AnimationDistance(player, 0, 0.8, (t, s) => -(t * 2 * Math.PI) * Math.Sin(t * 2 * Math.PI) * s, Math.Max(0, player.Target.Distance(player) - 8) / 5);
        }

        public static void BackSwing(IDescription d)
        {
            Player player = d as Player;
            if (player == null)
            {
                return;
            }

            AnimationDistance(player, 0.8, 1.05, (t, s) => -(t * 2 * Math.PI) * Math.Sin(t * 2 * Math.PI) * s, Math.Max(0, player.Target.Distance(player) - 8) / 5);
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

            if (stun > 0 || animations.Any() && animations.Peek().Peek().Name == "blocked")
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
