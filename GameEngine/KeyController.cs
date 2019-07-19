using GameEngine.Interfaces;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GameEngine
{
    class KeyController : Controller
    {
        private Dictionary<Keys, KeyAction> keys = new Dictionary<Keys, KeyAction>();

        public void Hook(GamePanel panel)
        {
            panel.KeyDown += KeyDown;
            panel.KeyUp += KeyUp;
        }

        public KeyController(Dictionary<int, ControllerAction> keymap)
        {
            SetActions(keymap);
            foreach(KeyValuePair<int, ControllerAction> kvp in keymap)
            {
                keys[((KeyAction)kvp.Value).Key] = (KeyAction)kvp.Value;
            }
        }

        public void KeyDown(object sender, KeyEventArgs args)
        {
            if (keys.ContainsKey(args.KeyCode))
            {
                if (keys[args.KeyCode].State != KeyState.HOLD)
                {
                    keys[args.KeyCode].State = KeyState.PRESSED;
                }
            }
        }

        public void KeyUp(object sender, KeyEventArgs args)
        {
            if (keys.ContainsKey(args.KeyCode))
            {
                if (keys[args.KeyCode].State != KeyState.UP)
                {
                    keys[args.KeyCode].State = KeyState.RELEASE;
                }
            }
        }

        public KeyState this[int key]
        {
            get
            {
                return ((KeyAction)Actions[key]).State;
            }
        }

        public override void Input()
        {
            foreach (KeyValuePair<int, ControllerAction> kvp in Actions)
            {
                KeyAction act = (KeyAction)kvp.Value;
                if (act.State == KeyState.PRESSED && act.Duration > 1)
                {
                    keys[((KeyAction)kvp.Value).Key].State = KeyState.HOLD;
                }
                else if (act.State == KeyState.RELEASE && act.Duration > 1)
                {
                    keys[((KeyAction)kvp.Value).Key].State = KeyState.UP;
                }
                else
                {
                    act.Duration++;
                }
            }
        }
    }

    public enum KeyState { PRESSED, HOLD, RELEASE, UP }

    public class KeyAction : ControllerAction
    {
        public KeyState State;
        public Keys Key { get; private set; }
        public int Duration { get; internal set; }

        public KeyAction(Keys key)
        {
            State = KeyState.UP;
            Key = key;
        }
    }
}
