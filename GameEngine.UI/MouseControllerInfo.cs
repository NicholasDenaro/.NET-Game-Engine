using GameEngine._2D;
using System;

namespace GameEngine.UI
{
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

    public class MouseEventArgs
    {
        public int Button { get; private set; }
        public int Clicks { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Wheel { get; private set; }

        public MouseEventArgs(int button, int clicks, int x, int y, int wheelbumps)
        {
            Button = button;
            Clicks = clicks;
            X = x;
            Y = y;
            Wheel = wheelbumps;
        }

        public void AlterButton(int button)
        {
            this.Button = button;
        }
    }
}
