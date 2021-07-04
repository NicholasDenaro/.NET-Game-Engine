using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using GameEngine._2D;
using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.UI.AvaloniaUI
{
    public class Drawer2DAvalonia : IDrawer2D<AvaloniaBitmap>
    {
        private RenderTargetBitmap tiles;

        private AvaloniaBitmap[] bmps;
        private AvaloniaBitmap[] overlays;

        public Drawer2DAvalonia()
        {
        }

        public void Init(int width, int height, int xScale, int yScale)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (Avalonia.Application.Current != null)
                        {
                            return;
                        }
                    }
                    catch
                    {

                    }
                    Task.Delay(500).Wait();
                }
            }).ContinueWith(ca =>
            {
                try
                {
                    Task t = Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        RenderTargetBitmap b0 = new RenderTargetBitmap(new PixelSize(width, height));

                        RenderTargetBitmap[] b = new RenderTargetBitmap[] { b0, new RenderTargetBitmap(new PixelSize(width, height)) };
                        RenderTargetBitmap[] o = new RenderTargetBitmap[] { new RenderTargetBitmap(new PixelSize(width * xScale, height * yScale)), new RenderTargetBitmap(new PixelSize(width * xScale, height * yScale)) };

                        bmps = new AvaloniaBitmap[] { new AvaloniaBitmap(b[0]), new AvaloniaBitmap(b[1]) };
                        overlays = new AvaloniaBitmap[] { new AvaloniaBitmap(o[0]), new AvaloniaBitmap(o[1]) };
                    }, Avalonia.Threading.DispatcherPriority.Render);

                    while (!t.Wait(5000))
                    {

                    }
                }
                catch (Exception ex)
                {

                }
            });
        }

        public void Clear(int buffer, System.Drawing.Color color) 
        {
            this.bmps?[buffer]?.Context?.PlatformImpl?.Clear(Colors.Transparent);
        }

        public void TranslateTransform(int buffer, int x, int y)
        {
            this.bmps?[buffer]?.Context?.PushSetTransform(new Matrix(1, 0, 0, 1, x, y));
        }

        public void Draw(int buffer, Interfaces.IDescription description)
        {
            if (description is TileMap)
            {
                Draw(this.bmps?[0]?.Context, description as TileMap);
            }
            else
            {
                Description2D d2d = description as Description2D;
                DrawingContext gfx = d2d.DrawInOverlay ? this.overlays?[buffer]?.Context : this.bmps?[buffer]?.Context;
                Draw(gfx, d2d);
            }
        }

        public AvaloniaBitmap Image(int buf) => bmps?[0];

        public AvaloniaBitmap Overlay(int buf) => overlays?[0];

        public void Draw(DrawingContext gfx, Description2D description)
        {
            if (description != null)
            {
                if (description.HasImage())
                {
                    Rect dest = new Rect((int)(description.X + description.DrawOffsetX) - (description?.Sprite.X ?? 0), (int)(description.Y + description.DrawOffsetY) - (description?.Sprite.Y ?? 0), description.Width, description.Height);

                    using MemoryStream stream = new MemoryStream();
                    description.Image().Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    gfx?.DrawImage(new Avalonia.Media.Imaging.Bitmap(stream), new Rect(0, 0, description.Width, description.Height), dest);
                }
            }
        }

        public void RedrawTiles()
        {
            tiles = null;
        }

        public void Draw(DrawingContext gfx, TileMap map)
        {
            if (map.Sprite != null)
            {
                if (tiles == null)
                {
                    tiles = new RenderTargetBitmap(new PixelSize(map.Width, map.Height));
                    using DrawingContext mgfx = new DrawingContext(tiles.CreateDrawingContext(null));
                    //mgfx.Clip = new Region(new Rectangle(new Point(), new Size(map.Width, map.Height)));
                    int x = 0;
                    int y = 0;
                    foreach (int tile in map.Tiles)
                    {
                        using MemoryStream stream = new MemoryStream();
                        map.Image(tile).Save(stream, ImageFormat.Png);
                        stream.Position = 0;
                        mgfx.DrawImage(new Avalonia.Media.Imaging.Bitmap(stream), new Rect(0, 0, map.Sprite.Width, map.Sprite.Height), new Rect(x * map.Sprite.Width, y * map.Sprite.Height, map.Sprite.Width, map.Sprite.Height));
                        x++;
                        if (x >= map.Columns)
                        {
                            x = 0;
                            y++;
                        }
                    }
                }

                gfx?.DrawImage(tiles, new Rect(0, 0, tiles.Size.Width, tiles.Size.Height), new Rect(0, 0, tiles.Size.Width, tiles.Size.Height));
            }
        }
    }
}
