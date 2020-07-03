using GameEngine.UI.AvaloniaUI;
using System.Collections.Generic;
using System.Text;

namespace GameEngine.UI.AvaloniaUI
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

        public void Hook(GameFrame frame)
        {
            if (frame.window == null)
            {
                return;
            }

            frame.window.KeyDown += Frame_KeyDown;
            frame.window.KeyUp += Frame_KeyUp;
            hooked = true;
        }

        public bool IsHooked()
        {
            return hooked;
        }

        private void Frame_KeyUp(object sender, Avalonia.Input.KeyEventArgs e)
        {
            if (keymap.ContainsKey((int)e.Key))
            {
                ActionEnd(keymap[(int)e.Key], null);
            }
        }

        private void Frame_KeyDown(object sender, Avalonia.Input.KeyEventArgs e)
        {
            if (keymap.ContainsKey((int)e.Key))
            {
                ActionStart(keymap[(int)e.Key], null);
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
