using System.Collections.Generic;
using System.Text;

namespace GameEngine.UI.Controllers
{
    public class WindowsKeyController : Controller
    {
        private Dictionary<int, object> keymap;

        public WindowsKeyController() : base()
        {

        }

        public WindowsKeyController(Dictionary<int, object> keymap) : base(keymap.Values)
        {
            this.keymap = keymap;
        }

        public void Frame_KeyUp(object sender, KeyEventArgs e)
        {
            if (keymap.ContainsKey(e.KeyCode))
            {
                ActionEnd(keymap[e.KeyCode], null);
            }
        }

        public void Frame_KeyDown(object sender, KeyEventArgs e)
        {
            if (keymap.ContainsKey(e.KeyCode))
            {
                ActionStart(keymap[e.KeyCode], null);
            }
        }

        public override string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetType().FullName);
            sb.Append(":");
            sb.Append("{");
            sb.Append(base.Serialize());
            sb.Append(",");
            sb.Append(StringConverter.Serialize(keymap));
            sb.Append("}");
            return sb.ToString();
        }

        public override void Deserialize(string state)
        {
            List<string> tokens = StringConverter.DeserializeTokens(state);
            base.Deserialize(tokens[0]);
            keymap = StringConverter.Deserialize<int, object>(tokens[1], str => int.Parse(str), str => int.Parse(str));
        }
    }
}
