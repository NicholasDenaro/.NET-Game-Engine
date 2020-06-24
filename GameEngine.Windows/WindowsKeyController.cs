using System.Collections.Generic;
using System.Windows.Forms;

namespace GameEngine.Windows
{
    public class WindowsKeyController : Controller
    {
        private Dictionary<int, int> keymap;

        public WindowsKeyController(Dictionary<int, int> keymap) : base(keymap.Values)
        {
            this.keymap = keymap;
        }

        public void Hook(GameFrame frame)
        {
            frame.KeyDown += Frame_KeyDown;
            frame.KeyUp += Frame_KeyUp;
        }

        private void Frame_KeyUp(object sender, KeyEventArgs e)
        {
            if (keymap.ContainsKey((int)e.KeyCode))
            {
                ActionEnd(keymap[(int)e.KeyCode]);
            }
        }

        private void Frame_KeyDown(object sender, KeyEventArgs e)
        {
            if (keymap.ContainsKey((int)e.KeyCode))
            {
                ActionStart(keymap[(int)e.KeyCode]);
            }
        }
    }
}
