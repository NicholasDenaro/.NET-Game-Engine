using GameEngine.Interfaces;
using System.Collections.Generic;

namespace GameEngine
{
    public class KeyController : Controller
    {
        private Dictionary<int, KeyAction> keys = new Dictionary<int, KeyAction>();

        public KeyController(Dictionary<int, ControllerAction> keymap)
        {
            SetActions(keymap);
            foreach(KeyValuePair<int, ControllerAction> kvp in keymap)
            {
                keys[((KeyAction)kvp.Value).Key] = (KeyAction)kvp.Value;
            }
        }

        public void KeyDown(int keyCode)
        {
            if (keys.ContainsKey(keyCode))
            {
                if (keys[keyCode].State != KeyState.HOLD)
                {
                    keys[keyCode].State = KeyState.PRESSED;
                }
            }
        }

        public void KeyUp(int keyCode)
        {
            if (keys.ContainsKey(keyCode))
            {
                if (keys[keyCode].State != KeyState.UP)
                {
                    keys[keyCode].State = KeyState.RELEASE;
                }
            }
        }

        public KeyAction this[int key]
        {
            get
            {
                return (KeyAction)Actions[key];
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
        public int Key { get; private set; }
        public int Duration { get; internal set; }

        public KeyAction(int key)
        {
            State = KeyState.UP;
            Key = key;
        }

        public bool IsDown()
        {
            return State == KeyState.HOLD || State == KeyState.PRESSED;
        }
    }
}
