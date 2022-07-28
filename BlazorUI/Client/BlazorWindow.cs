using BlazorUI.Shared;
using GameEngine;
using GameEngine.UI;
using GameEngine.UI.Controllers;

namespace BlazorUI.Client
{
    public class BlazorWindow : IGameWindow
    {
        public static BlazorWindow Instance;

        private BlazorPanel panel;
        public IGamePanel Panel => panel;

        public BlazorWindow(int width, int height, double xScale, double yScale)
        {
            Instance = this;
            this.panel = new BlazorPanel(width, height, xScale, yScale);
        }

        public void Init(MainLayout layout)
        {
            panel.SetLayout(layout);
        }
        

        public bool Hook(Controller controller)
        {
            Task.Run(async () =>
            {
                await MainLayout.WaitInitialized();
                if (controller is WindowsMouseController)
                {
                    WindowsMouseController mwc = controller as WindowsMouseController;
                    return HookMouse(mwc.Frame_KeyInfo, mwc.Frame_KeyDown, mwc.Frame_KeyUp);
                }
                else if (controller is WindowsKeyController)
                {
                    WindowsKeyController wkc = controller as WindowsKeyController;
                    return HookKeyboard(wkc.Frame_KeyDown, wkc.Frame_KeyUp);
                }
                else if (controller is XBoxController)
                {
                    throw new ArgumentOutOfRangeException($"{this.GetType().Name} does not use {controller.GetType().Name}");
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"{this.GetType().Name} does not use {controller.GetType().Name}");
                }
            });
            
            return true;
        }

        public bool HookKeyboard(Action<object, KeyEventArgs> frame_KeyDown, Action<object, KeyEventArgs> frame_KeyUp)
        {
            MainLayout.Instance.HookKeyboard(
                (args) =>
                {
                    frame_KeyDown(this, new KeyEventArgs(KeyCodeToInt(args.Code)));
                }, 
                (args) =>
                {
                    frame_KeyUp(this, new KeyEventArgs(KeyCodeToInt(args.Code)));
                });
            return true;
        }

        private static int KeyCodeToInt(string code)
        {
            switch (code)
            {
                default:
                    return 0;
                case nameof(KeyCodes.KeyA):
                    return KeyCodes.KeyA;
                case nameof(KeyCodes.KeyS):
                    return KeyCodes.KeyS;
                case nameof(KeyCodes.KeyX):
                    return KeyCodes.KeyX;
                case nameof(KeyCodes.KeyZ):
                    return KeyCodes.KeyZ;
                case nameof(KeyCodes.ArrowLeft):
                    return KeyCodes.ArrowLeft;
                case nameof(KeyCodes.ArrowUp):
                    return KeyCodes.ArrowUp;
                case nameof(KeyCodes.ArrowRight):
                    return KeyCodes.ArrowRight;
                case nameof(KeyCodes.ArrowDown):
                    return KeyCodes.ArrowDown;
                case "[":
                    return KeyCodes.OEMBracketOpen;
                case "]":
                    return KeyCodes.OEMBracketClose;
            }
        }

        public bool HookMouse(Action<object, MouseEventArgs> frame_KeyInfo, Action<object, MouseEventArgs> frame_KeyDown, Action<object, MouseEventArgs> frame_KeyUp)
        {
            return false;
        }

        public void SetBounds(int x, int y, int width, int height)
        {
        }

        public static class KeyCodes
        {
            public const int ArrowLeft = 37;
            public const int ArrowUp = 38;
            public const int ArrowRight = 39;
            public const int ArrowDown = 40;
            public const int KeyA = 'A';
            public const int KeyX = KeyA + 'x' - 'a';
            public const int KeyZ = KeyA + 'z' - 'a';
            public const int KeyS = KeyA + 's' - 'a';
            public const int OEMBracketOpen = '[';
            public const int OEMBracketClose = ']';
        }
    }
}
