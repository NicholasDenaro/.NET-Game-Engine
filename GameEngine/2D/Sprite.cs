using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GameEngine._2D
{
    public class Sprite
    {
        public static readonly float dpiX;
        public static readonly float dpiY;

        static Sprite()
        {
        }

        public static Dictionary<string, Sprite> Sprites { get; set; } = new Dictionary<string, Sprite>();

        private Bitmap image;
        private int hSubImages;
        private int vSubImages;
        
        public string Name { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int HImages => hSubImages;
        public int VImages => vSubImages;

        public Sprite(string name, string bmpFile, int tileWidth, int tileHeight, int x = 0, int y = 0) : this(name, Assembly.GetEntryAssembly().GetManifestResourceStream($"{Assembly.GetEntryAssembly().GetName().Name}.{bmpFile.Replace("/", ".")}"), x, y)
        {
            Width = tileWidth;
            Height = tileHeight;


            hSubImages = (image.Width / Width);
            vSubImages = (image.Height / Height);

        }

        public Sprite(string name, Stream bmp, Point origin = default(Point), Rectangle tile = default(Rectangle)) : this(name, bmp, origin.X, origin.Y)
        {
            Width = tile.Width;
            Height = tile.Height;


            hSubImages = (image.Width / Width);
            vSubImages = (image.Height / Height);

        }

        public Sprite(string name, Stream sbmp, int x, int y)
        {
            Name = name;
            Bitmap bmp = Bitmap.Create(sbmp);
            this.image = bmp;
            hSubImages = 1;
            vSubImages = 1;
            Width = bmp.Width;
            Height = bmp.Height;
            X = x;
            Y = y;
            Sprites.Add(name, this);
        }

        public Sprite(string name, int x, int y)
        {
            Name = name;
            this.image = null;
            hSubImages = 1;
            vSubImages = 1;
            Width = 0;
            Height = 0;
            X = x;
            Y = y;
            Sprites.Add(name, this);
        }

        public Sprite(string name, Bitmap bmp, int x, int y)
        {
            Name = name;
            this.image = bmp;
            hSubImages = 1;
            vSubImages = 1;
            Width = bmp.Width;
            Height = bmp.Height;
            X = x;
            Y = y;
            Sprites.Add(name, this);
        }
        public Bitmap getImage()
        {
            return image;
        }

        public BitmapSection GetImage(int index)
        {
            return new BitmapSection(this.image, new Rectangle((index % this.HImages) * this.Width, ((index / this.HImages) % this.VImages) * this.Height, this.Width, this.Height));
        }
    }
}
