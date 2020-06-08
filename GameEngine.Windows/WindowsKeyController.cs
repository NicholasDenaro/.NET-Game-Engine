using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameEngine;
using GameEngine.Interfaces;

namespace GameEngine.Windows
{
    public class WindowsKeyController : Controller
    {
        private KeyController controller;

        public WindowsKeyController(Dictionary<int, ControllerAction> keymap)
        {
            controller = new KeyController(keymap);
        }

        public void Hook(GameFrame frame)
        {
            frame.KeyDown += Frame_KeyDown;
            frame.KeyUp += Frame_KeyUp;
        }

        private void Frame_KeyUp(object sender, KeyEventArgs e)
        {
            controller.KeyUp((int)e.KeyCode);
        }

        private void Frame_KeyDown(object sender, KeyEventArgs e)
        {
            controller.KeyDown((int)e.KeyCode);
        }

        public override void Input()
        {
            controller.Input();
        }
        public KeyAction this[int key]
        {
            get
            {
                return controller[key];
            }
        }
    }
}
