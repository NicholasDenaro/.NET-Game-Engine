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
        public static readonly Color Black = new Color(0, 0, 0, 255);
        public static readonly Color White = new Color(255, 255, 255, 255);

        public static Color FromArgb(int alpha, Color color)
        {
            return new Color(color.R, color.G, color.B, alpha);
        }

        public static readonly Color Gray = new Color(200, 200, 200);
        public static readonly Color Cyan = new Color(0, 255, 255);
        public static readonly Color Red = new Color(255, 0, 0);
        public static readonly Color MediumPurple = new Color(200, 50, 200);
        public static readonly Color IndianRed = new Color(200, 50, 0);
        public static readonly Color Aquamarine = new Color(0, 50, 255);
        public static readonly Color Chartreuse = new Color(100, 255, 0);
        public static readonly Color Teal = new Color(100, 0, 100);
        public static readonly Color DarkOrange = new Color(100, 50, 0);
        public static readonly Color SaddleBrown = new Color(255, 200, 100);
        public static readonly Color LightYellow = new Color(255, 255, 150);
        public static readonly Color DarkViolet = new Color(100, 0, 100);
        public static readonly Color Yellow = new Color(255, 255, 0);
        public static readonly Color DarkSlateGray = new Color(50, 50, 100);
    }
}
