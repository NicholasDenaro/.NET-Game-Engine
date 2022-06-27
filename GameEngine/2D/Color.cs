using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine._2D
{
    public class Color
    {
        public byte R { get; private set; }
        public byte G { get; private set; }
        public byte B { get; private set; }
        public byte A { get; private set; }

        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = 255;
        }

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color(byte r, byte g, byte b, double a)
        {
            R = r;
            G = g;
            B = b;
            A = (byte)(a * 255);
        }

        public static readonly Color Transparent = new Color(0, 0, 0, 0);
        public static readonly Color Black = new Color(0, 0, 0);

        public static Color FromArgb(int alpha, Color color)
        {
            return new Color(color.R, color.G, color.B, alpha);
        }

        public static readonly Color Gray = new Color(200, 200, 200);
    }
}
