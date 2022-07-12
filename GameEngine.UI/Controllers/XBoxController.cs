using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static GameEngine.UI.Controllers.XInputWindows;

namespace GameEngine.UI.Controllers
{
    public class XBoxController : Controller
    {

        private Dictionary<Key, object> keymap;
        private XInputState lastState;

        public XBoxController(Dictionary<Key, object> keymap) : base(keymap.Values)
        {
            this.keymap = keymap;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                lastState = XInputWindows.GetState(0);
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                lastState = DevInputJSLinux.GetState(0);
            }
            else
            {
                Console.WriteLine($"Platform not supported {Environment.OSVersion.Platform}");
                throw new Exception($"Platform not supported {Environment.OSVersion.Platform}");
            }
        }

        public void Start()
        {
            new Thread(Polling).Start();
        }

        private void Polling()
        {
            while (true)
            {
                XInputState state;
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    state = XInputWindows.GetState(0);
                }
                else if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    state = DevInputJSLinux.GetState(0);
                }
                else
                {
                    throw new Exception($"Platform not supported {Environment.OSVersion.Platform}");
                }

                CheckButtons(state);
                CheckDpad(state);
                CheckTriggers(state);
                CheckSticks(state);

                lastState = state;

                Thread.Sleep(10);
            }
        }

        private void CheckButtons(XInputState state)
        {
            if (state.Buttons.A != lastState.Buttons.A)
            {
                DoAction(Key.A, state.Buttons.A);
            }
            if (state.Buttons.B != lastState.Buttons.B)
            {
                DoAction(Key.B, state.Buttons.B);
            }
            if (state.Buttons.X != lastState.Buttons.X)
            {
                DoAction(Key.X, state.Buttons.X);
            }
            if (state.Buttons.Y != lastState.Buttons.Y)
            {
                DoAction(Key.Y, state.Buttons.Y);
            }
            if (state.Buttons.Start != lastState.Buttons.Start)
            {
                DoAction(Key.START, state.Buttons.Start);
            }
            if (state.Buttons.Select != lastState.Buttons.Select)
            {
                DoAction(Key.SELECT, state.Buttons.Select);
            }
            if (state.Buttons.BumperL != lastState.Buttons.BumperL)
            {
                DoAction(Key.BUMPER_LEFT, state.Buttons.BumperL);
            }
            if (state.Buttons.BumperR != lastState.Buttons.BumperR)
            {
                DoAction(Key.BUMPER_RIGHT, state.Buttons.BumperR);
            }
        }

        private void CheckDpad(XInputState state)
        {
            if (state.DPad.Up != lastState.DPad.Up)
            {
                DoAction(Key.DPAD_UP, state.DPad.Up);
            }
            if (state.DPad.Down != lastState.DPad.Down)
            {
                DoAction(Key.DPAD_DOWN, state.DPad.Down);
            }
            if (state.DPad.Left != lastState.DPad.Left)
            {
                DoAction(Key.DPAD_LEFT, state.DPad.Left);
            }
            if (state.DPad.Right != lastState.DPad.Right)
            {
                DoAction(Key.DPAD_RIGHT, state.DPad.Right);
            }
        }

        private void CheckTriggers(XInputState state)
        {

        }
        private void CheckSticks(XInputState state)
        {

        }

        private void DoAction(Key key, XInputValue state)
        {
            if (!state.Pressed)
            {
                if (keymap.ContainsKey(key))
                {
                    ActionEnd(keymap[key], null);
                }
            }
            else
            {
                if (keymap.ContainsKey(key))
                {
                    ActionStart(keymap[key], null);
                }
            }
        }

        public enum Key { DPAD_UP, DPAD_LEFT, DPAD_DOWN, DPAD_RIGHT, A, B, X, Y, START, SELECT, BUMPER_LEFT, BUMPER_RIGHT };
    }
}
