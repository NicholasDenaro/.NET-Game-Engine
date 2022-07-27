using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace GameEngine._2D
{
    public class Sprite
    {
        public static readonly float dpiX;
        public static readonly float dpiY;

        public static Assembly loadingAssembly;

        static Sprite()
        {
        }

        public static Dictionary<string, Sprite> Sprites { get; set; } = new Dictionary<string, Sprite>();

        private Stream stream;
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

        private Action initCallback;

        private Sprite(string name, string bmpFile, int tileWidth, int tileHeight, int x = 0, int y = 0) : this(name, loadingAssembly.GetManifestResourceStream($"{loadingAssembly.GetName().Name}.{bmpFile.Replace("/", ".")}") ?? File.OpenRead(bmpFile), x, y)
        {
            Console.WriteLine($"==spriteload=={loadingAssembly.GetName().Name}.{bmpFile.Replace("/", ".")}");
            Width = tileWidth;
            Height = tileHeight;

            initCallback = () =>
            {
                Console.WriteLine($"img w{image.Width} h{image.Height}");
                hSubImages = Math.Max(image.Width / Width, 1);
                vSubImages = Math.Max(image.Height / Height, 1);
            };
        }

        private Sprite(string name, Stream bmp, Point origin = default(Point), Rectangle tile = default(Rectangle)) : this(name, bmp, origin.X, origin.Y)
        {
            Width = tile.Width;
            Height = tile.Height;

            initCallback = () =>
            {
                hSubImages = Math.Max(image.Width / Width, 1);
                vSubImages = Math.Max(image.Height / Height, 1);
            };
        }

        private Sprite(string name, Stream sbmp, int x, int y)
        {
            Name = name;
            stream = sbmp;
            hSubImages = 1;
            vSubImages = 1;
            X = x;
            Y = y;

            initCallback = () =>
            {
                Width = image.Width;
                Height = image.Height;
            };
            Sprites.Add(name, this);
        }

        private async Task Init()
        {
            if (stream != null)
            {
                Bitmap bmp = await Bitmap.CreateAsync(Name, stream);
                this.image = bmp;
            }
            initCallback?.Invoke();
        }

        private Sprite(string name, int x, int y)
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

        private Sprite(string name, Bitmap bmp, int x, int y)
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

        public static async Task<Sprite> Create(string name, Bitmap bmp, int x, int y)
        {
            var sp = new Sprite(name, bmp, x, y);
            await sp.Init();
            return sp;
        }

        public static async Task<Sprite> Create(string name, int x, int y)
        {
            var sp = new Sprite(name, x, y);
            await sp.Init();
            return sp;
        }

        public static async Task<Sprite> Create(string name, Stream bmp, Point origin = default(Point), Rectangle tile = default(Rectangle))
        {
            var sp = await Task.FromResult(new Sprite(name, bmp, origin, tile));
            await sp.Init();
            return sp;
        }

        public static async Task<Sprite> Create(string name, string bmpFile, int tileWidth, int tileHeight, int x = 0, int y = 0)
        {
            var sp = new Sprite(name, bmpFile, tileWidth, tileHeight, x, y);
            await sp.Init();
            return sp;
        }

        public static async Task<Sprite> Create(string name, Stream sbmp, int x, int y)
        {
            var sp = await Task.FromResult(new Sprite(name, sbmp, x, y));
            await sp.Init();
            return sp;
        }

        public Bitmap GetImage()
        {
            return image;
        }

        public BitmapSection GetImage(int index)
        {
            return new BitmapSection(this.image, new Rectangle((index % this.HImages) * this.Width, ((index / this.HImages) % this.VImages) * this.Height, this.Width, this.Height));
        }
    }
}
