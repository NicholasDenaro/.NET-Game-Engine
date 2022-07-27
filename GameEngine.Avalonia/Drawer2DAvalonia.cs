using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameEngine.UI.AvaloniaUI
{
    public class Drawer2DAvalonia : IDrawer2D<AvaloniaGameBitmap>
    {
        private SortedDictionary<int, List<Action<DrawingContext>>> drawings;
        private SortedDictionary<int, List<Action<DrawingContext>>> overlays;

        public Drawer2DAvalonia()
        {
            drawings = new SortedDictionary<int, List<Action<DrawingContext>>>();
            overlays = new SortedDictionary<int, List<Action<DrawingContext>>>();
        }

        private int width;
        private int height;
        private int xScale;
        private int yScale;

        public void Init(int width, int height, int xScale, int yScale)
        {
            this.width = width;
            this.height = height;
            this.xScale = xScale;
            this.yScale = yScale;
        }

        public void Clear(int buffer, _2D.Color color) 
        {
            drawings.Clear();
            overlays.Clear();
            this.FillRectangle(buffer, color, 0, 0, this.width, this.height);
        }

        public void TranslateTransform(int buffer, int x, int y)
        {
        }

        public void FillRectangle(int buffer, _2D.Color color, int x, int y, int w, int h)
        {
            AddDrawing(0, (g) => g.FillRectangle(new SolidColorBrush(Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B)), new Rect(x, y, w, h)));
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
                if (description.DrawInOverlay)
                {
                    AddOverlay(description.ZIndex, (g) => Draw(g, description));
                }
                else
                {
                    AddDrawing(description.ZIndex, (g) => Draw(g, description));
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

        private void AddOverlay(int index, Action<DrawingContext> action)
        {
            if (!overlays.ContainsKey(index))
            {
                overlays.Add(index, new List<Action<DrawingContext>>());
            }

            overlays[index].Add(action);
        }

        public AvaloniaGameBitmap Image(int buf) => throw new NotImplementedException("Avalonia doesn't use buffers");

        public AvaloniaGameBitmap Overlay(int buf) => throw new NotImplementedException("Avalonia doesn't use buffers");

        public SortedDictionary<int, List<Action<DrawingContext>>> Drawings => drawings;
        public SortedDictionary<int, List<Action<DrawingContext>>> Overlays => overlays;

        //private Dictionary<_2D.Bitmap, Avalonia.Media.Imaging.Bitmap> storedBitmaps = new Dictionary<_2D.Bitmap, Avalonia.Media.Imaging.Bitmap>();
        private object locker = new object();

        public void Draw(DrawingContext gfx, Description2D description)
        {
            if (description != null)
            {
                if (description.HasImage())
                {
                    Rect dest = new Rect((int)(description.X + description.DrawOffsetX) - (description?.Sprite?.X ?? 0), (int)(description.Y + description.DrawOffsetY) - (description?.Sprite?.Y ?? 0), description.Width, description.Height);

                    lock (locker)
                    {
                        _2D.BitmapSection bmpSection = description.Image();
                        _2D.Bitmap img = bmpSection.Bitmap as AvaloniaGameBitmap;

                        gfx?.DrawImage(img.Image<RenderTargetBitmap>(), new Rect(bmpSection.Bounds.X, bmpSection.Bounds.Y, bmpSection.Bounds.Width, bmpSection.Bounds.Height), dest);
                    }
                }
            }
        }

        //RenderTargetBitmap tiles;
        AvaloniaGameBitmap tiles;
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
                    //tiles = new RenderTargetBitmap(new PixelSize(map.Width, map.Height));
                    //using DrawingContext mgfx = new DrawingContext(tiles.CreateDrawingContext(null));
                    tiles = (AvaloniaGameBitmap)_2D.Bitmap.CreateAsync("_tiles", map.Width, map.Height).Result;
                    var mgfx = tiles.GetGraphics();
                    int x = 0;
                    int y = 0;

                    foreach (int tile in map.Tiles)
                    {
                        _2D.BitmapSection bmpSection = map.Image(tile);
                        _2D.Bitmap img = bmpSection.Bitmap as AvaloniaGameBitmap;

                        mgfx.DrawImageAsync(img, bmpSection.Bounds, new Rectangle(x * map.Sprite.Width, y * map.Sprite.Height, map.Sprite.Width, map.Sprite.Height));
                        x++;
                        if (x >= map.Columns)
                        {
                            x = 0;
                            y++;
                        }
                    }
                }

                gfx?.DrawImage(tiles.Image<RenderTargetBitmap>(), new Rect(0, 0, tiles.Width, tiles.Height), new Rect(0, 0, tiles.Width, tiles.Height));
            }
        }
    }
}
