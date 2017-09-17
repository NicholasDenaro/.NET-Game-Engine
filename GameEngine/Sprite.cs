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
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public Sprite(string name, Bitmap bmp, int x, int y, int width, int height)
        {
            this.name = name;
            this.image = new[] { bmp };
            hSubImages = 1;
            vSubImages = 1;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Sprites.Add(name, this);
        }

        public Sprite(string name, string bmpFile, int x, int y, int width, int height) : this(name, (Bitmap)Image.FromFile(bmpFile), x, y, width, height)
        {
            
        }

        public Sprite(string name, Bitmap bmp, int x, int y, int width, int height, int tilewidth, int tileheight) : this(name, (Bitmap)null, x, y, width, height)
        {
            TileWidth = tilewidth;
            TileHeight = tileheight;

            hSubImages = (bmp.Width / TileWidth);
            vSubImages = (bmp.Height / TileHeight);

            image = new Bitmap[hSubImages * vSubImages];

            for(int j = 0; j < vSubImages; j++)
            {
                for (int i = 0; i < hSubImages; i++)
                {
                    image[i + j * hSubImages] = bmp.Clone(new Rectangle(i, j, TileWidth, TileHeight), System.Drawing.Imaging.PixelFormat.DontCare);
                }
            }
        }

        public Sprite(string name, string bmpFile, int x, int y, int width, int height, int tilewidth, int tileheight) : this(name, (Bitmap)Image.FromFile(bmpFile), x, y, width, height, tilewidth, tileheight)
        {

        }

        public Bitmap GetImage(int index)
        {
            return image[index % image.Length];
        }
    }
}
