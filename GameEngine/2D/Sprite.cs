using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace GameEngine._2D
{
    public class Sprite
    {
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
            SetupSubImages(image[0], tileWidth, tileHeight);
        }

        public Sprite(string name, Stream sbmp, int x, int y)
        {
            Name = name;
            Bitmap bmp = (Bitmap)Image.FromStream(sbmp);
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
                    image[i + j * hSubImages] = bmp.Clone(new Rectangle(i * Width, j * Height, Width, Height), System.Drawing.Imaging.PixelFormat.DontCare);
                }
            }
        }

        public Image[] Images => image;

        public Bitmap GetImage(int index)
        {
            return image[index % image.Length];
        }
    }
}
