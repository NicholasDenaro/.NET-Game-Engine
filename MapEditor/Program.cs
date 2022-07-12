using GameEngine._2D;
using GameEngine.Interfaces;
using System;
using System.IO;
using System.Windows.Forms;

namespace MapEditor
{
    public class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        public static void Main(string[] args)
        {
            Bitmap.SetBitmapImpl(new SystemBitmapCreator());
            SetProcessDPIAware();
            Application.Run(new Form1());
        }
    }

    public class SystemBitmapCreator : IBitmapCreator
    {
        public Bitmap Create(int width, int height)
        {
            return new SystemDrawingBitmap(width, height);
        }

        public Bitmap Create(int width, int height, bool dpiMode)
        {
            throw new NotImplementedException();
        }

        public Bitmap Create(Stream stream)
        {
            return new SystemDrawingBitmap(new System.Drawing.Bitmap(System.Drawing.Image.FromStream(stream)));
        }
    }

    class SystemDrawingBitmap : Bitmap
    {
        private System.Drawing.Bitmap _bitmap;
        private Graphics _graphics;

        public SystemDrawingBitmap(int width, int height)
        {
            _bitmap = new System.Drawing.Bitmap(width, height);
            _graphics = new SystemDrawingGraphics(System.Drawing.Graphics.FromImage(_bitmap));
            this.Width = width;
            this.Height = height;
        }

        public SystemDrawingBitmap(System.Drawing.Bitmap bitmap)
        {
            _bitmap = bitmap;
            _graphics = new SystemDrawingGraphics(System.Drawing.Graphics.FromImage(_bitmap));
            this.Width = bitmap.Width;
            this.Height = bitmap.Height;
        }

        public override Graphics GetGraphics()
        {
            return _graphics;
        }

        public override Color GetPixel(int x, int y)
        {
            System.Drawing.Color sdc = this._bitmap.GetPixel(x, y);
            return new Color(sdc.R, sdc.G, sdc.B, sdc.A);
        }

        public override T Image<T>()
        {
            return this._bitmap as T;
        }

        public override void SetPixel(int x, int y, Color color)
        {
            this._bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }

    class SystemDrawingGraphics : Graphics
    {
        private System.Drawing.Graphics _graphics;

        public SystemDrawingGraphics(System.Drawing.Graphics graphics)
        {
            _graphics = graphics;
        }

        public override void Clear(Color color)
        {
            _graphics.Clear(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public override void DrawArc(Color color, float v1, float v2, int v3, int v4, int v5, int v6)
        {
            throw new NotImplementedException();
        }

        public override void DrawEllipse(Color color, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public override void DrawImage(Bitmap bmp, RectangleF source, RectangleF dest)
        {
            _graphics.DrawImage(bmp.Image<System.Drawing.Bitmap>(),
                new System.Drawing.RectangleF(dest.X, dest.Y, dest.Width, dest.Height),
                new System.Drawing.RectangleF(source.X, source.Y, source.Width, source.Height),
                System.Drawing.GraphicsUnit.Pixel);
        }

        public override void DrawLine(Color color, int x1, int y1, int x2, int y2)
        {
            throw new NotImplementedException();
        }

        public override void DrawRectangle(Color color, int x, int y, int width, int height)
        {
            _graphics.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)), x, y, width, height);
        }

        public override void DrawText(string text, int x, int y, Color color, int size)
        {
            throw new NotImplementedException();
        }

        public override void FillEllipse(Color color, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public override void FillRectangle(Color color, int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public override IDisposable ScaleTransform(float x, float y)
        {
            throw new NotImplementedException();
        }

        public override IDisposable TranslateTransform(int x, int y)
        {
            throw new NotImplementedException();
        }

        protected override void disposeManaged()
        {
            throw new NotImplementedException();
        }

        protected override void disposeUnmanaged()
        {
            throw new NotImplementedException();
        }
    }

    public class Drawer2DSystemDrawing : IDrawer2D<SystemBitmap>
    {
        private SystemBitmap[] sbuffer;
        private SystemBitmap[] soverlay;

        private Bitmap tiles;

        public SystemBitmap Image(int buf) => sbuffer[buf];
        public SystemBitmap Overlay(int buf) => soverlay[buf];

        public Drawer2DSystemDrawing()
        {
        }

        public void Init(int width, int height, int xScale, int yScale)
        {
            Bitmap[] buffer = new Bitmap[] { Bitmap.Create(width, height), Bitmap.Create(width, height) };
            //buffer[0].MakeTransparent();
            //buffer[1].MakeTransparent();
            Bitmap[] overlay = new Bitmap[] { Bitmap.Create(width * xScale, height * yScale), Bitmap.Create(width * xScale, height * yScale) };
            //overlay[0].MakeTransparent();
            //overlay[1].MakeTransparent();

            sbuffer = new SystemBitmap[] { new SystemBitmap(buffer[0]), new SystemBitmap(buffer[1]) };
            soverlay = new SystemBitmap[] { new SystemBitmap(overlay[0]), new SystemBitmap(overlay[1]) };
        }

        public void Clear(int buffer, Color color)
        {
            this.sbuffer[buffer].Context.Clear(color);
            this.soverlay[buffer].Context.Clear(Color.Transparent);
        }

        public void TranslateTransform(int buffer, int x, int y)
        {
            this.sbuffer[buffer].Context.TranslateTransform(x, y);
        }

        public void FillRectangle(int buf, Color color, int x, int y, int w, int h)
        {
            this.sbuffer[buf].Context.FillRectangle(color, x, y, w, h);
        }

        public void Draw(int buffer, IDescription description)
        {
            if (description is TileMap)
            {
                Draw(this.sbuffer[buffer].Context, description as TileMap);
            }
            else
            {
                Description2D d2d = description as Description2D;
                Graphics gfx = d2d.DrawInOverlay ? this.soverlay[buffer].Context : this.sbuffer[buffer].Context;
                Draw(gfx, d2d);
            }
        }

        public void Draw(Graphics gfx, Description2D description)
        {
            if (description != null)
            {
                if (description.HasImage())
                {
                    Rectangle dest = new Rectangle((int)(description.X + description.DrawOffsetX) - (description?.Sprite.X ?? 0), (int)(description.Y + description.DrawOffsetY) - (description?.Sprite.Y ?? 0), description.Width, description.Height);
                    gfx.DrawImage(description.Image().Bitmap, dest, new Rectangle(0, 0, description.Width, description.Height));
                }
            }
        }

        public void RedrawTiles()
        {
            tiles = null;
        }

        public void Draw(Graphics gfx, TileMap map)
        {
            if (map.Sprite != null)
            {
                if (tiles == null)
                {
                    tiles = Bitmap.Create(map.Width, map.Height);
                    Graphics mgfx = tiles.GetGraphics();
                    mgfx.Clear(Color.Transparent);
                    // TODO: Decide if clip is necessary
                    //mgfx.Clip = new Region(new Rectangle(new Point(), new Size(map.Width, map.Height)));
                    int x = 0;
                    int y = 0;
                    foreach (int tile in map.Tiles)
                    {
                        BitmapSection section = map.Image(tile);
                        mgfx.DrawImage(section, new RectangleF(x * map.Sprite.Width, y * map.Sprite.Height, map.Sprite.Width, map.Sprite.Height));
                        x++;
                        if (x >= map.Columns)
                        {
                            x = 0;
                            y++;
                        }
                    }

                    mgfx.DrawRectangle(Color.Black, 0, 0, map.Width - 1, map.Height - 1);
                }

                gfx.DrawImage(tiles, new RectangleF(0, 0, tiles.Width, tiles.Height), new RectangleF(0, 0, tiles.Width, tiles.Height));
            }
        }

        public System.Drawing.Bitmap GetImage()
        {
            return this.sbuffer[0].Image.Image<System.Drawing.Bitmap>();
        }
    }
}
