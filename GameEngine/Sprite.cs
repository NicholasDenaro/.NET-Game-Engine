using System.Collections.Generic;
using System.Drawing;

namespace GameEngine
{
    public class Sprite
    {
        public static Dictionary<string, Sprite> Sprites { get; set; } = new Dictionary<string, Sprite>();

        private string name;
        private Bitmap[] image;
        private int hSubImages;
        private int vSubImages;
        
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Sprite(string name, string bmpFile, int tileWidth, int tileHeight) : this(name, (Bitmap)Image.FromFile(bmpFile), 0, 0)
        {
            SetupSubImages(image[0], tileWidth, tileHeight);
        }

        public Sprite(string name, Bitmap bmp, int x, int y)
        {
            this.name = name;
            this.image = new[] { bmp };
            hSubImages = 1;
            vSubImages = 1;
            Width = bmp.Width;
            Height = bmp.Height;
            X = x;
            Y = y;
            Sprites.Add(name, this);
        }

        public Sprite(string name, string bmpFile, int x, int y, int width, int height) : this(name, (Bitmap)Image.FromFile(bmpFile), x, y)
        {
            
        }

        public Sprite(string name, Bitmap bmp, int x, int y, int width, int height, int tileWidth, int tileHeight) : this(name, bmp, x, y)
        {
            SetupSubImages(image[0], tileWidth, tileHeight);
        }

        public Sprite(string name, string bmpFile, int x, int y, int width, int height, int tileWidth, int tileHeight) : this(name, (Bitmap)Image.FromFile(bmpFile), x, y, width, height, tileWidth, tileHeight)
        {

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
