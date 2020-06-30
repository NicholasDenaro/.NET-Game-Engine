using AnimationTransitionExample.Animations;
using GameEngine;
using GameEngine._2D;
using GameEngine.Windows;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AnimationTransitionExample
{

    public class Program
    {
        public const int TPS = 30;
        public const int SCREENWIDTH = 160;
        public const int SCREENHEIGHT = 144;

        public static GameEngine.GameEngine Engine { get; private set; }
        public static void Main(string[] args)
        {
            Engine = new FixedTickEngine(TPS);
            GameView2D view = new GameView2D(SCREENWIDTH, SCREENHEIGHT, Color.DarkSlateGray);
            Engine.View = view;

            GameFrame frame = new GameFrame(0, 0, 160, 144, 4, 4);
            frame.Start();
            Engine.DrawEnd += frame.Pane.DrawHandle;

            Location location = new Location(new Description2D(0, 0, 100, 100));
            Engine.SetLocation(location);

            WindowsKeyController keyController = new WindowsKeyController(keyMap.ToDictionary(kvp => (int)kvp.Key, kvp => (int)kvp.Value));
            keyController.Hook(frame);
            Engine.AddController(keyController);

            WindowsMouseController mouseController = new WindowsMouseController(mouseMap.ToDictionary(kvp => (int)kvp.Key, kvp => (int)kvp.Value));
            mouseController.Hook(frame);
            Engine.AddController(mouseController);

            SetupAnimations();

            SetupSprites();

            Entity marker = Marker.Create(new Marker(0, 0));
            Engine.AddEntity(marker);

            Entity enemy = Enemy.Create(new Enemy(64, 64));
            Engine.AddEntity(enemy);

            Entity player = Player.Create(new Player(50, 50, Engine.GetControllerIndex(keyController), Engine.GetControllerIndex(mouseController)));
            Engine.AddEntity(player);

            Entity hud = Hud.Create(new Hud(Engine.GetControllerIndex(keyController), Engine.GetControllerIndex(mouseController), SCREENWIDTH, SCREENHEIGHT));
            Engine.AddEntity(hud);

            Engine.Start();

            while (true) { };
        }

        public static Dictionary<System.Windows.Forms.Keys, Actions> keyMap = new Dictionary<System.Windows.Forms.Keys, Actions>()
        {
            { System.Windows.Forms.Keys.Z, Actions.TARGET },
        };

        public static Dictionary<System.Windows.Forms.MouseButtons, Actions> mouseMap = new Dictionary<System.Windows.Forms.MouseButtons, Actions>()
        {
            { System.Windows.Forms.MouseButtons.Left, Actions.MOVE },
            { System.Windows.Forms.MouseButtons.None, Actions.MOUSEINFO },
        };

        private static void SetupSprites()
        {
            new Sprite("marker", 4, 4);

            new Sprite("enemy", 8, 8);

            new Sprite("player", 8, 8);

            new Sprite("hud", 0, 0);
        }

        private static void SetupAnimations()
        {
            new Animation(
                "move",
                -1,
                tick: LivingEntity.Move);

            new Animation(
                "playermove",
                -1,
                tick: Player.PlayerMove);

            new AttackAnimation(
                "sword1",
                24,
                first: LivingEntity.StartAttack,
                tick: Player.Swing,
                final: d2d => LivingEntity.Strike(d2d, false, 40, 20));

            new Animation(
                "-sword1",
                6,
                tick: Player.BackSwing,
                final: d2d => { Player.BackSwing(d2d); LivingEntity.Combo(d2d); });

            new AttackAnimation(
                "sword2",
                12,
                first: LivingEntity.StartAttack,
                tick: Player.Swing,
                final: d2d => LivingEntity.Strike(d2d, false, 40, 15));

            new Animation(
                "-sword2",
                3,
                tick: Player.BackSwing,
                final: d2d => { Player.BackSwing(d2d); LivingEntity.Combo(d2d); });

            new AttackAnimation(
                "sword3",
                24,
                first: LivingEntity.StartAttack,
                tick: Player.Swing,
                final: d2d => LivingEntity.Strike(d2d, true, 80, 30));

            new Animation(
                "-sword3",
                6,
                tick: Player.BackSwing,
                final: d2d => { Player.BackSwing(d2d); LivingEntity.Combo(d2d); });

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
                30,
                first: LivingEntity.StartAttack,
                tick: Enemy.Bite,
                final: d2d => LivingEntity.Strike(d2d, false, 40, 5));

            new Animation(
                "-bite1",
                8,
                tick: Enemy.BiteRecovery,
                final: d2d => { Enemy.BiteRecovery(d2d); LivingEntity.Combo(d2d); });

            new AttackAnimation(
                "bite2",
                12,
                first: LivingEntity.StartAttack,
                tick: Enemy.FastBite,
                final: d2d => LivingEntity.Strike(d2d, false, 40, 2));

            new Animation(
                "-bite2",
                3,
                tick: Enemy.BiteRecovery,
                final: d2d => { Enemy.BiteRecovery(d2d); LivingEntity.Combo(d2d); });

            new AttackAnimation(
                "bite3",
                12,
                first: LivingEntity.StartAttack,
                tick: Enemy.FastBite,
                final: d2d => LivingEntity.Strike(d2d, false, 40, 2));

            new Animation(
                "-bite3",
                15,
                tick: Enemy.BiteRecovery,
                final: d2d => { Enemy.BiteRecovery(d2d); LivingEntity.Combo(d2d); });
        }
    }

    public enum Actions { UP = 0, DOWN = 1, LEFT = 2 , RIGHT = 3, MOVE = 4, TARGET = 5, MOUSEINFO = 6 };
}
