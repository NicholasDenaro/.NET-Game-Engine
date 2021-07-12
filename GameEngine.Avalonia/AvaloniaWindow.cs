using Avalonia.Controls;
using Avalonia.Input;
using System;

namespace GameEngine.UI.AvaloniaUI
{
    public class AvaloniaWindow : Window, IGameWindow
    {
        public AvaloniaWindow(int width, int height)
        {
            ClientSize = new Avalonia.Size(width / PlatformImpl.DesktopScaling, height / PlatformImpl.DesktopScaling);
            this.CanResize = false;
        }

        private GamePanel panel;
        public IGamePanel Panel => panel;

        public void Add(GamePanel panel)
        {
            this.panel = panel;
            this.VisualChildren.Add(panel);
        }

        public void SetBounds(int x, int y, int width, int height)
        {
            this.Position = new Avalonia.PixelPoint(x, y);
            ClientSize = new Avalonia.Size(width / PlatformImpl.DesktopScaling, height / PlatformImpl.DesktopScaling);
            this.Panel.Resize(width, height);
        }

        public bool Hook(Controller controller)
        {
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
            else
            {
                throw new ArgumentOutOfRangeException($"{this.GetType().Name} does not use {controller.GetType().Name}");
            }
        }

        public bool HookMouse(Action<object, MouseEventArgs> frame_KeyInfo, Action<object, MouseEventArgs> frame_KeyDown, Action<object, MouseEventArgs> frame_KeyUp)
        {
            this.PointerMoved += (s, e) =>
            {
                MouseEventArgs mea = ScaleEvent(Convert(e));
                bool pressed = false;

                if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
                {
                    mea.AlterButton(Key(PointerUpdateKind.LeftButtonPressed));
                    frame_KeyInfo(s, mea);
                    pressed = true;
                }

                if (e.GetCurrentPoint(null).Properties.IsRightButtonPressed)
                {
                    mea.AlterButton(Key(PointerUpdateKind.RightButtonPressed));
                    frame_KeyInfo(s, mea);
                    pressed = true;
                }

                if (pressed)
                {
                    return;
                }

                frame_KeyInfo(s, mea);
            };

            this.PointerPressed += (s, e) => frame_KeyDown(s, ScaleEvent(Convert(e)));
            this.PointerReleased += (s, e) => frame_KeyUp(s, ScaleEvent(Convert(e)));

            return true;
        }

        public bool HookKeyboard(Action<object, KeyEventArgs> frame_KeyDown, Action<object, KeyEventArgs> frame_KeyUp)
        {
            if (this.panel == null)
            {
                return false;
            }

            this.KeyDown += (s, e) => frame_KeyDown(s, new KeyEventArgs((int)e.Key));
            this.KeyUp += (s, e) => frame_KeyUp(s, new KeyEventArgs((int)e.Key));

            return true;
        }

        public MouseEventArgs Convert(PointerEventArgs pea)
        {
            PointerPoint pp = pea.GetCurrentPoint(this);
            Avalonia.Point point = pea.GetPosition(this);
            int key = Key(pp.Properties.PointerUpdateKind);

            return new MouseEventArgs(key, 1, (int)(point.X * this.PlatformImpl.DesktopScaling), (int)(point.Y * this.PlatformImpl.DesktopScaling), 0);
        }

        public MouseEventArgs ScaleEvent(MouseEventArgs e)
        {
            return new MouseEventArgs(e.Button, e.Clicks, (int)(e.X / this.Panel.ScaleX), (int)(e.Y / this.Panel.ScaleY), e.Wheel);
        }

        public static int Key(PointerUpdateKind puk)
        {
            int key;
            switch (puk)
            {
                case PointerUpdateKind.LeftButtonPressed:
                case PointerUpdateKind.LeftButtonReleased:
                    key = 0;
                    break;

                case PointerUpdateKind.RightButtonPressed:
                case PointerUpdateKind.RightButtonReleased:
                    key = 1;
                    break;

                case PointerUpdateKind.MiddleButtonPressed:
                case PointerUpdateKind.MiddleButtonReleased:
                    key = 2;
                    break;

                case PointerUpdateKind.Other:
                    key = 6;
                    break;

                default:
                    key = -1;
                    break;
            }

            return key;
        }
    }
}
