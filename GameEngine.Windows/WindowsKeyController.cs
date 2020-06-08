using System.Collections.Generic;
using System.Windows.Forms;

namespace GameEngine.Windows
{
    public class WindowsKeyController : Controller
    {

        public WindowsKeyController(Dictionary<int, ActionState> keymap) : base(keymap)
        {
        }

        public void Hook(GameFrame frame)
        {
            frame.KeyDown += Frame_KeyDown;
            frame.KeyUp += Frame_KeyUp;
        }

        private void Frame_KeyUp(object sender, KeyEventArgs e)
        {
            ActionEnd((int)e.KeyCode);
        }

        private void Frame_KeyDown(object sender, KeyEventArgs e)
        {
            ActionStart((int)e.KeyCode);
        }
    }
}
