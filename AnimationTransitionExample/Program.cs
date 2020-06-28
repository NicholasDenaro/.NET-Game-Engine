using GameEngine;
using GameEngine._2D;
using GameEngine.Windows;
using System.Collections.Generic;

namespace AnimationTransitionExample
{

    public class Program
    {
        public const int SCREENWIDTH = 160;
        public const int SCREENHEIGHT = 144;

        public static GameEngine.GameEngine Engine { get; private set; }
        public static void Main(string[] args)
        {
            Engine = new FixedTickEngine(30);
            GameView2D view = new GameView2D(SCREENWIDTH, SCREENHEIGHT);
            Engine.View = view;

            GameFrame frame = new GameFrame(0, 0, 160, 144);
            frame.Start();
            Engine.DrawEnd += frame.Pane.DrawHandle;

            Location location = new Location(new Description2D(0, 0, 100, 100));
            Engine.SetLocation(location);

            WindowsKeyController keyController = new WindowsKeyController(keyMap);
            keyController.Hook(frame);
            Engine.AddController(keyController);

            WindowsMouseController mouseController = new WindowsMouseController(mouseMap);
            mouseController.Hook(frame);
            Engine.AddController(mouseController);

            new Sprite("marker", 4, 4);
            Entity marker = Marker.Create(new Marker(0, 0));
            Engine.AddEntity(marker);

            new Sprite("enemy", 8, 8);
            Entity enemy = Enemy.Create(new Enemy(64, 64));
            Engine.AddEntity(enemy);

            new Sprite("player", 8, 8);
            Entity player = Player.Create(new Player(50, 50, Engine.GetControllerIndex(keyController), Engine.GetControllerIndex(mouseController)));
            Engine.AddEntity(player);

            new Sprite("hud", 0, 0);
            Entity hud = Hud.Create(new Hud(Engine.GetControllerIndex(keyController), Engine.GetControllerIndex(mouseController), SCREENWIDTH, SCREENHEIGHT));
            Engine.AddEntity(hud);

            Engine.Start();

            while (true) { };
        }

        private static Dictionary<int, int> keyMap = new Dictionary<int, int>()
        {
            { (int)System.Windows.Forms.Keys.W, (int)Actions.UP },
            { (int)System.Windows.Forms.Keys.Z, (int)Actions.ALT },
        };

        private static Dictionary<int, int> mouseMap = new Dictionary<int, int>()
        {
            { (int)System.Windows.Forms.MouseButtons.Left, (int)Actions.MOVE },
            { (int)System.Windows.Forms.MouseButtons.None, (int)Actions.MOUSEINFO },
        };
    }

    public enum Actions { UP = 0, DOWN = 1, LEFT = 2 , RIGHT = 3, MOVE = 4, ALT = 5, MOUSEINFO = 6 };
}
