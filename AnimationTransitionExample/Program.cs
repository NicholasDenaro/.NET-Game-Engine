﻿using AnimationTransitionExample.Animations;
using GameEngine;
using GameEngine._2D;
using GameEngine.UI;
using GameEngine.UI.Controllers;
using GameEngine.UI.NAudio;
#if net6
using GameEngine.UI.AvaloniaUI;
#endif
#if net48
using GameEngine.UI.WinForms;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace AnimationTransitionExample
{

    public class Program
    {
        public const int TPS = 30;
        public const int SCREENWIDTH = 160;
        public const int SCREENHEIGHT = 144;

        public static long tickTime => builder.tickTime;
        public static long drawTime => builder.drawTime;
        public static long tps => builder.tps;
        public static long frameTime => builder.frameTime;
        private static GameBuilder builder;

        public const int scale = 4;

        public static GameEngine.GameEngine Engine { get; private set; }
        public static GameUI Frame { get; private set; }

        public static async Task Main(string[] args)
        {
            builder = new GameBuilder();

            (Engine, Frame) = builder
                .GameEngine(new FixedTickEngine(TPS))
                .GameView(new GameView2D(new Drawer2DAvalonia(), SCREENWIDTH, SCREENHEIGHT, scale, scale, Color.DarkSlateGray))
                .GameFrame(new GameUI(
#if net6
                    new AvaloniaWindowBuilder()
#endif
#if net48
                    new WinFormWindowBuilder()
#endif
                    , 0, 0, 160, 144, scale, scale))
                .SoundPlayer(new NAudioSoundPlayer())
                .Controller(new WindowsKeyController(keyMap.ToDictionary(kvp => (int)kvp.Key, kvp => (object)kvp.Value)))
                .Controller(new WindowsMouseController(mouseMap.ToDictionary(kvp => Convert(kvp.Key), kvp => (object)kvp.Value)))
                .StartingLocation(new Location(new Description2D(0, 0, 160, 144)))
                .Build();

            SetupAnimations();

            SetupSprites();

            SetupSkills();

            Entity marker = Marker.Create(new Marker(0, 0));
            Engine.AddEntity(0, marker);

            Random r = new Random(0);
            for (int i = 0; i < 5; i++)
            {
                Entity enemy = Enemy.Create(new Enemy(r.Next(8, SCREENWIDTH - 8), r.Next(8, SCREENHEIGHT - 8)));
                Engine.AddEntity(0, enemy);
            }

            Entity player = Player.Create(new Player(50, 50, 0, 1));
            Engine.AddEntity(0, player);

            Entity hud = Hud.Create(new Hud(0, 1, SCREENWIDTH * scale, SCREENHEIGHT * scale, scale));
            Engine.AddEntity(0, hud);

            watchSecond = new Stopwatch();
            watchSecond.Start();
            watchTickTime = new Stopwatch();

            Engine.TickEnd(0) += TickInfo;
            Engine.TickStart(0) += TickTimer;
            Engine.DrawEnd(0) += (s, v) => TickTimer(null, null);

            await Engine.Start();
        }

#if net48
        public static Dictionary<System.Windows.Forms.Keys, Actions> keyMap = new Dictionary<System.Windows.Forms.Keys, Actions>()
        {
            { System.Windows.Forms.Keys.Z, Actions.TARGET },
            { System.Windows.Forms.Keys.A, Actions.HOTBAR1 },
            { System.Windows.Forms.Keys.S, Actions.HOTBAR2 },
            { System.Windows.Forms.Keys.D, Actions.HOTBAR3 },
            { System.Windows.Forms.Keys.F, Actions.HOTBAR4 },
        };

        public static Dictionary<System.Windows.Forms.MouseButtons, Actions> mouseMap = new Dictionary<System.Windows.Forms.MouseButtons, Actions>()
        {
            { System.Windows.Forms.MouseButtons.Left, Actions.MOVE },
            { System.Windows.Forms.MouseButtons.None, Actions.MOUSEINFO },
            { System.Windows.Forms.MouseButtons.Right, Actions.CANCEL },
        };

        public static int Convert(System.Windows.Forms.MouseButtons key)
        {
            return (int)key;
        }
#endif

#if net6
        public static Dictionary<Avalonia.Input.Key, Actions> keyMap = new Dictionary<Avalonia.Input.Key, Actions>()
        {
            { Avalonia.Input.Key.Z, Actions.TARGET },
            { Avalonia.Input.Key.A, Actions.HOTBAR1 },
            { Avalonia.Input.Key.S, Actions.HOTBAR2 },
            { Avalonia.Input.Key.D, Actions.HOTBAR3 },
            { Avalonia.Input.Key.F, Actions.HOTBAR4 },
        };

        public static Dictionary<Avalonia.Input.PointerUpdateKind, Actions> mouseMap = new Dictionary<Avalonia.Input.PointerUpdateKind, Actions>()
        {
            { Avalonia.Input.PointerUpdateKind.LeftButtonPressed, Actions.MOVE },
            { Avalonia.Input.PointerUpdateKind.Other, Actions.MOUSEINFO },
            { Avalonia.Input.PointerUpdateKind.RightButtonPressed, Actions.CANCEL },
        };
        public static int Convert(Avalonia.Input.PointerUpdateKind key)
        {
            return AvaloniaWindow.Key(key);
        }
#endif

        public static Stream GetStream(string resource)
        {
            return Assembly.GetEntryAssembly().GetManifestResourceStream($"{Assembly.GetEntryAssembly().GetName().Name}.{resource.Replace("/", ".")}");
        }

        private static void SetupSkills()
        {
            new CombatSkill(
                "heavy",
                new SkillIcon(13, 3),
                LivingEntity.HeavyAttack,
                5,
                4 * Program.TPS);

            new CombatSkill(
                "block",
                new SkillIcon(5, 4),
                (l, d) => false,
                3,
                3 * Program.TPS);

            new CombatSkill(
                "counter",
                new SkillIcon(10, 10),
                (l, d) => false,
                10,
                7 * Program.TPS);

            new CombatSkill(
                "ranged",
                new SkillIcon(3, 4),
                (l, d) => false,
                5,
                2 * Program.TPS);
        }

        private static void SetupSprites()
        {
            new Sprite("marker", 4, 4);

            new Sprite("enemy", 8, 8);

            new Sprite("player", 8, 8);

            new Sprite("player2", "Sprites/player.png", 16, 16, 8, 16);
            new Sprite("enemy2", "Sprites/monster_03.png", 16, 16, 8, 16);

            new Sprite("hud", 0, 0);

            new Sprite("skills", "Sprites/skills.png", 16, 16);

            new Sprite("window", "Sprites/_sheet_window_20.png", 16, 16);

            new Sprite("bars", "Sprites/bars.png", 8, 8);

            //http://www.pentacom.jp/pentacom/bitfontmaker2/gallery/?id=102
            //AddFont("Sprites/BetterPixels.ttf");
        }

        private static void SetupAnimations()
        {
            new Animation(
                "attackmove",
                -1,
                tick: LivingEntity.AttackMove,
                final: LivingEntity.EndMove);

            new Animation(
                "move",
                -1,
                tick: LivingEntity.Move,
                final: LivingEntity.EndMove);

            new AttackAnimation(
                "punch1",
                30,
                first: LivingEntity.StartAttack,
                tick: Player.Swing,
                final: d2d => LivingEntity.Strike(d2d, false, 0, 40, 5)
                );

            new AttackAnimation(
                "-punch1",
                45,
                first: LivingEntity.StartAttack,
                tick: Player.Swing,
                final: LivingEntity.ResetToAttackPosition
                );

            new AttackAnimation(
                "sword1",
                24,
                first: LivingEntity.StartAttack,
                tick: Player.Swing,
                final: d2d => LivingEntity.Strike(d2d, false, 2, 40, 20));

            new AttackAnimation(
                "-sword1",
                6,
                tick: Player.BackSwing,
                final: d2d => { LivingEntity.Combo(d2d); LivingEntity.ResetToAttackPosition(d2d); });

            new AttackAnimation(
                "sword2",
                12,
                first: LivingEntity.StartAttack,
                tick: Player.Swing,
                final: d2d => LivingEntity.Strike(d2d, false, 2, 40, 15));

            new AttackAnimation(
                "-sword2",
                3,
                tick: Player.BackSwing,
                final: d2d => { LivingEntity.Combo(d2d); LivingEntity.ResetToAttackPosition(d2d); });

            new AttackAnimation(
                "sword3",
                20,
                first: LivingEntity.StartAttack,
                tick: Player.Swing,
                final: d2d => LivingEntity.Strike(d2d, true, 2, 80, 30));

            new AttackAnimation(
                "-sword3",
                15,
                tick: Player.BackSwing,
                final: d2d => { LivingEntity.Combo(d2d); LivingEntity.ResetToAttackPosition(d2d); });

            new Animation(
                "knockback",
                15,
                tick: LivingEntity.KnockBack);

            new Animation(
                "slideback",
                15,
                tick: LivingEntity.SlideBack);

            new Animation(
                "getup",
                15,
                final: LivingEntity.GetUp);

            new AttackAnimation(
                "bite1",
                10,
                first: LivingEntity.StartAttack,
                tick: Enemy.Bite,
                final: d2d => LivingEntity.Strike(d2d, false, 2, 40, 5));

            new AttackAnimation(
                "-bite1",
                8,
                tick: Enemy.BiteRecovery,
                final: d2d => { LivingEntity.Combo(d2d); LivingEntity.ResetToAttackPosition(d2d); });

            new AttackAnimation(
                "bite2",
                10,
                first: LivingEntity.StartAttack,
                tick: Enemy.FastBite,
                final: d2d => LivingEntity.Strike(d2d, false, 2, 40, 2));

            new AttackAnimation(
                "-bite2",
                3,
                tick: Enemy.BiteRecovery,
                final: d2d => { LivingEntity.Combo(d2d); LivingEntity.ResetToAttackPosition(d2d); });

            new AttackAnimation(
                "bite3",
                30,
                first: LivingEntity.StartAttack,
                tick: Enemy.FastBite,
                final: d2d => LivingEntity.Strike(d2d, false, 2, 40, 2));

            new AttackAnimation(
                "-bite3",
                15,
                tick: Enemy.BiteRecovery,
                final: d2d => { LivingEntity.Combo(d2d); LivingEntity.ResetToAttackPosition(d2d); });

            new AttackAnimation(
                "heavy",
                24,
                first: LivingEntity.StartAttack,
                tick: LivingEntity.HeavyAnimation,
                final: d2d => { LivingEntity.Strike(d2d, true, 0, 100, 50); LivingEntity.CombatSkillEnd(d2d); });

            new AttackAnimation(
                "-heavy",
                6,
                tick: Player.HeavyBackAnimation,
                final: d2d => { LivingEntity.Combo(d2d); LivingEntity.ResetToAttackPosition(d2d); });

            new Animation(
                "blocked",
                30);

            new Animation(
                "activateskill",
                30,
                final: LivingEntity.ActivateSkill);
        }

        private static Stopwatch watchSecond;
        private static int ticks;
        private static int hudTicks = 1;
        private static long hudMinTime;
        private static long hudMaxTime;
        private static long hudTotalTime;
        public static void TickInfo(object sender, GameState state)
        {
            ticks++;
            if (watchSecond.ElapsedMilliseconds >= 1000)
            {
                Console.WriteLine($"TPS: {ticks} | Timings: min: {minTime} avg: {totalTime / ticks} max: {maxTime} avg remaining: {((int)TimeSpan.FromSeconds(1).Ticks - TPS) - (totalTime / ticks)}");

                hudTicks = ticks;
                hudMinTime = minTime;
                hudMaxTime = maxTime;
                hudTotalTime = totalTime;

                ticks = 0;
                minTime = long.MaxValue;
                maxTime = long.MinValue;
                totalTime = 0;
                watchSecond.Restart();
            }
        }

        private static Stopwatch watchTickTime;
        private static long minTime;
        private static long maxTime;
        private static long totalTime;
        public static void TickTimer(object sender, GameState state)
        {
            if (watchTickTime.IsRunning)
            {
                long time = watchTickTime.ElapsedTicks;
                totalTime += time;
                if (time < minTime)
                {
                    minTime = time;
                }
                if (time > maxTime)
                {
                    maxTime = time;
                }
                watchTickTime.Stop();
            }
            else
            {
                watchTickTime.Restart();
            }
        }
    }

    public enum Actions { UP = 0, DOWN = 1, LEFT = 2 , RIGHT = 3, MOVE = 4, TARGET = 5, MOUSEINFO = 6, HOTBAR1 = 7, HOTBAR2 = 8, HOTBAR3 = 9, HOTBAR4 = 10, CANCEL = 11 };
}
