using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GameEngine.Windows
{
    public class WindowsMouseController : Controller
    {
        private Dictionary<int, int> keymap;
        private bool hooked = false;

        private GameFrame frame;

        public WindowsMouseController() : base()
        {

        }

        public WindowsMouseController(Dictionary<int, int> keymap) : base(keymap.Values)
        {
            this.keymap = keymap;
        }

        public void Hook(GameFrame frame)
        {
            this.frame = frame;
            frame.Pane.MouseDown += Frame_KeyDown;
            frame.Pane.MouseUp += Frame_KeyUp;
            frame.Pane.MouseMove += Frame_KeyInfo;
            hooked = true;
        }

        private void Frame_KeyUp(object sender, MouseEventArgs e)
        {
            if (keymap.ContainsKey((int)e.Button))
            {
                ActionEnd(keymap[(int)e.Button], new MouseControllerInfo(e.Location));
            }
        }

        private void Frame_KeyDown(object sender, MouseEventArgs e)
        {
            if (keymap.ContainsKey((int)e.Button))
            {
                ActionStart(keymap[(int)e.Button], new MouseControllerInfo(e.Location));
            }
        }

        private void Frame_KeyInfo(object sender, MouseEventArgs e)
        {
            if (keymap.ContainsKey((int)e.Button))
            {
                ActionInfo(keymap[(int)e.Button], new MouseControllerInfo(e.Location));
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

    public class MouseControllerInfo : IControllerActionInfo
    {
        public int X { get; set; }

        public int Y { get; set; }

        public MouseControllerInfo(Point point)
        {
            X = point.X;
            Y = point.Y;
        }
    }
}
