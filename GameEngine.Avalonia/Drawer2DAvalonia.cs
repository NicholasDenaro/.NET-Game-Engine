using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;

namespace GameEngine.UI.AvaloniaUI
{
    public class Drawer2DAvalonia : IDrawer2D<AvaloniaBitmap>
    {
        private SortedDictionary<int, List<Action<DrawingContext>>> drawings;

        public Drawer2DAvalonia()
        {
            drawings = new SortedDictionary<int, List<Action<DrawingContext>>>();
        }

        public void Init(int width, int height, int xScale, int yScale)
        {
        }

        public void Clear(int buffer, System.Drawing.Color color) 
        {
            drawings.Clear();
        }

        public void TranslateTransform(int buffer, int x, int y)
        {
        }

        public void FillRectangle(int buffer, System.Drawing.Color color, int x, int y, int w, int h)
        {
            AddDrawing(0, (g) => g.FillRectangle(new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B)), new Rect(x, y, w, h)));
        }

        public void Draw(int buffer, Interfaces.IDescription d)
        {
            if (!(d is Description2D))
            {
                throw new ArgumentOutOfRangeException($"{d.GetType()} is not a ${nameof(Description2D)}");
            }

            Draw(d as Description2D);
        }


        public void Draw(Description2D description)
        {
            if (description is TileMap)
            {
                AddDrawing(description.ZIndex, (g) => Draw(g, description as TileMap));
            }
            else
            {
                Description2D d2d = description as Description2D;
                if (d2d != null)
                {
                    AddDrawing(description.ZIndex, (g) => Draw(g, d2d));
                }
            }
        }

        private void AddDrawing(int index, Action<DrawingContext> action)
        {
            if (!drawings.ContainsKey(index))
            {
                drawings.Add(index, new List<Action<DrawingContext>>());
            }

            drawings[index].Add(action);
        }

        public AvaloniaBitmap Image(int buf) => throw new NotImplementedException("Avalonia doesn't use buffers");

        public AvaloniaBitmap Overlay(int buf) => throw new NotImplementedException("Avalonia doesn't use buffers");

        public SortedDictionary<int, List<Action<DrawingContext>>> Drawings => drawings;


        private Dictionary<System.Drawing.Image, Bitmap> storedBitmaps = new Dictionary<System.Drawing.Image, Bitmap>();

        public void Draw(DrawingContext gfx, Description2D description)
        {
            if (description != null)
            {
                if (description.HasImage())
                {
                    Rect dest = new Rect((int)(description.X + description.DrawOffsetX) - (description?.Sprite?.X ?? 0), (int)(description.Y + description.DrawOffsetY) - (description?.Sprite?.Y ?? 0), description.Width, description.Height);

                    System.Drawing.Image img = description.Image();
                    if (!storedBitmaps.ContainsKey(description.Image()))
                    {
                        using MemoryStream stream = new MemoryStream();
                        img.Save(stream, ImageFormat.Png);
                        stream.Position = 0;
                        storedBitmaps.Add(img, new Bitmap(stream));
                    }

                    gfx?.DrawImage(storedBitmaps[img], new Rect(0, 0, description.Width, description.Height), dest);
                }
            }
        }

        RenderTargetBitmap tiles;
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
