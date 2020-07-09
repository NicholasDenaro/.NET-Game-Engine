using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GameEngine.UI.WinForms
{
    public class WindowsKeyController : Controller
    {
        private Dictionary<int, int> keymap;
        private bool hooked = false;

        public WindowsKeyController() : base()
        {

        }

        public WindowsKeyController(Dictionary<int, int> keymap) : base(keymap.Values)
        {
            this.keymap = keymap;
        }

        public void Hook(IGameWindow window)
        {
            hooked = window.HookKeyboard(Frame_KeyDown, Frame_KeyUp);
        }

        public bool IsHooked()
        {
            return hooked;
        }

        private void Frame_KeyUp(object sender, KeyEventArgs e)
        {
            if (keymap.ContainsKey(e.KeyCode))
            {
                ActionEnd(keymap[e.KeyCode], null);
            }
        }

        private void Frame_KeyDown(object sender, KeyEventArgs e)
        {
            if (keymap.ContainsKey(e.KeyCode))
            {
                ActionStart(keymap[e.KeyCode], null);
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
