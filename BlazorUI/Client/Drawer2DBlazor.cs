using Blazor.Extensions.Canvas.Canvas2D;
using GameEngine._2D;
using GameEngine._2D.Interfaces;
using GameEngine.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Client
{
    public class Drawer2DBlazor : IDrawer2D<BlazorBitmap>
    {
        private SortedDictionary<int, List<Func<Canvas2DContext, Task>>> drawings;
        private SortedDictionary<int, List<Func<Canvas2DContext, Task>>> overlays;

        public SortedDictionary<int, List<Func<Canvas2DContext, Task>>> Drawings => drawings;
        public SortedDictionary<int, List<Func<Canvas2DContext, Task>>> Overlays => overlays;
        public Drawer2DBlazor()
        {
            drawings = new SortedDictionary<int, List<Func<Canvas2DContext, Task>>>();
            overlays = new SortedDictionary<int, List<Func<Canvas2DContext, Task>>>();
        }
        public void Clear(int buffer, Color color)
        {
            //throw new NotImplementedException();
        }

        public void Draw(int buffer, IDescription description)
        {
            if (!(description is Description2D))
            {
                throw new ArgumentOutOfRangeException($"{description.GetType()} is not a ${nameof(Description2D)}");
            }

            Draw(description as Description2D);
        }

        private void Draw(Description2D description)
        {
            if (description is TileMap)
            {
                AddDrawing(description.ZIndex, async (g) => await Draw(g, description as TileMap));
            }
            else
            {
                if (description.DrawInOverlay)
                {
                    AddOverlay(description.ZIndex, async (g) => await Draw(g, description));
                }
                else
                {
                    AddDrawing(description.ZIndex, async (g) => await Draw(g, description));
                }
            }
        }

        public async Task Draw(Canvas2DContext gfx, Description2D description)
        {
            if (description != null)
            {
                if (description.HasImage())
                {
                    Rectangle dest = new Rectangle((int)(description.X + description.DrawOffsetX) - (description?.Sprite?.X ?? 0), (int)(description.Y + description.DrawOffsetY) - (description?.Sprite?.Y ?? 0), description.Width, description.Height);

                    //lock (locker)
                    {
                        GameEngine._2D.BitmapSection bmpSection = description.Image();
                        BlazorBitmap img = bmpSection.Bitmap as BlazorBitmap;

                        //Console.WriteLine($"drawing image: {img.Image().Id}");
                        if (img.IsInitialized && gfx != null)
                        {
                            await gfx.DrawImageAsync(img.Image(),
                                bmpSection.Bounds.X,
                                bmpSection.Bounds.Y,
                                bmpSection.Bounds.Width,
                                bmpSection.Bounds.Height,
                                dest.X,
                                dest.Y,
                                dest.Width,
                                dest.Height);
                        }
                    }
                }
            }
        }

        BlazorBitmap tiles;
        private bool createdMap;
        public async Task Draw(Canvas2DContext gfx, TileMap map)
        {
            if (map.Sprite != null)
            {
                if (tiles == null)
                {
                    //tiles = new RenderTargetBitmap(new PixelSize(map.Width, map.Height));
                    //using DrawingContext mgfx = new DrawingContext(tiles.CreateDrawingContext(null));
                    Console.WriteLine("Trying to create _tiles");
                    tiles = (await GameEngine._2D.Bitmap.CreateAsync("_tiles", map.Width, map.Height)) as BlazorBitmap;
                    Console.WriteLine("Created tile map");
                }
                else if (tiles.IsInitialized && !createdMap)
                {
                    var mgfx = tiles.GetGraphics();
                    int x = 0;
                    int y = 0;

                    //await (mgfx as BlazorGraphics).BatchStart();
                    foreach (int tile in map.Tiles)
                    {
                        GameEngine._2D.BitmapSection bmpSection = map.Image(tile);
                        //Console.WriteLine($"{tile}: {bmpSection.Bounds.X},{bmpSection.Bounds.Y},{bmpSection.Bounds.Width},{bmpSection.Bounds.Height}");
                        GameEngine._2D.Bitmap img = bmpSection.Bitmap as BlazorBitmap;

                        await mgfx.DrawImageAsync(img, bmpSection.Bounds, new Rectangle(x * map.Sprite.Width, y * map.Sprite.Height, map.Sprite.Width, map.Sprite.Height));
                        x++;
                        if (x >= map.Columns)
                        {
                            x = 0;
                            y++;
                        }
                    }
                    //await (mgfx as BlazorGraphics).BatchEnd();
                    createdMap = true;
                    Console.WriteLine("==Finished creating tilemap image==");
                }
                if (gfx != null && tiles != null && tiles.IsInitialized && createdMap)
                {
                    //Console.WriteLine("drawing tilemap to screen");
                    await gfx.DrawImageAsync(tiles.Image(), 0, 0, tiles.Width, tiles.Height, 0, 0, tiles.Width, tiles.Height);
                }
            }
        }

        public async Task Draw(Canvas2DContext gfx)
        {
            var localDrawings = new SortedDictionary<int, List<Func<Canvas2DContext, Task>>>(drawings);
            var localOverlays = new SortedDictionary<int, List<Func<Canvas2DContext, Task>>>(overlays);

            drawings.Clear();
            overlays.Clear();
            foreach (var key in localDrawings.Keys)
            {
                foreach (Func<Canvas2DContext, Task> act in localDrawings[key])
                {
                    await act(gfx);
                }
            }

            foreach (var key in localOverlays.Keys)
            {
                foreach (Func<Canvas2DContext, Task> act in localOverlays[key])
                {
                    await act(gfx);
                }
            }
        }

        private void AddDrawing(int index, Func<Canvas2DContext, Task> action)
        {
            if (!drawings.ContainsKey(index))
            {
                drawings.Add(index, new List<Func<Canvas2DContext, Task>>());
            }

            drawings[index].Add(action);
        }

        private void AddOverlay(int index, Func<Canvas2DContext, Task> action)
        {
            if (!overlays.ContainsKey(index))
            {
                overlays.Add(index, new List<Func<Canvas2DContext, Task>>());
            }

            overlays[index].Add(action);
        }

        public void FillRectangle(int buffer, Color color, int x, int y, int w, int h)
        {
            //throw new NotImplementedException();
        }

        public BlazorBitmap Image(int buf)
        {
            throw new NotImplementedException();
        }

        public void Init(int width, int height, int xScale, int yScale)
        {
            //throw new NotImplementedException();
        }

        public BlazorBitmap Overlay(int buf)
        {
            throw new NotImplementedException();
        }

        public void TranslateTransform(int buffer, int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
