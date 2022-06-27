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

        private Bitmap[] image;
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
            //SetupSubImages(image[0], tileWidth, tileHeight);

            Width = tileWidth;
            Height = tileHeight;


            hSubImages = (image[0].Width / Width);
            vSubImages = (image[0].Height / Height);

        }

        public Sprite(string name, Stream bmp, Point origin = default(Point), Rectangle tile = default(Rectangle)) : this(name, bmp, origin.X, origin.Y)
        {
            //SetupSubImages(image[0], tile.Width, tile.Height);

            Width = tile.Width;
            Height = tile.Height;


            hSubImages = (image[0].Width / Width);
            vSubImages = (image[0].Height / Height);

        }

        public Sprite(string name, Stream sbmp, int x, int y)
        {
            Name = name;
            Bitmap bmp = Bitmap.Create(sbmp);
            this.image = new[] { bmp };
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
            this.image = new Bitmap[] { };
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
            this.image = new Bitmap[] { bmp };
            hSubImages = 1;
            vSubImages = 1;
            Width = bmp.Width;
            Height = bmp.Height;
            X = x;
            Y = y;
            Sprites.Add(name, this);
        }

        private void SetupSubImages(Bitmap bmp, int tileWidth, int tileHeight)
        {
            Width = tileWidth;
            Height = tileHeight;

            hSubImages = (bmp.Width / Width);
            vSubImages = (bmp.Height / Height);

            image = new Bitmap[hSubImages * vSubImages];

            for (int j = 0; j < vSubImages; j++)
            {
                for (int i = 0; i < hSubImages; i++)
                {
                    image[i + j * hSubImages] = Bitmap.Create(Width, Height);
                    using (Graphics gfx = image[i + j * hSubImages].GetGraphics())
                    {
                        gfx.DrawImage(bmp, new RectangleF(i * Width, j * Height, Width, Height), new RectangleF(0, 0, Width, Height));
                    }
                }
            }
        }

        public Bitmap[] Images => image;

        public Bitmap GetImage(int index)
        {
            //return image[index % image.Length];
            return image[0];
        }
    }
}
