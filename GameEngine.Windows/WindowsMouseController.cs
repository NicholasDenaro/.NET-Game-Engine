using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameEngine.UI.WinForms
{
    public class WindowsMouseController : Controller
    {
        private Dictionary<int, int> keymap;
        private bool hooked = false;

        public WindowsMouseController() : base()
        {

        }

        public WindowsMouseController(Dictionary<int, int> keymap) : base(keymap.Values)
        {
            this.keymap = keymap;
        }

        public void Hook(IGameWindow window)
        {
            hooked = window.HookMouse(Frame_KeyInfo, Frame_KeyDown, Frame_KeyUp);
        }

        public bool IsHooked()
        {
            return hooked;
        }

        private void Frame_KeyUp(object sender, MouseEventArgs e)
        {
            if (keymap.ContainsKey((int)e.Button))
            {
                ActionEnd(keymap[(int)e.Button], new MouseControllerInfo(new Point(e.X, e.Y)));
            }
        }

        private void Frame_KeyDown(object sender, MouseEventArgs e)
        {
            if (keymap.ContainsKey((int)e.Button))
            {
                ActionStart(keymap[(int)e.Button], new MouseControllerInfo(new Point(e.X, e.Y)));
            }
        }

        private void Frame_KeyInfo(object sender, MouseEventArgs e)
        {
            if (keymap.ContainsKey((int)e.Button))
            {
                ActionInfo(keymap[(int)e.Button], new MouseControllerInfo(new Point(e.X, e.Y)));
            }
        }

        public override string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.GetType().FullName);
            sb.Append(":");
            sb.Append("{");
            sb.Append(base.Serialize());
            sb.Append(",");
            sb.Append(StringConverter.Serialize<int, int>(keymap));
            sb.Append("}");
            return sb.ToString();
        }

        public override void Deserialize(string state)
        {
            List<string> tokens = StringConverter.DeserializeTokens(state);
            base.Deserialize(tokens[0]);
            keymap = StringConverter.Deserialize<int, int>(tokens[1], str => int.Parse(str), str => int.Parse(str));
        }
    }
}
