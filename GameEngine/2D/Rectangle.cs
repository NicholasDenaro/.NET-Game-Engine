using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine._2D
{
    public class Rectangle : RectangleF, IRectangle<int>
    {
        private int x;
        public new int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        private int y;
        public new int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
        private int width;
        public new int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }
        private int height;
        public new int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public Rectangle(int x, int y, int width, int height) : base(x, y, width, height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Rectangle(Point p, int width, int height) : base(p.X, p.Y, width, height)
        {
            this.x = p.X;
            this.y = p.Y;
            this.width = width;
            this.height = height;
        }

        public Rectangle(Point p, Size s) : base(p.X, p.Y, s.Width, s.Height)
        {
            this.x = p.X;
            this.y = p.Y;
            this.width = s.Width;
            this.height = s.Height;
        }
    }
}
