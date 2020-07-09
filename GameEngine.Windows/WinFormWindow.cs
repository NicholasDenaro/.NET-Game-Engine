using System;
using System.Windows.Forms;

namespace GameEngine.UI.WinForms
{
    public class WinFormWindow : Form, IGameWindow
    {
        private GamePanel panel;
        internal void Add(GamePanel gamePanel)
        {
            panel = gamePanel;
            this.Controls.Add(gamePanel);
        }

        public bool HookMouse(Action<object, MouseEventArgs> frame_KeyInfo, Action<object, MouseEventArgs> frame_KeyDown, Action<object, MouseEventArgs> frame_KeyUp)
        {
            if (this.panel == null)
            {
                return false;
            }

            this.panel.MouseMove += (s, e) => frame_KeyInfo(s, ScaleEvent(Convert(e)));
            this.panel.MouseDown += (s, e) => frame_KeyDown(s, ScaleEvent(Convert(e)));
            this.panel.MouseUp += (s, e) => frame_KeyUp(s, ScaleEvent(Convert(e)));

            return true;
        }

        public bool HookKeyboard(Action<object, KeyEventArgs> frame_KeyDown, Action<object, KeyEventArgs> frame_KeyUp)
        {
            if (this.panel == null)
            {
                return false;
            }

            this.KeyDown += (s, e) => frame_KeyDown(s, new KeyEventArgs((int)e.KeyCode));
            this.KeyUp += (s, e) => frame_KeyUp(s, new KeyEventArgs((int)e.KeyCode));

            return true;
        }

        public MouseEventArgs Convert(System.Windows.Forms.MouseEventArgs mea)
        {
            return new MouseEventArgs((int)mea.Button, mea.Clicks, mea.X, mea.Y, mea.Delta);
        }

        public MouseEventArgs ScaleEvent(MouseEventArgs e)
        {
            return new MouseEventArgs(e.Button, e.Clicks, (int)(e.X / this.panel.ScaleX), (int)(e.Y / this.panel.ScaleY), e.Wheel);
        }

        public IGamePanel Panel => panel;
    }
}
