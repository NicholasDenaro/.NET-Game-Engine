using GameEngine._2D;
using System.Collections.Generic;
using System.Text;

namespace GameEngine.UI.Controllers
{
    public class WindowsMouseController : Controller
    {
        private Dictionary<int, object> keymap;

        public WindowsMouseController() : base()
        {

        }

        public WindowsMouseController(Dictionary<int, object> keymap) : base(keymap.Values)
        {
            this.keymap = keymap;
        }

        public void Frame_KeyUp(object sender, MouseEventArgs e)
        {
            if (keymap.ContainsKey(e.Button))
            {
                ActionEnd(keymap[e.Button], new MouseControllerInfo(new Point(e.X, e.Y)));
            }
        }

        public void Frame_KeyDown(object sender, MouseEventArgs e)
        {
            if (keymap.ContainsKey(e.Button))
            {
                ActionStart(keymap[e.Button], new MouseControllerInfo(new Point(e.X, e.Y)));
            }
        }

        public void Frame_KeyInfo(object sender, MouseEventArgs e)
        {
            if (keymap.ContainsKey(e.Button))
            {
                ActionInfo(keymap[e.Button], new MouseControllerInfo(new Point(e.X, e.Y)));
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
